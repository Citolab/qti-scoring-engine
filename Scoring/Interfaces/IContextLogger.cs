using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Citolab.QTI.ScoringEngine.Interfaces
{
    internal interface IContextLogger
    {
        void LogInformation(string value);
        void LogWarning(string value);

        void LogError(string value);
    }
}
