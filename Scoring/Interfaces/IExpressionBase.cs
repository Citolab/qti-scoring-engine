using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IExpressionBase
    {
        string Name { get; }
        void Init(XElement qtiElement);
    }
}
