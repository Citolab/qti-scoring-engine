using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class ResponseDeclaration
    {
        public string Identifier { get; set; }
        public Cardinality Cardinality { get; set; }
        public BaseType BaseType { get; set; }
        public string CorrectResponse { get; set; }
        public List<string> CorrectResponses { get; set; }
        public string CorrectResponseInterpretation { get; set; }
        public List<string> DefaultValues { get; set; }

        /// <summary>
        /// The default of a record declaration, by field identifier.
        /// </summary>
        public Dictionary<string, BaseValue> DefaultFields { get; set; }
        public Mapping Mapping { get; set; }

        public AreaMapping AreaMapping { get; set; }
    }
}
