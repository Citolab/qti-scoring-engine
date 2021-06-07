using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    interface IOperatorWrapper : IExpression { }
    internal class OperatorWrapper : IOperatorWrapper
    {
        private readonly IOperatorBase _operatorBase;
        private string _name;
        public OperatorWrapper(IOperatorBase operatorBase, string name)
        {
            _operatorBase = operatorBase;
            _name = name;
        }
        public string Name => _name;

        public BaseValue Apply(XElement qtiElement, IProcessingContext ctx)
        {
            var firstElement = qtiElement.Elements().FirstOrDefault();
            var baseValue = ctx.GetValue(firstElement);
            return _operatorBase.Apply(baseValue);
        }
    }
}
