using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AreaMapEntry
    {
        // public
        public string Coords { get; set; }

        public Shape Shape { get; set; }

        public float MappedValue { get; set; }
    }
}
