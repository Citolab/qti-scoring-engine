using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helper;
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

        public abstract void Upgrade();

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
            outcome.BaseType = baseTypeString.ToBaseType();
            outcome.Cardinality = cardinalityString.ToCardinality();
            outcome.Identifier = identifier;
            var defaultValue = outcomeDeclaration.FindElementsByName("qti-default-value").FirstOrDefault()?.FindElementsByName("qti-value").FirstOrDefault();
            if (defaultValue != null)
            {
                // TODO: check type
                outcome.DefaultValue = defaultValue.Value;
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
