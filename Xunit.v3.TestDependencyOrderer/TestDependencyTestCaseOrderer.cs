using Xunit.Sdk;
using Xunit.v3.TestDependencyOrderer.Attributes;

namespace Xunit.v3.TestDependencyOrderer;

/// <summary>
/// Orders test cases based on their dependencies.
/// </summary>
public class TestDependencyTestCaseOrderer : ITestCaseOrderer
{
    /// <summary>
    /// Orders the test cases based on their dependencies.
    /// </summary>
    /// <typeparam name="TTestCase">The type of the test case.</typeparam>
    /// <param name="testCases">The collection of test cases to order.</param>
    /// <returns>An ordered collection of test cases.</returns>
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases) where TTestCase : ITestCase
    {
        // Map test cases to their dependencies
        var testCaseDictionary = testCases.ToDictionary(testCase => testCase, testCase => GetDependentMethods(testCase));

        var orderedTests = new List<TTestCase>();
        var visited = new HashSet<TTestCase>();

        foreach (var testCase in testCases)
        {
            Visit(testCase, testCaseDictionary, orderedTests, visited);
        }

        return orderedTests;
    }

    /// <summary>
    /// Visits the test case and its dependencies to order them.
    /// </summary>
    /// <typeparam name="TTestCase">The type of the test case.</typeparam>
    /// <param name="testCase">The test case to visit.</param>
    /// <param name="testCaseDictionary">The dictionary of test cases and their dependencies.</param>
    /// <param name="orderedTests">The list of ordered test cases.</param>
    /// <param name="visited">The set of visited test cases.</param>
    private static void Visit<TTestCase>(TTestCase testCase, Dictionary<TTestCase, List<string>> testCaseDictionary, List<TTestCase> orderedTests, HashSet<TTestCase> visited) where TTestCase : ITestCase
    {
        // Avoid reprocessing
        if (!visited.Add(testCase))
            return;

        // Process each dependency
        foreach (var dependency in testCaseDictionary[testCase])
        {
            var dependentTestCase = testCaseDictionary.Keys.FirstOrDefault(tc => tc.TestMethodName == dependency);

            if (dependentTestCase is not null)
            {
                Visit(dependentTestCase, testCaseDictionary, orderedTests, visited);
            }
        }

        if (!orderedTests.Contains(testCase))
        {
            orderedTests.Add(testCase);
        }
    }

    /// <summary>
    /// Gets the dependent methods for a given test case.
    /// </summary>
    /// <param name="testCase">The test case to get dependencies for.</param>
    /// <returns>A list of dependent method names.</returns>
    private static List<string> GetDependentMethods(ITestCase testCase)
    {
        // Fetch all dependencies specified using multiple attributes
        var xunitTestCase = testCase as IXunitTestCase;
        var attributes = xunitTestCase?.TestMethod.Method.GetCustomAttributes(typeof(DependsOnMethodAttribute), true);

        var dependsOnAttributes = attributes as DependsOnMethodAttribute[];

        return dependsOnAttributes?.Select(dependsOnMethodAttribute => dependsOnMethodAttribute.DependentMethodName).Where(method => !string.IsNullOrEmpty(method)).ToList() ?? [];
    }
}
