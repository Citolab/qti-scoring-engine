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
    internal class ResponseCondition : ConditionExpressionBase
    {
        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.OutcomeProcessing };

        public override bool Execute(IProcessingContext ctx)
        {
            foreach (var expression in conditionalExpressions)
            {
                var result = expression.Execute(ctx);
                if (result == true) // if handled then exit for;
                {
                    break;
                }
            }
            return true;
        }
    }
}
