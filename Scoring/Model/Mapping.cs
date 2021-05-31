using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{
    //<mapping defaultValue = "0" lowerBound="0" upperBound="1">
    //  <mapEntry mapKey = "B" mappedValue="1"/>
    //  <mapEntry mapKey = "C" mappedValue="2"/>
    //</mapping>
    internal class Mapping
    {
        public float DefaultValue { get; set; }
        public float? LowerBound { get; set; }
        public float? UpperBound { get; set; }
        public List<MapEntry> MapEntries { get; set; }
    }
}
