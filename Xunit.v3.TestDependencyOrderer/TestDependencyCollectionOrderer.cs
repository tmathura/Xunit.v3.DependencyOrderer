using System.Text.RegularExpressions;
using Xunit.Sdk;
using Xunit.v3.TestDependencyOrderer.Attributes;

namespace Xunit.v3.TestDependencyOrderer;

/// <summary>
/// Orders test collections based on their dependencies.
/// </summary>
public class TestDependencyCollectionOrderer : ITestCollectionOrderer
{
    /// <summary>
    /// Orders the test collections based on their dependencies.
    /// </summary>
    /// <typeparam name="TTestCollection">The type of the test collection.</typeparam>
    /// <param name="testCollections">The test collections to order.</param>
    /// <returns>A read-only collection of ordered test collections.</returns>
    public IReadOnlyCollection<TTestCollection> OrderTestCollections<TTestCollection>(IReadOnlyCollection<TTestCollection> testCollections) where TTestCollection : ITestCollection
    {
        var sortedCollections = new List<TTestCollection>();
        var visited = new HashSet<string>();

        // Process each collection
        foreach (var collection in testCollections)
        {
            Visit(collection, testCollections, sortedCollections, visited);
        }

        return sortedCollections;
    }

    /// <summary>
    /// Visits the test collection and its dependencies recursively to order them.
    /// </summary>
    /// <typeparam name="TTestCollection">The type of the test collection.</typeparam>
    /// <param name="collection">The test collection to visit.</param>
    /// <param name="allCollections">All test collections.</param>
    /// <param name="sorted">The list of sorted test collections.</param>
    /// <param name="visited">The set of visited test collections.</param>
    private static void Visit<TTestCollection>(TTestCollection collection, IReadOnlyCollection<TTestCollection> allCollections, List<TTestCollection> sorted, HashSet<string> visited) where TTestCollection : ITestCollection
    {
        var testClassNameWithNamespace = GetClassNameWithNamespace(collection.TestCollectionDisplayName);
        var testClassNameWithNamespaceAndAssemblyQualifiedName = GetClassNameWithNamespaceAndAssemblyQualifiedName(collection.TestAssembly.AssemblyName, testClassNameWithNamespace);

        // Avoid reprocessing
        if (!visited.Add(testClassNameWithNamespace))
            return;

        // Get the class type associated with the collection
        var testClassType = Type.GetType(testClassNameWithNamespaceAndAssemblyQualifiedName);

        if (testClassType is not null)
        {
            // Fetch dependencies from DependsOn attributes
            var dependencies = testClassType.GetCustomAttributes(typeof(DependsOnClassAttribute), true).Cast<DependsOnClassAttribute>().Select(attr => attr.DependentClassName).ToList();

            foreach (var dependency in dependencies)
            {
                var dependentCollection = allCollections.FirstOrDefault(c => Type.GetType(GetClassNameWithNamespaceAndAssemblyQualifiedName(c.TestAssembly.AssemblyName, GetClassNameWithNamespace(c.TestCollectionDisplayName))) == dependency);

                if (dependentCollection is not null)
                {
                    Visit(dependentCollection, allCollections, sorted, visited);
                }
            }
        }

        // Add the current collection to the sorted list
        sorted.Add(collection);
    }

    /// <summary>
    /// Extracts the class name with namespace from the test collection display name.
    /// </summary>
    /// <param name="testCollectionDisplayName">The test collection display name.</param>
    /// <returns>The class name with namespace.</returns>
    private static string GetClassNameWithNamespace(string testCollectionDisplayName)
    {
        const string getClassNamePattern = @"for ([\w\.]+)";

        var match = Regex.Match(testCollectionDisplayName, getClassNamePattern);

        if (!match.Success)
        {
            return string.Empty;
        }

        var classNameWithNamespace = match.Groups[1].Value;

        return classNameWithNamespace;

    }

    /// <summary>
    /// Constructs the assembly qualified name for the class.
    /// </summary>
    /// <param name="assemblyName">The assembly name.</param>
    /// <param name="testClassNameWithNamespace">The class name with namespace.</param>
    /// <returns>The assembly qualified name for the class.</returns>
    private static string GetClassNameWithNamespaceAndAssemblyQualifiedName(string assemblyName, string testClassNameWithNamespace)
    {
        var testClassNameWithNamespaceAndAssemblyQualifiedName = $"{testClassNameWithNamespace}, {assemblyName}";

        return testClassNameWithNamespaceAndAssemblyQualifiedName;

    }
}
