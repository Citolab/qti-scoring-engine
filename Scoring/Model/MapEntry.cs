using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{

    //  <mapEntry mapKey = "B" mappedValue="1" caseSensitive="false" />
    //  <mapEntry mapKey = "C" mappedValue="2" caseSensitive="false" />
    internal class MapEntry
    {
        public string MapKey { get; set; }
        public float MappedValue { get; set; }
        public bool CaseSensitive { get; set; }
    }
}
