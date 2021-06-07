namespace FunctionalKanban.Core.Domain.Project.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetProjectByIdQuery : Query
    {
        protected override Expression<Func<ViewProjection, bool>> Predicate { get; init; } = (p) => p is ProjectViewProjection;

        public GetProjectByIdQuery WithId(Guid id) => this with { Predicate = PredicateBuilder.And(Predicate, (p) => p.Id == id) };

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetProjectByIdQuery, Guid>(parameters, "id", WithId).
            ToExceptional();
    }
}