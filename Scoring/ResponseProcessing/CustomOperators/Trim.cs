using Citolab.QTI.Scoring.Interfaces;
using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Citolab.QTI.Scoring.ResponseProcessing.CustomOperators
{
    internal class Trim : ICustomOperator
    {
        public string Definition => "depcp:Trim";

        public BaseValue Apply(BaseValue value)
        {
            if (value?.Value != null)
            {
                value.Value = value.Value.Trim();
            }
            return value;
        }
    }
}
