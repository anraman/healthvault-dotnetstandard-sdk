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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information related to a care plan and tasks, goals associated with the care plan.
    /// </summary>
    ///
    /// <remarks>
    /// The care plan contains tasks and goals.
    /// </remarks>
    ///
    public class CarePlan : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlan"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public CarePlan()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CarePlan"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        /// <param name="name">
        /// Name of the care plan.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <b>null</b>, empty or contains only whitespace.
        /// </exception>
        ///
        public CarePlan(string name)
        : base(TypeId)
        {
            Name = name;
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
            new Guid("415c95e0-0533-4d9c-ac73-91dc5031186c");

        /// <summary>
        /// Populates this <see cref="CarePlan"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the CarePlan data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a CarePlan node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, nameof(typeSpecificXml), Resources.ParseXmlNavNull);

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("care-plan");

            Validator.ThrowInvalidIfNull(itemNav, Resources.CarePlanUnexpectedNode);

            _name = itemNav.SelectSingleNode("name").Value;
            _startDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "start-date");
            _endDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "end-date");
            _status = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "status");
            _carePlanManager = XPathHelper.GetOptNavValue<PersonItem>(itemNav, "care-plan-manager");

            // collections
            _careTeam = XPathHelper.ParseXmlCollection<PersonItem>(itemNav, "care-team/person");
            _tasks = XPathHelper.ParseXmlCollection<CarePlanTask>(itemNav, "tasks/task");
            _goalGroups = XPathHelper.ParseXmlCollection<CarePlanGoalGroup>(itemNav, "goal-groups/goal-group");
        }

        /// <summary>
        /// Writes the XML representation of the CarePlan into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the CarePlan should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Name"/> is <b>null</b> or empty or contains only whitespace.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            if (string.IsNullOrEmpty(_name) || string.IsNullOrEmpty(_name.Trim()))
            {
                throw new ThingSerializationException(Resources.CarePlanNameNullOrEmpty);
            }

            writer.WriteStartElement("care-plan");
            {
                writer.WriteElementString("name", _name);
                XmlWriterHelper.WriteOpt(writer, "start-date", _startDate);
                XmlWriterHelper.WriteOpt(writer, "end-date", _endDate);
                XmlWriterHelper.WriteOpt(writer, "status", _status);

                XmlWriterHelper.WriteXmlCollection(writer, "care-team", _careTeam, "person");

                XmlWriterHelper.WriteOpt(writer, "care-plan-manager", _carePlanManager);
                XmlWriterHelper.WriteXmlCollection(writer, "tasks", _tasks, "task");
                XmlWriterHelper.WriteXmlCollection(writer, "goal-groups", _goalGroups, "goal-group");
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets name of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about name the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Name");
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Name");

                _name = value;
            }
        }

        private string _name;

        /// <summary>
        /// Gets or sets start date of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about startDate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                _startDate = value;
            }
        }

        private ApproximateDateTime _startDate;

        /// <summary>
        /// Gets or sets end date of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about endDate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                _endDate = value;
            }
        }

        private ApproximateDateTime _endDate;

        /// <summary>
        /// Gets or sets status of the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about status the value should be set to <b>null</b>.
        /// </remarks>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }

        private CodableValue _status;

        /// <summary>
        /// Gets or sets list of person contacts associated with the care plan.
        /// </summary>
        ///
        public Collection<PersonItem> CareTeam => _careTeam;

        private Collection<PersonItem> _careTeam =
            new Collection<PersonItem>();

        /// <summary>
        /// Gets or sets person managing the care plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about carePlanManager the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public PersonItem CarePlanManager
        {
            get
            {
                return _carePlanManager;
            }

            set
            {
                _carePlanManager = value;
            }
        }

        private PersonItem _carePlanManager;

        /// <summary>
        /// Gets or sets list of tasks associated with the care plan.
        /// </summary>
        ///
        public Collection<CarePlanTask> Tasks => _tasks;

        private Collection<CarePlanTask> _tasks =
            new Collection<CarePlanTask>();

        /// <summary>
        /// Gets or sets list of goals associated with the care plan.
        /// </summary>
        ///
        public Collection<CarePlanGoalGroup> GoalGroups => _goalGroups;

        private Collection<CarePlanGoalGroup> _goalGroups =
            new Collection<CarePlanGoalGroup>();

        /// <summary>
        /// Gets a string representation of the CarePlan.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the CarePlan.
        /// </returns>
        ///
        public override string ToString()
        {
            if (_status == null)
            {
                return _name;
            }

            return string.Format(
                CultureInfo.CurrentUICulture,
                Resources.CarePlanFormat,
                _name,
                _status);
        }
    }
}
