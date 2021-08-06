using Citolab.QTI.ScoringEngine.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine
{
    public class ScoringContext : IScoringContext
    {
        public List<XDocument> AssessmentItems { get; set; }
        public XDocument AssessmentTest { get; set; }
        public List<XDocument> AssessmentmentResults { get; set; }
        public ILogger Logger { get; set; }
        public Dictionary<string, ICustomOperator> CustomOperators { get; set; } = new Dictionary<string, ICustomOperator>();
        public bool? ProcessParallel { get; set; }
    }

    public class ResponseProcessingContext : IResponseProcessingContext
    {
        public List<XDocument> AssessmentItems { get; set; }
        public List<XDocument> AssessmentmentResults { get; set; }
        public ILogger Logger { get; set; }
        public Dictionary<string, ICustomOperator> CustomOperators { get; set; } = new Dictionary<string, ICustomOperator>();
        public bool? ProcessParallel { get; set; }
    }

    public class OutcomeProcessingContext : IOutcomeProcessingContext
    {
        public XDocument AssessmentTest { get; set; }
        public List<XDocument> AssessmentmentResults { get; set; }
        public ILogger Logger { get; set; }
        public Dictionary<string, ICustomOperator> CustomOperators { get; set; } = new Dictionary<string, ICustomOperator>();
        public bool? ProcessParallel { get; set; }
    }
}
