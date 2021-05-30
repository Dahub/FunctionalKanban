namespace FunctionalKanban.Infrastructure.SqlServer.EventDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Infrastructure.SqlServer.EventDatabase.EfEntities;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    internal static class SqlServerEventDatabaseExt
    {
        public static EventEfEntity ToEfEntity<T>(this T @event) where T : notnull, Event=>
            new()
            {
                EntityId = @event.EntityId,
                EntityName = @event.EntityName,
                EventName = @event.EventName,
                Id = Guid.NewGuid(),
                TimeStamp = @event.TimeStamp,
                Version = (int)@event.EntityVersion,
                EventDatas = BsonHelper.ToBson(@event)
            };

        public static Option<Event> ToEvent(this EventEfEntity eventEfEntity)
        {
            var eventType = Assembly.GetAssembly(typeof(Event))?.GetType(eventEfEntity.EventName);
            return eventType == null
                ? None
                : BsonHelper.FromBson(eventType, eventEfEntity.EventDatas);
        }

        public static IEnumerable<Event> ToEvent(this IEnumerable<EventEfEntity> eventEfEntities) =>
            eventEfEntities.Aggregate(
                seed: new List<Event>(),
                func: (list, next) => next.ToEvent().Match(
                    Some: (e)   => { list.Add(e); return list; },
                    None: ()    => list));

    }
}
