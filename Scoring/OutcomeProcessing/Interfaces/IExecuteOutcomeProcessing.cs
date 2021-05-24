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
    public interface IExecuteOutcomeProcessing
    {
        string Name { get; }
        bool Execute(XElement qtiElement, OutcomeProcessContext context);
    }
}
