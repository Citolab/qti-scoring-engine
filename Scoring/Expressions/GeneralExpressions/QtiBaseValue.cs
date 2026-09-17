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
        private BaseType _baseType;
        private string _value;
        private string _identifier;

        /// <summary>
        /// Returns a new BaseValue on every call. The expression tree is built once per
        /// assessmentItem and reused for every result and every thread, while callers
        /// (custom operators, the case-insensitive compare path) write to BaseValue.Value.
        /// Handing out a single shared instance let one result's edit reach all the others.
        /// </summary>
        public override BaseValue Apply(IProcessingContext ctx)
        {
            return new BaseValue
            {
                BaseType = _baseType,
                Value = _value,
                Identifier = _identifier
            };
        }

        public override void Init(XElement qtiElement, IExpressionFactory expressionFactory)
        {
            _baseType = qtiElement.GetAttributeValue("base-type").ToBaseType();
            _value = qtiElement.Value.RemoveXData();
            _identifier = qtiElement.Identifier();
        }
    }
}
