using DatePlanner.Domain.Abstractions;

namespace DatePlanner.UnitTests.Abstractions;

public class BaseTest
{
    protected static T AssertDomainEventWasPublished<T>(Entity entity) where T : IDomainEvent
    {
        T[] domainEvents = entity.DomainEvents.OfType<T>().ToArray();

        if (domainEvents.Length != 1)
        {
            throw new Xunit.Sdk.XunitException(
                $"Expected exactly one {typeof(T).Name}, but found {domainEvents.Length}.");
        }

        return domainEvents[0];
    }
}
