namespace FunctionalKanban.Domain.Common
{
    using System;

    public record Command
    {
        private readonly DateTime _timeStamp = DateTime.Now;

        public Guid AggregateId { get; init; }

        public DateTime TimeStamp => _timeStamp;
    }
}
