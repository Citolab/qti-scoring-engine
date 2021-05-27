using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.Scoring.Interfaces
{
    internal interface IProcessingContext
    {
        string Identifier { get; }
        XDocument AssessmentResult { get; }
        Dictionary<string, OutcomeDeclaration> OutcomeDeclarations { get; }
        void LogInformation(string value);
        void LogWarning(string value);
        void LogError(string value);
    }
}
