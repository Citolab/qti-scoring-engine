using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AreaMapping
    {
        public float DefaultValue { get; set; }
        public float? LowerBound { get; set; }
        public float? UpperBound { get; set; }
        public List<AreaMapEntry> AreaMappings { get; set; }

    }
}
