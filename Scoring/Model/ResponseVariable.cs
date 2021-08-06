using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class ResponseVariable : VariableBase
    {
          public string Value { get; set; }
        public List<string> Values { get; set; }
    }
}
