namespace Xunit.v3.TestDependencyOrderer.Models;

/// <summary>
/// Represents information about a test, including its name and completion status.
/// </summary>
internal class TestInformation
{
    /// <summary>
    /// Gets or sets the name of the test.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the test is completed.
    /// </summary>
    public bool Completed { get; set; }
}
