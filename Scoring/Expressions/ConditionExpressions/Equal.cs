using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class Equal : ConditionExpressionBase
    {

        public override bool Execute(IProcessingContext ctx)
        {
            // todo toleranceMode mode, couldn't find any example of absolute or relative.
            var toleranceMode = GetAttributeValue("tolerance-mode");
            if (!string.IsNullOrEmpty(toleranceMode) && toleranceMode != "exact")
            {
                ctx.LogError($"Unsupported tolerance-mode: {toleranceMode}");
                return false;
            }
            if (expressions.Count == 2)
            {
                var value1 = expressions[0]?.Apply(ctx);
                var value2 = expressions[1]?.Apply(ctx);
                return Helper.CompareSingleValues(value1?.Value, value2?.Value, Model.BaseType.Float, ctx);
            }
            else
            {
                ctx.LogError($"Unexpected number of childElements: {expressions.Count}");
                return false;
            }
        }

    }
}