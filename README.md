# Citolab QTI Scoring Engine

Scores QTI assessment results: it runs the response processing of your items and the outcome
processing of your test, and writes the outcome variables into the assessment results it
returns.

- Reads QTI **2.x and 3.0** items and tests, and **QTI Results Reporting 2.x** assessment results.
- Works on `XDocument`s, so nothing is read from or written to disk.
- Not every expression of the specification is implemented, but everything in
  [3.2 Simple Items](http://www.imsglobal.org/question/qtiv2p2/imsqti_v2p2_impl.html) is scored
  correctly. [What is supported](#what-is-supported) lists the rest.

## Contents

- [Installation](#installation)
- [Quick start](#quick-start)
- [Usage](#usage)
  - [The context](#the-context)
  - [Options](#options)
  - [What the engine changes](#what-the-engine-changes)
- [What is supported](#what-is-supported)
  - [Cardinality](#cardinality)
  - [Base types](#base-types)
  - [Response processing templates](#response-processing-templates)
  - [Rules](#rules)
  - [Expressions](#expressions)
  - [Lookup tables](#lookup-tables)
  - [How the operators behave](#how-the-operators-behave)
- [Records and QTI_CONTEXT](#records-and-qti_context)
- [Custom operators](#custom-operators)
- [Errors and logging](#errors-and-logging)
- [Processing a package](#processing-a-package)

## Installation

```bash
dotnet add package Citolab.QTI.ScoringEngine
```

The package targets `net9.0` and `netstandard2.0`.

## Quick start

```C#
using Citolab.QTI.ScoringEngine;

var scoringEngine = new ScoringEngine();

var scoredAssessmentResults = scoringEngine.ProcessResponsesAndOutcomes(new ScoringContext
{
    AssessmentItems = assessmentItemXDocs,      // List<XDocument>
    AssessmentTest = assessmentTestXDoc,        // XDocument
    AssessmentmentResults = assessmentResultXDocs, // List<XDocument>
    Logger = logger                             // optional
});
```

`scoredAssessmentResults` holds the assessment results with the calculated outcome variables.

## Usage

`IScoringEngine` has three methods:

```C#
public interface IScoringEngine
{
    List<XDocument> ProcessResponses(IResponseProcessingContext ctx, ResponseProcessingScoringsOptions options = null);
    List<XDocument> ProcessOutcomes(IOutcomeProcessingContext ctx);
    List<XDocument> ProcessResponsesAndOutcomes(IScoringContext ctx, ResponseProcessingScoringsOptions options = null);
}
```

| method                       | runs                                             | needs                            |
| ---------------------------- | ------------------------------------------------ | -------------------------------- |
| `ProcessResponses`           | the response processing of every item            | `AssessmentItems`                |
| `ProcessOutcomes`            | the outcome processing of the test               | `AssessmentTest`                 |
| `ProcessResponsesAndOutcomes`| response processing first, then outcome processing | both                           |

### The context

Pass one of `ScoringContext`, `ResponseProcessingContext` or `OutcomeProcessingContext` -
or your own implementation of the matching interface.

| property                                           | used by            | description                                                                                                                        |
| -------------------------------------------------- | ------------------ | ---------------------------------------------------------------------------------------------------------------------------------- |
| `List<XDocument> AssessmentmentResults`             | both               | The assessment results to score. For response processing an `itemResult` should hold the `candidateResponse` of the item.            |
| `List<XDocument> AssessmentItems`                   | responses          | The assessment items, in QTI 2.x or 3.0.                                                                                             |
| `XDocument AssessmentTest`                          | outcomes           | The assessment test, in QTI 2.x or 3.0.                                                                                              |
| `ILogger Logger`                                    | both               | Optional. Logs every processing step, and the errors that do not throw. Nothing is logged when it is left out.                       |
| `Dictionary<string, ICustomOperator> CustomOperators` | responses        | Optional. See [Custom operators](#custom-operators).                                                                                 |
| `bool? ProcessParallel`                             | both               | Optional. Scores the results in parallel, which can make a real difference when scoring a lot of results at once.                     |

### Options

Options are passed **as the argument of the method**, not through the context:

```C#
scoringEngine.ProcessResponses(ctx, new ResponseProcessingScoringsOptions
{
    StripAlphanumericsFromNumericResponses = true,
    QtiContextFields = new Dictionary<string, string> { { "environmentIdentifier", "ENV_1" } }
});
```

| option                                   | description                                                                                                                                    |
| ---------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| `StripAlphanumericsFromNumericResponses` | A best effort to compare a response like `"12 euro"` as the number `12`: everything that is not a digit or the first dot is dropped before an integer or float is compared. |
| `QtiContextFields`                       | Fields for the `QTI_CONTEXT` record. See [Records and QTI_CONTEXT](#records-and-qti_context).                                                     |

### What the engine changes

The engine scores **copies**: the `XDocument`s you pass in are not modified. Use the list that
is returned - the `AssessmentmentResults` of your context is replaced by that same list.

Per item result:

- Outcomes that the response processing writes (`qti-set-outcome-value`, `qti-lookup-outcome-value`)
  are reset to the default of their declaration - `0` for a number without one - and recalculated.
- Outcomes that the response processing does not write are left exactly as they are, so an
  outcome like `numAttempts` or a human scored outcome survives a rescore. An outcome variable
  marked `external-scored="human"` is never overwritten.
- An item that has no `itemResult` is skipped; an item result that has no candidate response
  still gets the outcome variables of the response processing, with their default value.

Per test result:

- A `testResult` is added when the result does not have one yet.
- The outcomes that the outcome processing writes are reset to 0 and recalculated.

## What is supported

The lists below use the QTI 3.0 element names. QTI 2.x items and tests are converted to the 3.0
spelling while they are read, so a 2.x `<responseCondition>` is the `qti-response-condition`
below, and a 2.x attribute `baseType` is `base-type`. Assessment results keep their 2.x
Results Reporting spelling.

### Cardinality

`single`, `multiple`, `ordered` and `record` (see [Records and QTI_CONTEXT](#records-and-qti_context)).

### Base types

| base type                            | how it is handled                                                                                              |
| ------------------------------------ | -------------------------------------------------------------------------------------------------------------- |
| `identifier`, `string`               | compared as text, case-sensitive unless the expression says otherwise                                            |
| `integer`, `float`                   | compared as numbers                                                                                              |
| `boolean`                            | compared as booleans                                                                                             |
| `pair`, `directedPair`               | compared as two identifiers; the order within a `pair` does not matter                                           |
| `point`                              | scored with `qti-map-response-point` and `qti-inside`; two points are not compared directly                      |
| `duration`                           | a number of seconds, compared as a number (`qti-duration-lt`, `qti-duration-gte`)                                |
| `uri`, `intOrIdentifier`             | compared as plain text, with no further interpretation                                                           |
| `file`                               | not supported                                                                                                    |

### Response processing templates

An item that refers to one of the standard templates is scored by that template, so it does not
need response processing of its own:

- `match_correct`
- `map_response`
- `map_response_point`

### Rules

Both kinds of processing:

- `qti-set-outcome-value`
- `qti-lookup-outcome-value`

Response processing only:

- `qti-response-condition`, `qti-response-if`, `qti-response-else-if`, `qti-response-else`
- `qti-exit-response` - ends the response processing of the attempt; the outcomes that were
  already set are kept
- `qti-response-processing-fragment` - processed in place. Fragments in a separate file are not
  pulled in, because `qti-include` is not resolved.

Outcome processing only:

- `qti-outcome-condition`, `qti-outcome-if`, `qti-outcome-else-if`, `qti-outcome-else`

Not supported: `qti-include`.

### Expressions

Supported:

|                       |                          |                      |
| --------------------- | ------------------------ | -------------------- |
| `qti-and`             | `qti-index`              | `qti-not`            |
| `qti-any-n`           | `qti-inside`             | `qti-null`           |
| `qti-base-value`      | `qti-integer-divide`     | `qti-or`             |
| `qti-container-size`  | `qti-integer-modulus`    | `qti-ordered`        |
| `qti-contains`        | `qti-integer-to-float`   | `qti-pattern-match`  |
| `qti-correct`         | `qti-is-null`            | `qti-power`          |
| `qti-custom-operator` | `qti-lcm`                | `qti-product`        |
| `qti-default`         | `qti-lt`                 | `qti-random`         |
| `qti-delete`          | `qti-lte`                | `qti-repeat`         |
| `qti-divide`          | `qti-map-response`       | `qti-round`          |
| `qti-duration-gte`    | `qti-map-response-point` | `qti-round-to`       |
| `qti-duration-lt`     | `qti-match`              | `qti-stats-operator` |
| `qti-equal` \*        | `qti-math-constant`      | `qti-string-match`   |
| `qti-equal-rounded`   | `qti-math-operator`      | `qti-substring`      |
| `qti-field-value`     | `qti-max`                | `qti-subtract`       |
| `qti-gcd`             | `qti-member`             | `qti-sum`            |
| `qti-gt`              | `qti-min`                | `qti-truncate`       |
| `qti-gte`             | `qti-multiple`           | `qti-variable`       |

\* `qti-equal` only supports `tolerance-mode="exact"`.

Outcome processing only: `qti-test-variables`, `qti-number-selected`.

Not supported:

- `qti-number-correct` - an item is left untouched rather than scored 0: what the delivery
  engine calculated stays in the result.
- `qti-number-incorrect`, `qti-number-presented`, `qti-number-responded`
- `qti-outcome-maximum`, `qti-outcome-minimum`
- `qti-random-float`, `qti-random-integer`

### Lookup tables

`qti-lookup-outcome-value` reads the table of the outcome declaration it writes to:

- `qti-interpolation-table` with `qti-interpolation-table-entry` - matched on an exact
  `source-value`, not on a range.
- `qti-match-table` with `qti-match-table-entry`.

### How the operators behave

- The operators that take numbers (`qti-product`, `qti-divide`, `qti-power`,
  `qti-math-operator`, ...) are NULL when a child is NULL, is not a number, or when there is no
  finite result - dividing by zero, the square root of a negative number. `qti-sum` and
  `qti-subtract` predate this and count a child that is not a number as 0.
- `qti-integer-divide` rounds down: `integerDivide(7, 3) = 2`, and the remainder of
  `qti-integer-modulus` goes with it: `integerModulus(7, 3) = 1`.
- `qti-round-to` takes `rounding-mode="decimalPlaces"` or `"significantFigures"`;
  `significantFigures` is the default, as it is for `qti-equal-rounded`.
- `qti-math-operator` supports `sin`, `cos`, `tan`, `secant`, `cosecant`, `cotangent`, `asin`,
  `acos`, `atan`, `atan2`, `sinh`, `cosh`, `tanh`, `exp`, `log` (base 10), `ln`, `sqrt`, `abs`,
  `floor`, `ceil`, `signum`, `toDegrees` and `toRadians`. Angles are in radians.
- `qti-stats-operator` supports `mean`, `median`, `popVariance`, `popSD`, `sampleVariance` and
  `sampleSD`.
- `qti-pattern-match` matches the pattern against the whole value, the way XML Schema patterns
  work, and gives up on a pattern that runs for more than a second.
- `qti-repeat` evaluates its children at most 1000 times, so a `number-repeats` that is read
  from a variable cannot build an endless container.

## Records and QTI_CONTEXT

A record is a set of named fields, each with its own base type. `qti-field-value` reads one
field of a record:

```XML
<qti-field-value field-identifier="environmentIdentifier">
    <qti-variable identifier="QTI_CONTEXT"/>
</qti-field-value>
```

A record can come from:

- `QTI_CONTEXT`, the built-in record below.
- A response or outcome variable of the assessment result declared `cardinality="record"`. Its
  values carry a field identifier and their own base type:
  `<value fieldIdentifier="floatValue" baseType="float">1.5</value>`.
- A [custom operator](#custom-operators) that returns a `BaseValue` with `Cardinality.Record`
  and its `Fields` set.
- The `qti-default-value` of a record declaration, read with `qti-default`.

`qti-set-outcome-value` can write a record to an outcome of record cardinality: it is added to
the item result as one `value` element per field.

`qti-field-value` is NULL when the record has no such field, so `qti-is-null` is how to check
whether a field is there.

### QTI_CONTEXT

`QTI_CONTEXT` can be read with `qti-variable` in response and outcome processing. The engine
fills what the assessment result knows:

| field                   | read from                                                        |
| ----------------------- | ---------------------------------------------------------------- |
| `candidateIdentifier`   | the `sourcedId` attribute of the `context` element                |
| `testIdentifier`        | the `testResult` of the result, or the test being processed       |
| `environmentIdentifier` | the `environmentIdentifier` attribute of the `context` element    |

Everything else is only known to the delivery engine, so fields can be passed in - and the ones
above overridden - through the options:

```C#
scoringEngine.ProcessResponses(ctx, new ResponseProcessingScoringsOptions
{
    QtiContextFields = new Dictionary<string, string>
    {
        { "environmentIdentifier", "ENV_1" }
    }
});
```

A field without a value is left out of the record instead of being set to an empty string.

## Custom operators

Custom operators are usually specific to a delivery engine, so they are passed to the scoring
engine. The key of the dictionary is the `definition` attribute of the element:

```XML
<qti-custom-operator definition="depcp:Trim">
    <qti-variable identifier="RESPONSE"/>
</qti-custom-operator>
```

```C#
public class Trim : ICustomOperator
{
    public BaseValue Apply(List<BaseValue> values)
    {
        var value = values.FirstOrDefault();
        if (value?.Value != null)
        {
            value.Value = value.Value.Trim();
        }
        return value;
    }
}

var ctx = new ResponseProcessingContext
{
    // ...
    CustomOperators = new Dictionary<string, ICustomOperator>
    {
        { "depcp:Trim", new Trim() }
    }
};
```

An operator is handed the values of its child expressions and returns the value of the
expression - including, if that is what it computes, a record.

Three operators are built in, each under the prefixes `depcp:`, `questify:` and `qade:`:

| operator            | does                                     |
| ------------------- | ---------------------------------------- |
| `Trim`              | trims the value                          |
| `ToAscii`           | replaces diacritics                      |
| `ParseCommaDecimal` | replaces a decimal comma with a dot      |

An operator you pass in overrides a built-in one with the same key.

## Errors and logging

### ScoringEngineException

Thrown when the engine is called with nothing to work with:

- a context that is null,
- response processing without `AssessmentItems`,
- outcome processing without an `AssessmentTest`.

### Other exceptions

Items, tests and results are indexed by identifier for fast lookups, so a duplicate identifier
throws:

- the same item reference twice in a test,
- more than one `itemResult` for the same item in an assessment result,
- more than one outcome or response declaration with the same identifier in an item.

Two assessment items with the same identifier are an exception to this: that is logged as a
warning and the first one is used.

### Logging

Every step is logged as information, which makes it possible to follow how a score came about.
Problems that do not stop the scoring are logged as a warning or an error, and usually end in a
score of 0 or in a NULL value. For example:

- a value that does not fit its base type, such as a float of `'hello'`,
- a lookup that finds nothing in an interpolation table or a mapping,
- an expression or a rule the engine does not know.

## Processing a package

`Console.ScoringEngine` is a worked example that scores the assessment results of a QTI
package: it reads the `imsmanifest.xml` of the package, hands the items, the test and the
results to the engine, and writes the scored results to a `processed` folder next to them.

```bash
dotnet run Console.ScoringEngine c:/mypackage.zip c:/assessmentResults
```

The first argument is the package, the second the folder holding the assessment results. Both
can also be set as `AppSettings:PackageLocation` and `AppSettings:AssessmentResultFolder` in
`appsettings.json`.
