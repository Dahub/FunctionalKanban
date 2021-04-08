namespace FunctionalKanban.Application
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Functional;
    using QueriesBuilders;

    public static class QueryBuilder
    {
        public static Exceptional<Query> BuildQuery<TQuery>(IDictionary<string, string> parameters) where TQuery : Query, new() => 
            new TQuery() switch
            {
                GetTaskQuery q      => GetTaskQueryBuilder.Build(q, parameters),
                GetTaskByIdQuery q  => GetTaskByIdQueryBuilder.Build(q, parameters),
                _                   => new Exception("Type de requête non pris en charge")
            };
    }
}
