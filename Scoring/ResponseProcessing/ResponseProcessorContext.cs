using Microsoft.Extensions.Logging;
using Citolab.QTI.ScoringEngine.Const;
using Citolab.QTI.ScoringEngine.Helper;
using Citolab.QTI.ScoringEngine.Interfaces;
using Citolab.QTI.ScoringEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    public class ResponseProcessorContext
    {

        private readonly ILogger _logger;
        public AssessmentResult AssessmentResult { get; }
        public AssessmentItem AssessmentItem { get; }
        public ItemResult ItemResult { get; set; }
        public Dictionary<string, IExecuteReponseProcessing> Executors;
        public Dictionary<string, ICalculateResponseProcessing> Calculators;
        public ResponseProcessorContext(ILogger logger, AssessmentResult assessmentResult, AssessmentItem assessmentItem)
        {
            _logger = logger;
            AssessmentResult = assessmentResult;
            AssessmentItem = assessmentItem;
            if (AssessmentItem != null && AssessmentResult.ItemResults.ContainsKey(AssessmentItem.Identifier))
            {
                ItemResult = AssessmentResult.ItemResults[AssessmentItem.Identifier];
            }
            else
            {
                LogWarning("Item result not found. Skipping ResponseProcessing");
            }
        }

        public ICalculateResponseProcessing GetCalculator(XElement element, ResponseProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (Calculators == null)
            {
                var type = typeof(ICalculateResponseProcessing);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (ICalculateResponseProcessing)Activator.CreateInstance(t));

                Calculators = instances.ToDictionary(t => t.Name, t => t);
            }
            if (Calculators.TryGetValue(element?.Name.LocalName, out var calculator))
            {
                context.LogInformation($"Processing {calculator.Name}");
                return calculator;
            }
            if (logErrorIfNotFound)
            {
                context.LogError($"Cannot find calculator for tag-name:{element?.Name.LocalName}");
            }
            return null;
        }

        public IExecuteReponseProcessing GetExecutor(XElement element, ResponseProcessorContext context)
        {
            if (Executors == null)
            {
                var type = typeof(IExecuteReponseProcessing);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IExecuteReponseProcessing)Activator.CreateInstance(t));

                Executors = instances.ToDictionary(t => t.Name, t => t);
            }
            if (Executors.TryGetValue(element?.Name.LocalName, out var executor))
            {
                context.LogInformation($"Processing {executor.Name}");
                return executor;
            }
            context.LogError($"Cannot find executor for tag-name:{element?.Name.LocalName}");
            return null;
        }
        public void LogInformation(string value)
        {
            _logger.LogInformation($"{AssessmentItem?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }
        public void LogWarning(string value)
        {
            _logger.LogWarning($"{AssessmentItem?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }

        public void LogError(string value)
        {
            _logger.LogError($"{AssessmentItem?.Identifier} - {AssessmentResult?.SourcedId}: {value}");
        }
    }
}
