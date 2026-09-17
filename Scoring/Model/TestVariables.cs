using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class SumTestVariables
    {
        public string Identifier { get; set; } = "SCORE";
        public string ItemIdentifier { get; set; } = "SCORE";
        public string WeightIdentifier { get; set; }
        public List<string> IncludedCategories { get; set; } = new List<string>();
        public List<string> ExcludedCategories { get; set; } = new List<string>();

        public XElement ToSummedSetOutcomeElement()
        {
            var testVariable = new XElement("qti-test-variables",
                new XAttribute("variable-identifier", ItemIdentifier ?? string.Empty));
            if (IncludedCategories != null && IncludedCategories.Any())
            {
                testVariable.Add(new XAttribute("include-category", string.Join(" ", IncludedCategories)));
            }
            if (ExcludedCategories != null && ExcludedCategories.Any())
            {
                testVariable.Add(new XAttribute("exclude-category", string.Join(" ", ExcludedCategories)));
            }
            if (!string.IsNullOrEmpty(WeightIdentifier))
            {
                testVariable.Add(new XAttribute("weight-identifier", WeightIdentifier));
            }
            return new XElement("qti-set-outcome-value",
                new XAttribute("identifier", Identifier ?? string.Empty),
                new XElement("qti-sum", testVariable));
        }

        public XElement OutcomeElement()
        {
            return 0.0F.ToOutcomeDeclaration(Identifier).ToElement();
        }
    }
}
