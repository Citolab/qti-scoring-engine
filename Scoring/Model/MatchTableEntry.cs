namespace Citolab.QTI.ScoringEngine.Model
{
    /// <summary>
    /// An entry of a qti-match-table: an exact source value and what it maps to. Unlike the
    /// interpolation table it does not cover a range, so the source value is kept as written.
    /// </summary>
    internal class MatchTableEntry
    {
        public string SourceValue { get; set; }
        public string TargetValue { get; set; }
    }
}
