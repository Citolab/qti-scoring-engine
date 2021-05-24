
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Executors
{
    public class Member : IExecuteReponseProcessing
    {
        public string Name { get => "member"; }

        bool IExecuteReponseProcessing.Execute(XElement qtiElement, ResponseProcessingContext context)
        {
            return Helper.CompareTwoValues(qtiElement, context);
        }
    }
}
