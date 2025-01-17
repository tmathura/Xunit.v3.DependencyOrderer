namespace Xunit.v3.TestDependencyOrderer.Exceptions;

/// <summary>
/// Exception that is thrown when a dependent test class is not complete.
/// </summary>
internal class DependentTestClassNotCompleteException(string message) : Exception(message);
