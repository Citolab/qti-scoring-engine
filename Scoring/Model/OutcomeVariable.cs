using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class OutcomeVariable : VariableBase
    {
        public object Value
        {
            get { return ObjectValue; }  
            set { ObjectValue = value; }
        }

        /// <summary>
        /// The values of a multiple or ordered outcome, in order. Null or empty is NULL.
        /// Not used for single and record outcomes.
        /// </summary>
        public List<string> Values { get; set; }

        public bool IsContainer => Cardinality == Model.Cardinality.Multiple || Cardinality == Model.Cardinality.Ordered;

        public XElement ToElement()
        {
            if (Cardinality == Model.Cardinality.Record)
            {
                // a record has no base-type of its own: every field carries its own.
                var recordElement = new XElement("outcomeVariable",
                    new XAttribute("identifier", Identifier ?? string.Empty),
                    new XAttribute("cardinality", Cardinality.GetString()));
                foreach (var valueElement in Fields.ToRecordValueElements())
                {
                    recordElement.Add(valueElement);
                }
                return recordElement;
            }
            if (IsContainer)
            {
                // Results Reporting: one value element per member, none at all when it is NULL.
                var containerElement = new XElement("outcomeVariable",
                    new XAttribute("identifier", Identifier ?? string.Empty),
                    new XAttribute("cardinality", Cardinality.GetString()),
                    new XAttribute("baseType", BaseType.GetString()));
                // members are only reformatted when they are numbers: an identifier such as "01"
                // is not a score and is written as it is.
                var isNumeric = BaseType == BaseType.Float || BaseType == BaseType.Int;
                foreach (var member in Values ?? new List<string>())
                {
                    containerElement.Add(new XElement("value", (isNumeric ? FormatValue(member) : member) ?? string.Empty));
                }
                return containerElement;
            }
            var value = FormatValue(Value?.ToString());
            // built rather than parsed from a string: a string outcome holding & or < is
            // ordinary data, and interpolating it into XML threw XmlException.
            return new XElement("outcomeVariable",
                new XAttribute("identifier", Identifier ?? string.Empty),
                new XAttribute("cardinality", Cardinality.GetString()),
                new XAttribute("baseType", BaseType.GetString()),
                new XElement("value", value ?? string.Empty));
        }

        private static string FormatValue(string value)
        {
            if (value.TryParseFloat(out var v))
            {
                // prevent scores to be written as 0.0 but 0 instead.
                return v.ToString("0.############", CultureInfo.InvariantCulture);
            }
            return value;
        }
    }
}
