using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing.BaseValueExpressions;
using Citolab.QTI.ScoringEngine.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace ScoringEngine.Tests.ResponseProcessingTests.BaseValueExpressions
{
   public  class MapResponseTest
    {
        [Fact]
        public void MapA()
        {
            // arrange
            var responseVariable = "A".ToResponseVariable();
            var responseDeclaration = new ResponseDeclaration
            {
                BaseType = BaseType.String,
                Cardinality = Cardinality.Single,
                Identifier = "RESPONSE",
                Mapping = new Mapping
                {
                    MapEntries = new List<MapEntry>
                    {
                        new MapEntry {MapKey = "A", MappedValue = 1},
                        new MapEntry {MapKey = "B", MappedValue = 2},
                        new MapEntry {MapKey = "C", MappedValue = 3}
                    }
                }
            };
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration>(), new List<ResponseDeclaration> { responseDeclaration });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);
            context.ItemResult.ResponseVariables = new List<ResponseVariable> { responseVariable }
                .ToDictionary(r => r.Identifier, r => r);
            var mapResponseElement = XElement.Parse("<mapResponse identifier=\"RESPONSE\"/>");


            var mapResponse = new MapResponse();

            // act
            var score = mapResponse.Calculate(mapResponseElement, context);

            //assert
            Assert.Equal(1, int.Parse(score.Value));
        }
        [Fact]
        public void MapB()
        {
            // arrange
            var responseVariable = "B".ToResponseVariable();
            var responseDeclaration = new ResponseDeclaration
            {
                BaseType = BaseType.String,
                Cardinality = Cardinality.Single,
                Identifier = "RESPONSE",
                Mapping = new Mapping
                {
                    MapEntries = new List<MapEntry>
                    {
                        new MapEntry {MapKey = "A", MappedValue = 1},
                        new MapEntry {MapKey = "B", MappedValue = 2},
                        new MapEntry {MapKey = "C", MappedValue = 3}
                    }
                }
            };
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration>(), new List<ResponseDeclaration> { responseDeclaration });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);
            context.ItemResult.ResponseVariables = new List<ResponseVariable> { responseVariable }
                .ToDictionary(r => r.Identifier, r => r);
            var mapResponseElement = XElement.Parse("<mapResponse identifier=\"RESPONSE\"/>");


            var mapResponse = new MapResponse();

            // act
            var score = mapResponse.Calculate(mapResponseElement, context);

            //assert
            Assert.Equal(2, int.Parse(score.Value));
        }

        [Fact]
        public void MapC()
        {
            // arrange
            var responseVariable = "C".ToResponseVariable();
            var responseDeclaration = new ResponseDeclaration
            {
                BaseType = BaseType.String,
                Cardinality = Cardinality.Single,
                Identifier = "RESPONSE",
                Mapping = new Mapping
                {
                    MapEntries = new List<MapEntry>
                    {
                        new MapEntry {MapKey = "A", MappedValue = 1},
                        new MapEntry {MapKey = "B", MappedValue = 2},
                        new MapEntry {MapKey = "C", MappedValue = 3}
                    }
                }
            };
            var assessmentItem = TestHelper.CreateAssessmentItem(new List<OutcomeDeclaration>(), new List<ResponseDeclaration> { responseDeclaration });
            var context = TestHelper.GetDefaultResponseProcessingContext(assessmentItem);
            context.ItemResult.ResponseVariables = new List<ResponseVariable> { responseVariable }
                .ToDictionary(r => r.Identifier, r => r);
            var mapResponseElement = XElement.Parse("<mapResponse identifier=\"RESPONSE\"/>");


            var mapResponse = new MapResponse();

            // act
            var score = mapResponse.Calculate(mapResponseElement, context);

            //assert
            Assert.Equal(3, int.Parse(score.Value));
        }
    }
}
