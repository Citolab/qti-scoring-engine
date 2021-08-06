using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Interfaces;
using System.Drawing;
using Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression;
using Citolab.QTI.ScoringEngine.ResponseProcessing;

namespace Citolab.QTI.ScoringEngine.Helpers
{
    internal static class Helper
    {
        public static bool IsInsideRegion(string coordsValue, string pointResponse, Shape shapeType, IProcessingContext logContext)
        {
            if (string.IsNullOrWhiteSpace(pointResponse))
            {
                return false;
            }
            var splittedPoints = pointResponse.Split(' ');
            if (splittedPoints.Length != 2)
            {
                logContext.LogError("candidateResponse point should contain out of an x and y value separated by a space: '100 200'");
                return false;
            }
            IShape shape = null;

            switch (shapeType)
            {
                case Shape.Circle:
                    shape = new Circle(coordsValue, logContext);
                    break;
                case Shape.Poly:
                    shape = new Polygon(coordsValue, logContext);
                    break;
                case Shape.Rect:
                    shape = new Rect(coordsValue, logContext);
                    break;
                case Shape.Ellipse:
                    logContext.LogError("ellipse is deprecated, and scoring is not implemented");
                    break;
                case Shape.Default:
                    break;
                default:
                    logContext.LogError("unsupported shape");
                    break;
            }
            if (shapeType == Shape.Default)
            {
                var point = GetPointsFromResponse(pointResponse, logContext);
                return point.HasValue; // default are all points
            }
            if (shape == null)
            {
                return false;
            }
            else
            {
                return shape.IsInside(pointResponse);
            }
        }

        public static PointF? GetPointsFromResponse(string response, IProcessingContext contextLogger)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                return null;
            }
            var splittedPoints = response.Split(' ');
            if (splittedPoints.Length != 2)
            {
                contextLogger.LogError($"Unexpected format of x,y values in response: {response}");
                return null;
            }
            if (splittedPoints[0].TryParseFloat(out var x))
            {
                if (splittedPoints[1].TryParseFloat(out var y))
                {
                    return new PointF { X = x, Y = y };
                }
                else
                {
                    contextLogger.LogError($"Cannot convert {splittedPoints[1]} to y value of type float");
                }
            }
            else
            {
                contextLogger.LogError($"Cannot convert {splittedPoints[0]} to x value of type float");
            }
            return null;
        }


        internal static bool CompareSingleValues(string value1, string value2, BaseType baseType, IProcessingContext logContext)
        {
            switch (baseType)
            {
                case BaseType.Identifier: return value1 == value2;
                case BaseType.String: return value1 == value2;
                case BaseType.Int:
                    {
                        if (value1.TryParseInt(out var int1) && value2.TryParseInt(out var int2))
                        {
                            return int1 == int2;
                        }
                        else
                        {
                            logContext.LogError($"Cannot convert {value1} and/or {value2} to int.");
                        }
                        break;
                    }
                case BaseType.Float:
                    {
                        if (value1.TryParseFloat(out var float1) && value2.TryParseFloat(out var float2))
                        {
                            return float1 == float2;
                        }
                        else
                        {
                            logContext.LogError($"couldn't convert {value1} and/or {value2} to float.");
                        }
                        break;
                    }
                case BaseType.Pair:
                case BaseType.DirectedPair:
                    {
                        var pair1 = value1.Split(' ').ToList();
                        var pair2 = value2.Split(' ').ToList();

                        if (pair1.Count() == 2 && pair2.Count() == 2)
                        {
                            // sort values because order is not important
                            if (baseType == BaseType.Pair)
                            {
                                pair1.Sort();
                                pair2.Sort();
                            }
                            return string.Join(" ", pair1) == string.Join(" ", pair2);
                        }
                        else
                        {
                            logContext.LogWarning($"compared two pair but one of the values does not have 2 values: 1: {value1} 2: {value2}");
                        }
                        break;
                    }
            }
            return false;
        }

        public static OutcomeVariable GetOutcomeVariable(string id, OutcomeDeclaration outcomeDeclaration, IProcessingContext context, bool createIfNotExists = true)
        {
            if (string.IsNullOrEmpty(id))
            {
                context.LogError($"Cannot find attribute identifier");
                return null;
            };
            if (!context.OutcomeVariables.ContainsKey(id))
            {
                if (createIfNotExists)
                {
                    context.OutcomeVariables.Add(id, outcomeDeclaration.CreateVariable());
                }
                else
                {
                    context.LogError($"Outcome {id} couldn't be found.");
                }
            }
            return context.OutcomeVariables[id];
        }

        public static OutcomeDeclaration GetOutcomeDeclaration(string id, IProcessingContext ctx)
        {
            if (string.IsNullOrEmpty(id))
            {
                ctx.LogError($"Cannot find attribute identifier");
                return null;
            };
            if (!ctx.OutcomeDeclarations.ContainsKey(id))
            {
                ctx.LogError($"Cannot find outcomeDeclaration: {id}");
                return null;
            }
            return ctx.OutcomeDeclarations[id];
        }

        public static (BaseValue value1, BaseValue value2)? PrepareForCompare(List<IValueExpression> expressions, IProcessingContext ctx, Dictionary<string, string> attributes, BaseType? forceBaseType = null)
        {
            var values = expressions.Select(e => e.Apply(ctx)).ToList();
            ctx.LogInformation($"member check. Values: {string.Join(", ", values.Where(v => v?.Value != null).Select(v => v.Value).ToArray())}");
            if (attributes.ContainsKey("case-sensitive") && bool.TryParse(attributes["case-sensitive"], out var result) && result == false)
            {
                ctx.LogInformation($"member check not case-sensitive");
                values = values.Select(v =>
                {
                    v.Value = v.Value.ToLower();
                    return v;
                }).ToList();
            }
            if (values.Count != 2)
            {
                ctx.LogError($"unexpected values to compare: expected: 2, retrieved: {values.Count}");
                return null;
            }
            if (values[0] == null || values[1] == null)
            {
                return null; // return false if one of the values is null
            }
            if (forceBaseType != null)
            {
                values[0].BaseType = forceBaseType.Value;
                values[1].BaseType = forceBaseType.Value;
            }
            if (values[0].BaseType != values[1].BaseType)
            {
                ctx.LogWarning($"baseType response and outcome does not match: {values[0]?.BaseType.GetString()} and {values[1]?.BaseType.GetString()}. Proceeding with type: {values[1]?.BaseType.GetString()}");
            }
            return (values[0], values[1]);
        }

         public static bool ValueIsMemberOf(List<IValueExpression> expressions, IProcessingContext ctx, Dictionary<string, string> attributes, BaseType? forceBaseType = null)
        {
            var values = PrepareForCompare(expressions, ctx, attributes, forceBaseType);
            if (values == null)
            {
                return false;
            }
            else
            {
                var values2 = values.Value.value2;
                // values is not set correct. somehow
                var possibleMatches = values2.Values == null ?
                    new List<BaseValue> { values2 } :
                    values2.Values.Select(value =>
                    {
                        return new BaseValue
                        {
                            BaseType = values2.BaseType,
                            Identifier = values2.Identifier,
                            Value = value
                        };
                    }).ToList();
                foreach (var possibleMatch in possibleMatches)
                {
                    var isMatch = CompareSingleValues(values.Value.value1?.Value, possibleMatch.Value, possibleMatch.BaseType, ctx);
                    if (isMatch)
                    {
                        return true;
                    }
                };
                return false;
            }
        }


    

    }
}
