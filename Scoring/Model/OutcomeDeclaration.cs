using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class OutcomeDeclaration
    {
        public string Identifier { get; set; }
        public Cardinality Cardinality { get; set; }

        public BaseType BaseType { get; set; }

        public object DefaultValue { get; set; }

        /// <summary>
        /// The default of a record declaration: qti-default-value holds one qti-value per field
        /// instead of a single value.
        /// </summary>
        public Dictionary<string, BaseValue> DefaultFields { get; set; }

        public List<InterpolationTableEntry> InterpolationTable { get; set; }

        public List<MatchTableEntry> MatchTable { get; set; }
    }
}
