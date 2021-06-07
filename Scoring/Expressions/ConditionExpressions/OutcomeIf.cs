using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class OutcomeIf : IOutcomeProcessingConditionExpression
    {
        public string Name { get => "qti-outcome-if"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            return new ResponseIf().Execute(qtiElement, context);
        }
    }
}
