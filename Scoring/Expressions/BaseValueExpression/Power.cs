using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-power: the first child raised to the power of the second.
    /// </summary>
    internal class Power : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 2)
            {
                ctx.LogError($"qti-power should have two child expressions, found: {expressions.Count}");
                return null;
            }
            var numbers = Helper.GetNumbers(expressions, ctx, "qti-power");
            if (numbers == null)
            {
                return null;
            }
            var result = Math.Pow(numbers[0], numbers[1]);
            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                ctx.LogInformation($"qti-power({numbers[0]}, {numbers[1]}) has no finite result, returning null.");
                return null;
            }
            return result.ToBaseValue();
        }
    }
}
