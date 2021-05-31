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
        XElement ToElement();
}
}
