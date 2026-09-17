using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Interfaces;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AssessmentTest : QtiDocument
    {
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations;
        public Dictionary<string, AssessmentItemRef> AssessmentItemRefs;
        public List<string> Categories;
        public HashSet<string> CalculatedOutcomes;
        public XElement OutcomeProcessingElement => Content.FindElementByName("qti-outcome-processing");

        public List<IConditionExpression> Expressions { get; } = new List<IConditionExpression>();
        public AssessmentTest(ILogger logger, XDocument assessmentTest, IExpressionFactory expressionFactory) : base(logger, assessmentTest)
        {
            Init(expressionFactory);
        }

        public void Init(IExpressionFactory expressionFactory)
        {
            OutcomeDeclarations = Content.FindElementsByName("qti-outcome-declaration").Select(outcomeDeclaration =>
            {
                return GetOutcomeDeclaration(outcomeDeclaration);
            }).ToDictionary(o => o.Identifier, o => o);

            AssessmentItemRefs = Content.FindElementsByName("qti-assessment-item-ref")
               .Select(assessmentItemRefElement =>
               {
                   var itemIdentifier = assessmentItemRefElement.Identifier();

                   var assessmentItemRef = new AssessmentItemRef
                   {
                       Identifier = itemIdentifier,
                       Weights = assessmentItemRefElement.FindElementsByName("qti-weight").Select(weight =>
                       {
                           var weightString = weight.GetAttributeValue("value");
                           // parse weight to float with invariant culture
                           if (weightString.TryParseFloat(out var weightValue))
                           {
                               return new { Value = weightValue, Identifier = weight.Identifier() };
                           }
                           else
                           {
                               Logger.LogError($"sourceId weight: {weightString} cannot be parsed to int.");
                               return null;
                           }
                       }).Where(w => w != null)
                       .ToDictionary(w => w.Identifier, w => w.Value),
                       Categories = assessmentItemRefElement.GetAttributeValue("category").Split(' ')?.ToHashSet(),
                   };
                   return assessmentItemRef;
               }).ToDictionary(a => a.Identifier, a => a);
            Categories = AssessmentItemRefs.Values
                .SelectMany(itemRef => itemRef.Categories)
                .Distinct()
                .ToList();

            var setOutcomeElements = OutcomeProcessingElement?
              .FindElementsByName("qti-set-outcome-value");
            var setOutcomes = setOutcomeElements?.Select(v => v.Identifier()) ?? Enumerable.Empty<string>();
            CalculatedOutcomes = setOutcomes.Distinct().ToHashSet();

            if (OutcomeProcessingElement != null)
            {
                foreach (var outcomeProcessingChild in OutcomeProcessingElement.Elements())
                {
                    // an unknown or unsupported rule comes back null; it is already logged
                    var expression = expressionFactory.GetConditionExpression(outcomeProcessingChild, true);
                    if (expression != null)
                    {
                        Expressions.Add(expression);
                    }
                }
            }
        }

        /// <summary>Upgrades a document that is not wrapped in an AssessmentTest yet.</summary>
        public static void Upgrade(XDocument doc) => UpgradeToQti3(doc);
    }
}
