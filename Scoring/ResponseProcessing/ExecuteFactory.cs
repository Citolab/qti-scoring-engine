using Citolab.QTI.ScoringEngine.ResponseProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    public static class ExecuteFactory
    {
        private static Dictionary<string, IExecuteReponseProcessing> _executors;
        public static IExecuteReponseProcessing GetExecutor(XElement element, ResponseProcessorContext context)
        {
            if (_executors == null)
            {
                var type = typeof(IExecuteReponseProcessing);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                      .SelectMany(s => s.GetTypes())
                      .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var instances = types.Select(t => (IExecuteReponseProcessing)Activator.CreateInstance(t));

                _executors = instances.ToDictionary(t => t.Name, t => t);
            }
            if (_executors.TryGetValue(element?.Name.LocalName, out var executor))
            {
                context.LogInformation($"Processing {executor.Name}");
                return executor;
            }
            context.LogError($"Cannot find executor for tag-name:{element?.Name.LocalName}");
            return null;
        }
    }
}
