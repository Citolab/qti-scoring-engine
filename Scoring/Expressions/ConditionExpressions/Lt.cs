using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class Lt : IConditionExpression
    {
        public string Name => "qti-lt";

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            var values = qtiElement.Elements().Select(context.GetValue).ToList();
            context.LogInformation($"lt check. Values: {string.Join(", ", values.Select(v => v.Value).ToArray())}");
            if (values.Count() != 2)
            {
                context.LogError($"unexpected values to compare: expected: 2, retrieved: {values.Count}");
                return false;
            }
            if (values[0].BaseType != values[1].BaseType)
            {
                context.LogWarning($"baseType response and outcome does not match: {values[0]?.BaseType.GetString()} and {values[1]?.BaseType.GetString()}. Proceeding with type: {values[1].BaseType.GetString()}");
            }
            switch (values[0].BaseType)
            {
                case BaseType.Int:
                case BaseType.Float:
                    {
                        if (values[0].Value.TryParseFloat(out var value1) && values[1].Value.TryParseFloat(out var value2))
                        {
                            return value1 < value2;
                        }
                        else
                        {
                            context.LogError($"value cannot be casted to numeric value in lt operator: {values[0]?.Value}, {values[1]?.Value}");
                        }
                        break;
                    }
                default:
                    {
                        context.LogError($"values other than float and int cannot be used in lt operator.");
                        break;
                    }
            }
            return false;
        }

    }
}
