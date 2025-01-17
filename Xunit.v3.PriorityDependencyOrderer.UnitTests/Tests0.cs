namespace Xunit.v3.PriorityDependencyOrderer.UnitTests;

[TestCaseOrderer(typeof(PriorityDependencyTestCaseOrderer))]
public class Tests0
{
    private static bool _test1Called;
    private static bool _test2Called;
    private static bool _test4Called;
    private static bool _test3Called;

    [Fact, Priority(5)]
    public void Test3()
    {
        _test3Called = true;

        Assert.True(_test1Called, $"'{nameof(_test1Called)}' is the wrong value!");
        Assert.True(_test2Called);
        Assert.True(_test4Called);
    }

    [Fact, Priority(0)]
    public void Test4()
    {
        _test4Called = true;

        Assert.True(_test1Called);
        Assert.False(_test2Called);
        Assert.False(_test3Called);
    }

    [Fact]
    public void Test2()
    {
        _test2Called = true;

        Assert.True(_test1Called);
        Assert.True(_test4Called);
        Assert.False(_test3Called);
    }

    [Fact, Priority(-5)]
    public void Test1()
    {
        _test1Called = true;

        Assert.False(_test2Called);
        Assert.False(_test4Called);
        Assert.False(_test3Called);
    }
}
