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
    }
}
