using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.Executors
{
    public class Match : IExecuteReponseProcessing
    {
        public string Name { get => "match"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return Helper.CompareTwoValues(qtiElement, context);
        }

    }
}