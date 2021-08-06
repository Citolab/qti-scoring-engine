using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.CustomOperators
{
    internal class ParseCommaDecimal : ICustomOperator
    {
        public BaseValue Apply(List<BaseValue> values)
        {
            var value = values.FirstOrDefault();
            if (value != null)
            {
                value.Value = value.Value.Replace(",", ".");
            }
            return value;
        }
    }
}
