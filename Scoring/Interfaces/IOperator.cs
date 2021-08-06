using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IOperator : IOperatorBase
    {
        List<ProcessingType> UnsupportedProcessingTypes { get; }
    }
}
