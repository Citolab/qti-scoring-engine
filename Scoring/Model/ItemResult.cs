using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class ItemResult : IQtiResult
    {
        public string Identifier { get; set; }
        public Dictionary<string, ResponseVariable> ResponseVariables { get; set; } = new Dictionary<string, ResponseVariable>();
        public Dictionary<string, OutcomeVariable> OutcomeVariables { get; set; } = new Dictionary<string, OutcomeVariable>();

        public XElement ToElement()
        {
            return XElement.Parse($"<itemResult identifier=\"{Identifier}\" sessionStatus=\"final\" datestamp=\"{DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}\"></itemResult>");
        }
    }

}
