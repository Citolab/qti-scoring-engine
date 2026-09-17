using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-contains: whether the container of the first child contains the one of the second.
    /// For an ordered container that means the values in the same order, for a multiple one it
    /// means every value is found, each matching a different member.
    /// </summary>
    internal class Contains : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            if (expressions.Count != 2)
            {
                ctx.LogError($"qti-contains should have two child expressions, found: {expressions.Count}");
                return false;
            }
            var container = expressions[0].Apply(ctx);
            var lookFor = expressions[1].Apply(ctx);
            if (container == null || lookFor == null)
            {
                return false;
            }
            var values = container.ToValueList();
            var valuesToFind = lookFor.ToValueList();
            if (valuesToFind.Count == 0)
            {
                return false;
            }
            var baseType = lookFor.BaseType;
            if (container.Cardinality == Cardinality.Ordered && valuesToFind.Count > 1)
            {
                for (var start = 0; start + valuesToFind.Count <= values.Count; start++)
                {
                    var matchesFromHere = true;
                    for (var i = 0; i < valuesToFind.Count; i++)
                    {
                        if (!Helper.CompareSingleValues(values[start + i], valuesToFind[i], baseType, ctx))
                        {
                            matchesFromHere = false;
                            break;
                        }
                    }
                    if (matchesFromHere)
                    {
                        return true;
                    }
                }
                return false;
            }
            var remaining = values.ToList();
            foreach (var valueToFind in valuesToFind)
            {
                var index = remaining.FindIndex(value => Helper.CompareSingleValues(value, valueToFind, baseType, ctx));
                if (index == -1)
                {
                    return false;
                }
                remaining.RemoveAt(index);
            }
            return true;
        }
    }
}
