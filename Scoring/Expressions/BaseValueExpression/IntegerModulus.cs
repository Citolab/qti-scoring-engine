using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-integer-modulus: x mod y, the remainder that goes with the floor division of
    /// qti-integer-divide. A modulus of zero is NULL.
    /// </summary>
    internal class IntegerModulus : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 2)
            {
                ctx.LogError($"qti-integer-modulus should have two child expressions, found: {expressions.Count}");
                return null;
            }
            var numbers = Helper.GetNumbers(expressions, ctx, "qti-integer-modulus");
            if (numbers == null)
            {
                return null;
            }
            if (numbers[1] == 0.0)
            {
                ctx.LogInformation("qti-integer-modulus by zero, returning null.");
                return null;
            }
            var modulus = numbers[0] - (numbers[1] * Math.Floor(numbers[0] / numbers[1]));
            return ((int)modulus).ToBaseValue();
        }
    }
}
