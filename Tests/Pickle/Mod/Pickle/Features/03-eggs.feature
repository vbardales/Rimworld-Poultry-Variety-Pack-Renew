# TEST_SCENARIOS.md P05. Every fertilized egg hatches into the bird it names, as a newborn. The Bielefelder egg
# of the original looked for a hatcher called Bielefelder where the bird is BielefelderChicken, so it never
# hatched: the step goes through the game's own CompHatcher, so that fault would fail here by name.
Feature: the fertilized eggs hatch into their birds

  Background:
    Given the save "test-colony" is loaded

  Scenario Outline: an egg hatches into its bird
    Then Poultry: a fertilized "<egg>" egg hatches into a "<bird>" in life stage 0
    And no errors were logged

    Examples:
      | egg                          | bird               |
      | EggMaransFertilized          | MaranChicken       |
      | EggCinnamonQueenFertilized   | CinnamonQueen      |
      | EggBielefelderFertilized     | BielefelderChicken |
      | EggRunnerDuckFertilized      | RunnerDuck         |
      | EggMuscovyDuckFertilized     | MuscovyDuck        |
      | EggRouenDuckFertilized       | RouenDuck          |
