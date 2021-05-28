
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.Scoring.ResponseProcessing.Interfaces;

namespace Citolab.QTI.Scoring.ResponseProcessing.Operators
{
    internal class Member : IResponseProcessingOperator
    {
        public string Name { get => "member"; }

        bool IResponseProcessingOperator.Execute(XElement qtiElement, ResponseProcessorContext context)
        {
            return Helper.CompareTwoValues(qtiElement, context);
        }
    }
}
