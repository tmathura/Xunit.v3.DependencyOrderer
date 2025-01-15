namespace Xunit.v3.TestDependencyOrderer.Attributes;

/// <summary>
/// Specifies that the decorated class depends on another class.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnClassAttribute(Type dependentClassName) : Attribute
{
    /// <summary>
    /// Gets the type of the class that the decorated class depends on.
    /// </summary>
    public Type DependentClassName { get; } = dependentClassName;
}
