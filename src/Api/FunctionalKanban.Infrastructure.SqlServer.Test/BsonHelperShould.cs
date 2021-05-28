namespace FunctionalKanban.Infrastructure.SqlServer.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Infrastructure.SqlServer.EventDatabase;
    using Xunit;

    public class BsonHelperShould
    {
        [Fact]
        public void SerializeAndDeserializeEvent()
        {
            var @event = new DumbEvent()
            {
                EntityId = Guid.NewGuid(),
                EntityName = "dumb",
                EntityVersion = 1,
                TimeStamp = DateTime.Now
            };

            var serializedEvent = BsonHelper.ToBson(@event);

            var deserializedEvent = BsonHelper.FromBson<DumbEvent>(serializedEvent);

            deserializedEvent.Match(
                None: () => false,
                Some: (evt) => evt.Equals(@event)).Should().BeTrue();
        }

        private record DumbEvent : Event { }
    }
}
