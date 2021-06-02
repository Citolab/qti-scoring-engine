using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Model
{
    internal class AssessmentTest : QtiDocument
    {
        public Dictionary<string, OutcomeDeclaration> OutcomeDeclarations;
        public Dictionary<string, AssessmentItemRef> AssessmentItemRefs;
        public List<string> Categories;
        public HashSet<string> CalculatedOutcomes;
        public XElement OutcomeProcessingElement => Content.FindElementByName("outcomeProcessing");
        public AssessmentTest(ILogger logger, XDocument assessmentTest) : base(logger, assessmentTest)
        {
            Init();
        }

        public void Init()
        {
            OutcomeDeclarations = Content.FindElementsByName("outcomeDeclaration").Select(outcomeDeclaration =>
            {
                return GetOutcomeDeclaration(outcomeDeclaration);
            }).ToDictionary(o => o.Identifier, o => o);

            AssessmentItemRefs = Content.FindElementsByName("assessmentItemRef")
               .Select(assessmentItemRefElement =>
               {
                   var itemIdentifier = assessmentItemRefElement.Identifier();

                   var assessmentItemRef = new AssessmentItemRef
                   {
                       Identifier = itemIdentifier,
                       Weights = assessmentItemRefElement.FindElementsByName("weight").Select(weight =>
                       {
                           var weightString = weight.GetAttributeValue("value");
                           if (int.TryParse(weightString, out var weightValue))
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
            var setOutcomes = OutcomeProcessingElement?
              .FindElementsByName("setOutComeValue")?
              .Select(v => v.Identifier());
            CalculatedOutcomes = setOutcomes.Distinct().ToHashSet();
        }


    }
}
