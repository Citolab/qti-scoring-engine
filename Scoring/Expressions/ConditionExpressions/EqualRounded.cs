using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class EqualRounded : ConditionExpressionBase
    {

        public override bool Execute(IProcessingContext ctx)
        {
            var mode = GetRoundingMode();
            var fg = attributes.ContainsKey("figures") ? attributes["figures"] : "";
            if (!int.TryParse(fg, out var figures)) {
                ctx.LogError("for qti-equal-rounded figures is required");
                return false;
            }
            if (figures < 0)
            {
                ctx.LogError("figures cannot be smaller than zero");
                return false;
            }
            if (figures < 1 && mode == RoundingMode.SignificantFigures)
            {
                ctx.LogError("figures cannot be smaller than one for RoundingMode.SignificantFigures");
                return false;
            }
            var values =  expressions.Select(e => e.Apply(ctx)).ToList();
            ctx.LogInformation($"qti-equal-rounded check. Values: {string.Join(", ", values.Select(v => v.Value).ToArray())}");
            if (values.Count() != 2)
            {
                ctx.LogError($"unexpected values to compare: expected: 2, retrieved: {values.Count}");
                return false;
            }
            switch (values[0].BaseType)
            {
                case BaseType.Int:
                case BaseType.Float:
                    {
                        if (values[0].Value.TryParseFloat(out var value1) && values[1].Value.TryParseFloat(out var value2))
                        {
                            if (mode == RoundingMode.SignificantFigures)
                            {
                                return ((double)value1).RoundToSignificantDigits(figures) ==
                                    ((double)value2).RoundToSignificantDigits(figures);
                            } else
                            {
                                return Math.Round(value1, figures, MidpointRounding.AwayFromZero) ==
                                   Math.Round(value2, figures, MidpointRounding.AwayFromZero);
                            }
                        }
                        else
                        {
                            ctx.LogError($"value cannot be casted to numeric value in equalRounded operator: {values[0]?.Value}, {values[1]?.Value}");
                        }
                        break;
                    }
                default:
                    {
                        ctx.LogError($"values other than float and int cannot be used in equalRounded operator.");
                        break;
                    }
            }
            return false;
        }

        //        Attribute : roundingMode[1]: roundingMode = significantFigures
        //       Numbers are rounded to a given number of significantFigures or decimalPlaces.
        //       Attribute : figures[1]: integerOrVariableRef
        //       The number of figures to round to. If roundingMode = "significantFigures", the value of figures must be a non-zero positive integer.If roundingMode = "decimalPlaces", the value of figures must be an integer greater than or equal to zero.
        //For example, if significantFigures mode is used with figures= 3, and the values are 3.175 and 3.183, the result is true, but for 3.175 and 3.1749, the result is false; if decimalPlaces mode is used with figures=2, 1.68572 and 1.69 the result is true, but for 1.68572 and 1.68432, the result is false.
        //Enumeration: roundingMode
        //significantFigures
        //decimalPlaceslPlaces
        enum RoundingMode
        {
            SignificantFigures,
            DecimalPlaces
        }

        RoundingMode GetRoundingMode()
        {
            var roundingElement = attributes.ContainsKey("rounding-mode") ? attributes["rounding-mode"] : "";
            if (roundingElement == "decimalPlaces")
            {
                return RoundingMode.DecimalPlaces;
            }
            return RoundingMode.SignificantFigures;
        }

        //double RoundToSignificantDigits(float d, int digits)
        //{
        //    if (d == 0)
        //        return 0;

        //    double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
        //    return scale * Math.Round(d / scale, digits);
        //}




    }
}
