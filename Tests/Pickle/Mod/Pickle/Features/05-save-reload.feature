# TEST_SCENARIOS.md P07. A Marans chick is generated, its colour noted, the game is saved and reloaded: the
# bird must come back as the save left it, same life stage and the same colour, then grow and keep it. The
# colour is drawn from the bird's thing id, which the save keeps, so a reload that changed the id would
# repaint the flock.
#
# Not here: the restart case (state a previous process wrote) is not applicable, as for every Pickle suite;
# the round trip is within one process.
Feature: a Marans comes back from a save in the same colour

  Background:
    Given the save "test-colony" is loaded

  Scenario: colour and life stage survive a save and a reload
    Given Poultry: a "MaranChicken" named "Colette" of sex female and life stage 0 stands at x=140 z=150
    And Poultry: the colour of "Colette" is noted
    And I wait 60 ticks
    Then the save round trips
    When I save and reload
    Then Poultry: "Colette" is in life stage 0
    And Poultry: the colour of "Colette" is still the noted one
    When Poultry: "Colette" grows to life stage 2
    Then Poultry: the colour of "Colette" is still the noted one
    And no errors were logged
