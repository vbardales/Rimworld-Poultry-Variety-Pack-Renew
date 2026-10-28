# TEST_SCENARIOS.md P06. Asserted against the value written here FOR THE LANGUAGE THE PASS RUNS IN: the same
# scenario is green in the English pass (English values) and in the French pass (French values), and it fails,
# naming the language, in a pass whose language it has no value for. In developer mode, which every Pickle run
# is, a missing key in the active language shows as accented gibberish rather than clean English, so an absent
# French entry fails.
Feature: the birds' texts are in the language of the pass

  Scenario: a chicken and a duck, label and description
    Then Poultry: the label of the ThingDef "MaranChicken" is "Marans chicken" in English and "poulet Marans" in French
    And Poultry: the label of the ThingDef "MuscovyDuck" is "Muscovy duck" in English and "canard de Barbarie" in French
    And Poultry: the description of the ThingDef "RouenDuck" starts with "An unusual duck that stands upright. It is a good layer" in English and "Un canard inhabituel qui se tient debout. Cette race est une bonne pondeuse" in French
    And Poultry: the label of the ThingDef "EggBielefelderFertilized" is "chicken egg (fert.)" in English and "œuf de poule (fécondé)" in French

  Scenario: the Bielefelder hen is spelt right
    Then Poultry: the PawnKindDef "BielefelderChicken" calls its females "Bielefelder hen" in English and "poule Bielefelder" in French
