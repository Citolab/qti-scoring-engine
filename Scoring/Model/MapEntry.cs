using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.Scoring.Model
{

    //  <mapEntry mapKey = "B" mappedValue="1"/>
    //  <mapEntry mapKey = "C" mappedValue="2"/>
    internal class MapEntry
    {
        public string MapKey { get; set; }
        public float MappedValue { get; set; }
    }
}
