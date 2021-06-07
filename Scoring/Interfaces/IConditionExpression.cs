using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IConditionExpression
    {
        string Name { get; }
        bool Execute(XElement qtiElement, IProcessingContext context);
    }
}
