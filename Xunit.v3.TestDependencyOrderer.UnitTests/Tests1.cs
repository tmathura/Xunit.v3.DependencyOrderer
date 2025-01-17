using Xunit.v3.TestDependencyOrderer.Attributes;
using Xunit.v3.TestDependencyOrderer.TestContext;

namespace Xunit.v3.TestDependencyOrderer.UnitTests;

/// <summary>
/// Test class that demonstrates the use of test dependencies and orderer.
/// </summary>
[TestCaseOrderer(typeof(TestDependencyTestCaseOrderer))]
public class Tests1 : TestBase, IClassFixture<CurrentTestContext>
{
    private readonly CurrentTestContext _currentTestContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tests1"/> class.
    /// </summary>
    /// <param name="testContextAccessor">The test context accessor.</param>
    /// <param name="globalTestContext">The global test context.</param>
    /// <param name="currentTestContext">The current test context.</param>
    public Tests1(ITestContextAccessor testContextAccessor, GlobalTestContext globalTestContext, CurrentTestContext currentTestContext) : base(testContextAccessor, globalTestContext)
    {
        _currentTestContext = currentTestContext;

        AddTestClassInformation(_currentTestContext);
        ValidateTestDependency(_currentTestContext);
    }

    /// <summary>
    /// Test method that depends on <see cref="Test1"/>.
    /// </summary>
    [Fact]
    [DependsOnMethod(nameof(Test1))]
    public void Test2()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Independent test method.
    /// </summary>
    [Fact]
    public void Test1()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Independent test method.
    /// </summary>
    [Fact]
    public void Test4()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Test method that depends on <see cref="TestB"/>.
    /// </summary>
    [Fact]
    [DependsOnMethod(nameof(TestB))]
    public void TestA()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Independent test method.
    /// </summary>
    [Fact]
    public void TestB()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Skipped test method.
    /// </summary>
    [Fact(Skip = "Skipping this test.")]
    public void TestC()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Test method that depends on <see cref="Test1"/> and <see cref="Test2"/>.
    /// </summary>
    [Fact]
    [DependsOnMethod(nameof(Test1))]
    [DependsOnMethod(nameof(Test2))]
    public void Test3()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Performs cleanup operations after each test.
    /// </summary>
    protected override void TestCleanUp()
    {
        MarkCurrentTestComplete(_currentTestContext);
    }
}
