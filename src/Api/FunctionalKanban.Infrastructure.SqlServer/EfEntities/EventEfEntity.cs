namespace FunctionalKanban.Infrastructure.SqlServer.EfEntities
{
    using System;

    internal class EventEfEntity
    {
        public EventEfEntity()
        {
            EntityName = string.Empty;
            EventName = string.Empty;
            EventDatas = Array.Empty<byte>();
        }

        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        public string EntityName { get; set; }

        public int Version { get; set; }

        public string EventName { get; set; }

        public DateTime TimeStamp { get; set; }

        public byte[] EventDatas { get; set; }
    }
}
