using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    internal class StringMatch : IConditionExpression
    {
        public string Name { get => "qti-string-match"; }

        public bool Execute(XElement qtiElement, IProcessingContext context)
        {
            return Helper.CompareTwoValues(qtiElement, context, Model.BaseType.String);
        }

    }
}