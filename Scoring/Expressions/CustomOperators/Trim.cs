using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.CustomOperators
{
    public class Trim : ICustomOperator
    {
        public BaseValue Apply(List<BaseValue> values)
        {
            var value = values.FirstOrDefault();
            if (value?.Value != null)
            {
                value.Value = value.Value.Trim();
            }
            return value;
        }
    }
}
