# TEST_SCENARIOS.md P03 and P04. The single thing the mod's assembly exists for: a Marans is drawn in one of
# four colours, and keeps it from chick to adult, because the colour index is drawn once on the first life
# stage and reused. The step reads the texture the render tree really resolves, not the def.
#
# The chick is generated in life stage 0 and grown to the later stages by moving its biological age, which is
# what the game does as time passes. Both sexes are played: the hen and the rooster have their own textures.
Feature: a Marans keeps its colour from chick to adult

  Background:
    Given the save "test-colony" is loaded

  Scenario Outline: the colour follows a bird through its growth
    Given Poultry: a "MaranChicken" named "Colette" of sex <sex> and life stage 0 stands at x=140 z=150
    Then Poultry: "Colette" is drawn in one of the four Marans colours
    And Poultry: the texture drawn for "Colette" names the stage word "Chick"
    And Poultry: the colour of "Colette" is noted
    When Poultry: "Colette" grows to life stage 1
    Then Poultry: the colour of "Colette" is still the noted one
    When Poultry: "Colette" grows to life stage 2
    Then Poultry: the colour of "Colette" is still the noted one
    And Poultry: the texture drawn for "Colette" names the stage word "<adult>"
    And no errors were logged

    Examples:
      | sex    | adult   |
      | female | Hen     |
      | male   | Rooster |

