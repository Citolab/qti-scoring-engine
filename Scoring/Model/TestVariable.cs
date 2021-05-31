using Citolab.QTI.ScoringEngine.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class SumTestVariable
    {
        public string Identifier { get; set; } = "SCORE";
        public string ItemIdentifier { get; set; } = "SCORE";
        public string WeightIdentifier { get; set; }
        public List<string> IncludedCategories { get; set; } = new List<string>();
        public List<string> ExcludedCategories { get; set; } = new List<string>();

        public XElement ToSummedSetOutcomeElement()
        {
            var includedCategory = IncludedCategories != null && IncludedCategories.Any() ? $"includeCategory=\"{ string.Join(" ", IncludedCategories) }\"" : "";
            var excludeCategory = ExcludedCategories!= null && ExcludedCategories.Any() ? $"excludeCategory=\"{ string.Join(" ", ExcludedCategories) }\"" : "";
            var weigthIdentifier = !string.IsNullOrEmpty(WeightIdentifier) ? $"weightIdentifier=\"{ WeightIdentifier }\"" : "";

            var testVariable = $"<testVariables { includedCategory } {excludeCategory} variableIdentifier=\"{ItemIdentifier}\" {weigthIdentifier} />";
            var setOutcome = $"<setOutcomeValue identifier=\"{Identifier}\"><sum>{testVariable}</sum></setOutcomeValue>";
            return XElement.Parse(setOutcome);
        }

        public XElement OutcomeElement()
        {
            return 0.0F.ToOutcomeDeclaration(Identifier).ToElement();
        }
    }
}
