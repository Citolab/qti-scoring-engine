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
    /// <summary>
    /// null check
    /// </summary>
    internal class IsNull : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx) { 
            if (expressions.Count() != 1)
            {
                ctx.LogError($"Unexpected child count: {expressions.Count()} in qti-is-null");
                return false;
            };
            var value = expressions.First().Apply(ctx);
            return value == null;
        }
    }
}
