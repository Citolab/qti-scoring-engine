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

        /// <summary>
        /// The default of a multiple or ordered declaration: every qti-value of qti-default-value,
        /// in order. Null when there is none, which makes the outcome start as NULL.
        /// </summary>
        public List<string> DefaultValues { get; set; }

        public bool IsContainer => Cardinality == Cardinality.Multiple || Cardinality == Cardinality.Ordered;

        public List<InterpolationTableEntry> InterpolationTable { get; set; }

        public List<MatchTableEntry> MatchTable { get; set; }
    }
}
