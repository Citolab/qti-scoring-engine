﻿using Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions;
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
    internal class NumberSelected : ValueExpressionBase
    {
        private string[] _excludedCategories;
        private string[] _includeCategories;
        public override List<ProcessingType> UnsupportedProcessingTypes => new List<ProcessingType> { ProcessingType.ResponseProcessig };
        public override BaseValue Apply(IProcessingContext ctx)
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
            var count = itemRefs.Count(itemRef =>
            {
                return outcomeProcessorContext.ItemResultExists(itemRef.Identifier);
            });
            return count.ToBaseValue();
        }

        public BaseValue Apply(List<IValueExpression> childExpressions, IProcessingContext ctx)
        {
            throw new NotImplementedException();
        }

        public override void Init(XElement qtiElement, IExpressionFactory expressionFactory)
        {
            var excludedCategoriesString = qtiElement.GetAttributeValue("exclude-category");
            _excludedCategories = !string.IsNullOrWhiteSpace(excludedCategoriesString) ?
                excludedCategoriesString.Split(' ') : null;

            var includeCategoriesString = qtiElement.GetAttributeValue("include-category");
            _includeCategories = !string.IsNullOrWhiteSpace(includeCategoriesString) ?
            includeCategoriesString.Split(' ') : null;
        }
    }
}
