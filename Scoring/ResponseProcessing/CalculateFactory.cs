using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    public static class CalculateFactory
    {
        private static Dictionary<string, ICalculateResponseProcessing> _calculators;
        public static ICalculateResponseProcessing GetCalculator(XElement element, ResponseProcessorContext context, bool logErrorIfNotFound = false)
        {
            if (_calculators == null)
            {
                var type = typeof(ICalculateResponseProcessing);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (ICalculateResponseProcessing)Activator.CreateInstance(t));

                _calculators = instances.ToDictionary(t => t.Name, t => t);
            }
            if (_calculators.TryGetValue(element?.Name.LocalName, out var calculator))
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
    }
}
