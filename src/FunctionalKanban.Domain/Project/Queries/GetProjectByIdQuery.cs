namespace FunctionalKanban.Domain.Project.Queries
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;

    public record GetProjectByIdQuery : Query
    {
        public Guid Id { get; private set; }

        public GetProjectByIdQuery WithId(Guid id) => this with { Id = id };

        public override Func<ViewProjection, bool> BuildPredicate() => (p) =>
            ((ProjectViewProjection)p).Id.Equals(Id)
            && EqualToValue(((ProjectViewProjection)p).IsDeleted, false);
    }
}