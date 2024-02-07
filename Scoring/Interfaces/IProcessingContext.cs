using Citolab.QTI.ScoringEngine.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IProcessingContext
    {
        Dictionary<string, ResponseDeclaration> ResponseDeclarations { get; set; }
        Dictionary<string, OutcomeDeclaration> OutcomeDeclarations { get; set; }
        Dictionary<string, ResponseVariable> ResponseVariables { get; set; }
        Dictionary<string, OutcomeVariable> OutcomeVariables { get; set; }
        HashSet<string> CalculatedOutcomes { get; set; }
        
        bool? StripAlphanumericsFromNumericResponses { get; set; }
        void LogInformation(string value);
        void LogWarning(string value);
        void LogError(string value);
    }
}
