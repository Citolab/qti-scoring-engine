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
