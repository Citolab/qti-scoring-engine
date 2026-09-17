using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-integer-divide: floor(x / y). Dividing by zero is NULL.
    /// </summary>
    internal class IntegerDivide : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 2)
            {
                ctx.LogError($"qti-integer-divide should have two child expressions, found: {expressions.Count}");
                return null;
            }
            var numbers = Helper.GetNumbers(expressions, ctx, "qti-integer-divide");
            if (numbers == null)
            {
                return null;
            }
            if (numbers[1] == 0.0)
            {
                ctx.LogInformation("qti-integer-divide by zero, returning null.");
                return null;
            }
            return ((int)Math.Floor(numbers[0] / numbers[1])).ToBaseValue();
        }
    }
}
