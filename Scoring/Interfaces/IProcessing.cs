using System.Collections.Generic;
using System.Xml.Linq;
using Citolab.QTI.Scoring.Model;

namespace Citolab.QTI.Scoring.Interfaces
{
    internal interface IProcessing
    {
        XDocument Score(XDocument assessmentTest, List<XDocument> AssessmentItems, XDocument AssessmentResult);
    }
}