// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a medical annotation.
    /// </summary>
    ///
    public class Annotation : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Annotation"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public Annotation()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Annotation"/> class with
        /// the specified date and time.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the annotation was taken.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Annotation(HealthServiceDateTime when)
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
            new Guid("7AB3E662-CC5B-4BE2-BF38-78F8AAD5B161");

        /// <summary>
        /// Populates this <see cref="Annotation"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the annotation data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an annotation node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("annotation");

            Validator.ThrowInvalidIfNull(itemNav, Resources.AnnotationUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _content =
                XPathHelper.GetOptNavValue(itemNav, "content");

            _author =
                XPathHelper.GetOptNavValue<PersonItem>(itemNav, "author");

            _classification =
                XPathHelper.GetOptNavValue(itemNav, "classification");

            _index =
                XPathHelper.GetOptNavValue(itemNav, "index");

            _version =
                XPathHelper.GetOptNavValue(itemNav, "version");
        }

        /// <summary>
        /// Writes the annotation data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the annotation data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// A mandatory property is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);
            Validator.ThrowSerializationIfNull(_when, Resources.AnnotationWhenNotSet);

            // <annotation>
            writer.WriteStartElement("annotation");

            // <when>
            _when.WriteXml("when", writer);

            // <content>
            XmlWriterHelper.WriteOptString(
                writer,
                "content",
                _content);

            // <author>
            XmlWriterHelper.WriteOpt(
                writer,
                "author",
                Author);

            // <classification>
            XmlWriterHelper.WriteOptString(
                writer,
                "classification",
                _classification);

            // <index>
            XmlWriterHelper.WriteOptString(
                writer,
                "index",
                _index);

            // <version>
            XmlWriterHelper.WriteOptString(
                writer,
                "version",
                _version);

            // </annotation>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the annotation was created.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/>. The default value is
        /// the current year, month, and day.
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
        /// Gets or sets the content for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the content.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the content should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Content
        {
            get { return _content; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Content");
                _content = value;
            }
        }

        private string _content;

        /// <summary>
        /// Gets or sets the author contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the author contact information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the author contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem Author
        {
            get { return _author; }
            set { _author = value; }
        }

        private PersonItem _author;

        /// <summary>
        /// Gets or sets the classification for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the classification.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the classification should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Classification
        {
            get { return _classification; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Classification");
                _classification = value;
            }
        }

        private string _classification;

        /// <summary>
        /// Gets or sets the index for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the index.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the index should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Index
        {
            get { return _index; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Index");
                _index = value;
            }
        }

        private string _index;

        /// <summary>
        /// Gets or sets the version for the annotation.
        /// </summary>
        ///
        /// <value>
        /// A string representing the version.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the version should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Version
        {
            get { return _version; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Version");
                _version = value;
            }
        }

        private string _version;

        /// <summary>
        /// Gets a string representation of the annotation item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the annotation item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result;

            if (Content != null)
            {
                result = Content;

                if (Content.Length > 50)
                {
                    result = Content.Substring(0, 50) +
                             Resources.Ellipsis;
                }
            }
            else if (Author != null)
            {
                result =
                    string.Format(
                        Resources.AnnotationAuthorFormat,
                        Author.ToString());
            }
            else
            {
                result =
                    string.Format(
                        Resources.AnnotationDateFormat,
                        When.ToString());
            }

            return result;
        }
    }
}
