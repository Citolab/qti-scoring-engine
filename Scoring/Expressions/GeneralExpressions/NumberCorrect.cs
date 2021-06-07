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
    // NOT SUPPORTED. EXISTING OUTCOME IS UNTOUCHED.
    internal class NumberCorrect : IOutcomeProcessingExpression
    {
        public string Name => "qti-number-correct";

        public BaseValue Apply(XElement qtiElement, IProcessingContext ctx)
        {
            // with response processing it's diffult to determine what's correct.
            // of course with a simple mapping of and choiceInteraction it's easy but for polymous scoring
            // and multiple interactions it gets complicated. Therefor not supperted and don't touch the value
            // in case the delivery system did calculate this.
            ctx.LogWarning("qti-number-correct is not support. Existing value will left untouched.");
            var setOutcomeElement = qtiElement.FindParentElement("qti-set-outcome-value");
            if (setOutcomeElement != null && !string.IsNullOrEmpty(setOutcomeElement.Identifier())) {
                // removed from values that will be persisted.
                ctx.CalculatedOutcomes.Remove(setOutcomeElement.Identifier());
            }
            
            return 0.0F.ToBaseValue();
        }
    }
}
