using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-inside: whether a point lies in the area given by the shape and coords attributes.
    /// A container of points is true when any of them is inside.
    /// </summary>
    internal class Inside : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-inside should have exactly one child expression, found: {expressions.Count}");
                return false;
            }
            var coords = GetAttributeValue("coords");
            var shape = GetAttributeValue("shape").ToShape();
            var baseValue = expressions[0].Apply(ctx);
            var points = baseValue.ToValueList();
            if (points == null || points.Count == 0)
            {
                return false;
            }
            return points.Any(point => Helper.IsInsideRegion(coords, point, shape, ctx));
        }
    }
}
