using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-response-processing-fragment: a group of rules that is executed in place. Fragments
    /// in another file are pulled in by qti-include, which is not resolved here; a fragment
    /// that is present in the item itself is processed like the rules around it.
    /// </summary>
    internal class ResponseProcessingFragment : ConditionExpressionBase
    {
        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.OutcomeProcessing };

        public override bool Execute(IProcessingContext ctx)
        {
            foreach (var expression in conditionalExpressions)
            {
                expression.Execute(ctx);
            }
            return true;
        }
    }
}
