using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.Scoring.Interfaces
{
    public interface ICustomOperator
    {
        string Definition { get; }
        BaseValue Apply(BaseValue value);
    }
}
