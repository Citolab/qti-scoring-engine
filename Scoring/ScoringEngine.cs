using Citolab.QTI.ScoringEngine.Model;
using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.Helpers;
using System.Threading.Tasks;
using Citolab.QTI.ScoringEngine.OutcomeProcessing;
using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.ResponseProcessing;
using System.Collections.Concurrent;

namespace Citolab.QTI.ScoringEngine
{
    public class ScoringEngine : IScoringEngine
    {
        public List<XDocument> ProcessOutcomes(IOutcomeProcessingContext ctx)
        {
            if (ctx == null)
            {
                throw new ScoringEngineException("context cannot be null");
            }
            if (ctx.AssessmentTest == null)
            {
                throw new ScoringEngineException("AssessmentTest cannot be null when calling outcomeProcessing");
            }
            if (ctx.Logger == null)
            {
                ctx.Logger = ctx.Logger = new NullLogger<ScoringEngine>();
            }
            // built per call: caching it meant CustomOperators passed to a later call were dropped.
            var expressionFactory = new ExpressionFactory(ctx.CustomOperators, ctx.Logger);
            var assessmentTest = new AssessmentTest(ctx.Logger, ctx.AssessmentTest, expressionFactory);

            if (ctx.ProcessParallel == true)
            {
                // written by index so the results come back in the order they were handed in.
                var processed = new XDocument[ctx.AssessmentmentResults.Count];
                Parallel.For(0, ctx.AssessmentmentResults.Count,
                  index =>
                  {
                      var assessmentResultDoc = ctx.AssessmentmentResults[index];
                      processed[index] = AssessmentResultOutcomeProcessing(assessmentResultDoc, assessmentTest, ctx.Logger);
                  });
                ctx.AssessmentmentResults = processed.ToList();
            }
            else
            {
                ctx.AssessmentmentResults = ctx.AssessmentmentResults.Select(assessmentResultDoc =>
                {
                    var processedAssessmentResult = AssessmentResultOutcomeProcessing(assessmentResultDoc, assessmentTest, ctx.Logger);
                    return processedAssessmentResult;
                })
              .OfType<XDocument>()
              .ToList();
            }


            //}
            return ctx.AssessmentmentResults;
        }

        public List<XDocument> ProcessResponses(IResponseProcessingContext ctx, ResponseProcessingScoringsOptions options = null)
        {
            if (ctx == null)
            {
                throw new ScoringEngineException("context cannot be null");
            }
            if (ctx.AssessmentItems == null)
            {
                throw new ScoringEngineException("AssessmentItems cannot be null when calling responseProcessing");
            }
            if (ctx.Logger == null)
            {
                ctx.Logger = ctx.Logger = new NullLogger<ScoringEngine>();
            }
            // built per call: caching it meant CustomOperators passed to a later call were dropped.
            var expressionFactory = new ExpressionFactory(ctx.CustomOperators, ctx.Logger);
            var assessmentItems = ctx.AssessmentItems
                .Select(assessmentItemDoc => new AssessmentItem(ctx.Logger, assessmentItemDoc, expressionFactory))
                .ToList();
            // Indexed once, so each result only walks the items it actually has a result for
            // instead of every item in the test.
            var assessmentItemsByIdentifier = new Dictionary<string, AssessmentItem>();
            foreach (var assessmentItem in assessmentItems)
            {
                if (assessmentItem.Identifier == null)
                {
                    ctx.Logger.LogError("Skipping an assessmentItem without an identifier.");
                }
                else if (assessmentItemsByIdentifier.ContainsKey(assessmentItem.Identifier))
                {
                    ctx.Logger.LogWarning($"More than one assessmentItem has identifier: {assessmentItem.Identifier}. Using the first.");
                }
                else
                {
                    assessmentItemsByIdentifier.Add(assessmentItem.Identifier, assessmentItem);
                }
            }
            if (ctx.ProcessParallel == true)
            {
                // written by index so the results come back in the order they were handed in.
                var processed = new XDocument[ctx.AssessmentmentResults.Count];
                Parallel.For(0, ctx.AssessmentmentResults.Count,
                  index =>
                  {
                      var assessmentResultDoc = ctx.AssessmentmentResults[index];
                      processed[index] = AssessmentResultResponseProcessing(assessmentResultDoc, assessmentItemsByIdentifier, ctx.Logger, options);
                  });
                ctx.AssessmentmentResults = processed.ToList();
            }
            else
            {
                ctx.AssessmentmentResults = ctx.AssessmentmentResults
              .Select(assessmentResultDoc => (XDocument)AssessmentResultResponseProcessing(assessmentResultDoc, assessmentItemsByIdentifier, ctx.Logger, options))
              .ToList();
            }

            //}
            return ctx.AssessmentmentResults;
        }

        public List<XDocument> ProcessResponsesAndOutcomes(IScoringContext ctx, ResponseProcessingScoringsOptions options = null)
        {
            ProcessResponses(ctx, options);
            ProcessOutcomes(ctx);
            return ctx.AssessmentmentResults;
        }


        private AssessmentResult AssessmentResultOutcomeProcessing(XDocument assessmentResultDocument, AssessmentTest assessmentTest, ILogger logger)
        {
            var assessmentResult = new AssessmentResult(logger, assessmentResultDocument);
            assessmentResult = OutcomeProcessor.Process(assessmentTest, assessmentResult, logger);
            return assessmentResult;
        }

        private AssessmentResult AssessmentResultResponseProcessing(XDocument assessmentResultDocument, Dictionary<string, AssessmentItem> assessmentItemsByIdentifier, ILogger logger, ResponseProcessingScoringsOptions options = null)
        {
            var assessmentResult = new AssessmentResult(logger, assessmentResultDocument);
            // An item the candidate has no itemResult for is a no-op in ResponseProcessor, so
            // driving the loop from the result skips building a context for every unanswered item.
            foreach (var itemIdentifier in assessmentResult.ItemResults.Keys)
            {
                if (assessmentItemsByIdentifier.TryGetValue(itemIdentifier, out var assessmentItem))
                {
                    assessmentResult = ResponseProcessor.Process(assessmentItem, assessmentResult, logger, options);
                }
            }
            return assessmentResult;
        }
    }
}
