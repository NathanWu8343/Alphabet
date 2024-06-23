using MassTransit;
using MassTransit.Metadata;
using System.Reflection;

namespace UrlShortener.Application.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IBusRegistrationConfigurator"/>.
    /// </summary>
    public static class MassTransitRegistrationExtensions
    {
        /// <summary>
        /// Adds all consumers from the assembly containing the specified type.
        /// </summary>
        /// <remarks>
        /// This method differs slightly from <see cref="RegistrationExtensions.AddConsumersFromNamespaceContaining(IRegistrationConfigurator, Type, Func{Type, bool})"/> in the following ways:
        /// 1. It only grabs consumers from the assembly of the specified anchor type, instead of all nested assemblies matching the specified assembly's namespace.
        /// 2. It grabs all consumers regardless of the accessibility, instead of only public ones.
        /// </remarks>
        /// <typeparam name="T">The anchor type.</typeparam>
        /// <param name="configurator">The container registration configurator.</param>
        public static void AddConsumersFromAssemblyContaining(this IMediatorRegistrationConfigurator configurator, Assembly assembly)
        {
            //TODO: 需要再區分 Command or Query 來避免與事件中重複註冊
            IEnumerable<Type> types = assembly.GetTypes().Where(t => RegistrationMetadata.IsConsumerOrDefinition(t));
            configurator.AddConsumers(types.ToArray());
        }
    }
}