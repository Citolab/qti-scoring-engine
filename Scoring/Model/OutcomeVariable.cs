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
            var value = Value?.ToString();
            if (value.TryParseFloat(out var v))
            {
                // prevent scores to be written as 0.0 but 0 instead.
                value = v.ToString("0.############", CultureInfo.InvariantCulture);
            }
            // built rather than parsed from a string: a string outcome holding & or < is
            // ordinary data, and interpolating it into XML threw XmlException.
            return new XElement("outcomeVariable",
                new XAttribute("identifier", Identifier ?? string.Empty),
                new XAttribute("cardinality", Cardinality.GetString()),
                new XAttribute("baseType", BaseType.GetString()),
                new XElement("value", value ?? string.Empty));
        }
    }
}
