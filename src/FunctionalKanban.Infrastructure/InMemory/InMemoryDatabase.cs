namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Task.ViewProjections;

    public class InMemoryDatabase : IInMemoryDatabase
    {
        private readonly List<EventLine> _eventLines;

        private readonly ConcurrentDictionary<Guid, TaskViewProjection> _taskViewProjections;

        public InMemoryDatabase()
        {
            _eventLines = new List<EventLine>();
            _taskViewProjections = new ConcurrentDictionary<Guid, TaskViewProjection>();
        }

        public IEnumerable<EventLine> EventLines => _eventLines.AsReadOnly();

        public IEnumerable<TaskViewProjection> TaskViewProjections => _taskViewProjections.Values.ToList().AsReadOnly();

        public void Add(EventLine evtLine) => _eventLines.Add(evtLine);

        public void Upsert(TaskViewProjection viewProjection)
        {
            if(_taskViewProjections.ContainsKey(viewProjection.Id))
            {
                _taskViewProjections[viewProjection.Id] = viewProjection;
            }
            else
            {
                _taskViewProjections.TryAdd(viewProjection.Id, viewProjection);
            }
        }
    }

    public interface IInMemoryDatabase
    {
        IEnumerable<EventLine> EventLines { get; }

        IEnumerable<TaskViewProjection> TaskViewProjections { get; }

        void Add(EventLine evtLine);

        void Upsert(TaskViewProjection viewProjection);
    }
}
