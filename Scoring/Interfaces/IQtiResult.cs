using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IQtiResult
    {
        string Identifier { get; set; } 
        Dictionary<string, ResponseVariable> ResponseVariables { get; set; }
        Dictionary<string, OutcomeVariable> OutcomeVariables  { get; set; }
        /// <summary>
        /// The element this result was read from, kept so writing an outcome back does not
        /// have to rescan the whole assessmentResult document to find it again.
        /// </summary>
        XElement SourceElement { get; set; }
        XElement ToElement();
}
}
