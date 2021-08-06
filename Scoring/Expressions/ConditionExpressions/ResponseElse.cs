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
    internal class ResponseElse : ConditionExpressionBase
    {
        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.OutcomeProcessing };
        public override bool Execute(IProcessingContext ctx)
        {
            var maxLoops = conditionalExpressions.Count() >= 100 ? 100 : conditionalExpressions.Count();
            foreach (var expression in conditionalExpressions.Take(maxLoops))
            {
                expression.Execute(ctx);
            }
            return true;
        }
    }
}
