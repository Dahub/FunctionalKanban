namespace FunctionalKanban.Infrastructure.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using Xunit;
    using Unit = System.ValueTuple;

    public class EventBusShould
    {
        public record DumbEvent() : Event;

        [Fact]
        public void PublishAndNotifyEvents()
        {
            var isPublished = false;
            var isNotified = false;

            var eventBus = new EventBus(
                (_) => { isPublished = true; return Unit.Create(); },
                (_) => { isNotified = true; return Unit.Create(); });

            eventBus.Publish(BuildDumbEvent());

            isPublished.Should().BeTrue();
            isNotified.Should().BeTrue();
        }

        [Fact]
        public void NotNotifyWhenPublishFail()
        {
            var isNotified = false;

            var eventBus = new EventBus(
                (_) => new Exception("fail"),
                (_) => { isNotified = true; return Unit.Create(); });

            eventBus.Publish(BuildDumbEvent());

            isNotified.Should().BeFalse();
        }

        private static DumbEvent BuildDumbEvent() => new()
        {
            EntityId = Guid.NewGuid(),
            EntityName = Guid.NewGuid().ToString(),
            EntityVersion = 1,
            TimeStamp = DateTime.Now
        };
    }
}
