﻿using System;
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
        //public int Weight { get; set; } = 1;
    }
}
