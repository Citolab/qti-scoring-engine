using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    public static class Helper
    {
        public static bool CompareTwoChildren(string value1, string value2, BaseType baseType)
        {
            {
                switch (baseType)
                {
                    case BaseType.Identifier: return value1 == value2;
                    case BaseType.String: return value1 == value2;
                    case BaseType.Int:
                        {
                            var castedValue1 = (int)Convert.ChangeType(value1, typeof(int));
                            var castedValue2 = (int)Convert.ChangeType(value1, typeof(int));
                            return castedValue1 == castedValue2;
                        }
                    case BaseType.Float:
                        {
                            var castedValue1 = (float)Convert.ChangeType(value1, typeof(float));
                            var castedValue2 = (float)Convert.ChangeType(value1, typeof(float));
                            return castedValue1 == castedValue2;
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

        public static bool CompareTwoValues(XElement qtiElement, ResponseProcessorContext context)
        {
            var values = qtiElement.GetValues(context);// Helper.GetStringValueOfChildren(qtiElement, context).ToList();
            context.LogInformation($"member check. Values: {string.Join(", ", values.Where(v => v?.Value !=null).Select(v => v.Value).ToArray())}");
            if (bool.TryParse(qtiElement.GetAttributeValue("caseSensitive"), out var result) && result == true)
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
            if (values[0]?.BaseType != values[1]?.BaseType)
            {
                context.LogWarning($"baseType response and outcome does not match: {values[0]?.BaseType.GetString()} and {values[1]?.BaseType.GetString()}. Proceeding with type: {values[1]?.BaseType.GetString()}");
            }
            var equals = CompareTwoChildren(values[0].Value, values[1].Value, values[1].BaseType);
            return equals;
        }
    }

}
