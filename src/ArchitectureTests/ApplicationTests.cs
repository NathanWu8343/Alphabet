using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests
{
    public class ApplicationTests
    {
        private const string DomainNameSpace = "UrlShortener.Domain";

        [Fact]
        public void Handlers_Should_Have_DependencyOnDomain()
        {
            // Arrange
            var assembly = UrlShortener.Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .HaveNameStartingWith("Handler")
                .Should()
                .HaveDependencyOn(DomainNameSpace)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }
    }
}