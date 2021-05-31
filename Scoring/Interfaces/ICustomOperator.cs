using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    public interface ICustomOperator
    {
        string Definition { get; }
        BaseValue Apply(BaseValue value);
    }
}
