using Bogus;
using DatePlanner.Domain.Abstractions;

namespace DatePlanner.UnitTests.Abstractions;

public class BaseTest
{
    protected static readonly Faker Faker = new();

    public static T AssertDomainEventWasPublished<T>(Entity entity) where T : IDomainEvent
    {
        T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();

        if (domainEvent is null)
        {
            throw new Exception($"{typeof(T).Name} was not published");
        }

        return domainEvent;
    }
}