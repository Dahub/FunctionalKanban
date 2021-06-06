namespace FunctionalKanban.Core.Domain.Task.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;

    public record GetTaskByIdQuery : Query
    {
        public Guid Id { get; private set; }

        public GetTaskByIdQuery WithId(Guid id) => this with { Id = id };

        public override Expression<Func<ViewProjection, bool>> BuildPredicate() => (p) => p.Id.Equals(Id);

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetTaskByIdQuery, Guid>(parameters, "id", WithId).
            ToExceptional();
    }
}
