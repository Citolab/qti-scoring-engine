using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal abstract class ConditionExpressionBase : IConditionExpression
    {
        public virtual List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType>();

        protected List<IValueExpression> expressions;
        protected List<IConditionExpression> conditionalExpressions;
        protected Dictionary<string, string> attributes;
        public abstract bool Execute(IProcessingContext context);

        public virtual void Init(XElement qtiElement, IExpressionFactory expressionFactory)
        {
            conditionalExpressions = qtiElement.Elements()
                .Select(childElement => expressionFactory.GetConditionExpression(childElement, false))
                .Where(expression => expression != null)
                .ToList();
            expressions = qtiElement.Elements()
                .Select(childElement => expressionFactory.GetValueExpression(childElement, false))
                .Where(expression => expression != null)
                .ToList();
            attributes = qtiElement.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value);
        }

        public string GetAttributeValue(string name)
        {
            return attributes.ContainsKey(name) ? attributes[name] : "";
        }
    }
}
