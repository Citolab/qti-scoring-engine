using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Executors
{
    public class Gte : IExecuteReponseProcessing
    {
        public string Name => "gte";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var values = qtiElement.GetValues(context);// Helper.GetStringValueOfChildren(qtiElement, context).ToList();
            context.LogInformation($"gte check. Values: {string.Join(", ", values.Select(v => v.Value).ToArray())}");
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
            if (values[0].BaseType != values[1].BaseType)
            {
                context.LogWarning($"baseType response and outcome does not match: {values[0]?.BaseType.GetString()} and {values[1]?.BaseType.GetString()}. Proceeding with type: {values[1]}");
            }
            switch (values[0].BaseType)
            {
                case BaseType.Int:
                case BaseType.Float:
                    {
                        if (float.TryParse(values[0].Value, out var value1) && float.TryParse(values[1].Value, out var value2))
                        {
                            return value1 >= value2;
                        }
                        else
                        {
                            context.LogError($"value cannot be casted to numeric value in gte operator: {values[0]?.Value}, {values[1]?.Value}");
                        }
                        break;
                    }
                default:
                    {
                        context.LogError($"values other than float and int cannot be used in gte operator.");
                        break;
                    }
            }
            return false;
        }

    }
}
