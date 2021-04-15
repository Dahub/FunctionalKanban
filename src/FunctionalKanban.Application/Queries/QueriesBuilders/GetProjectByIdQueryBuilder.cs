namespace FunctionalKanban.Application.Queries.QueriesBuilders
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Queries;
    using FunctionalKanban.Functional;

    public static class GetProjectByIdQueryBuilder
    {
        public static Exceptional<Query> Build(GetProjectByIdQuery query, IDictionary<string, string> parameters) =>
              query.
              WithParameterValue<GetProjectByIdQuery, Guid>(parameters, "id", query.WithId).
              ToExceptional();
    }
}
