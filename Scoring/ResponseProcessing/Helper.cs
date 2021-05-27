using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Model;
using Microsoft.Extensions.Logging;
using Citolab.QTI.Scoring.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing
{
    internal static class Helper
    {
        public static bool CompareTwoChildren(string value1, string value2, BaseType baseType, IContextLogger logContext)
        {
            {
                switch (baseType)
                {
                    case BaseType.Identifier: return value1 == value2;
                    case BaseType.String: return value1 == value2;
                    case BaseType.Int:
                        {
                            if (int.TryParse(value1, out var int1) && int.TryParse(value2, out var int2))
                            {
                                return int1 == int2;
                            } else
                            {
                                logContext.LogError($"Cannot convert {value1} and/or {value2} to int.");
                            }
                            break;
                        }
                    case BaseType.Float:
                        {
                            if (float.TryParse(value1, out var float1) && float.TryParse(value2, out var float2))
                            {
                                return float1 == float2;
                            }
                            else
                            {
                                logContext.LogError($"Cannot convert {value1} and/or {value2} to float.");
                            }
                            break;
                        }
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
                } else
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

        public static bool CompareTwoValues(XElement qtiElement, ResponseProcessorContext context, BaseType? forceBaseType = null)
        {
            var values = qtiElement.GetValues(context);
            context.LogInformation($"member check. Values: {string.Join(", ", values.Where(v => v?.Value !=null).Select(v => v.Value).ToArray())}");
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
                return false;
            }
            if (values[0] ==null || values[1] ==null)
            {
                return false; // return false if one of the values is null
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
            var equals = CompareTwoChildren(values[0].Value, values[1].Value, values[1].BaseType, context);
            return equals;
        }
    }

}
