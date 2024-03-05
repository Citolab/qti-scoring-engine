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
