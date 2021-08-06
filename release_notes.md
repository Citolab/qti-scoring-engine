## 1.1.0
* Refactored for better (batch) performence:
    * don't use reflection to find exectors
    * setup executor before processing. Which means no more xPath on test definitions per assessmentResult.

## 1.0.1
* changed baseType: int to integer

## 1.0.0
* promoted to final version

## 1.0.0-beta.2

* refactored way of getting values
* 3.0 packages
* lt, lte, gt, substring, ordered in match, equalRounded, roud
* orderInteraction; implemented match for all cardinalities
* selectPoint, positionObjectInteraction; implemented scoring with baseType point

## 1.0.0-beta.1

Initial release