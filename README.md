# Citolab QTI Scoring Engine

This library can be used for response and outcome scoring.
Currenly it supports 2.x packages only.

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
- int
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

* lookupOutcomeValue
* responseCondition
* setOutcomeValue
* responseIf
* responseElseif
* responseElse

Unsupported:

* exitResponse
* include
* responseProcessingFragment 

## Expressions:

Supported:
* and
* baseValue
* correct
* customOperator
* equal* (toleranceMode: exact only)
* gt
* gte
* isNull
* equalRounded
* lt
* lte
* mapResponse
* mapResponsePoint
* match
* member
* or
* ordered* (in match)
* stringMatch
* substring
* sum
* variable

Unsupported:
* anyN
* containerSize
* contains
* default
* delete
* divide
* durationGTE
* durationLT
* fieldValue
* gcd
* lcm
* repeat
* index
* inside
* integerDivide
* integerModulus
* integerToFloat
* mathOperator
* mathConstant
* max
* min
* multiple
* not
* null
* outcomeMaximum
* outcomeMinimum
* patternMatch
* power
* product
* random
* randomFloat
* randomInteger
* round
* roundTo
* statsOperator
* subtract
* truncate

# Outcome processing
## Rules

Supported:

* setOutcomeValue

## Expressions:

Supported:
* baseValue
* sum
* testVariables
* variable

Unsupported:
* numberCorrect
* numberIncorrect
* numberPresented
* numberResponded
* numberSelected
* outcomeMaximum
* outcomeMinimum

## Usage

The Scoring Engine implement IScoringEngine which contains 3 functions:

- ProcessResponses: executes responseProcessing
- ProcessOutcomes: executes outcomeProcessing
- ProcessResponsesAndOutcomes: executes both outcomeProcessing and responseProcessing

The provided list of assessmentResults is updated with scoring info. The functions also return this list of assessmentResults.

The provided context contains:

- ```List<XDocument> AssessmentmentResults ```: list of assessmentResults. For responseProcessing ItemResult should at least contain the candidateResponse. For outcomeProcessing it should contain outcomeVariables.
- ```ILogger Logger``` (optional): logs the processing steps as informational and log warnings and errors. 

For responseProcessing the context extends the following properties:

- ```List<XDocument> AssessmentItems```: List of XDocuments with the assessmentItems.
- ```List<ICustomOperator> CustomOperators``` (optional): can be provided to handle customOperators. The definition property of the CustomOperator should map the definition attribute value of the customOperator.

For outcomeProcessing the context extends the following properties:

- ```Document AssessmentTest```: A XDocument with the assessmentTest.


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
A ```ScoringEngineException``` will be thrown when:

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

``` bash
 dotnet run Console.Scoring "C:\\mypackage.zip" "C:\\assessmentResults
 ```
 the file/folder settings can also be set from in the appSettings file.

 ## Custom operators

Because customOperators are often specific to delivery eniges they can be provided to this scoring engine.

example: 

 ```C#
public class Trim : ICustomOperator
{
    public string Definition => "depcp:Trim";

    public BaseValue Apply(BaseValue value)
    {
        if (value?.Value != null)
        {
            value.Value = value.Value.Trim();
        }
        return value;
    }
}
 ```
handles:
```XML
<customOperator definition="depcp:Trim">
    <variable identifier="RESPONSE"/>
</customOperator>
```

 There are three example implementations in this engine:
 - depcp:Trim: Trims the value
 - depcp:ToAscii: Handlers diacritics
 - depcp:ParseCommaDecimal: Replaces , to .