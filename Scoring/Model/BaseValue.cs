using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{
    public class BaseValue
    {
        public string Identifier { get; set; }
        public BaseType BaseType { get; set; }

        public Cardinality? Cardinality { get; set; }
        public string Value { get; set; }
        public List<string> Values { get; set; }

        /// <summary>
        /// The fields of a record, by field identifier. Only set when Cardinality is Record;
        /// qti-field-value reads a single one of them.
        /// </summary>
        public Dictionary<string, BaseValue> Fields { get; set; }
        //public int Weight { get; set; } = 1;
    }
}
