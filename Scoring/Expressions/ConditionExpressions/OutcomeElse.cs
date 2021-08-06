using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class OutcomeElse : ResponseElse
    {
        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.ResponseProcessig };
    }
}
