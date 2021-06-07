namespace FunctionalKanban.Core.Domain.Task.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetTaskByIdQuery : Query
    {
        protected override Expression<Func<ViewProjection, bool>> Predicate { get; init; } = (p) => p is TaskViewProjection;

        public GetTaskByIdQuery WithId(Guid id) => this with { Predicate = Predicate.And((p) => p.Id == id) };

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetTaskByIdQuery, Guid>(parameters, "id", WithId).
            ToExceptional();
    }
}
