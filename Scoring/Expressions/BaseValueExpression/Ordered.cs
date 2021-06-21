using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.BaseValueExpression
{
    internal class Ordered : IExpression
    {
        public string Name => "qti-ordered";

        public BaseValue Apply(List<IExpression> childExpressions, IProcessingContext ctx)
        {
            var values = childExpressions.Select(e => e.Apply(ctx)).ToList();
            return new BaseValue
            {
                Identifier = "ordered",
                BaseType = values.Any() ? values[0].BaseType : BaseType.Identifier,
                Cardinality = Cardinality.Ordered,
                Values = values?.Select(v => v.Value).ToList()
            };
        }

        public void Init(XElement qtiElement) { }
    }
}
