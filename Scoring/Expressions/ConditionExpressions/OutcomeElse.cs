using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class OucomeElse : IOutcomeProcessingConditionExpression
    {
        public string Name { get => "qti-outcome-else"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            return new ResponseElse().Execute(qtiElement, context);
        }
    }
}
