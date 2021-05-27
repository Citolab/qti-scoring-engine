using Citolab.QTI.Scoring.Interfaces;
using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.Scoring.ResponseProcessing.CustomOperators
{
    internal class ParseCommaDecimal : ICustomOperator
    {
        public string Type { get => "depcp:ParseCommaDecimal"; }

        public BaseValue Apply(BaseValue value)
        {
            if (value != null)
            {
                value.Value = value.Value.Replace(",", ".");
            }
            return value;
        }
    }
}
