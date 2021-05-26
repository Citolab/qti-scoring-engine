# Todo's

## Cardinality multiple

Support for responses of cardinality multiple. This is needed to be able to score choiceInteraction with maxChoices > 1

## Support and unittests for other interactionTypes

Starting with:

1. textInteraction: string and numerics (+ handle fractions)
2. choiceInteraction maxChoice > 1
3. inlineChoiceInteraction
4. gapMatchInteraction
5. ....

## Optimize for batch processing:

Optimize for multiple AssessmentResults for the same Assessment:

1. AssessmentTest and AssessmentItems should not inherit from XDocument. 
2. All executors and calculors must be setup during initialization. 
3. Combination of outcomesDeclaration and responseVariables can be cached e.g. OUTCOME_DEC=C|VARIABLE=C|SCORE=1. That would make the batch scoring of choiceInteraction super fast.  


##  QTI 3.0

Process packages using 3.0 spec. AssessmentTests and AssessmentItems of version 2.x should be converted to 3.0 before processing.
