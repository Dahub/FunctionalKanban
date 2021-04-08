namespace FunctionalKanban.Application.QueriesBuilders
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Functional;

    internal static class GetTaskQueryBuilder
    {
        public static Exceptional<Query> Build(GetTaskQuery query, IDictionary<string, string> parameters) =>
            query.WithParameterValue<GetTaskQuery, uint>(parameters, "minRemaningWork", query.WithMinRemaningWork)
                .Bind(q => q.WithParameterValue<GetTaskQuery, uint>(parameters, "maxRemaningWork", q.WithMaxRemaningWork))
                .Bind(q => q.WithParameterValue<GetTaskQuery, Domain.Task.TaskStatus>(parameters, "taskStatus", q.WithTaskStatus))
                .ToExceptional();
    }
}
