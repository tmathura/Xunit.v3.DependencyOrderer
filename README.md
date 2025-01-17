# Xunit.v3.DependencyOrderer

## Overview

`Xunit.v3.DependencyOrderer` is a .NET library that provides functionality to order xUnit tests based on their dependencies/priority. This library is particularly useful for scenarios where certain tests need to be executed in a specific order due to dependencies between them.

## Features

- **Test Dependency Management**: Define dependencies between classes/test methods to ensure they are executed in the correct order.
- **Priority Dependency Management**: Define priority between test methods to ensure they are executed in the prioritized order.
- **Integration with xUnit v3**: Seamlessly integrates with xUnit v3, leveraging its extensibility features.
- **Support for .NET 8**: Targets .NET 8, ensuring compatibility with the latest .NET features and improvements.

## Getting Started

### Prerequisites

- .NET 8 SDK
- xUnit v3

### Installation

To install the `Xunit.v3.TestDependencyOrderer` or `Xunit.v3.PriorityDependencyOrderer` library, add the following package reference to your project file:

```xml
<PackageReference Include="Xunit.v3.TestDependencyOrderer" Version="1.0.0" />
<PackageReference Include="Xunit.v3.PriorityDependencyOrderer" Version="1.0.0" />
```

### Usage

#### Xunit.v3.TestDependencyOrderer

1. **Define Class Dependencies**: Use the `[DependsOnClass]` attribute to specify dependencies between classes.

2. **Define Test Dependencies**: Use the `[DependsOnMethod]` attribute to specify dependencies between test methods.

3. **Run Tests**: Execute your tests using the xUnit test runner. The tests will be executed in the order defined by their dependencies.

#### Xunit.v3.PriorityDependencyOrderer

1. **Define Test Priorities**: Use the `[Priority]` attribute to specify the priority of test methods. Lower priority values are executed first.

2. **Run Tests**: Execute your tests using the xUnit test runner. The tests will be executed in the order defined by their priorities.

### Example

#### Xunit.v3.TestDependencyOrderer

```cs
using Xunit;
using Xunit.v3.DependencyOrderer;

public class MyTestClass1
{
    [Fact]
    public void Test1()
    {
        // Test code here
    }

    [Fact]
    [DependsOnMethod(nameof(Test1))]
    public void Test2()
    {
        // Test code here
    }
}

[DependsOnClass(typeof(MyTestClass1))]
public class MyTestClass2
{
    [Fact]
    public void Test3()
    {
        // Test code here
    }
}
```

#### Xunit.v3.PriorityDependencyOrderer

```cs
using Xunit;
using Xunit.v3.PriorityDependencyOrderer;

public class MyTestClass
{
    [Fact, Priority(1)]
    public void Test1()
    {
        // Test code here
    }

    [Fact, Priority(2)]
    public void Test2()
    {
        // Test code here
    }

    [Fact, Priority(0)]
    public void Test3()
    {
        // Test code here
    }
}
```

### Project Structure

- **Xunit.v3.PriorityDependencyOrderer**: Contains the core library for managing test priority ordering.
- **Xunit.v3.PriorityDependencyOrderer.UnitTests**: Contains unit tests for the `Xunit.v3.PriorityDependencyOrderer` library.
- **Xunit.v3.TestDependencyOrderer**: Contains the core library for managing class/test dependencies ordering.
- **Xunit.v3.TestDependencyOrderer.UnitTests**: Contains unit tests for the `Xunit.v3.TestDependencyOrderer` library.

### Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub. For detailed instructions, please refer to the [CONTRIBUTING](CONTRIBUTING.md) file.

### License

This project is licensed under the GNU General Public License v3.0. See the [LICENSE](LICENSE) file for details.

### Acknowledgements

- [xUnit.net](https://xunit.net/) for providing a robust testing framework.
- The .NET community for their continuous support and contributions.

## Contact

For any questions or feedback, please open an issue on the GitHub repository.

---

Thank you for using `Xunit.v3.DependencyOrderer`! I hope it helps you manage your test dependencies effectively.
