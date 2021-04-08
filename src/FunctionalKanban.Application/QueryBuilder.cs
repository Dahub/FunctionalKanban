namespace FunctionalKanban.Application
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public static class QueryBuilder
    {
        public static Exceptional<Query> BuildQuery<Q>(IDictionary<string, string> parameters) where Q : Query, new() => 
            Try<Query>(() =>
                new Q() switch
                {
                    GetTaskQuery q  => q.Build(parameters),
                    _               => throw new Exception("Type de projection non pris en charge")
                }).Run();

        private static GetTaskQuery Build(this GetTaskQuery query, IDictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("minRemaningWork"))
            {
                query = query.WithMinRemaningWork(uint.Parse(parameters["minRemaningWork"]));
            }

            if (parameters.ContainsKey("maxRemaningWork"))
            {
                query = query.WithMaxRemaningWork(uint.Parse(parameters["maxRemaningWork"]));
            }

            if (parameters.ContainsKey("taskStatus"))
            {
                Enum.TryParse<Domain.Task.TaskStatus>(parameters["taskStatus"], true, out var taskStatus);
                query = query.WithTaskStatus(taskStatus);
            }

            return query;
        }
    }
}
