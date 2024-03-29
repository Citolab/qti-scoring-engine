﻿using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing.CustomOperators
{
    internal class ToAscii : ICustomOperator
    {
        public BaseValue Apply(List<BaseValue> values)
        {
            var value = values.FirstOrDefault();
            if (value?.Value != null)
            {
                value.Value = RemoveDiacritics(value.Value);
            }
            return value;
        }
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
