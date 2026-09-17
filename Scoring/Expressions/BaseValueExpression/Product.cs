using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-product: the product of its numeric children.
    /// </summary>
    internal class Product : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count == 0)
            {
                ctx.LogError("qti-product should have at least one child expression.");
                return null;
            }
            var numbers = Helper.GetNumbers(expressions, ctx, "qti-product");
            if (numbers == null)
            {
                return null;
            }
            var product = 1.0;
            foreach (var number in numbers)
            {
                product *= number;
            }
            return product.ToBaseValue();
        }
    }
}
