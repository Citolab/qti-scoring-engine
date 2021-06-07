using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class OutcomeElseIf : IConditionExpression
    {
        public string Name { get => "qti-outcome-else-if"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            return new ResponseElseIf().Execute(qtiElement, context);
        }
    }
}
