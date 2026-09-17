using System;

namespace Citolab.QTI.ScoringEngine.ResponseProcessing
{
    /// <summary>
    /// Thrown by qti-exit-response to unwind out of the rules that are still pending.
    /// ResponseProcessor catches it and persists the outcomes that were set before it.
    /// </summary>
    internal class ExitResponseException : Exception
    {
        internal ExitResponseException() : base("qti-exit-response")
        {
        }
    }
}
