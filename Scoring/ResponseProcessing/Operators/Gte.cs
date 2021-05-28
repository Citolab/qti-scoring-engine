using Citolab.QTI.Scoring.ResponseProcessing;
using Citolab.QTI.Scoring.Helper;
using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing.Operators
{
    internal class Gte : IResponseProcessingOperator
    {
        public string Name => "gte";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            var values = qtiElement.GetValues(context);// Helper.GetStringValueOfChildren(qtiElement, context).ToList();
            context.LogInformation($"gte check. Values: {string.Join(", ", values.Select(v => v.Value).ToArray())}");
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
                        if (values[0].Value.TryParseFloat(out var value1) && values[1].Value.TryParseFloat(out var value2))
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
