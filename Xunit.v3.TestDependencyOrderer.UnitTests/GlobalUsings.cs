using Xunit.v3.TestDependencyOrderer;
using Xunit.v3.TestDependencyOrderer.TestContext;

[assembly: AssemblyFixture(typeof(GlobalTestContext))]
[assembly: TestCollectionOrderer(typeof(TestDependencyCollectionOrderer))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
