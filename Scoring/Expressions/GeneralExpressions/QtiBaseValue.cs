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
        public string Name => "qti-base-value";

        public BaseValue Apply(XElement qtiElement, IProcessingContext _)
        {
            return new BaseValue
            {
                BaseType = qtiElement.GetAttributeValue("base-type").ToBaseType(),
                Value = qtiElement.Value.RemoveXData(),
                Identifier = qtiElement.Identifier()
            };
        }
    }
}
