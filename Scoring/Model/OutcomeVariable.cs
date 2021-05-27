using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.Scoring.Model
{
    internal class OutcomeVariable
    {
        public string Identifier { get; set; }
        public Cardinality Cardinality { get; set; }

        public BaseType BaseType { get; set; }

        public object Value { get; set; }

        public XElement ToElement()
        {
            return XElement.Parse($"<outcomeVariable identifier=\"{Identifier}\" " +
                $"cardinality=\"{Cardinality.GetString()}\" " +
                $"baseType=\"{BaseType.GetString()}\"><value>{Value}</value></outcomeVariable>");
        }
    }
}
