using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-duration-lt: whether the first duration is shorter than the second. A duration is a
    /// number of seconds, so it compares as a number.
    /// </summary>
    internal class DurationLt : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            var durations = Helper.GetDurations(expressions, ctx, "qti-duration-lt");
            return durations != null && durations[0] < durations[1];
        }
    }
}
