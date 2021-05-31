using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.CustomOperators
{
    internal class ParseCommaDecimal : ICustomOperator
    {
        public string Definition { get => "depcp:ParseCommaDecimal"; }

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
