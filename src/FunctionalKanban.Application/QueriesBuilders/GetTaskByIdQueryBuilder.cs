namespace FunctionalKanban.Application.QueriesBuilders
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Functional;

    public static class GetTaskByIdQueryBuilder
    {
        public static Exceptional<Query> Build(GetTaskByIdQuery query, IDictionary<string, string> parameters) =>
            query.
            WithParameterValue<GetTaskByIdQuery, Guid>(parameters, "id", query.WithId).
            ToExceptional();
    }
}
