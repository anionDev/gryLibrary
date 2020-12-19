namespace GRYLibrary.Core
{
    public enum TestKind
    {
        /// <summary>
        /// Tests that a component work like defined.
        /// </summary>
        UnitTest = 0,
        /// <summary>
        /// Tests that 2 (or more) components work together like defined.
        /// </summary>
        IntegrationTest = 1,
        /// <summary>
        /// Tests that are always skipped when committing but can be useful to do or debug a certain task/function.
        /// </summary>
        DebugHelperTest = 2,
        /// <summary>
        /// Tests that a bug does not happen anymore.
        /// </summary>
        RegressionTest = 3,
        /// <summary>
        /// Tests that other systems/services used by the function under test are still available and work like specified so that the function under test is able to be working correctly.
        /// </summary>
        SystemTest = 4,
        /// <summary>
        /// Tests that will be done by reflection to assert a certain property for all types (or all methods or all attributes) in a certain scope.
        /// </summary>
        ReflectionTest = 5,
        /// <summary>
        /// Tests to demonstrate a certain thing.
        /// </summary>
        DemonstrationTest = 6,
    }
}
