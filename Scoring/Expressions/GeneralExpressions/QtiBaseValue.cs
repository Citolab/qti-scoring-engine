using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class QtiBaseValue : IGeneralExpression
    {
        private BaseValue _baseValue;

        public string Name => "qti-base-value";

        public BaseValue Apply(IProcessingContext ctx)
        {
            return _baseValue;
        }

        public void Init(XElement qtiElement)
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
