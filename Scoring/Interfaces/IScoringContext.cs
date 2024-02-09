using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    public interface IScoringContext : IResponseProcessingContext, IOutcomeProcessingContext
    {
    }
    public interface IResponseProcessingContext : IScoringContextBase
    {
        List<XDocument> AssessmentItems { get; set; }
        ResponseProcessingScoringsOptions Options { get; set; }
    }
    public interface IOutcomeProcessingContext : IScoringContextBase
    {
        XDocument AssessmentTest { get; set; }
    }
    public interface IScoringContextBase
    {
        Dictionary<string, ICustomOperator> CustomOperators { get; set; }
        List<XDocument> AssessmentmentResults { get; set; }
        ILogger Logger { get; set; }

        bool? ProcessParallel { get; set; }
    }
}
