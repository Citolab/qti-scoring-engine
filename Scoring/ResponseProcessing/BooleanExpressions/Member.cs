
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions
{
    internal class Member : IBooleanExpression
    {
        public string Name { get => "qti-member"; }

        bool IBooleanExpression.Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return Helper.ValueIsMemberOf(qtiElement, context);
        }
    }
}
