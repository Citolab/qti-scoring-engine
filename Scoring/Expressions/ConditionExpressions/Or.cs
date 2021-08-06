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
    internal class Or : ConditionExpressionBase
    {
        public override bool Execute(IProcessingContext ctx)
        {
            foreach (var expression in conditionalExpressions)
            {
                var result = expression.Execute(ctx);
                if (result == true)
                {
                    return true; // one condition true; return true
                }
            }
            return false; // all children false; return false
        }
    }
}
