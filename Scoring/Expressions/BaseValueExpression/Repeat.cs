using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System.Collections.Generic;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-repeat: evaluates its children number-repeats times into one ordered container.
    /// NULL children are left out; a number-repeats below 1 makes the whole expression NULL.
    /// </summary>
    internal class Repeat : ValueExpressionBase
    {
        // the same guard the response rules use: a number-repeats read from a variable should
        // not be able to build an endless container.
        private const int MaxRepeats = 1000;

        public override BaseValue Apply(IProcessingContext ctx)
        {
            var numberRepeats = GetAttributeValue("number-repeats");
            if (!Helper.TryResolveInteger(numberRepeats, ctx, out var repeats))
            {
                ctx.LogError($"qti-repeat has a missing or invalid number-repeats attribute: '{numberRepeats}'");
                return null;
            }
            if (repeats < 1)
            {
                ctx.LogInformation($"qti-repeat with number-repeats: {repeats}, returning null.");
                return null;
            }
            if (repeats > MaxRepeats)
            {
                ctx.LogWarning($"qti-repeat number-repeats: {repeats} is capped at {MaxRepeats}.");
                repeats = MaxRepeats;
            }
            var values = new List<string>();
            BaseType? baseType = null;
            for (var i = 0; i < repeats; i++)
            {
                foreach (var expression in expressions)
                {
                    var baseValue = expression.Apply(ctx);
                    if (baseValue == null)
                    {
                        continue;
                    }
                    if (baseType == null)
                    {
                        baseType = baseValue.BaseType;
                    }
                    values.AddRange(baseValue.ToValueList());
                }
            }
            return new BaseValue
            {
                Identifier = "repeat",
                BaseType = baseType ?? BaseType.Identifier,
                Cardinality = Cardinality.Ordered,
                Values = values
            };
        }
    }
}
