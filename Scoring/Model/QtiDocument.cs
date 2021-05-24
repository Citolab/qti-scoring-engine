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
    public abstract class QtiDocument : XDocument
    {
        public string Identifier { get; set; }
        protected ILogger Logger;
        public QtiDocument(ILogger logger, XDocument qtiDocument) : base(qtiDocument)
        {
            Logger = logger;
            Identifier = qtiDocument.Root.Identifier();
        }

        public OutcomeDeclaration GetOutcomeDeclaration(XElement outcomeDeclaration)
        {
            var outcome = new OutcomeDeclaration();
            var baseTypeString = outcomeDeclaration.GetAttributeValue("baseType");
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
            var defaultValue = outcomeDeclaration.FindElementsByName("defaultValue").FirstOrDefault()?.FindElementsByName("value").FirstOrDefault();
            if (defaultValue != null)
            {
                // TODO: check type
                outcome.DefaultValue = defaultValue.Value;
            }
            var interpolationTable = outcomeDeclaration.FindElementsByName("interpolationTable").FirstOrDefault();
            if (interpolationTable != null)
            {
                var interpolationTableEntries = interpolationTable.FindElementsByName("interpolationTableEntry")?
                    .Select(tableEntry =>
                    {
                        try
                        {
                            var entry = new InterpolationTableEntry
                            {
                                SourceValue = float.Parse(tableEntry.GetAttributeValue("sourceValue")),
                                TargetValue = float.Parse(tableEntry.GetAttributeValue("targetValue"))
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
