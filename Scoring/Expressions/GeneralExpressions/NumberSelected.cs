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
    internal class NumberSelected : IOutcomeProcessingExpression
    {
        public string Name => "qti-number-selected";

        public BaseValue Apply(XElement qtiElement, IProcessingContext ctx)
        {
            var outcomeProcessorContext = (OutcomeProcessorContext)ctx;

            var excludedCategoriesString = qtiElement.GetAttributeValue("exclude-category");
            var excludedCategories = !string.IsNullOrWhiteSpace(excludedCategoriesString) ?
                excludedCategoriesString.Split(' ') : null;

            var includeCategoriesString = qtiElement.GetAttributeValue("include-category");
            var includeCategories = !string.IsNullOrWhiteSpace(includeCategoriesString) ?
            includeCategoriesString.Split(' ') : null;

            var itemRefs = outcomeProcessorContext.AssessmentTest.AssessmentItemRefs.Values.Where(assessmentItemRef =>
            {
                if (excludedCategories?.Length > 0)
                {
                    foreach (var excludedCategory in excludedCategories)
                    {
                        if (assessmentItemRef.Categories.Contains(excludedCategory))
                        {
                            return false;
                        }
                    }
                }
                if (includeCategories?.Length > 0)
                {
                    foreach (var includeCategory in includeCategories)
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
            var count = itemRefs.Count(itemRef => {
                return outcomeProcessorContext.ItemResultExists(itemRef.Identifier);
            });
            return count.ToBaseValue();
        }
    }
}
