namespace Xunit.v3.PriorityDependencyOrderer;

internal class TestPriority(int priority, int classOrder, IXunitTestCase testcase)
{
    public int Priority { get; set; } = priority;

    public int ClassOrder { get; set; } = classOrder;

    public  IXunitTestCase Testcase { get; set; } = testcase;
}
