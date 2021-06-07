using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IExpression
    {
        string Name { get; }

        BaseValue Apply(XElement qtiElement, IProcessingContext ctx);
    }
}
