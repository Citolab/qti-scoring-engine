using System.Collections.Generic;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    public class AssessmentItemRef
    {
        public string Identifier { get; set; }
        public Dictionary<string, int> Weights { get; set; }
        public HashSet<string> Categories { get; set; }
    }
}
