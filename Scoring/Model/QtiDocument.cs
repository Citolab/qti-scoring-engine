using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal abstract class QtiDocument
    {
        public string Identifier { get; set; }
        public XDocument Content { get; }
        protected ILogger Logger;
        public QtiDocument(ILogger logger, XDocument qtiDocument)
        {
            Logger = logger;
            Identifier = qtiDocument.Root.Identifier();
            Content = qtiDocument;
            if (!Content.Root.Name.LocalName.Contains("qti-"))
            {
                Upgrade();
            }
        }

        public virtual void Upgrade() => UpgradeToQti3(Content);

        /// <summary>
        /// Rewrites a 2.x document to the 3.0 tag and attribute spelling.
        /// Just converts what is needed to be able to process it; the result does not have to be
        /// a valid 3.0 package. See https://github.com/Citolab/qti-converter for an attempt to
        /// convert to valid 3.0 packages.
        /// </summary>
        protected static void UpgradeToQti3(XDocument doc)
        {
            XNamespace xNamespace = "http://www.imsglobal.org/xsd/imsqtiasi_v3p0";
            foreach (var element in doc.Descendants())
            {
                var tagName = element.Name.LocalName;
                var kebabTagName = tagName.ToKebabCase();
                element.Name = xNamespace + $"qti-{kebabTagName}";
            }

            // fix attributes
            foreach (var element in doc.Descendants())
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


        public OutcomeDeclaration GetOutcomeDeclaration(XElement outcomeDeclaration)
        {
            var outcome = new OutcomeDeclaration();
            var baseTypeString = outcomeDeclaration.GetAttributeValue("base-type");
            var cardinalityString = outcomeDeclaration.GetAttributeValue("cardinality");
            var identifier = outcomeDeclaration.Identifier();
            if (string.IsNullOrEmpty(baseTypeString))
            {
                LogWarning("missing baseType, using default value");
            }
            if (string.IsNullOrEmpty(cardinalityString))
            {
                LogWarning("missing cardinality, using default value");
            }
            if (string.IsNullOrEmpty(identifier))
            {
                LogError("missing identifier in outcomeDeclaration");
                return null;
            }
            outcome.BaseType = baseTypeString.ToBaseType(Logger);
            outcome.Cardinality = cardinalityString.ToCardinality();
            outcome.Identifier = identifier;
            var defaultValue = outcomeDeclaration.FindElementsByName("qti-default-value").FirstOrDefault()?.FindElementsByName("qti-value").FirstOrDefault();
            if (defaultValue != null)
            {
                // TODO: check type
                outcome.DefaultValue = defaultValue.Value;
            }
            else
            {
                outcome.DefaultValue = outcome.GetDefaultValueIfNoValueIsSet();
            }
            var interpolationTable = outcomeDeclaration.FindElementsByName("qti-interpolation-table").FirstOrDefault();
            if (interpolationTable != null)
            {
                var interpolationTableEntries = interpolationTable.FindElementsByName("qti-interpolation-table-entry")?
                    .Select(tableEntry =>
                    {
                        try
                        {
                            var entry = new InterpolationTableEntry
                            {
                                SourceValue = tableEntry.GetAttributeValue("source-value").ParseFloat(Logger),
                                TargetValue = tableEntry.GetAttributeValue("target-value").ParseFloat(Logger)
                            };
                            return entry;
                        }
                        catch
                        {
                            Logger.LogError("interpolation value could not be converted to a float");
                        }
                        return null;
                    }).ToList();
                outcome.InterpolationTable = interpolationTableEntries;
            }
            var matchTable = outcomeDeclaration.FindElementsByName("qti-match-table").FirstOrDefault();
            if (matchTable != null)
            {
                outcome.MatchTable = matchTable.FindElementsByName("qti-match-table-entry")?
                    .Select(tableEntry => new MatchTableEntry
                    {
                        SourceValue = tableEntry.GetAttributeValue("source-value"),
                        TargetValue = tableEntry.GetAttributeValue("target-value")
                    })
                    .ToList();
            }
            return outcome;
        }

        protected void LogInformation(string value)
        {
            Logger.LogInformation($"{Identifier}: {value}");
        }
        public void LogWarning(string value)
        {
            Logger.LogWarning($"{Identifier}: {value}");
        }

        public void LogError(string value)
        {
            Logger.LogError($"{Identifier}: {value}");
        }
    }
}
