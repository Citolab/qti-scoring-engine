﻿using Citolab.QTI.ScoringEngine.ResponseProcessing;
using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;

namespace Citolab.QTI.ScoringEngine.OutcomeProcessing.BaseValueExpression
{
    internal class Round : ValueExpressionBase
    {
        public override BaseValue Apply(IProcessingContext ctx)
        {
            var baseValues = expressions.Select(e => e.Apply(ctx)).ToList();
            if (baseValues.Count == 1)
            {
                var baseValue = baseValues.FirstOrDefault();
                if (baseValue == null) return null;
                var culture = CultureInfo.InvariantCulture;
                if (baseValue.Value != null && baseValue.Value.ToString().TryParseFloat(out var result))
                {
                    baseValue.Value = Math.Round(result, MidpointRounding.AwayFromZero).ToString(culture);
                }
                return baseValue;
            }
            else
            {
                return 0.0f.ToBaseValue();
            }

        }
    }
}
