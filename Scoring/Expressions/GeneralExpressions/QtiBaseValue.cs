using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class QtiBaseValue : ValueExpressionBase
    {
        private BaseValue _baseValue;

        public override BaseValue Apply(IProcessingContext ctx)
        {
            return _baseValue;
        }

        public override void Init(XElement qtiElement, IExpressionFactory expressionFactory)
        {
            _baseValue = new BaseValue
            {
                BaseType = qtiElement.GetAttributeValue("base-type").ToBaseType(),
                Value = qtiElement.Value.RemoveXData(),
                Identifier = qtiElement.Identifier()
            };
        }
    }
}
