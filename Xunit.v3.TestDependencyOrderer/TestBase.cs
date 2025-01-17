using System.Text;
using Xunit.Sdk;
using Xunit.v3.TestDependencyOrderer.Attributes;
using Xunit.v3.TestDependencyOrderer.Exceptions;
using Xunit.v3.TestDependencyOrderer.TestContext;

namespace Xunit.v3.TestDependencyOrderer;

/// <summary>
/// Provides a base class for managing test context, including adding test class information, validating class and test dependencies, and marking tests as complete.
/// </summary>
public abstract class TestBase : IDisposable
{
    private readonly ITestContextAccessor _testContextAccessor;
    private readonly GlobalTestContext _globalTestContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestBase"/> class.
    /// </summary>
    /// <param name="testContextAccessor">The test context accessor.</param>
    /// <param name="globalTestContext">The global test context.</param>
    protected TestBase(ITestContextAccessor testContextAccessor, GlobalTestContext globalTestContext)
    {
        _testContextAccessor = testContextAccessor;
        _globalTestContext = globalTestContext;

        AddTestClassInformation(globalTestContext);
        ValidateClassDependency(globalTestContext);
        ValidateTestDependency(globalTestContext);
    }

    /// <summary>
    /// Adds information about the current test class to the test context.
    /// </summary>
    /// <param name="testContextBase">The test context base.</param>
    public void AddTestClassInformation(TestContextBase testContextBase)
    {
        var testClassType = GetCurrentTestClass();

        testContextBase.AddTestClassInformation(testClassType);
    }

    /// <summary>
    /// Validates that all class dependencies for the current test class are complete.
    /// </summary>
    /// <param name="testContextBase">The test context base.</param>
    public void ValidateClassDependency(TestContextBase testContextBase)
    {
        var testClassType = GetCurrentTestClass();

        var dependencies = testClassType.GetCustomAttributes(typeof(DependsOnClassAttribute), true).Cast<DependsOnClassAttribute>().Select(attr => attr.DependentClassName).ToList();

        IsTestClassComplete(dependencies);
    }

    /// <summary>
    /// Validates that all test dependencies for the current test case are complete.
    /// </summary>
    /// <param name="testContextBase">The test context base.</param>
    public void ValidateTestDependency(TestContextBase testContextBase)
    {
        var testCase = GetCurrentTestCase();

        var xunitTestCase = testCase as IXunitTestCase;

        ArgumentNullException.ThrowIfNull(xunitTestCase);

        var attributes = xunitTestCase.TestMethod.Method.GetCustomAttributes(typeof(DependsOnMethodAttribute), true);

        var dependsOnAttributes = attributes as DependsOnMethodAttribute[];

        ArgumentNullException.ThrowIfNull(dependsOnAttributes);

        IsTestComplete(testContextBase, dependsOnAttributes, testCase);
    }

    /// <summary>
    /// Disposes of the resources used by the <see cref="TestBase"/> class.
    /// </summary>
    public void Dispose()
    {
        MarkCurrentTestComplete(_globalTestContext);
        TestCleanUp();
    }

    /// <summary>
    /// Marks the current test as complete in the test context.
    /// </summary>
    /// <param name="testContextBase">The test context base.</param>
    public void MarkCurrentTestComplete(TestContextBase testContextBase)
    {
        var classFullName = _testContextAccessor.Current.TestClassInstance?.GetType().FullName;

        ArgumentNullException.ThrowIfNull(classFullName);

        var testState = _testContextAccessor.Current.TestState;

        ArgumentNullException.ThrowIfNull(testState);

        if (testState.Result is not TestResult.Passed)
        {
            return;
        }

        var test = _testContextAccessor.Current.TestMethod;

        ArgumentNullException.ThrowIfNull(test);

        var testMethodName = test.MethodName;

        if (!string.IsNullOrEmpty(testMethodName))
        {
            testContextBase.MarkTestComplete(classFullName, testMethodName);
        }
    }

    /// <summary>
    /// Checks if all dependencies for the current test class are complete.
    /// </summary>
    /// <param name="dependencies">The list of dependent classes.</param>
    /// <exception cref="DependentTestClassNotCompleteException">Thrown when a dependent test class is not complete.</exception>
    private void IsTestClassComplete(List<Type> dependencies)
    {
        var dependencyErrors = new StringBuilder();

        foreach (var dependency in dependencies)
        {
            var classFullName = dependency.FullName;

            ArgumentNullException.ThrowIfNull(classFullName);

            if (_globalTestContext.IsTestClassComplete(classFullName))
            {
                continue;
            }

            dependencyErrors.Append($"{Environment.NewLine}Test Class '{classFullName}' must complete first!");
        }

        if (dependencyErrors.Length > 0)
        {
            throw new DependentTestClassNotCompleteException(dependencyErrors.ToString());
        }
    }

    /// <summary>
    /// Checks if all dependencies for the current test case are complete.
    /// </summary>
    /// <param name="testContextBase">The test context base.</param>
    /// <param name="dependsOnAttributes">The array of <see cref="DependsOnMethodAttribute"/>.</param>
    /// <param name="testCase">The current test case.</param>
    /// <exception cref="DependentTestNotCompleteException">Thrown when a dependent test is not complete.</exception>
    private static void IsTestComplete(TestContextBase testContextBase, DependsOnMethodAttribute[] dependsOnAttributes, ITestCase testCase)
    {
        var dependencyErrors = new StringBuilder();

        foreach (var dependsOnMethodAttribute in dependsOnAttributes)
        {
            if (testContextBase.IsTestComplete(testCase.TestClassName, dependsOnMethodAttribute.DependentMethodName))
            {
                continue;
            }

            dependencyErrors.Append($"{Environment.NewLine}Test case '{testCase.TestClassName}.{dependsOnMethodAttribute.DependentMethodName}' must complete first!");
        }

        if (dependencyErrors.Length > 0)
        {
            throw new DependentTestNotCompleteException(dependencyErrors.ToString());
        }
    }

    /// <summary>
    /// Gets the current test case from the test context accessor.
    /// </summary>
    /// <returns>The current test case.</returns>
    private ITestCase GetCurrentTestCase()
    {
        var testCase = _testContextAccessor.Current.TestCase;

        ArgumentNullException.ThrowIfNull(testCase);

        return testCase;
    }

    /// <summary>
    /// Gets the current test class type from the test case.
    /// </summary>
    /// <returns>The current test class type.</returns>
    private Type GetCurrentTestClass()
    {
        var testCase = GetCurrentTestCase();

        var testClassType = Type.GetType($"{testCase.TestClassName}, {testCase.TestClassNamespace}");

        ArgumentNullException.ThrowIfNull(testClassType);
        return testClassType;
    }

    /// <summary>
    /// Performs test-specific cleanup operations.
    /// </summary>
    protected abstract void TestCleanUp();
}
