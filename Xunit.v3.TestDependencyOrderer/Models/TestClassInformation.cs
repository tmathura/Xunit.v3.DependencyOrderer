namespace Xunit.v3.TestDependencyOrderer.Models;

/// <summary>
/// Represents information about a test class, including its name and the list of tests it contains.
/// </summary>
internal class TestClassInformation
{
    /// <summary>
    /// Gets or sets the name of the test class.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the list of tests associated with the test class.
    /// </summary>
    public required List<TestInformation> Tests { get; set; }
}
