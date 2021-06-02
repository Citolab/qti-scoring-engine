using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.OutcomeProcessing;
using Citolab.QTI.ScoringEngine.Interfaces;
using System.Globalization;

namespace Citolab.QTI.ScoringEngine.Helper
{

    internal static class Extensions
    {
        internal static IEnumerable<XElement> FindElementsByName(this XDocument doc, string name)
        {
            return doc.Descendants().Where(d => d.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        internal static IEnumerable<XElement> FindElementsByLastPartOfName(this XDocument doc, string name)
        {
            return doc.Descendants().Where(d => d.Name.LocalName.EndsWith(name, StringComparison.OrdinalIgnoreCase));
        }
        internal static IEnumerable<XElement> FindElementsByLastPartOfName(this XElement el, string name)
        {
            return el.Descendants().Where(d => d.Name.LocalName.EndsWith(name, StringComparison.OrdinalIgnoreCase));
        }
        internal static IEnumerable<XElement> FindElementsByName(this XElement el, string name)
        {
            return el.Descendants().Where(d => d.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        internal static XElement FindElementByName(this XDocument doc, string name)
        {
            return doc.FindElementsByName(name).FirstOrDefault();
        }

        internal static OutcomeVariable CreateVariable(this OutcomeDeclaration outcomeDeclaration)
        {
            return new OutcomeVariable
            {
                Identifier = outcomeDeclaration.Identifier,
                BaseType = outcomeDeclaration.BaseType,
                Cardinality = outcomeDeclaration.Cardinality,
                Value = outcomeDeclaration.DefaultValue
            };
        }

        internal static string Identifier(this XElement element) =>
            element.GetAttributeValue("identifier");

        private static BaseValue GetBaseValue(this XElement qtiElement)
        {
            if (qtiElement.Name.LocalName == "baseValue")
            {
                return new BaseValue()
                {
                    BaseType = qtiElement.GetAttributeValue("baseType").ToBaseType(),
                    Value = qtiElement.Value.RemoveXData(),
                    Identifier = qtiElement.Identifier()
                };
            }
            return null;
        }

        private static IList<BaseValue> GetBaseValues(this XElement qtiElement)
        {
            var baseValues = qtiElement.FindElementsByName("baseValue")
                .Select(childElement =>
                {
                    return new BaseValue()
                    {
                        BaseType = childElement.GetAttributeValue("baseType").ToBaseType(),
                        Value = childElement.Value.RemoveXData(),
                        Identifier = childElement.Identifier()
                    };
                }).ToList();
            return baseValues;
        }

        // TODO: Remove should use GetOutcomeVariable;
        private static IList<BaseValue> GetOutcomeVariables(this XElement qtiElement, Dictionary<string, OutcomeVariable> outcomeVariables, AssessmentItem assessmentItem)
        {
            var variables = qtiElement.FindElementsByName("variable")
              .Select(childElement =>
              {
                  var identifier = childElement.Identifier();
                  if (outcomeVariables != null && outcomeVariables.ContainsKey(identifier))
                  {
                      return outcomeVariables[identifier].ToBaseValue();
                  }
                  else
                  {
                      if (assessmentItem != null && assessmentItem.OutcomeDeclarations.ContainsKey(identifier))
                      {
                          return assessmentItem.OutcomeDeclarations[identifier].ToVariable().ToBaseValue();
                      }
                      return null;
                  }
              })
              .Where(v => v != null)
              .ToList();
            return variables;
        }

        internal static string RemoveXData(this string value)
        {
            if (value.Contains("<![CDATA["))
            {
                return value.Replace("<![CDATA[", "").Replace("]]>", "");
            }
            return value;
        }

        private static BaseValue ToBaseValue(this OutcomeVariable outcomeVariable)
        {
            return new BaseValue { BaseType = outcomeVariable.BaseType, Value = outcomeVariable.Value.ToString(), Identifier = outcomeVariable.Identifier };
        }

        private static (BaseValue BaseValue, Cardinality Cardinality)? GetCorrect(this XElement qtiElement, ResponseProcessorContext context)
        {
            if (qtiElement.Name.LocalName == "correct")
            {
                var identifier = qtiElement.Identifier();
                if (context.AssessmentItem.ResponseDeclarations.ContainsKey(identifier))
                {

                    var dec = context.AssessmentItem.ResponseDeclarations[identifier];
                    if (dec.Cardinality == Cardinality.Single && string.IsNullOrWhiteSpace(dec.CorrectResponse))
                    {
                        context.LogError($"Correct: {identifier} references to a response without correctResponse");
                        return null;
                    }
                    if (dec.Cardinality != Cardinality.Single && (dec.CorrectResponses == null || !dec.CorrectResponses.Any()))
                    {
                        context.LogError($"Correct: {identifier} references to a response without correctResponse");
                        return null;
                    }
                    return (new BaseValue { Identifier = identifier, BaseType = dec.BaseType, Value = dec.CorrectResponse, Values = dec.CorrectResponses }, dec.Cardinality);
                }
                else
                {
                    context.LogError($"Cannot reference to response declaration for correct {identifier}");
                }
            }
            else if (qtiElement.Name.LocalName == "baseValue")
            {
                return (qtiElement.GetBaseValue(), Cardinality.Single);
            }
            else if (qtiElement.Name.LocalName == "ordered")
            {
                var values = qtiElement.Elements().Select(el => el.GetBaseValue()).ToList();
                var firstValue = values.FirstOrDefault();
                if (firstValue != null)
                {
                    return (new BaseValue { Identifier = qtiElement.Identifier(), BaseType = firstValue.BaseType, Value = null, Values = values.Select(v => v.Value).ToList() }, Cardinality.Ordered);
                }
            }
            return null;
        }

        private static BaseValue GetVariable(this XElement qtiElement, ResponseProcessorContext context)
        {
            if (qtiElement.Name.LocalName == "variable")
            {
                var identifier = qtiElement.Identifier();
                if (context.ItemResult?.ResponseVariables != null && context.ItemResult.ResponseVariables.ContainsKey(identifier))
                {
                    var responseVariable = context.ItemResult.ResponseVariables[identifier];
                    var cardinality = Cardinality.Single;
                    if (context.AssessmentItem.ResponseDeclarations.ContainsKey(identifier))
                    {
                        cardinality = context.AssessmentItem.ResponseDeclarations[identifier].Cardinality;
                    }
                    return new BaseValue
                    {
                        BaseType = responseVariable.BaseType,
                        Value = cardinality == Cardinality.Single ? responseVariable.Value : "",
                        Values = cardinality != Cardinality.Single ? responseVariable.Values : null,
                        Identifier = identifier
                    };
                }
                else
                {
                    var outcomeVariables = context.ItemResult?.OutcomeVariables;
                    if (outcomeVariables != null && outcomeVariables.ContainsKey(identifier))
                    {
                        return outcomeVariables[identifier].ToBaseValue();
                    }
                }
            }
            return null;
        }



        internal static IList<BaseValue> GetValues(this XElement qtiElement, OutcomeProcessorContext context)
        {
            var itemOutcomes = context.AssessmentResult.ItemResults
               .Select(i => i.Value)
               .SelectMany(i =>
               {
                   return i.OutcomeVariables.Select(o =>
                   {
                       // find the variable to apply weight
                       var weightedValue = o.Value.Value;
                       var itemVariableElement = qtiElement.FindElementsByElementAndAttributeValue("variable", "identifier", $"{i.Identifier}.{o.Key}").FirstOrDefault();
                       if (itemVariableElement != null)
                       {
                           var weightIdentifier = itemVariableElement.GetAttributeValue("weightIdentifier");
                           if (!string.IsNullOrEmpty(weightIdentifier))
                           {
                               var itemRef = context.AssessmentTest.AssessmentItemRefs[i.Identifier];
                               if (itemRef.Weights.ContainsKey(weightIdentifier))
                               {
                                   if (o.Value.Value.ToString().TryParseFloat(out var floatValue))
                                   {
                                       weightedValue = floatValue * itemRef.Weights[weightIdentifier];
                                   }
                               }
                               else
                               {
                                   context.LogError($"cannot find weight identifier: {weightIdentifier} for item: {i.Identifier}");
                               }
                           }
                       }
                       var outcome = new OutcomeVariable
                       {
                           BaseType = o.Value.BaseType,
                           Cardinality = o.Value.Cardinality,
                           Value = weightedValue,
                           Identifier = o.Value.Identifier
                       };
                       return new
                       {
                           Name = i.Identifier,
                           Identifier = o.Key,
                           Variable = outcome
                       };
                   });
               })
               .ToDictionary(o => $"{o.Name}.{o.Identifier}", o => o.Variable);

            var testOutcomes = context.AssessmentResult.TestResults
                .Select(i => i.Value)
                .SelectMany(i => i.OutcomeVariables.Select(o => o.Value));

            var allOutcomes = itemOutcomes.Concat(testOutcomes
                    .ToDictionary(t => t.Identifier, t => t))
                    .ToDictionary(x => x.Key, x => x.Value);

            var baseValues = qtiElement.GetBaseValues();
            var variables = qtiElement.GetOutcomeVariables(allOutcomes, null);

            var testVariables = qtiElement.FindElementsByName("testVariables")
                .Select(testVariableElement =>
                {
                    var qtiValues = context.AssessmentTest.AssessmentItemRefs.Values.Where(assessmentItemRef =>
                {
                    var excludedCategoriesString = testVariableElement.GetAttributeValue("excludeCategory");
                    var excludedCategories = !string.IsNullOrWhiteSpace(excludedCategoriesString) ?
                        excludedCategoriesString.Split(' ') : null;

                    var includeCategoriesString = testVariableElement.GetAttributeValue("includeCategory");
                    var includeCategories = !string.IsNullOrWhiteSpace(includeCategoriesString) ?
                    includeCategoriesString.Split(' ') : null;
                    if (excludedCategories?.Length > 0)
                    {
                        foreach (var excludedCategory in excludedCategories)
                        {
                            if (assessmentItemRef.Categories.Contains(excludedCategory))
                            {
                                return false;
                            }
                        }
                    }
                    if (includeCategories?.Length > 0)
                    {
                        foreach (var includeCategory in includeCategories)
                        {
                            if (assessmentItemRef.Categories.Contains(includeCategory))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    return true; // if not included or excluded categories are defined return all
                }).Select(assesmentItemRef =>
                {
                    var weightIdentifier = testVariableElement.GetAttributeValue("weightIdentifier");
                    var variableIdentifier = testVariableElement.GetAttributeValue("variableIdentifier");
                    var itemRefIdentifier = $"{assesmentItemRef.Identifier}.{variableIdentifier}";
                    if (allOutcomes.ContainsKey(itemRefIdentifier))
                    {
                        var outcome = allOutcomes[itemRefIdentifier];
                        if (!string.IsNullOrWhiteSpace(weightIdentifier))
                        {
                            if (assesmentItemRef.Weights != null && assesmentItemRef.Weights.ContainsKey(weightIdentifier))
                            {
                                if (outcome.Value.ToString().TryParseFloat(out var floatValue))
                                {
                                    return new OutcomeVariable
                                    {
                                        BaseType = outcome.BaseType,
                                        Cardinality = outcome.Cardinality,
                                        Identifier = outcome.Identifier,
                                        Value = floatValue * assesmentItemRef.Weights[weightIdentifier]
                                    };
                                }
                            }
                            else
                            {
                                context.LogWarning($"Cannot find weight with identifier: {weightIdentifier} from item: {itemRefIdentifier}");

                            }
                        }
                        return outcome;
                    }
                    else
                    {
                        context.LogError($"Cannot find assessmentItemRef outcomeVariable: {itemRefIdentifier}");
                        return null;
                    }
                    // return GetItemReferenceVariables(assesmentItemRef.Identifier, variableIdentifier, "testVariable", context);
                }).Where(v => v != null);
                    return new BaseValue
                    {
                        Identifier = "testIdentifier",
                        BaseType = BaseType.Float,
                        Value = qtiValues
                        .Where(v => v.Value.ToString().TryParseFloat(out var s))
                        .Sum(v => v.Value.ToString().ParseFloat(null) * 1).ToString()
                    };
                });
            return baseValues
                .Concat(variables)
                .Concat(testVariables)
                .ToList();
        }

        internal static BaseValue GetVariableOrBaseValue(this XElement qtiElement, ResponseProcessorContext context)
        {
            if (qtiElement.Name.LocalName == "baseValue" ||
                qtiElement.Name.LocalName == "variable" ||
                qtiElement.Name.LocalName == "customOperator")
            {
                var element = qtiElement.Name.LocalName != "customOperator" ?
                    qtiElement : qtiElement.Descendants().FirstOrDefault(el =>
                    el.Name.LocalName == "variable" ||
                    el.Name.LocalName == "baseValue");
                var customOperators = new List<ICustomOperator>();
                ResponseProcessing.Helper.GetCustomOperators(element, customOperators, context);
                var newValue = element.Name.LocalName == "variable" ? element.GetVariable(context) : element.GetBaseValue();
                if (customOperators.Any())
                {
                    foreach (var customOperator in customOperators)
                    {
                        newValue = customOperator.Apply(newValue);
                    }
                }
                return newValue;
            }
            return null;
        }

        internal static (BaseValue BaseValue, Cardinality Cardinality)? GetCorrectValue(this XElement qtiElement, ResponseProcessorContext context)
        {
            if (
                qtiElement.Name.LocalName == "baseValue" ||
                qtiElement.Name.LocalName == "ordered" ||
                qtiElement.Name.LocalName == "correct" ||
                qtiElement.Name.LocalName == "customOperator")
            {
                var element = qtiElement.Name.LocalName != "customOperator" ?
                    qtiElement : qtiElement.Descendants().FirstOrDefault(el =>
                    el.Name.LocalName == "correct" || el.Name.LocalName == "baseValue" ||
                    el.Name.LocalName == "ordered");
                var customOperators = new List<ICustomOperator>();
                ResponseProcessing.Helper.GetCustomOperators(element, customOperators, context);

                var newValueInfo = element.GetCorrect(context);
                if (newValueInfo.HasValue)
                {
                    var newValue = newValueInfo.Value.BaseValue;
                    if (customOperators.Any())
                    {
                        foreach (var customOperator in customOperators)
                        {
                            newValue = customOperator.Apply(newValue);
                        }
                    }
                    var responseDeclaration = context.AssessmentItem.ResponseDeclarations.ContainsKey(newValue?.Identifier) ?
                        context.AssessmentItem.ResponseDeclarations[newValue.Identifier] : null;
                    return (newValue, newValueInfo.Value.Cardinality);
                }
            }
            return null;
        }


        internal static IList<BaseValue> GetValues(this XElement qtiElement, ResponseProcessorContext context)
        {
            return qtiElement.Descendants().Where(element =>
             {
                 return element.Name.LocalName == "baseValue" ||
                  element.Name.LocalName == "variable" ||
                  element.Name.LocalName == "correct";
             }).Select(element =>
             {
                 if (element.Name.LocalName == "correct")
                 {
                     var s = element.GetCorrectValue(context);
                     if (s.HasValue)
                     {
                         return s.Value.BaseValue;
                     }
                     else
                     {
                         context.LogError("Something went wrong, while getting correct response");
                         return null;
                     }
                 }
                 else
                 {
                     return element.GetVariableOrBaseValue(context);
                 }
             }).Where(v => v != null).ToList();
        }

        internal static double RoundToSignificantDigits(this double d, int digits)
        {
            if (d == 0.0 || Double.IsNaN(d) || Double.IsInfinity(d))
            {
                return d;
            }
            decimal scale = (decimal)Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return (double)(scale * Math.Round((decimal)d / scale, digits, MidpointRounding.AwayFromZero));
        }

        internal static bool TryParseFloat(this string value, out Single result)
        {
            var style = NumberStyles.Float;
            var culture = CultureInfo.InvariantCulture;
            if (float.TryParse(value, style, culture, out var floatValue))
            {
                result = floatValue;
                return true;
            }
            else
            {
                result = default(float);
                return false;
            }
        }


        internal static float ParseFloat(this string value, ILogger logger)
        {
            var style = NumberStyles.AllowDecimalPoint;
            var culture = CultureInfo.CurrentCulture;
            if (float.TryParse(value, style, culture, out var floatValue))
            {
                return floatValue;
            }
            logger.LogError($"value: {value} could not be parsed to float");
            return default;
        }

        internal static bool TryParseInt(this string value, out Single result)
        {
            var style = NumberStyles.AllowDecimalPoint;
            var culture = CultureInfo.CurrentCulture;
            if (int.TryParse(value, style, culture, out var intValue))
            {
                result = intValue;
                return true;
            }
            else
            {
                result = 0;
                return false;
            }
        }

        internal static string GetAttributeValue(this XElement el, string name)
        {
            return el.GetAttribute(name)?.Value ?? string.Empty;
        }
        internal static XAttribute GetAttribute(this XElement el, string name)
        {
            return el.Attributes()
                .FirstOrDefault(a => a.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        internal static IEnumerable<XAttribute> GetAttributes(this XDocument doc, string name)
        {
            var s = doc.Descendants().SelectMany(d => d.Attributes()
                .Where(a => a.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase)));
            return s;
        }

        internal static IEnumerable<XElement> FindElementsByElementAndAttributeValue(this XElement element, string elementName, string attributeName, string attributeValue)
        {
            return element.FindElementsByName(elementName)
                .Where(d => d.Attributes()
                    .Any(a => a.Name.LocalName.Equals(attributeName, StringComparison.OrdinalIgnoreCase) &&
                              a.Value.Equals(attributeValue, StringComparison.OrdinalIgnoreCase)));
        }

        internal static IEnumerable<XElement> FindElementsByElementAndAttributeValue(this XDocument doc, string elementName, string attributeName, string attributeValue)
        {
            return doc.FindElementsByName(elementName)
                .Where(element => element.Attributes()
                    .Any(a => a.Name.LocalName.Equals(attributeName, StringComparison.OrdinalIgnoreCase) &&
                              a.Value.Equals(attributeValue, StringComparison.OrdinalIgnoreCase)));
        }

        internal static IEnumerable<XElement> FindElementsByElementAndAttributeThatContainsValue(this XDocument doc, string elementName, string attributeName, string attributeValue)
        {
            return doc.FindElementsByName(elementName)
                .Where(element => element.Attributes()
                    .Any(a => a.Name.LocalName.Equals(attributeName, StringComparison.OrdinalIgnoreCase) &&
                              a.Value.ToLower().Contains(attributeValue.ToLower())));
        }

        internal static IEnumerable<XElement> FindElementsByElementAndAttributeStartValue(this XDocument doc, string elementName, string attributeName, string attributeValue)
        {
            return doc.FindElementsByName(elementName)
                .Where(element => element.Attributes()
                    .Any(a => a.Name.LocalName.Equals(attributeName, StringComparison.OrdinalIgnoreCase) &&
                              a.Value.ToLower().StartsWith(attributeValue.ToLower())));
        }

        internal static string GetElementValue(this XElement el, string name)
        {
            return el.FindElementsByName(name).FirstOrDefault()?.Value ?? String.Empty;
        }
        //xmlns

        internal static IEnumerable<XElement> GetInteractions(this XDocument doc)
        {
            return doc.Document?.Root.GetInteractions();
        }

        internal static IEnumerable<XElement> GetInteractions(this XElement el)
        {
            var qti2Elements = el.FindElementsByLastPartOfName("interaction")
                .Where(d => d.Name.LocalName.ToLower().Contains("audio"))
                .Where(d => d.Attributes()
                    .Any(a => a.Name.LocalName.Equals("responseIdentifier", StringComparison.OrdinalIgnoreCase) &&
                              a.Value.Equals("RESPONSE", StringComparison.OrdinalIgnoreCase)));
            var qti3Elements = el.FindElementsByLastPartOfName("Interaction")
                .Where(d => d.Name.LocalName.ToLower().Contains("audio"))
                .Where(d => d.Attributes()
                    .Any(a => a.Name.LocalName.Equals("response-identifier", StringComparison.OrdinalIgnoreCase) &&
                              a.Value.Equals("RESPONSE", StringComparison.OrdinalIgnoreCase)));
            return qti2Elements.Concat(qti3Elements);
        }
        internal static XElement GetInteraction(this XElement element)
        {
            return element.GetInteractions().FirstOrDefault();
        }
        internal static XElement GetInteraction(this XDocument doc)
        {
            return doc.GetInteractions().FirstOrDefault();
        }
        internal static void SetAttributeValue(this XElement el, string name, string value)
        {
            el.GetAttribute(name)?.SetValue(value);
        }

        internal static XElement ToXElement(this BaseValue value)
        {
            return XElement.Parse($"<baseValue baseType=\"{value.BaseType.GetString()}\">{value.Value}</baseValue>");
        }

        internal static OutcomeVariable ToVariable(this OutcomeDeclaration outcomeDeclaration)
        {
            return new OutcomeVariable
            {
                BaseType = outcomeDeclaration.BaseType,
                Cardinality = outcomeDeclaration.Cardinality,
                Identifier = outcomeDeclaration.Identifier,
                Value = outcomeDeclaration.DefaultValue
            };
        }

        internal static XElement ToVariableElement(this OutcomeDeclaration outcomeDeclaration)
        {
            return XElement.Parse($"<variable identifier=\"{outcomeDeclaration.Identifier}\" />");
        }

        internal static XElement ToElement(this OutcomeDeclaration outcomeDeclaration)
        {
            return XElement.Parse($"<outcomeDeclaration " +
                $"identifier=\"{outcomeDeclaration.Identifier}\" cardinality=\"{outcomeDeclaration.Cardinality.GetString()}\" " +
                $"baseType=\"{outcomeDeclaration.BaseType.GetString()}\"><defaultValue><value>{outcomeDeclaration.DefaultValue}</value></defaultValue></outcomeDeclaration>");
        }

        internal static XElement ToValueElement(this string value)
        {
            return XElement.Parse($"<value>{value}</value>");
        }

        internal static HashSet<T> ToHashSet<T>(
       this IEnumerable<T> source,
       IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }

        internal static XElement AddDefaultNamespace(this XElement element, XNamespace xnamespace)
        {
            element.Name = xnamespace + element.Name.LocalName;
            foreach (var child in element.Descendants())
            {
                child.Name = xnamespace + child.Name.LocalName;
            }
            return element;
        }
        internal static OutcomeDeclaration ToOutcomeDeclaration(this float value, string identifier = "SCORE")
        {
            return new OutcomeDeclaration
            {
                Identifier = identifier,
                BaseType = BaseType.Float,
                Cardinality = Cardinality.Single,
                DefaultValue = value
            };
        }


        /// <summary>
        /// This adds total and weighted score for all summed items +
        /// total and weighted score for all categories.
        /// </summary>
        /// <param name="assessmentTest"></param>
        /// <returns></returns>
        public static XDocument AddTotalAndCategoryScores(this XDocument assessmentTest)
        {
            var changedTest = assessmentTest
                .AddTestOutcome("SCORE_TOTAL", "", null)
                .AddTestOutcome("SCORE_TOTAL_WEIGHTED", "WEIGHT", null)
                .AddTestOutcomeForCategories("SCORE_TOTAL", "")
                .AddTestOutcomeForCategories("SCORE_TOTAL_WEIGHTED", "WEIGHT");
            return changedTest;
        }

        internal static XDocument AddTestOutcomeForCategories(this XDocument assessmentTest, string identifierPrefix, string weightIdentifier)
        {
            var categories = assessmentTest.FindElementsByName("assessmentItemRef")
               .SelectMany(assessmentItemRefElement => assessmentItemRefElement.GetAttributeValue("category").Split(' '))
               .Distinct()
               .ToList();
            categories.ForEach(c =>
            {
                assessmentTest = assessmentTest.AddTestOutcome($"{identifierPrefix}_{c}", weightIdentifier, new List<string> { c });
            });
            return assessmentTest;
        }

        internal static XDocument AddTestOutcome(this XDocument assessmentTest, string identifier, string weightIdentifier, List<string> includedCategories)
        {
            var outcomeProcessing = assessmentTest.FindElementByName("outcomeProcessing");
            if (outcomeProcessing == null || !outcomeProcessing.FindElementsByElementAndAttributeValue("setOutcomeValue", "identifier", identifier).Any())
            {
                var testVariable = new SumTestVariable
                {
                    Identifier = identifier,
                    WeightIdentifier = weightIdentifier,
                    IncludedCategories = includedCategories
                };
                var outcomeElement = testVariable.OutcomeElement();
                var testVariableElement = testVariable.ToSummedSetOutcomeElement();

                assessmentTest.Root.Add(outcomeElement.AddDefaultNamespace(assessmentTest.Root.GetDefaultNamespace()));

                if (outcomeProcessing == null)
                {
                    assessmentTest.Add(XElement.Parse("<outcomeProcessing></outcomeProcessing>"));
                    outcomeProcessing = assessmentTest.FindElementByName("outcomeProcessing");
                }
                outcomeProcessing.Add(testVariableElement.AddDefaultNamespace(assessmentTest.Root.GetDefaultNamespace()));
                return assessmentTest;
            }
            else
            {
                // variable already exist
                return assessmentTest;
            }

        }
    }
}