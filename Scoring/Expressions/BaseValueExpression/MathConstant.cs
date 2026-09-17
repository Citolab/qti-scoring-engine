using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-math-constant: pi or e.
    /// </summary>
    internal class MathConstant : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var name = GetAttributeValue("name");
            switch (name)
            {
                case "pi": return Math.PI.ToBaseValue();
                case "e": return Math.E.ToBaseValue();
                default:
                    {
                        ctx.LogError($"Unsupported name in qti-math-constant: '{name}'");
                        return null;
                    }
            }
        }
    }
}
