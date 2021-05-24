using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    public interface IProcessingContext
    {
        public string Identifier { get; }
        public XDocument AssessmentResult { get; }
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations { get; }
        public void LogInformation(string value);
        public void LogWarning(string value);

        public void LogError(string value);
    }
}
