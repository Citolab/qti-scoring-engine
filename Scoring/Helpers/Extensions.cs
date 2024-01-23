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
using System.Text;
using static System.Char;
using Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression;

namespace Citolab.QTI.ScoringEngine.Helpers
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


        internal static XElement FindParentElement(this XElement el, string tagName)
        {
            if (el.Parent != null)
            {
                if (el.Parent.Name.LocalName.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return el.Parent;
                }
                else
                {
                    return el.Parent.FindParentElement(tagName);
                }
            }
            else
            {
                return null;
            }

        }

        internal static OutcomeVariable CreateVariable(this OutcomeDeclaration outcomeDeclaration)
        {
            return new OutcomeVariable
            {
                Identifier = outcomeDeclaration.Identifier,
                BaseType = outcomeDeclaration.BaseType,
                Cardinality = outcomeDeclaration.Cardinality,
                Value = outcomeDeclaration.DefaultValue == null ? outcomeDeclaration.GetDefaultValueIfNoValueIsSet() : outcomeDeclaration.DefaultValue
            };
        }

        internal static string Identifier(this XElement element) =>
            element.GetAttributeValue("identifier");

        internal static string RemoveXData(this string value)
        {
            if (value.Contains("<![CDATA["))
            {
                return value.Replace("<![CDATA[", "").Replace("]]>", "");
            }
            return value;
        }

        internal static BaseValue ToBaseValue(this OutcomeVariable outcomeVariable)
        {
            var baseValue = new BaseValue
            {
                BaseType = outcomeVariable.BaseType,
                Value = outcomeVariable?.Value == null ? outcomeVariable.GetDefaultValueIfNoValueIsSet() : outcomeVariable.Value.ToString()
            };
            return baseValue;
        }

        internal static string GetDefaultValueIfNoValueIsSet(this OutcomeDeclaration outcomeDeclaration)
        {
            return outcomeDeclaration.BaseType.GetDefaultValueByBaseType();
        }

        internal static string GetDefaultValueIfNoValueIsSet(this OutcomeVariable outcomeVariable)
        {
            return outcomeVariable.BaseType.GetDefaultValueByBaseType();

        }

        internal static string GetDefaultValueByBaseType(this BaseType baseType)
        {
            switch (baseType)
            {
                case BaseType.Float:
                    return "0";
                case BaseType.Int:
                    return "0";
                case BaseType.String:
                    return string.Empty;
                case BaseType.Boolean:
                    return "false";
                default:
                    return string.Empty;
            }
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
            var culture = CultureInfo.InvariantCulture;
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
            var qti2Elements = el.FindElementsByLastPartOfName("qti-interaction")
                .Where(d => d.Name.LocalName.ToLower().Contains("audio"))
                .Where(d => d.Attributes()
                    .Any(a => a.Name.LocalName.Equals("response-identifier", StringComparison.OrdinalIgnoreCase) &&
                              a.Value.Equals("RESPONSE", StringComparison.OrdinalIgnoreCase)));
            var qti3Elements = el.FindElementsByLastPartOfName("interaction")
                .Where(d => d.Name.LocalName.ToLower().Contains("audio"))
                .Where(d => d.Attributes()
                    .Any(a => a.Name.LocalName.Equals("response-identifier", StringComparison.OrdinalIgnoreCase) &&
                              a.Value.Equals("RESPONSE", StringComparison.OrdinalIgnoreCase)));
            return qti2Elements.Concat(qti3Elements);
        }

        internal static XElement ToXElement(this BaseValue value)
        {
            return XElement.Parse($"<qti-base-value base-type=\"{value.BaseType.GetString()}\">{value.Value}</qti-base-value>");
        }

        internal static OutcomeVariable ToVariable(this OutcomeDeclaration outcomeDeclaration)
        {
            return new OutcomeVariable
            {
                BaseType = outcomeDeclaration.BaseType,
                Cardinality = outcomeDeclaration.Cardinality,
                Identifier = outcomeDeclaration.Identifier,
                Value = outcomeDeclaration.DefaultValue == null ? outcomeDeclaration.GetDefaultValueIfNoValueIsSet() : outcomeDeclaration.DefaultValue
            };
        }

        internal static XElement ToVariableElement(this OutcomeDeclaration outcomeDeclaration)
        {
            return XElement.Parse($"<qti-variable identifier=\"{outcomeDeclaration.Identifier}\" />");
        }

        internal static XElement ToElement(this OutcomeDeclaration outcomeDeclaration)
        {
            return XElement.Parse($"<qti-outcome-declaration " +
                $"identifier=\"{outcomeDeclaration.Identifier}\" cardinality=\"{outcomeDeclaration.Cardinality.GetString()}\" " +
                $"base-type=\"{outcomeDeclaration.BaseType.GetString()}\"><qti-default-value><qti-value>{outcomeDeclaration.DefaultValue}</qti-value></qti-default-value></qti-outcome-declaration>");
        }

        internal static XElement ToValueElement(this string value)
        {
            if (value.TryParseFloat(out var v))
            {
                // prevent scores to be written as 0.0 but 0 instead.
                value = v.ToString("0.############", CultureInfo.InvariantCulture);
            }
            return XElement.Parse($"<Value>{value}</Value>");
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

        internal static BaseValue ToBaseValue(this float value)
        {
            var culture = CultureInfo.InvariantCulture;
            return new BaseValue
            {
                BaseType = BaseType.Float,
                Value = value.ToString(culture)
            };
        }

        internal static BaseValue ToBaseValue(this int value)
        {
            return new BaseValue
            {
                BaseType = BaseType.Int,
                Value = value.ToString()
            };
        }

        internal static BaseValue ToBaseValue(this string value)
        {
            return new BaseValue
            {
                BaseType = BaseType.String,
                Value = value
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
            if (assessmentTest.Root.Name.LocalName == "assessmentTest")
            {
                AssessmentTest.Upgrade(assessmentTest);
            }
            var changedTest = assessmentTest
                .AddTestOutcome("SCORE_TOTAL", "", null)
                .AddTestOutcome("SCORE_TOTAL_WEIGHTED", "WEIGHT", null)
                .AddTestOutcomeForCategories("SCORE_TOTAL", "")
                .AddTestOutcomeForCategories("SCORE_TOTAL_WEIGHTED", "WEIGHT");
            return changedTest;
        }

        internal static XDocument AddTestOutcomeForCategories(this XDocument assessmentTest, string identifierPrefix, string weightIdentifier)
        {
            var categories = assessmentTest.FindElementsByName("qti-assessment-item-ref")
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
            var outcomeProcessing = assessmentTest.FindElementByName("qti-outcome-processing");
            if (outcomeProcessing == null || !outcomeProcessing.FindElementsByElementAndAttributeValue("qti-set-outcome-value", "identifier", identifier).Any())
            {
                var testVariable = new SumTestVariables
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
                    assessmentTest.Add(XElement.Parse("<qti-outcome-processing></qti-outcome-processing>"));
                    outcomeProcessing = assessmentTest.FindElementByName("qti-outcome-processing");
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

        public static string ToKebabCase(this string source)
        {
            if (source is null) return null;

            if (source.Length == 0) return string.Empty;

            var builder = new StringBuilder();

            for (var i = 0; i < source.Length; i++)
            {
                if (IsLower(source[i])) // if current char is already lowercase
                {
                    builder.Append(source[i]);
                }
                else if (i == 0) // if current char is the first char
                {
                    builder.Append(ToLower(source[i]));
                }
                else if (IsLower(source[i - 1])) // if current char is upper and previous char is lower
                {
                    builder.Append("-");
                    builder.Append(ToLower(source[i]));
                }
                else if (i + 1 == source.Length || IsUpper(source[i + 1])) // if current char is upper and next char doesn't exist or is upper
                {
                    builder.Append(ToLower(source[i]));
                }
                else // if current char is upper and next char is lower
                {
                    builder.Append("-");
                    builder.Append(ToLower(source[i]));
                }
            }

            return builder.ToString();
        }



    }
}