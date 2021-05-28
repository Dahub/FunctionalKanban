namespace FunctionalKanban.Core.Domain.Project.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetProjectByIdQuery : Query
    {
        public Guid Id { get; private set; }

        public GetProjectByIdQuery WithId(Guid id) => this with { Id = id };

        public override Func<ViewProjection, bool> BuildPredicate() => (viewProjection) =>
            viewProjection is ProjectViewProjection p
            && p.Id.Equals(Id)
            && p.IsDeleted.EqualTo(false);

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetProjectByIdQuery, Guid>(parameters, "id", WithId).
            ToExceptional();
    }
}