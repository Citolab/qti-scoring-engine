using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Const;
using Citolab.QTI.ScoringEngine.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AssessmentItem : QtiDocument
    {
        public Dictionary<string, ResponseDeclaration> ResponseDeclarations;
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations;

        public HashSet<string> CalculatedOutcomes;

        public XElement ResponseProcessingElement => Content.FindElementByName("responseProcessing");

        public AssessmentItem(ILogger logger, XDocument assessmentItem) : base(logger, assessmentItem)
        {
            var responseProcessing = Content.FindElementByName("responseProcessing");
            if (responseProcessing != null && !string.IsNullOrWhiteSpace(responseProcessing.GetAttributeValue("template")))
            {
                var splittedTemplateName = responseProcessing.GetAttributeValue("template").Split('/');
                var templateName = splittedTemplateName[splittedTemplateName.Length - 1];
                switch (templateName.Trim())
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
                responseProcessing = Content.FindElementByName("responseProcessing");
            }
            OutcomeDeclarations = Content.FindElementsByName("outcomeDeclaration").Select(outcomeDeclaration =>
            {
                return GetOutcomeDeclaration(outcomeDeclaration);
            }).ToDictionary(o => o.Identifier, o => o);

            ResponseDeclarations = Content.FindElementsByName("responseDeclaration").Select(responseDeclaration =>
            {
                return GetResponseDeclaration(responseDeclaration);
            }).ToDictionary(o => o.Identifier, o => o);

            // outcomes in the itemResult that are not in the qtiItem file are left unchanged.
            // outcomes that are defined in the qti-item, present in the itemResult and not used in responseProcessing are left unchanged.
            // outcomes that are defined in the qti-item, not present in the itemResult and not used in the responseProcessing get the defaultValue.
            // outcomes that are defined in the qti-item and are used responseProcessing are recalcuted.
            var lookups = responseProcessing?
                .FindElementsByName("lookupOutcomeValue")?
                .Select(v => v.Identifier());
            var setOutcomes = responseProcessing?
              .FindElementsByName("setOutComeValue")?
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
        }

        private ResponseDeclaration GetResponseDeclaration(XElement el)
        {
            var responseDeclaration = new ResponseDeclaration();
            var baseTypeString = el.GetAttributeValue("baseType");
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
            var correctResponse = el.FindElementsByName("correctResponse")
                .FirstOrDefault();
            responseDeclaration.CorrectResponseInterpretation = correctResponse?.GetAttributeValue("interpretation");
            var correctValues = correctResponse?.FindElementsByName("value").Select(v => v.Value.RemoveXData())?.ToList();

            if (responseDeclaration.Cardinality == Cardinality.Single)
            {
                responseDeclaration.CorrectResponse = correctValues?.FirstOrDefault();
            }
            else
            {
                responseDeclaration.CorrectResponses = correctValues;
            }
            var mappingElement = el.FindElementsByName("mapping").FirstOrDefault();
            if (mappingElement != null)
            {
                float defaultValue = 0.0F;
                var defaultValueString = mappingElement.GetAttributeValue("defaultValue");
                var lowerBoundString = mappingElement.GetAttributeValue("lowerBound");
                var upperBoundString = mappingElement.GetAttributeValue("upperBound");
                if (!string.IsNullOrWhiteSpace(defaultValueString))
                {
                    if (!defaultValueString.TryParseFloat(out defaultValue))
                    {
                        Logger.LogError($"Cannot convert defaulValue: ${defaultValueString} to a float");
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
                mapping.MapEntries = mappingElement.FindElementsByName("mapEntry").Select(mapEntryElement =>
                {
                    if (mapEntryElement.GetAttributeValue("mappedValue").TryParseFloat(out var mapValue))
                    {
                        return new MapEntry
                        {
                            MapKey = mapEntryElement.GetAttributeValue("mapKey"),
                            MappedValue = mapValue
                        };
                    }
                    else
                    {
                        Logger.LogError($"Cannot convert mapValue: {mapEntryElement.GetAttributeValue("mapValue")} to a float");
                    }
                    return null;
                }).Where(m => m != null)
                .ToList();
                responseDeclaration.Mapping = mapping;
            }

            var areaMappingElement = el.FindElementsByName("areaMapping").FirstOrDefault();
            if (areaMappingElement != null)
            {
                float defaultValue = 0.0F;
                var defaultValueString = areaMappingElement.GetAttributeValue("defaultValue");
                var lowerBoundString = areaMappingElement.GetAttributeValue("lowerBound");
                var upperBoundString = areaMappingElement.GetAttributeValue("upperBound");
                if (!string.IsNullOrWhiteSpace(defaultValueString))
                {
                    if (!defaultValueString.TryParseFloat(out defaultValue))
                    {
                        Logger.LogError($"Cannot convert defaulValue: ${defaultValueString} to a float");
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
                areaMapping.AreaMappings = areaMappingElement.FindElementsByName("areaMapEntry").Select(mapEntryElement =>
                {
                    if (mapEntryElement.GetAttributeValue("mappedValue").TryParseFloat(out var mapValue))
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
                        Logger.LogError($"Cannot convert areaMapEntry: {mapEntryElement.GetAttributeValue("mapValue")} to a float");
                    }
                    return null;
                }).Where(m => m != null)
                .ToList();
                responseDeclaration.AreaMapping = areaMapping;
            }
            return responseDeclaration;
        }
    }
}
