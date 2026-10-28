# TEST_SCENARIOS.md P01 and P02 (the load-time half): the game's own load, and the six birds as it resolved them.
#
# What the offline checks cannot show is the game's load: Harmony has to be loaded first, and the six races
# and their eggs have to be this mod's after every other mod's patches ran. No label is asserted here, so the
# suite runs unchanged in English and in French.
Feature: Poultry Variety Pack Renew loads after Harmony and its six birds resolve

  Scenario: the mod is active and loads after its dependency
    Then mod "nelim.poultryvarietypackrenew" is loaded
    And mod "brrainz.harmony" is loaded
    And mod "nelim.poultryvarietypackrenew" loads after "brrainz.harmony"

  Scenario Outline: each bird is this mod's race and kind
    Then def "<bird>" of type "ThingDef" exists
    And def "<bird>" is defined by mod "nelim.poultryvarietypackrenew"
    And def "<bird>" of type "PawnKindDef" exists

    Examples:
      | bird               |
      | MaranChicken       |
      | CinnamonQueen      |
      | BielefelderChicken |
      | RunnerDuck         |
      | MuscovyDuck        |
      | RouenDuck          |

  Scenario: loading a game with the mod raises no error and no warning naming a bird
    Given the save "test-colony" is loaded
    Then no errors were logged
    And no warning matching "MaranChicken" was logged
    And no warning matching "BielefelderChicken" was logged
    And no warning matching "Duck" was logged
