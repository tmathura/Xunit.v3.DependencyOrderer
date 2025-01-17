namespace Xunit.v3.PriorityDependencyOrderer;

[AttributeUsage(AttributeTargets.Method)]
public class PriorityAttribute(int priority) : Attribute
{
    public int Priority { get; } = priority;
}