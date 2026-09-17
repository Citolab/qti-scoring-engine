using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-lcm: the lowest common multiple of its integer children. A zero among them makes
    /// the result 0, as the spec prescribes.
    /// </summary>
    internal class Lcm : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count == 0)
            {
                ctx.LogError("qti-lcm should have at least one child expression.");
                return null;
            }
            var numbers = Helper.GetNumbers(expressions, ctx, "qti-lcm");
            if (numbers == null)
            {
                return null;
            }
            var lcm = 1;
            foreach (var number in numbers)
            {
                var value = Math.Abs((int)number);
                if (value == 0)
                {
                    return ((int)0).ToBaseValue();
                }
                lcm = lcm / Gcd.GreatestCommonDivisor(lcm, value) * value;
            }
            return lcm.ToBaseValue();
        }
    }
}
