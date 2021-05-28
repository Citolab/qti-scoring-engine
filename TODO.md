# Todo's

## Cardinality multiple

Support for responses of cardinality multiple. This is needed to be able to score choiceInteraction with maxChoices > 1

## Support and unittests for other interactionTypes

Starting with:

1. choiceInteraction maxChoice > 1
2. inlineChoiceInteraction
3. gapMatchInteraction
4. ....

## Optimize for batch processing:

Optimize for multiple AssessmentResults for the same Assessment:

1. AssessmentTest and AssessmentItems should not inherit from XDocument. 
2. All executors, calculors, value etc. must be setup during initialization. 



##  QTI 3.0

Process packages using 3.0 spec. AssessmentTests and AssessmentItems of version 2.x should be converted to 3.0 before processing.
