namespace FunctionalKanban.Domain.Task.Queries
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;

    public record GetTaskByIdQuery : Query
    {
        public Guid Id { get; private set; }

        public GetTaskByIdQuery WithId(Guid id) => this with { Id = id };

        public override Func<ViewProjection, bool> BuildPredicate() => (p) => 
            ((TaskViewProjection)p).Id.Equals(Id)
            && EqualToValue(((TaskViewProjection)p).IsDeleted, false);
    }
}
