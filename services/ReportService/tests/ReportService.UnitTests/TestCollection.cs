using Xunit;

namespace ReportService.UnitTests;

[CollectionDefinition("SequentialTests")]
public class SequentialTestsCollection : ICollectionFixture<BaseTest> { }