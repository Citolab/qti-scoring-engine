﻿using Microsoft.Extensions.Logging;
using Moq;
using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Citolab.QTI.ScoringEngine.Tests;

namespace ScoringEngine.Tests.ResponseProcessingTests.ItemTypes
{
    public class InlineChoiceInteractionTests
    {
        [Fact]
        public void IMS_ExampleChoiceInlineResponseProcessing_Correct()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/inline_choice.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", "Y"
                , BaseType.Identifier, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("1", result);
        }

        [Fact]
        public void IMS_ExampleChoiceInlineResponseProcessing_Incorrect()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/inline_choice.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddCandidateResponse
                (assessmentItem.Identifier, "RESPONSE", "L"
                , BaseType.Identifier, Cardinality.Single);

            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }

        [Fact]
        public void IMS_ExampleChoiceInlineResponseProcessing_Missing()
        {
            var logger = new Mock<ILogger>().Object;

            var assessmentItem = new AssessmentItem(logger, XDocument.Load(File.OpenRead("Resources/2x/ResponseProcessing/IMS-examples/inline_choice.xml")), TestHelper.GetExpressionFactory());
            var assessmentResult = TestHelper.GetBasicAssessmentResult();
            assessmentResult.AddItemResult(assessmentItem.Identifier);
            ResponseProcessor.Process(assessmentItem, assessmentResult, logger);

            var result = assessmentResult.GetScoreForItem(assessmentItem.Identifier, "SCORE");
            Assert.Equal("0", result);
        }
    }
}
