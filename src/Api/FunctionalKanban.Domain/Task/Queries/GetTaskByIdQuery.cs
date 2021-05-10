namespace FunctionalKanban.Domain.Task.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetTaskByIdQuery : Query
    {
        public Guid Id { get; private set; }

        public GetTaskByIdQuery WithId(Guid id) => this with { Id = id };

        public override Func<ViewProjection, bool> BuildPredicate() => (p) =>
             ((TaskViewProjection)p).Id.Equals(Id);

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetTaskByIdQuery, Guid>(parameters, "id", WithId).
            ToExceptional();
    }
}
