namespace GRYLibrary.Core
{
    public enum TestKind
    {
        UnitTest = 0,// Tests that a component work like defined
        IntegrationTest = 1,// Tests that 2 (or more) components work together like defined
        DebugHelperTest = 2,// Tests that are always skipped when committing but can be useful to do or debug a certain task/function
        RegressionTest = 3,// Tests that a bug does not happen anymore
        SystemTest = 4,//Tests that other systems/services used by the function under test are still available and work like specified so that the function under test is able to be working correctly
    }
}
