using Citolab.QTI.Scoring.Interfaces;
using Citolab.QTI.Scoring.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Citolab.QTI.Scoring.ResponseProcessing.CustomOperators
{
    internal class ToAscii : ICustomOperator
    {
        public string Definition => "depcp:ToAscii";

        public BaseValue Apply(BaseValue value)
        {
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
