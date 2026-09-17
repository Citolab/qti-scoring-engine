# Citolab QTI Scoring Engine

This library can be used for response and outcome scoring.
Currenly it supports 2.x and 3.0 packages and v2.x Results Reporting.

It does not support all response and outcome scoring (yet) but all items in section [3.2 Simple Items](http://www.imsglobal.org/question/qtiv2p2/imsqti_v2p2_impl.html) are scored correctly.

# Response processing

## Cardinality

Supported:

- single
- multiple
- ordered

## BaseType

Supported:

- directedPair
- identifier
- integer
- float
- pair
- point
- string

Unsupported:

- Duration,
- file
- Uri
- IntOrIdentifier

## Rules

Supported:

- qti-lookup-outcome-value
- qti-outcome-condition
- qti-outcome-else
- qti-outcome-elseif
- qti-outcome-if
- qti-response-if
- qti-response-elseif
- qti-response-else
- qti-response-condition
- qti-set-outcome-value
- qti-exit-response
- qti-response-processing-fragment\*

\* a fragment that is present in the item itself is processed in place; fragments in a
separate file are not pulled in because qti-include is not resolved.

Unsupported:

- qti-include

## Lookup tables

Supported on qti-outcome-declaration, used by qti-lookup-outcome-value:

- qti-interpolation-table / qti-interpolation-table-entry (exact source values)
- qti-match-table / qti-match-table-entry

## Expressions:

Supported:

- qti-and
- qti-any-n
- qti-base-value
- qti-container-size
- qti-contains
- qti-correct
- qti-custom-operator
- qti-default
- qti-delete
- qti-divide
- qti-duration-gte
- qti-duration-lt
- qti-equal\* (toleranceMode: exact only)
- qti-gcd
- qti-gt
- qti-gte
- qti-index
- qti-inside
- qti-integer-divide
- qti-integer-modulus
- qti-integer-to-float
- qti-isNull
- qti-equal-rounded
- qti-lcm
- qti-lt
- qti-lte
- qti-map-response
- qti-map-response-point
- qti-math-constant
- qti-math-operator
- qti-max
- qti-min
- qti-match
- qti-member
- qti-multiple
- qti-not
- qti-null
- qti-number-selected
- qti-number-presented
- qti-or
- qti-ordered
- qti-pattern-match
- qti-power
- qti-product
- qti-random
- qti-repeat
- qti-round
- qti-round-to
- qti-stats-operator
- qti-string-match
- qti-substring
- qti-subtract
- qti-sum
- qti-truncate
- qti-variable

Unsupported:

- qti-field-value (record cardinality is not modelled)
- qti-number-correct
- qti-number-incorrect
- qti-number-responded
- qti-outcome-maximum
- qti-outcome-minimum
- qti-random-float
- qti-random-integer

### Notes on the numeric and container operators

- The operators that take numbers (qti-product, qti-divide, qti-power, qti-math-operator, ...)
  are NULL when a child is NULL, is not a number, or when the result is not finite: dividing by
  zero, the square root of a negative number. Note that qti-sum and qti-subtract predate this
  and count an unparsable child as 0 instead.
- qti-integer-divide rounds down: integerDivide(7, 3) = 2 and the remainder of
  qti-integer-modulus goes with it: integerModulus(7, 3) = 1.
- qti-round-to takes rounding-mode decimalPlaces or significantFigures; significantFigures is
  the default, as in qti-equal-rounded.
- qti-math-operator supports: sin, cos, tan, secant, cosecant, cotangent, asin, acos, atan,
  atan2, sinh, cosh, tanh, exp, log (base 10), ln, sqrt, abs, floor, ceil, signum, toDegrees
  and toRadians. Angles are in radians.
- qti-stats-operator supports: mean, median, popVariance, popSD, sampleVariance and sampleSD.
- qti-pattern-match anchors the pattern to the whole value, as XML Schema patterns are, and
  gives up on a pattern that takes more than a second to match.
- qti-repeat evaluates its children at most 1000 times, so a number-repeats read from a
  variable cannot build an endless container.

## Usage

The Scoring Engine implement IScoringEngine which contains 3 functions:

- ProcessResponses: executes responseProcessing
- ProcessOutcomes: executes outcomeProcessing
- ProcessResponsesAndOutcomes: executes both outcomeProcessing and responseProcessing

optional you can pass options, which for now just contains a parameter: StripAlphanumericsFromNumericResponses which can be used to do a best effort to remove non-numeric characters to numberic chars when comparing as numbers is the scoring.

The provided list of assessmentResults is updated with scoring info. The functions also return this list of assessmentResults.

The provided context contains:

- `List<XDocument> AssessmentmentResults `: list of assessmentResults. For responseProcessing ItemResult should at least contain the candidateResponse.
- `ILogger Logger` (optional): logs the processing steps as informational and log warnings and errors.

For responseProcessing the context extends the following properties:

- `List<XDocument> AssessmentItems`: List of XDocuments with the assessmentItems.
- `Dictionary<string, ICustomOperator> CustomOperators` (optional): can be provided to handle customOperators. The definition property of the CustomOperator should map the definition attribute value of the customOperator.
- `bool ProcessParallel`: process results parallel. Could gain significant performance with processing lots of assessmentResults at onces.

For outcomeProcessing the context extends the following properties:

- `Document AssessmentTest`: A XDocument with the assessmentTest.

```C#
public interface IScoringEngine
{
    List<XDocument> ProcessResponses(IResponseProcessingContext ctx);
    List<XDocument> ProcessOutcomes(IOutcomeProcessingContext ctx);
    List<XDocument> ProcessResponsesAndOutcomes(IScoringContext ctx);
}
```

### Example

```C#
var qtiScoringEngine = new ScoringEngine();
var scoredAssessmentResults = qtiScoringEngine.ProcessResponsesAndOutcomes(new ScoringContext
{
    AssessmentItems = assessmentItemXDocs,
    AssessmentTest = assessmentTestXDoc,
    AssessmentmentResults = assessmentResultXDocs,
    Logger = _logger
});
```

## Error handling

### ScoringEngineException

A `ScoringEngineException` will be thrown when:

- Calling responseProcessing without context or not a list of items.
- Calling outcomeProcessing without context or without an assessmentTest.

### Unhandled exceptions

Because some dictionaries are created to be able to do fast lookups some it will break on double identifiers e.g.

- Same itemRef twice in a test.
- Multiple itemResults for the same item in an AssessmentResult
- Multiple outcome-/responseDeclarations with the same identifier in the same AssessmentItem.

### Logging

A lot is logged. Some informational to be able to track the prossing but also errors that will not throw an exception and just result in score 0.

E.g.

- BaseType does not map to actual value. E.g. float = 'hello'
- Invalid lookup in interpolation, mapResponse etc.

### missings

If response processing is called without candidateResponses it will add the outcomeVariables that are used in responseProcessing with the defaultValue if defined, otherwise 0.

All oucomeVariables that are used in the response processing are reset to zero and re-calcuted.

OutcomeVariable that are not used in the response processing. For example if there is an outcomeVariable 'numAttempts' or 'toolsUsed' it won't be touched and will be in the AssessmentResult with the same value.

## Processing Packages

The project: Console.ScoringEngine contains an example of how to process assessmentResults for a package.

It can be run in the console:

The first argument should be the path to the package. The second argument should be the folder where the assessmentResults are located.

```bash
 dotnet run Console.Scoring "C:\\mypackage.zip" "C:\\assessmentResults
```

the file/folder settings can also be set from in the appSettings file.

## Custom operators

Because customOperators are often specific to delivery eniges they can be provided to this scoring engine.

example:

```C#
public class Trim : ICustomOperator
{
   public BaseValue Apply(BaseValue value)
   {
       if (value?.Value != null)
       {
           value.Value = value.Value.Trim();
       }
       return value;
   }
}

CustomOperators = new Dictionary<string, Citolab.QTI.ScoringEngine.Interfaces.ICustomOperator>
{
   {  "decp:Trim", new Trim() }
}

```

handles:

```XML
<customOperator definition="depcp:Trim">
    <variable identifier="RESPONSE"/>
</customOperator>
```

There are three example implementations in this engine:

- depcp:Trim|questify:Trim|qade:Trim: Trims the value
- depcp:ToAscii|questify:ToAscii|questify:qade: Handlers diacritics
- depcp:ParseCommaDecimal|questify:ParseCommaDecimal|qade:ParseCommaDecimal: Replaces , to .
