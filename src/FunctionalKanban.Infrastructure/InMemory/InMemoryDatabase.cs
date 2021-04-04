namespace FunctionalKanban.Infrastructure.InMemory
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.ViewProjections;

    public class InMemoryDatabase : IInMemoryDatabase
    {
        private readonly List<EventLine> _eventLines;

        private readonly IDictionary<Guid, TaskViewProjection> _taskViewProjections;

        public InMemoryDatabase()
        {
            _eventLines = new List<EventLine>();
            _taskViewProjections = new Dictionary<Guid, TaskViewProjection>();
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
                _taskViewProjections.Add(viewProjection.Id, viewProjection);
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
