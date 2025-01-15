namespace Xunit.v3.TestDependencyOrderer.Exceptions;

/// <summary>
/// Exception that is thrown when a dependent test has not been completed.
/// </summary>
internal class DependentTestNotCompleteException(string message) : Exception(message);
