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
    internal class IsNull : IConditionExpression
    {
        public string Name => "qti-is-null";

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            if (qtiElement.Elements().Count() != 1)
            {
                context.LogError($"Unexpected child count: {qtiElement.Elements().Count()} in qti-is-null");
                return false;
            };
            var value = context.GetValue(qtiElement.Elements().FirstOrDefault());
            return value == null;
        }
    }
}
