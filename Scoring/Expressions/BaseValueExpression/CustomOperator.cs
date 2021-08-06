using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression
{
    internal class CustomOperator : ValueExpressionBase
    {
        private ICustomOperator _customOperator;
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var baseValues = expressions.Select(e => e.Apply(ctx)).ToList();
            return _customOperator.Apply(baseValues);
        }

        public override void Init(XElement qtiElement, IExpressionFactory expressionFactory)
        {
            base.Init(qtiElement, expressionFactory);
            var definition = GetAttributeValue("definition");
            if (!string.IsNullOrEmpty(definition))
            {
                _customOperator = expressionFactory.GetCustomOperator(definition);
            }
        }
    }
}
