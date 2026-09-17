using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-divide: the first child divided by the second, as a float. Dividing by zero is
    /// NULL rather than infinity.
    /// </summary>
    internal class Divide : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 2)
            {
                ctx.LogError($"qti-divide should have two child expressions, found: {expressions.Count}");
                return null;
            }
            var numbers = Helper.GetNumbers(expressions, ctx, "qti-divide");
            if (numbers == null)
            {
                return null;
            }
            if (numbers[1] == 0.0)
            {
                ctx.LogInformation("qti-divide by zero, returning null.");
                return null;
            }
            return (numbers[0] / numbers[1]).ToBaseValue();
        }
    }
}
