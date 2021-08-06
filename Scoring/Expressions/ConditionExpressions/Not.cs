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
    /// All children should return true
    /// </summary>
    internal class Not : ConditionExpressionBase
    {

        public override bool Execute(IProcessingContext ctx)
        { 
            if (conditionalExpressions.Count != 1)
            {
                ctx.LogError("Not element should contain only one child");
                return true;
            }
            var result = conditionalExpressions.First().Execute(ctx);
            return !result;
        }
    }
}
