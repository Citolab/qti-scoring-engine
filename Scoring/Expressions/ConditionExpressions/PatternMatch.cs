using Citolab.QTI.ScoringEngine.Helpers;
using Citolab.QTI.ScoringEngine.Interfaces;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Citolab.QTI.ScoringEngine.Expressions.ConditionExpressions
{
    /// <summary>
    /// qti-pattern-match: whether a string matches the regular expression in the pattern
    /// attribute. The pattern has to match the whole value, as XML Schema patterns do.
    /// </summary>
    internal class PatternMatch : ConditionExpressionBase
    {
        // a pattern comes from the item, so it is built once and guarded against a pattern
        // that backtracks forever on an unexpected candidate response.
        private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(1);
        private Regex _regex;
        private string _pattern;

        public override void Init(XElement qtiElement, IExpressionFactory expressionFactory)
        {
            base.Init(qtiElement, expressionFactory);
            _pattern = GetAttributeValue("pattern");
            if (!string.IsNullOrEmpty(_pattern))
            {
                try
                {
                    _regex = new Regex($"^(?:{_pattern})$", RegexOptions.None, MatchTimeout);
                }
                catch (ArgumentException)
                {
                    _regex = null;
                }
            }
        }

        public override bool Execute(IProcessingContext ctx)
        {
            if (_regex == null)
            {
                ctx.LogError($"qti-pattern-match has a missing or invalid pattern attribute: '{_pattern}'");
                return false;
            }
            if (expressions.Count != 1)
            {
                ctx.LogError($"qti-pattern-match should have exactly one child expression, found: {expressions.Count}");
                return false;
            }
            var baseValue = expressions[0].Apply(ctx);
            if (baseValue?.Value == null)
            {
                return false;
            }
            try
            {
                return _regex.IsMatch(baseValue.Value);
            }
            catch (RegexMatchTimeoutException)
            {
                ctx.LogError($"qti-pattern-match timed out matching '{baseValue.Value}' against pattern: '{_pattern}'");
                return false;
            }
        }
    }
}
