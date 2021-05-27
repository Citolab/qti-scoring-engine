# Citolab QTI Scoring Engine

This library can be used for response and outcome scoring.

It does not support all response and outcome scoring (yet).

Currenly supports:

Response Processing:

Calculators:
- MapResponse
- Sum

Executors
- And
- Gte
- IsNull
- LookupOutcomeValue
- Match
- Member
- Or
- ResponseCondition
- ResponseElse
- ResponseIf
- ResponseElseIf
- SetOutcomeValue

Outcome processing:

- Sum
- SetOutcomeValue (testVariables (incl. weight, includeCategory, excludeCategory), itemVariables (incl. weight))

## Usage

The Scoring Engine implement IScoringEngine which contains 3 functions:

- ProcessResponses: executes responseProcessing
- ProcessOutcomes: executes outcomeProcessing
- ProcessResponsesAndOutcomes: executes both outcomeProcessing and responseProcessing

The provided context contains:

-```List<XDocument> AssessmentmentResults ```: list of assessmentResults. For responseProcessing ItemResult should at least contain the candidateResponse. For outcomeProcessing it should contain outcomeVariables.
- ```ILogger Logger```: optional, logs the processing steps as informational and log warnings and errors. 
- ```bool? ProcessParallel```: for bulk processing it can process assessmentResults in parallel. (default: false)

For responseProcessing the context needs a list of items too. ```List<XDocument> AssessmentItems```

For outcomeProcessing the context needs a ```Document AssessmentTest``` too.


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

## Processing Packages

The project: Console.ScoringEngine contains an example of how to process assessmentResults for a package.

It can be run by the console:

The first argument should be the path to the package and the second argument should be the folder where the assessmentResults are located.

``` bash
 dotnet run Console.Scoring "C:\\mypackage.zip" "C:\\assessmentResults
 ```
 the file/folder settings can also be set from in the appSettings file.

 