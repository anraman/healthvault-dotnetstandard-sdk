// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information related to a medical problem (please see remarks).
    /// </summary>
    /// <remarks>
    /// We are in the process of merging the problem and condition types, and
    /// recommend that applications use the condition type instead of the problem type.
    /// </remarks>
    ///
    public class Problem : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Problem"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Problem()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Problem"/> class with the
        /// specified date and time.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time for the health problem.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Problem(HealthServiceDateTime when)
            : base(TypeId)
        {
            When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("5E2C027E-3417-4CFC-BD10-5A6F2E91AD23");

        /// <summary>
        /// Populates this health problem instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the health problem data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a health problem node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("problem");

            Validator.ThrowInvalidIfNull(itemNav, Resources.ProblemUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // <diagnosis>
            XPathNodeIterator diagnosisIterator =
                itemNav.Select("diagnosis");

            _diagnosis.Clear();
            foreach (XPathNavigator diagnosisNav in diagnosisIterator)
            {
                CodableValue value = new CodableValue();
                value.ParseXml(diagnosisNav);
                _diagnosis.Add(value);
            }

            // <duration>
            XPathNodeIterator durationIterator =
                itemNav.Select("duration");

            _duration.Clear();
            foreach (XPathNavigator durationNav in durationIterator)
            {
                DurationValue value = new DurationValue();
                value.ParseXml(durationNav);
                _duration.Add(value);
            }

            // <importance>
            _importance =
                XPathHelper.GetOptNavValueAsInt(
                    itemNav,
                    "importance");
        }

        /// <summary>
        /// Writes the health problem data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the health problem data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="When"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.ProblemWhenNotSet);

            // <problem>
            writer.WriteStartElement("problem");

            // <when>
            _when.WriteXml("when", writer);

            // <diagnosis>
            foreach (CodableValue value in _diagnosis)
            {
                value.WriteXml("diagnosis", writer);
            }

            // <duration>
            foreach (DurationValue value in _duration)
            {
                value.WriteXml("duration", writer);
            }

            // <importance>
            XmlWriterHelper.WriteOptInt(
                writer,
                "importance",
                _importance);

            // </problem>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the health problem occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing
        /// the date and time. The default value is the current year, month,
        /// and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets a collection of diagnoses for the health problem.
        /// </summary>
        ///
        /// <value>
        /// A collection of type <see cref="CodableValue"/> containing
        /// the diagnoses.
        /// </value>
        ///
        /// <remarks>
        /// To add or remove a diagnosis, call Add or Remove on the returned
        /// collection.
        /// </remarks>
        ///
        public Collection<CodableValue> Diagnosis => _diagnosis;

        private readonly Collection<CodableValue> _diagnosis =
            new Collection<CodableValue>();

        /// <summary>
        /// Gets a collection of duration information for the health problem.
        /// </summary>
        ///
        /// <value>
        /// A collection of type <see cref="CodableValue"/> containing
        /// the duration information.
        /// </value>
        ///
        /// <remarks>
        /// To add or remove a duration, call Add or Remove on the returned
        /// collection.
        /// </remarks>
        ///
        public Collection<DurationValue> Duration => _duration;

        private readonly Collection<DurationValue> _duration =
            new Collection<DurationValue>();

        /// <summary>
        /// Gets or sets the importance of the health problem.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the importance value.
        /// </value>
        ///
        /// <remarks>
        /// If the importance of the problem is unknown, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        ///
        public int? Importance
        {
            get { return _importance; }

            set
            {
                if (value != null && (value.Value < 1 || value.Value > 5))
                {
                    throw new ArgumentOutOfRangeException(nameof(Importance), Resources.ProblemImportanceOutOfRange);
                }

                _importance = value;
            }
        }

        private int? _importance;

        /// <summary>
        /// Gets a string representation of the problem.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the problem.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(250);

            if (Diagnosis.Count > 0)
            {
                foreach (CodableValue diagnosis in Diagnosis)
                {
                    if (result.Length > 0)
                    {
                        result.Append(
                            Resources.ListSeparator);
                    }

                    result.Append(diagnosis.Text);
                }
            }
            else
            {
                result.Append(When);
            }

            return result.ToString();
        }
    }
}
