using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.BooleanExpressions
{
    /// <summary>
    /// All children should return true
    /// </summary>
    internal class And : IBooleanExpression
    {
        public string Name => "and";

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            foreach (var child in qtiElement.Elements())
            {
                var executor = context.GetOperator(child, context);
                var result = executor?.Execute(child, context);
                if (result == false) 
                {
                    return false; // one condition false; return false
                }
            }
            return true; // all children true; return true
        }

    }
}
