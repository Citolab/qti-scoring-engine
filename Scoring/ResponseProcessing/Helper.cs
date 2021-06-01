using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    internal static class Helper
    {
        public static bool CompareSingleValues(string value1, string value2, BaseType baseType, IContextLogger logContext)
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
                        } else
                        {
                            logContext.LogWarning($"compared two pair but one of the values does not have 2 values: 1: {value1} 2: {value2}" );
                        }
                        break;
                    }
            }
            return false;
        }

        public static OutcomeVariable GetOutcomeVariable(string id, OutcomeDeclaration outcomeDeclaration, ResponseProcessorContext context, bool createIfNotExists = true)
        {
            if (string.IsNullOrEmpty(id))
            {
                context.LogError($"Cannot find attribute identifier");
                return null;
            };
            if (!context.ItemResult.OutcomeVariables.ContainsKey(id))
            {
                if (createIfNotExists)
                {
                    context.ItemResult.OutcomeVariables.Add(id, outcomeDeclaration.CreateVariable());
                }
                else
                {
                    context.LogError($"Outcome {id} couldn't be found.");
                }
            }
            return context.ItemResult.OutcomeVariables[id];
        }

        public static OutcomeDeclaration GetOutcomeDeclaration(string id, ResponseProcessorContext context)
        {
            if (string.IsNullOrEmpty(id))
            {
                context.LogError($"Cannot find attribute identifier");
                return null;
            };
            if (!context.AssessmentItem.OutcomeDeclarations.ContainsKey(id))
            {
                context.LogError($"Cannot find outcomeDeclaration: {id}");
                return null;
            }
            return context.AssessmentItem.OutcomeDeclarations[id];
        }

        public static (BaseValue value1, BaseValue value2)? PrepareForCompare(XElement qtiElement, ResponseProcessorContext context, BaseType? forceBaseType = null)
        {
            var values = qtiElement.GetValues(context);
            context.LogInformation($"member check. Values: {string.Join(", ", values.Where(v => v?.Value != null).Select(v => v.Value).ToArray())}");
            if (bool.TryParse(qtiElement.GetAttributeValue("caseSensitive"), out var result) && result == false)
            {
                context.LogInformation($"member check not caseSensitive");
                values = values.Select(v =>
                {
                    v.Value = v.Value.ToLower();
                    return v;
                }).ToList();
            }
            if (values.Count != 2)
            {
                context.LogError($"unexpected values to compare: expected: 2, retrieved: {values.Count}");
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
                context.LogWarning($"baseType response and outcome does not match: {values[0]?.BaseType.GetString()} and {values[1]?.BaseType.GetString()}. Proceeding with type: {values[1]?.BaseType.GetString()}");
            }
            return (values[0], values[1]);
        }

        public static bool CompareTwoValues(XElement qtiElement, ResponseProcessorContext context, BaseType? forceBaseType = null)
        {
            var values = PrepareForCompare(qtiElement, context, forceBaseType);
            if (values == null)
            {
                return false;
            }
            else
            {
                return CompareSingleValues(values.Value.value1?.Value, values.Value.value2.Value, values.Value.value2.BaseType, context);
            }
        }

        public static bool CompareTwoValues(BaseValue value1, BaseValue value2, Cardinality cardinality, ResponseProcessorContext context)
        {
            return CompareSingleValues(value1?.Value, value2.Value, value2.BaseType, context);
        }

        public static bool ValueIsMemberOf(XElement qtiElement, ResponseProcessorContext context, BaseType? forceBaseType = null)
        {
            var values = PrepareForCompare(qtiElement, context, forceBaseType);
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
                    var isMatch = CompareSingleValues(values.Value.value1?.Value, possibleMatch.Value, possibleMatch.BaseType, context);
                    if (isMatch)
                    {
                        return true;
                    }
                };
                return false;
            }
        }

        internal static void GetCustomOperators(XElement qtiElement, List<ICustomOperator> customOperators, ResponseProcessorContext context)
        {
            if (qtiElement != null && qtiElement.Parent != null &&
                qtiElement.Parent.Name.LocalName.Equals("customOperator", StringComparison.InvariantCultureIgnoreCase))
            {
                var definition = qtiElement.Parent.GetAttributeValue("definition");
                var customOperator = context.GetCustomOperator(qtiElement.Parent, context);
                if (customOperator != null)
                {
                    customOperators.Insert(0, context.CustomOperators[definition]);
                    GetCustomOperators(qtiElement.Parent, customOperators, context);
                }
            }
        }
    }
}
