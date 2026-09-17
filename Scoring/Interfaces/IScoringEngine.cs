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

        /// <summary>
        /// Fields to add to the QTI_CONTEXT record, by field identifier. The engine fills
        /// candidateIdentifier and testIdentifier from the assessmentResult itself; everything
        /// else, environmentIdentifier above all, is only known to the delivery engine.
        /// A field given here overrides the one the engine worked out.
        /// </summary>
        public Dictionary<string, string> QtiContextFields = null;
    }
}
