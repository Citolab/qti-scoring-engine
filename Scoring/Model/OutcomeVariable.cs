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
    internal class OutcomeVariable
    {
        public string Identifier { get; set; }
        public Cardinality Cardinality { get; set; }

        public BaseType BaseType { get; set; }

        public object Value { get; set; }

        public XElement ToElement()
        {
            var value = Value?.ToString();
            if (value.TryParseFloat(out var v))
            {
                // prevent scores to be written as 0.0 but 0 instead.
                value = v.ToString("0.############", CultureInfo.InvariantCulture);
            }
            return XElement.Parse($"<outcomeVariable identifier=\"{Identifier}\" " +
                $"cardinality=\"{Cardinality.GetString()}\" " +
                $"baseType=\"{BaseType.GetString()}\"><value>{value}</value></outcomeVariable>");
        }
    }
}
