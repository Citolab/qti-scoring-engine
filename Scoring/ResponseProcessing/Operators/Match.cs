using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing.Operators
{
    internal class Match : IResponseProcessingOperator
    {
        public string Name { get => "match"; }

        public bool Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return Helper.CompareTwoValues(qtiElement, context);
        }

    }
}