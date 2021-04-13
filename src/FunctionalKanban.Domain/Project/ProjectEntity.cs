namespace FunctionalKanban.Domain.Project
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Domain.Project.Events;
    using FunctionalKanban.Functional;

    public static class ProjectEntity
    {
        private static readonly string _aggregateName = typeof(ProjectEntity).FullName ?? string.Empty;

        public static Validation<EventAndState> Create(CreateProject cmd)
        {
            var @event = new ProjectCreated()
            {
                AggregateId = cmd.AggregateId,
                AggregateName = _aggregateName,
                Name = cmd.Name,
                IsDeleted = false,
                TimeStamp = cmd.TimeStamp,
                Status = ProjectStatus.New,
                EntityVersion = 1
            };

            return new ProjectEntityState().ApplyEvent(@event);
        }
    }
}
