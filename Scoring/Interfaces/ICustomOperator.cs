using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.Scoring.Interfaces
{
    internal interface ICustomOperator
    {
        string Type { get; }
        BaseValue Apply(BaseValue value);
    }
}
