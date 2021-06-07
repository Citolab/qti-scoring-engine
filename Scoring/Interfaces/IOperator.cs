using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IOperator: IOperatorBase
    {
        string Name { get;  }
    }
}
