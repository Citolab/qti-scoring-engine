using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    /// <summary>
    /// qti-round-to: rounds to a number of decimal places or significant figures, set by the
    /// rounding-mode and figures attributes.
    /// </summary>
    internal class RoundTo : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-round-to should have exactly one child expression, found: {expressions.Count}");
                return null;
            }
            var figuresAttribute = GetAttributeValue("figures");
            if (!Helper.TryResolveInteger(figuresAttribute, ctx, out var figures))
            {
                ctx.LogError($"qti-round-to has a missing or invalid figures attribute: '{figuresAttribute}'");
                return null;
            }
            var roundingMode = GetAttributeValue("rounding-mode");
            if (string.IsNullOrEmpty(roundingMode))
            {
                roundingMode = "significantFigures"; // the default of the spec
            }
            var baseValue = expressions[0].Apply(ctx);
            if (!baseValue.TryGetNumber(out var value))
            {
                ctx.LogInformation($"qti-round-to: '{baseValue?.Value}' is not a number, returning null.");
                return null;
            }
            switch (roundingMode)
            {
                case "decimalPlaces":
                    {
                        if (figures < 0 || figures > 15)
                        {
                            ctx.LogError($"qti-round-to with rounding-mode decimalPlaces needs figures between 0 and 15, found: {figures}");
                            return null;
                        }
                        return Math.Round(value, figures, MidpointRounding.AwayFromZero).ToBaseValue();
                    }
                case "significantFigures":
                    {
                        if (figures < 1)
                        {
                            ctx.LogError($"qti-round-to with rounding-mode significantFigures needs figures of at least 1, found: {figures}");
                            return null;
                        }
                        return value.RoundToSignificantDigits(figures).ToBaseValue();
                    }
                default:
                    {
                        ctx.LogError($"Unsupported rounding-mode in qti-round-to: {roundingMode}");
                        return null;
                    }
            }
        }
    }
}
