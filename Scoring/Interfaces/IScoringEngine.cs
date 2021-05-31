using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    public interface IScoringEngine
    {
        List<XDocument> ProcessResponses(IResponseProcessingContext ctx);
        List<XDocument> ProcessOutcomes(IOutcomeProcessingContext ctx);
        List<XDocument> ProcessResponsesAndOutcomes(IScoringContext ctx);
    }
}
