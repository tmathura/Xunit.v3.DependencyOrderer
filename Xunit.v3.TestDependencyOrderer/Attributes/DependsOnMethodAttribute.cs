namespace Xunit.v3.TestDependencyOrderer.Attributes;

/// <summary>
/// Specifies that the test method depends on another method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DependsOnMethodAttribute(string dependentMethodName) : Attribute
{
    /// <summary>
    /// Gets the name of the method that the test method depends on.
    /// </summary>
    public string DependentMethodName { get; } = dependentMethodName;
}
