# Citolab QTI Scoring Engine

This library can be used for response and outcome scoring.

It does not support all response and outcome scoring (yet).

Currenly support for:

Response Processing:

- MapResponse
- Sum

- And
- Gte
- IsNull,
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

### Response processing

AssessmentTest, AssessmentItem and AssessmentResult should be provided as ```System.Xml.Linq.XDocument```

``` C#
 var assessmentItem = new AssessmentItem(logger, assesmentItemXDocument);
var assessmentResult = new AssessmentResult(logger, assesmentResultXDocument);

var responseProcessing = new ResponseProcessing();
responseProcessing.Process(assessmentItem, assessmentResult, logger);
```

### Outcome processing

``` C#
var assessmentTest = new AssessmentTest(logger, assessmentTestXDocument);
var assessmentResult = new AssessmentResult(logger, assesmentResultXDocument);

var responseProcessing = new ResponseProcessing();
responseProcessing.Process(assessmentItem, assessmentResult, logger);

var outcomeProcessing = new OutcomeProcessing();

outcomeProcessing.Process(assessmentTest, assessmentResult, logger);
```

AssessmentTest and AssessmentItems can be used to handle multiple AssessmentResults when processing eesults in batch.