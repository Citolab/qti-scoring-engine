using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-gcd: the greatest common divisor of its integer children. gcd of only zeros is 0.
    /// </summary>
    internal class Gcd : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count == 0)
            {
                ctx.LogError("qti-gcd should have at least one child expression.");
                return null;
            }
            var numbers = Helper.GetNumbers(expressions, ctx, "qti-gcd");
            if (numbers == null)
            {
                return null;
            }
            var gcd = 0;
            foreach (var number in numbers)
            {
                gcd = GreatestCommonDivisor(gcd, Math.Abs((int)number));
            }
            return gcd.ToBaseValue();
        }

        internal static int GreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
            {
                var remainder = a % b;
                a = b;
                b = remainder;
            }
            return a;
        }
    }
}
