using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions
{
    internal class EqualRounded : IBooleanExpression
    {
        public string Name => "equalRounded";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var mode = GetRoundingMode(qtiElement);
            if (!int.TryParse(qtiElement.GetAttributeValue("figures"), out var figures)) {
                context.LogError("for equalRounded figures is required");
                return false;
            }
            if (figures < 0)
            {
                context.LogError("figures cannot be smaller than zero");
                return false;
            }
            if (figures < 1 && mode == RoundingMode.SignificantFigures)
            {
                context.LogError("figures cannot be smaller than one for RoundingMode.SignificantFigures");
                return false;
            }
            var values = qtiElement.GetValues(context);// Helper.GetStringValueOfChildren(qtiElement, context).ToList();
            context.LogInformation($"equalRounded check. Values: {string.Join(", ", values.Select(v => v.Value).ToArray())}");
            if (values.Count != 2)
            {
                context.LogError($"unexpected values to compare: expected: 2, retrieved: {values.Count}");
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
                            context.LogError($"value cannot be casted to numeric value in equalRounded operator: {values[0]?.Value}, {values[1]?.Value}");
                        }
                        break;
                    }
                default:
                    {
                        context.LogError($"values other than float and int cannot be used in equalRounded operator.");
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

        RoundingMode GetRoundingMode(XElement qtiElement)
        {
            var roundingElement = qtiElement.GetAttributeValue("roundingMode");
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
