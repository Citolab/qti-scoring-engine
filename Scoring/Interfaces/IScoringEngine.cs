using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    public interface IScoringEngine
    {
        List<XDocument> ProcessResponses(IResponseProcessingContext ctx, ResponseProcessingScoringsOptions options = null);
        List<XDocument> ProcessOutcomes(IOutcomeProcessingContext ctx);
        List<XDocument> ProcessResponsesAndOutcomes(IScoringContext ctx, ResponseProcessingScoringsOptions options = null);
    }

    public class ResponseProcessingScoringsOptions
    {
        public bool StripAlphanumericsFromNumericResponses = false;
    }
}
