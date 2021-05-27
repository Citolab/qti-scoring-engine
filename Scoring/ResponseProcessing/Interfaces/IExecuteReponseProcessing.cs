using Microsoft.Extensions.Logging;
using Citolab.QTI.Scoring.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.Scoring.ResponseProcessing.Interfaces
{
    internal interface IExecuteReponseProcessing
    {
        string Name { get; }
        bool Execute(XElement qtiElement, ResponseProcessorContext context);
    }
}
