## 1.6.0

### Added

- Adaptive items (`adaptive="true"`): the outcomes keep their values from one attempt to the next. The engine no longer resets the outcomes the response processing writes; it starts from the `outcomeVariable`s in the `itemResult` - the state the previous attempt left - and an outcome that is not there yet starts at its default. Score such an item attempt by attempt, passing the scored `itemResult` along with the responses of the next attempt (README: Adaptive items)
- The built-in outcome `completionStatus` of an adaptive item: an identifier (default `unknown`) without a declaration in the item. It used to be added as a float, so `completed` was written as `0`

### Fixed

- `qti-is-null` was never true for an outcome variable: `qti-variable` returns an outcome without a value as a value object, not as NULL. An outcome without a value - no default, or not set yet - is now NULL

## 1.5.0

### Fixed

- Record responses from portable custom interactions score as expected: a `record` response declaration has no `base-type` of its own (each field carries one) and no longer logs "missing baseType"; a record without any field yet - no response - reads every field as NULL in `qti-field-value` instead of logging that it is not a record
- `qti-gt`, `qti-gte`, `qti-lt` and `qti-lte` with a NULL operand are NULL (false), as QTI defines, instead of throwing a `NullReferenceException`; for instance a time range on a field of a record response the candidate did not give

## 1.4.1

### Fixed

- `qti-set-outcome-value` threw a `NullReferenceException` when it wrote an outcome of `multiple` or `ordered` cardinality, e.g. a `FEEDBACK` built with `qti-multiple`. A container outcome now holds all its values: a `qti-multiple` or `qti-ordered` (also one that adds to the outcome itself through `qti-variable`), a single value as a container of one, and `qti-null` as NULL. `qti-variable` reads it back as a container, so `qti-member`, `qti-contains`, `qti-match`, `qti-index` and `qti-container-size` work on it
- A container outcome is written to the assessmentResult as one `value` per member - none when it is NULL - and read back that way on a rescore and by the outcome processing; it used to be read as its first value only
- The `qti-default-value` of a container outcome declaration kept only its first `qti-value`, and a container outcome of a numeric base type without a default started as `0`; it now starts with every declared value, or as NULL. `qti-default` returns all of them
- `qti-ordered` threw a `NullReferenceException` on a NULL child and did not flatten a container child; it now leaves NULL out and flattens, like `qti-multiple`
- `qti-match` threw a `NullReferenceException` when its second child was NULL, `qti-lookup-outcome-value` when the value to look up was NULL or a container, and a case-insensitive comparison when one of its values was a container

## 1.4.0

### Added

- Control flow: `qti-exit-response` ends the response processing of an attempt and keeps the outcomes that were already set; `qti-response-processing-fragment` is processed in place
- Container operators: `qti-multiple`, `qti-container-size`, `qti-contains`, `qti-delete`, `qti-random`, `qti-repeat`
- Numeric operators: `qti-product`, `qti-divide`, `qti-integer-divide`, `qti-integer-modulus`, `qti-power`, `qti-round-to`, `qti-truncate`, `qti-integer-to-float`, `qti-gcd`, `qti-lcm`, `qti-math-constant`, `qti-math-operator` (24 functions), `qti-stats-operator` (mean, median, popVariance, popSD, sampleVariance, sampleSD)
- Logic, geometry and pattern operators: `qti-any-n`, `qti-pattern-match`, `qti-inside`, `qti-duration-lt`, `qti-duration-gte`
- `qti-default`: the declared default value of an outcome or response variable
- `qti-match-table` / `qti-match-table-entry` on an outcome declaration, used by `qti-lookup-outcome-value`
- Record cardinality, end to end: record response and outcome variables are read from the assessmentResult, `qti-field-value` reads a field, `qti-set-outcome-value` writes a record back, a record `qti-default-value` is read, and a custom operator can return one
- `QTI_CONTEXT`: a built-in record with `candidateIdentifier`, `testIdentifier` and `environmentIdentifier`, filled from the assessmentResult. Extra fields - and overrides - can be passed through the new `QtiContextFields` option

### Fixed

- `qti-map-response` and `qti-map-response-point` mapped the values of a multiple or ordered response as one string joined with `&` when the response was read from an assessmentResult, so those items could only score the default value of the mapping. The cardinality of a response variable is now read back from the result, and both operators map every value separately
- `qti-lookup-outcome-value` threw a `NullReferenceException` when the outcome declaration it writes to has no lookup table at all; it now logs an error and leaves the outcome alone

### Documentation

- Rewrote the README: installation, a table of contents, the QTI_CONTEXT and record documentation, and corrections to the custom operator sample, the `IScoringEngine` signatures and the list of supported expressions

## 1.3.7

- support: `qti-index` — take the value at position n from an ordered container
- Fixes order interactions that score each position separately with `qti-index` inside `qti-match`: these previously scored 0 for every response, including a fully correct one

## 1.3.6

- `VariableResponseProcessing` now always exposes the full list of values for multiple cardinality responses, also when the list holds a single value (e.g. a gapMatch with one correct pair)

## 1.3.5

- Null-safety fix in `AssessmentTest`: guard against missing `qti-set-outcome-value` elements and a null `OutcomeProcessingElement` when building calculated outcomes and expressions

## 1.3.4

- Enhanced ResponseProcessorContext to initialize outcome variables for declared outcomes
- Ensures expressions like `<variable identifier="MAXSCORE"/>` resolve correctly even when the outcome is only read (never set) during responseProcessing

## 1.3.3

- Updated target framework to .NET 9.0
- Refactored ResponseProcessorContext constructor for clarity
- Updated target frameworks and version in project file

## 1.3.2

- String comparison supports case sensitive

## 1.3.1

- removed temp fix for delivery engine error to sort directPairs. Only pairs are sorted as the QTI spec describes.

## 1.3.0

- support: qti-max + qti-min

## 1.3.0-beta2

Added option to pass ResponseProcessingOptions instead of single stripAlphanumericsFromNumericResponses parameter

## 1.3.0-beta1

- support for qti-subtract

#### added work-arounds.

- added optional parameter: stripAlphanumericsFromNumericResponses to strip alphanumeric values in reponse variables of type int and float, before comparing with the correct response.

* multiple values of correct responses will be loaded despite of cardinatity, so even if it's declared as single.
* directedPair is sorted before comparing too. Because it has a source and a target it should not be sorted (only a normal pair) but to support a error in a delivery engine this is supported only in this version. Probably won't effect other scoring because in general target and sources will have different ids.

## 1.2.5

- null ref fix ToBaseValue
- return NULL as default value for outcomes. Only integer and float should have a default value, which is 0.

## 1.2.4

- if outcomeVariables don't have a default value, then set a default value depending on the baseType

## 1.2.3

- fixed null reference error when calling equals expression without a response

## 1.2.2

- fixed empty xmlns in assessmentResult

## 1.2.1

- fixed parsing float fails on specific cultures

## 1.2.0

- upgraded to netstandard2.1
- don't update itemResults when variable is manual set: attribute and value: external-scored="human"
- itemRef weigths can be of type: float.
- added support for custom operators with prefix: qade

## 1.1.0

- Refactored for better (batch) performence:
  - don't use reflection to find exectors
  - setup executor before processing. Which means no more xPath on test definitions per assessmentResult.
- Added option to process results parallel

## 1.0.1

- changed baseType: int to integer

## 1.0.0

- promoted to final version

## 1.0.0-beta.2

- refactored way of getting values
- 3.0 packages
- lt, lte, gt, substring, ordered in match, equalRounded, roud
- orderInteraction; implemented match for all cardinalities
- selectPoint, positionObjectInteraction; implemented scoring with baseType point

## 1.0.0-beta.1

Initial release
