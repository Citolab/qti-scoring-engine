using System.Collections.Generic;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Model;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    public interface IProcessing
    {
        XDocument Score(XDocument assessmentTest, List<XDocument> AssessmentItems, XDocument AssessmentResult);
    }
}