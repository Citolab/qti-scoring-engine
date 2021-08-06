using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class SetOutcomeValue : ConditionExpressionBase
    {

        public override bool Execute(IProcessingContext ctx)
        {
            var outcomeIdentifier = GetAttributeValue("identifier");
            if (string.IsNullOrEmpty(outcomeIdentifier))
            {
                ctx.LogError($"Cannot find attribute identifier");
                return false;
            };
            if (!ctx.OutcomeDeclarations.ContainsKey(outcomeIdentifier))
            {
                ctx.LogError($"Cannot find outcomeDeclaration: {outcomeIdentifier}");
                return false;
            }
            else
            {
                var outcomeDeclaration = Helper.GetOutcomeDeclaration(outcomeIdentifier, ctx);
                var outcomeVariable = Helper.GetOutcomeVariable(outcomeIdentifier, outcomeDeclaration, ctx);

                if (expressions.Count() == 1)
                {
                    var childElement = expressions.FirstOrDefault();
                    var culture = CultureInfo.InvariantCulture;
                    outcomeVariable.Value = childElement.Apply(ctx)?.Value.ToString(culture);
                }
                else
                {
                    ctx.LogError($"Unexpected childs: {expressions.Count()} in SetOutcomeValue");
                }
            }
            return true;
        }
    }
}
