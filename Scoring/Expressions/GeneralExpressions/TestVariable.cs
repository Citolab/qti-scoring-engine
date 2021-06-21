using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.OutcomeProcessing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.GeneralExpressions
{
    internal class TestVariable : IOutcomeProcessingExpression
    {
        private string[] _excludedCategories;
        private string[] _includeCategories;
        private string _outcomeIdentifier;
        private string _weightIdentifier;

        public string Name => "qti-test-variables";

        public BaseValue Apply(IProcessingContext ctx)
        {
            var outcomeProcessorContext = (OutcomeProcessorContext)ctx;
                       
            var itemRefs = outcomeProcessorContext.AssessmentTest.AssessmentItemRefs.Values.Where(assessmentItemRef =>
            {
                if (_excludedCategories?.Length > 0)
                {
                    foreach (var excludedCategory in _excludedCategories)
                    {
                        if (assessmentItemRef.Categories.Contains(excludedCategory))
                        {
                            return false;
                        }
                    }
                }
                if (_includeCategories?.Length > 0)
                {
                    foreach (var includeCategory in _includeCategories)
                    {
                        if (assessmentItemRef.Categories.Contains(includeCategory))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return true;
            });

            if (string.IsNullOrWhiteSpace(_outcomeIdentifier))
            {
                ctx.LogError("variable-identifier is required");
                return 0.0F.ToBaseValue();
            }
            var style = NumberStyles.Float;
            var culture = CultureInfo.InvariantCulture;
            var values = itemRefs.Select(itemRef =>
            {

                return outcomeProcessorContext.GetItemResultBaseValue(itemRef.Identifier, _outcomeIdentifier, _weightIdentifier);
            }).Select(value => float.Parse(value.Value, style, culture));

            return values.Sum().ToBaseValue();
        }

        public void Init(XElement qtiElement)
        {
            var excludedCategoriesString = qtiElement.GetAttributeValue("exclude-category");
            _excludedCategories = !string.IsNullOrWhiteSpace(excludedCategoriesString) ?
                excludedCategoriesString.Split(' ') : null;

            var includeCategoriesString = qtiElement.GetAttributeValue("include-category");
            _includeCategories = !string.IsNullOrWhiteSpace(includeCategoriesString) ?
            includeCategoriesString.Split(' ') : null;

            _outcomeIdentifier = qtiElement.GetAttributeValue("variable-identifier");
            _weightIdentifier = qtiElement.GetAttributeValue("weight-identifier");
        }
    }
}
