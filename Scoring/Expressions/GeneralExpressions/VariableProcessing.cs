using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.OutcomeProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class VariableProcessing : ValueExpressionBase
    {
        private ValueExpressionBase _variableProcessor;

        public override BaseValue Apply(IProcessingContext ctx)
        {
            return _variableProcessor.Apply(ctx);
        }

        public override void Init(XElement qtiElement, IExpressionFactory expressionFactory)
        {
            if (qtiElement.FindParentElement("qti-outcome-processing") != null)
            {
                _variableProcessor = new VariableOutcomeProcessing();
                _variableProcessor.Init(qtiElement, expressionFactory);
            } else
            {
                _variableProcessor = new VariableResponseProcessing();
                _variableProcessor.Init(qtiElement, expressionFactory);
            }
        }
    }
}
