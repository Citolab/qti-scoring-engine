using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Const;
using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AssessmentItem : QtiDocument
    {
        public Dictionary<string, ResponseDeclaration> ResponseDeclarations;
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations { get; set; }

        public HashSet<string> CalculatedOutcomes;

        public List<IConditionExpression> Expressions { get; } = new List<IConditionExpression>();

        public AssessmentItem(ILogger logger, XDocument assessmentItem, IExpressionFactory expressionFactory) : base(logger, assessmentItem)
        {
            var responseProcessing = Content.FindElementByName("qti-response-processing");
            if (responseProcessing != null && !string.IsNullOrWhiteSpace(responseProcessing.GetAttributeValue("template")))
            {
                var splittedTemplateName = responseProcessing.GetAttributeValue("template").Split('/');
                var templateName = splittedTemplateName[splittedTemplateName.Length - 1];
                switch (System.IO.Path.GetFileNameWithoutExtension(templateName.Trim()))
                {
                    case "map_response":
                        {
                            Content.Root.Add(Templates.MapResponse.AddDefaultNamespace(Content.Root.GetDefaultNamespace()));
                            responseProcessing.Remove();
                            break;
                        }
                    case "map_response_point":
                        {
                            Content.Root.Add(Templates.MapResponsePoint.AddDefaultNamespace(Content.Root.GetDefaultNamespace()));
                            responseProcessing.Remove();
                            break;
                        }
                    case "match_correct":
                        {
                            Content.Root.Add(Templates.MatchCorrect.AddDefaultNamespace(Content.Root.GetDefaultNamespace()));
                            responseProcessing.Remove();
                            break;
                        }
                }
                responseProcessing = Content.FindElementByName("qti-response-processing");
            }
            OutcomeDeclarations = Content.FindElementsByName("qti-outcome-declaration").Select(outcomeDeclaration =>
            {
                return GetOutcomeDeclaration(outcomeDeclaration);
            }).ToDictionary(o => o.Identifier, o => o);

            ResponseDeclarations = Content.FindElementsByName("qti-response-declaration").Select(responseDeclaration =>
            {
                return GetResponseDeclaration(responseDeclaration);
            }).ToDictionary(o => o.Identifier, o => o);

            // outcomes in the itemResult that are not in the qtiItem file are left unchanged.
            // outcomes that are defined in the qti-item, present in the itemResult and not used in responseProcessing are left unchanged.
            // outcomes that are defined in the qti-item, not present in the itemResult and not used in the responseProcessing get the defaultValue.
            // outcomes that are defined in the qti-item and are used responseProcessing are recalcuted.
            var lookups = responseProcessing?
                .FindElementsByName("qti-lookup-outcome-value")?
                .Select(v => v.Identifier());
            var setOutcomes = responseProcessing?
              .FindElementsByName("qti-set-outcome-value")?
              .Select(v => v.Identifier())?.ToList();
            if (setOutcomes == null)
            {
                setOutcomes = new List<string>();
            }
            if (lookups == null)
            {
                lookups = new List<string>();
            }
            CalculatedOutcomes = setOutcomes.Concat(lookups).Distinct().ToHashSet();
            var responseProcessingElement = Content.FindElementByName("qti-response-processing");
            if (responseProcessingElement != null)
            {
                foreach (var responseProcessingChild in responseProcessingElement.Elements())
                {
                    Expressions.Add(expressionFactory.GetConditionExpression(responseProcessingChild, true));
                }
            }
            else
            {
                logger.LogInformation("No responseProcessing found");
            }
        }

        private ResponseDeclaration GetResponseDeclaration(XElement el)
        {
            var responseDeclaration = new ResponseDeclaration();
            var baseTypeString = el.GetAttributeValue("base-type");
            var cardinalityString = el.GetAttributeValue("cardinality");
            var identifier = el.Identifier();
            if (string.IsNullOrEmpty(baseTypeString))
            {
                Logger.LogWarning("missing baseType, using default value");
            }
            if (string.IsNullOrEmpty(cardinalityString))
            {
                Logger.LogWarning("missing cardinality, using default value");
            }
            if (string.IsNullOrEmpty(identifier))
            {
                Logger.LogError("missing identifier in responseDeclaration");
                return null;
            }
            responseDeclaration.BaseType = baseTypeString.ToBaseType();
            responseDeclaration.Cardinality = cardinalityString.ToCardinality();
            responseDeclaration.Identifier = identifier;
            var correctResponse = el.FindElementsByName("qti-correct-response")
                .FirstOrDefault();
            responseDeclaration.CorrectResponseInterpretation = correctResponse?.GetAttributeValue("interpretation");
            var correctValues = correctResponse?.FindElementsByName("qti-value").Select(v => v.Value.RemoveXData())?.ToList();

            if (responseDeclaration.Cardinality == Cardinality.Single)
            {
                responseDeclaration.CorrectResponse = correctValues?.FirstOrDefault();
            }
            responseDeclaration.CorrectResponses = correctValues; // WORKAROUND TO SUPPORT INVALID RESPONSE DECLARATIOJN
            var mappingElement = el.FindElementsByName("qti-mapping").FirstOrDefault();
            if (mappingElement != null)
            {
                float defaultValue = 0.0F;
                var defaultValueString = mappingElement.GetAttributeValue("default-value");
                var lowerBoundString = mappingElement.GetAttributeValue("lower-bound");
                var upperBoundString = mappingElement.GetAttributeValue("upper-bound");
                if (!string.IsNullOrWhiteSpace(defaultValueString))
                {
                    if (!defaultValueString.TryParseFloat(out defaultValue))
                    {
                        Logger.LogError($"Cannot convert defaul-value: ${defaultValueString} to a float");
                    }
                }
                var mapping = new Mapping { DefaultValue = defaultValue };
                if (!string.IsNullOrWhiteSpace(lowerBoundString))
                {
                    if (lowerBoundString.TryParseFloat(out var lowerBound))
                    {
                        mapping.LowerBound = lowerBound;
                    }
                    else
                    {
                        Logger.LogError($"Cannot convert lowerbound: ${lowerBoundString} to a float");
                    }
                }
                if (!string.IsNullOrWhiteSpace(upperBoundString))
                {
                    if (upperBoundString.TryParseFloat(out var upperbound))
                    {
                        mapping.UpperBound = upperbound;
                    }
                    else
                    {
                        Logger.LogError($"Cannot convert upperbound: ${upperBoundString} to a float");
                    }
                }
                mapping.MapEntries = mappingElement.FindElementsByName("qti-map-entry").Select(mapEntryElement =>
                {
                    if (mapEntryElement.GetAttributeValue("mapped-value").TryParseFloat(out var mapValue))
                    {
                        var caseSensitiveValue = mapEntryElement.GetAttributeValue("case-sensitive");
                        var caseSensitive = false;
                        if (!string.IsNullOrWhiteSpace(caseSensitiveValue))
                        {
                            bool.TryParse(caseSensitiveValue, out caseSensitive);
                        }
                        return new MapEntry
                        {
                            MapKey = mapEntryElement.GetAttributeValue("map-key"),
                            MappedValue = mapValue,
                            CaseSensitive = caseSensitive
                        };
                    }
                    else
                    {
                        Logger.LogError($"Cannot convert map-value: {mapEntryElement.GetAttributeValue("map-value")} to a float");
                    }
                    return null;
                }).Where(m => m != null)
                .ToList();
                responseDeclaration.Mapping = mapping;
            }

            var areaMappingElement = el.FindElementsByName("qti-area-mapping").FirstOrDefault();
            if (areaMappingElement != null)
            {
                float defaultValue = 0.0F;
                var defaultValueString = areaMappingElement.GetAttributeValue("default-value");
                var lowerBoundString = areaMappingElement.GetAttributeValue("lower-bound");
                var upperBoundString = areaMappingElement.GetAttributeValue("upper-bound");
                if (!string.IsNullOrWhiteSpace(defaultValueString))
                {
                    if (!defaultValueString.TryParseFloat(out defaultValue))
                    {
                        Logger.LogError($"Cannot convert defaul-value: ${defaultValueString} to a float");
                    }
                }
                var areaMapping = new AreaMapping { DefaultValue = defaultValue };
                if (!string.IsNullOrWhiteSpace(lowerBoundString))
                {
                    if (lowerBoundString.TryParseFloat(out var lowerBound))
                    {
                        areaMapping.LowerBound = lowerBound;
                    }
                    else
                    {
                        Logger.LogError($"Cannot convert lowerbound: ${lowerBoundString} to a float");
                    }
                }
                if (!string.IsNullOrWhiteSpace(upperBoundString))
                {
                    if (upperBoundString.TryParseFloat(out var upperbound))
                    {
                        areaMapping.UpperBound = upperbound;
                    }
                    else
                    {
                        Logger.LogError($"Cannot convert upperbound: ${upperBoundString} to a float");
                    }
                }
                areaMapping.AreaMappings = areaMappingElement.FindElementsByName("qti-area-map-entry").Select(mapEntryElement =>
                {
                    if (mapEntryElement.GetAttributeValue("mapped-value").TryParseFloat(out var mapValue))
                    {
                        return new AreaMapEntry
                        {
                            MappedValue = mapValue,
                            Coords = mapEntryElement.GetAttributeValue("coords"),
                            Shape = mapEntryElement.GetAttributeValue("shape").ToShape()
                        };
                    }
                    else
                    {
                        Logger.LogError($"Cannot convert area-map-entry: {mapEntryElement.GetAttributeValue("map-value")} to a float");
                    }
                    return null;
                }).Where(m => m != null)
                .ToList();
                responseDeclaration.AreaMapping = areaMapping;
            }
            return responseDeclaration;
        }


        public override void Upgrade()
        {
            XNamespace xNamespace = "http://www.imsglobal.org/xsd/imsqtiasi_v3p0";
            // Just convert to what is need to be able to process.
            // It does not have to be a valid 3.0 package
            // see: https://github.com/Citolab/qti-converter for an 
            // attempt to convert to valid 3.0 packages.

            foreach (var element in Content.Descendants())
            {
                var tagName = element.Name.LocalName;
                var kebabTagName = tagName.ToKebabCase();
                element.Name = xNamespace + $"qti-{kebabTagName}";
            }

            // fix attributes
            foreach (var element in Content.Descendants())
            {
                var attributesToRemove = new List<XAttribute>();
                var attributesToAdd = new List<XAttribute>();
                foreach (var attribute in element.Attributes()
                    .Where(attr => !attr.IsNamespaceDeclaration && string.IsNullOrEmpty(attr.Name.NamespaceName)))
                {
                    var attributeName = attribute.Name.LocalName;
                    var kebabAttributeName = attributeName.ToKebabCase();
                    if (attributeName != kebabAttributeName)
                    {
                        var newAttr = new XAttribute($"{kebabAttributeName}", attribute.Value);
                        attributesToRemove.Add(attribute);
                        attributesToAdd.Add(newAttr);
                    }
                }
                attributesToRemove.ForEach(a => a.Remove());
                attributesToAdd.ForEach(a => element.Add(a));
            }
        }

    }
}
