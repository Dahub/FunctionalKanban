namespace FunctionalKanban.Application.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Queries;
    using FunctionalKanban.Domain.Task.Queries;
    using LaYumba.Functional;
    using QueriesBuilders;

    public static class QueryBuilder
    {
        public static Exceptional<Query> BuildQuery<TQuery>(IDictionary<string, string> parameters) where TQuery : Query, new() => 
            new TQuery() switch
            {
                GetTaskQuery q          => GetTaskQueryBuilder.Build(q, parameters),
                GetTaskByIdQuery q      => GetTaskByIdQueryBuilder.Build(q, parameters),
                GetProjectByIdQuery q   => GetProjectByIdQueryBuilder.Build(q, parameters),
                _                       => new Exception("Type de requête non pris en charge")
            };
    }
}
