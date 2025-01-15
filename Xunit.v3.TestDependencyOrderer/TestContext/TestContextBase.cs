using System.Reflection;
using Xunit.v3.TestDependencyOrderer.Models;

namespace Xunit.v3.TestDependencyOrderer.TestContext;

/// <summary>
/// Provides a base class for managing test context, including adding test class information, marking tests as complete, and checking the completion status of tests and test classes.
/// </summary>
public abstract class TestContextBase : IDisposable
{
    private readonly HashSet<TestClassInformation> _testClasses = [];

    /// <summary>
    /// Disposes of the resources used by the <see cref="TestContextBase"/> class.
    /// </summary>
    public void Dispose()
    {
        // Nothing to dispose
    }

    /// <summary>
    /// Adds information about the test class, including its methods with [Fact] or [Theory] attributes.
    /// </summary>
    /// <param name="type">The type of the test class.</param>
    public void AddTestClassInformation(Type type)
    {
        // Get all methods of the class
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        // Get methods with [Fact] or [Theory] attribute that are not skipped
        var testMethods = methods.Where(method =>
            method.GetCustomAttributes(typeof(FactAttribute), false).Length is not 0 &&
            method.GetCustomAttributes(typeof(FactAttribute), false).All(attr => ((FactAttribute)attr).Skip == null) ||
            method.GetCustomAttributes(typeof(TheoryAttribute), false).Length is not 0 &&
            method.GetCustomAttributes(typeof(TheoryAttribute), false).All(attr => ((TheoryAttribute)attr).Skip == null)).ToList();

        var testClass = _testClasses.FirstOrDefault(testClass => testClass.Name == type.FullName);

        if (testClass is null)
        {
            testClass = new TestClassInformation
            {
                Name = type.FullName ?? type.Name,
                Tests = [],
            };

            _testClasses.Add(testClass);
        }

        foreach (var testMethod in testMethods)
        {
            if (testClass.Tests.Any(x => x.Name == testMethod.Name))
            {
                continue;
            }

            var testInformation = new TestInformation
            {
                Name = testMethod.Name,
                Completed = false
            };

            testClass.Tests.Add(testInformation);
        }
    }

    /// <summary>
    /// Marks a specific test as complete.
    /// </summary>
    /// <param name="className">The name of the test class.</param>
    /// <param name="testName">The name of the test method.</param>
    public void MarkTestComplete(string className, string testName)
    {
        var testClass = _testClasses.FirstOrDefault(testClass => testClass.Name == className);

        var test = testClass?.Tests.FirstOrDefault(test => test.Name == testName);

        if (test is null)
        {
            return;
        }

        test.Completed = true;
    }

    /// <summary>
    /// Checks if a specific test is complete.
    /// </summary>
    /// <param name="className">The name of the test class.</param>
    /// <param name="testName">The name of the test method.</param>
    /// <returns>True if the test is complete; otherwise, false.</returns>
    public bool IsTestComplete(string? className, string testName)
    {
        var testClass = _testClasses.FirstOrDefault(testClass => testClass.Name == className);

        var test = testClass?.Tests.FirstOrDefault(test => test.Name == testName);

        return test is not null && test.Completed;
    }

    /// <summary>
    /// Checks if all tests in a specific test class are complete.
    /// </summary>
    /// <param name="className">The name of the test class.</param>
    /// <returns>True if all tests in the class are complete; otherwise, false.</returns>
    public bool IsTestClassComplete(string className)
    {
        var testClass = _testClasses.FirstOrDefault(testClass => testClass.Name == className);
        var test = testClass?.Tests.All(test => test.Completed) ?? false;

        return test;
    }
}
