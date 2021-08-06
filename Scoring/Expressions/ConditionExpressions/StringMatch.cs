using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class StringMatch : ConditionExpressionBase
    {

        public override bool Execute(IProcessingContext ctx)
        {
            if (expressions.Count == 2)
            {
                var caseSensitiveAttribute = GetAttributeValue("case-sensitive");
                if (!(!string.IsNullOrEmpty(caseSensitiveAttribute) && bool.TryParse(caseSensitiveAttribute, out var caseSensitive)))
                {
                    caseSensitive = true;
                }
                var values = expressions.Select(e => e.Apply(ctx)).ToList();
                var value1 = caseSensitive ? values[0].Value : values[0].Value.ToLower();
                var value2 = caseSensitive ? values[1].Value : values[1].Value.ToLower();

                return Helper.CompareSingleValues(value1, value2, Model.BaseType.String, ctx);
            } else
            {
                ctx.LogError($"Unexpected number of childElements: {expressions.Count}, expected: 2");
                return false;
            }
          
        }

    }
}