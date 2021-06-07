using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class Null : IGeneralExpression
    {
        public string Name => "qti-null";

        public BaseValue Apply(XElement qtiElement, IProcessingContext _)
        {
            return null;
        }
    }
}
