using System.Collections.Generic;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AssessmentItemRef
    {
        public string Identifier { get; set; }
        public Dictionary<string, float> Weights { get; set; }
        public HashSet<string> Categories { get; set; }
    }
}
