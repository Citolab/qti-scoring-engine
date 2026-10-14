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
    internal class Gte : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {

            var values = expressions.Select(e => e.Apply(ctx)).ToList();
            if (values.Count == 2 && values.Any(v => v?.Value == null))
            {
                // a comparison with NULL is NULL, and NULL is false in a condition: for instance a field
                // of a record response that the candidate did not give.
                ctx.LogInformation("gte check: one of the values is NULL, so the result is NULL (false).");
                return false;
            }
            ctx.LogInformation($"gte check. Values: {string.Join(", ", values.Select(v => v.Value).ToArray())}");
            if (values.Count != 2)
            {
                ctx.LogError($"unexpected values to compare: expected: 2, retrieved: {values.Count}");
                return false;
            }
            if (values[0].BaseType != values[1].BaseType)
            {
                ctx.LogWarning($"baseType response and outcome does not match: {values[0]?.BaseType.GetString()} and {values[1]?.BaseType.GetString()}. Proceeding with type: {values[1].BaseType.GetString()}");
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
                            ctx.LogError($"value cannot be casted to numeric value in gte operator: {values[0]?.Value}, {values[1]?.Value}");
                        }
                        break;
                    }
                default:
                    {
                        ctx.LogError($"values other than float and int cannot be used in gte operator.");
                        break;
                    }
            }
            return false;
        }

    }
}
