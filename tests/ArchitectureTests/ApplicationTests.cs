using FluentAssertions;
using NetArchTest.Rules;
using SharedKernel.Messaging;

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
                .HaveNameEndingWith("Handler")
                .Should()
                .HaveDependencyOn(DomainNameSpace)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void CommandHandler_Should_Have_NameEndingWithCommandHandler()
        {
            // Arrange
            var assembly = UrlShortener.Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .ImplementInterface(typeof(ICommandHandler<,>))
                .Should()
                .HaveNameEndingWith("CommandHandler")
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void CommandHandlers_Should_NotBePublicAndBeSealed()
        {
            // Arrange
            var assembly = UrlShortener.Application.AssemblyReference.Assembly;

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .That()
                .HaveNameEndingWith("CommandHandler")
                .Should()
                .NotBePublic()
                .And()
                .BeSealed()
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }
    }
}