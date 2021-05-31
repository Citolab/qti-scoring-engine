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
        //public bool? ProcessParallel { get; set; }
        public List<ICustomOperator> CustomOperators { get; set; } = new List<ICustomOperator>();
    }

    public class ResponseProcessingContext : IResponseProcessingContext
    {
        public List<XDocument> AssessmentItems { get; set; }
        public List<XDocument> AssessmentmentResults { get; set; }
        public List<ICustomOperator> CustomOperators { get; set; } = new List<ICustomOperator>();
        public ILogger Logger { get; set; }
        //public bool? ProcessParallel
        //{
        //    get; set;
        //}
    }

    public class OutcomeProcessingContext : IOutcomeProcessingContext
    {
        public XDocument AssessmentTest { get; set; }
        public List<XDocument> AssessmentmentResults { get; set; }
        public ILogger Logger { get; set; }
        //public bool? ProcessParallel
        //{
        //    get; set;
        //}
    }
}
