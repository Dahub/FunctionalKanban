namespace FunctionalKanban.Core.Domain.Task.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetTaskByIdQuery : Query
    {
        public Guid Id { get; private set; }

        public GetTaskByIdQuery WithId(Guid id) => this with { Id = id };

        public override Func<ViewProjection, bool> BuildPredicate() => (viewProjection) =>
             viewProjection is TaskViewProjection p
             && p.Id.Equals(Id);

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetTaskByIdQuery, Guid>(parameters, "id", WithId).
            ToExceptional();
    }
}
