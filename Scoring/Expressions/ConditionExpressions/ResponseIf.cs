using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class ResponseIf : ConditionExpressionBase
    {
        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.OutcomeProcessing };
        public override bool Execute(IProcessingContext ctx)
        {
            var firstChild = conditionalExpressions.FirstOrDefault();
            var result = firstChild.Execute(ctx);
            if (result == true)
            {
                var otherChilds = conditionalExpressions.Skip(1);
                var maxLoops = otherChilds.Count() >= 100 ? 100 : otherChilds.Count();
                foreach (var otherChild in otherChilds.Take(maxLoops))
                {
                    otherChild.Execute(ctx);
                }
                return true;
            }
            return false;
        }
    }
}
