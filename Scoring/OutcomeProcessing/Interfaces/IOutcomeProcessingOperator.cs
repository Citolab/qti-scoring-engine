using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing
{
    internal interface IOutcomeProcessingOperator
    {
        string Name { get; }
        bool Execute(XElement qtiElement, OutcomeProcessorContext context);
    }
}
