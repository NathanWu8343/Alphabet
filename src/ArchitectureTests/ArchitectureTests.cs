using FluentAssertions;
using NetArchTest.Rules;

namespace ArchitectureTests
{
    public class ArchitectureTests
    {
        private const string ApplicationNameSpace = "UrlShortener.Application";
        private const string InfrastructureNameSpace = "UrlShortener.Infrastructure";
        private const string PersistenceNameSpace = "UrlShortener.Persistence";
        private const string PresentationNameSpace = "UrlShortener.Api";
        //private const string WebNameSpace = "Web";

        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = UrlShortener.Domain.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                ApplicationNameSpace,
                InfrastructureNameSpace,
                PresentationNameSpace,
                PersistenceNameSpace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = UrlShortener.Application.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                InfrastructureNameSpace,
                PresentationNameSpace,
                PersistenceNameSpace,
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void PersistenceNameSpace_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = UrlShortener.Persistence.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                InfrastructureNameSpace,
                PresentationNameSpace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .Should()
                .NotHaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void InfrastructureNameSpace_Should_Not_HaveDependencyOnOtherProjects()
        {
            // Arrange
            var assembly = UrlShortener.Infrastructure.AssemblyReference.Assembly;

            var otherProjects = new[]
            {
                PresentationNameSpace
            };

            // Act
            var testResult = Types
                .InAssembly(assembly)
                .Should()
                .NotHaveDependencyOnAny(otherProjects)
                .GetResult();

            // Assert
            testResult.IsSuccessful.Should().BeTrue();
        }
    }
}