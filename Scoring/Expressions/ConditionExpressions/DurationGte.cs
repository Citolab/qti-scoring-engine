using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-duration-gte: whether the first duration is at least as long as the second.
    /// </summary>
    internal class DurationGte : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            var durations = Helper.GetDurations(expressions, ctx, "qti-duration-gte");
            return durations != null && durations[0] >= durations[1];
        }
    }
}
