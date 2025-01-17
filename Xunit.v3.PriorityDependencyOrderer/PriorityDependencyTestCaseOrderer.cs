using System.Reflection;
using Xunit.Sdk;

namespace Xunit.v3.PriorityDependencyOrderer;

public class PriorityDependencyTestCaseOrderer : ITestCaseOrderer
{
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases) where TTestCase : ITestCase
    {
        var testsWithPriority = new List<TestPriority>();

        for (var index = 0; index < testCases.Count; index++)
        {
            var testCase = (IXunitTestCase)testCases.ElementAt(index);

            var attr = testCase.TestMethod.Method.GetCustomAttributes<PriorityAttribute>().FirstOrDefault();

            var testPriority = new TestPriority(attr?.Priority ?? 0, index, testCase);
            testsWithPriority.Add(testPriority);
        }

        var orderedTests = testsWithPriority.OrderBy(test => test.Priority).ThenBy(test => test.ClassOrder).Select(test => (TTestCase)test.Testcase).ToList();

        return orderedTests;
    }
}
