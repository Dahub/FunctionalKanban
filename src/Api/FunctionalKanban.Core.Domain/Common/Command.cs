namespace FunctionalKanban.Core.Domain.Common
{
    using System;

    public abstract record Command
    {
        private readonly DateTime _timeStamp = DateTime.Now;

        public Guid EntityId { get; init; }

        public DateTime TimeStamp => _timeStamp;
    }
}
