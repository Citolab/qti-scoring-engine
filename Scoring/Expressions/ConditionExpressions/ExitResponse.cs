using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System.Collections.Generic;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-exit-response: ends response processing for this attempt. The rules that follow it
    /// are skipped; the outcomes that were already set are kept.
    /// </summary>
    internal class ExitResponse : ConditionExpressionBase
    {
        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.OutcomeProcessing };

        public override bool Execute(IProcessingContext ctx)
        {
            ctx.LogInformation("qti-exit-response: response processing ends here.");
            throw new ExitResponseException();
        }
    }
}
