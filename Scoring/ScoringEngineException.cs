using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.Scoring
{
    public class ScoringEngineException : Exception
    {
        public ScoringEngineException(string message)
       : base(message)
        {
        }
    }
}
