using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class VariableBase
    {
        public string Identifier { get; set; }
        public Cardinality Cardinality { get; set; }
        public BaseType BaseType { get; set; }
        public object ObjectValue { get; set; }

        /// <summary>
        /// The fields of a record variable, by field identifier. Null for every other cardinality.
        /// </summary>
        public Dictionary<string, BaseValue> Fields { get; set; }
    }
}
