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
    /// One of the children should return true
    /// </summary>
    internal class Or : IConditionExpression
    {
        public string Name => "qti-or";

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var result = context.CheckCondition(child);
                if (result == true)
                {
                    return true; // one is true, return true
                }
            }
            return false; // all false: return false
        }

    }
}
