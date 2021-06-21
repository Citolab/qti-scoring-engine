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
    internal class And : IConditionExpression
    {
        public string Name => "qti-and";

        public bool Execute(IProcessingContext ctx)
        {
            foreach (var child in qtiElement.Elements())
            {
                var result = ctx.CheckCondition(child);
                if (result == false) 
                {
                    return false; // one condition false; return false
                }
            }
            return true; // all children true; return true
        }

        public void Init(XElement qtiElement)
        {
            qtiElement.Elements().Select(childElement => ExpressionFactory.())
        }
    }
}
