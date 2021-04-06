namespace FunctionalKanban.Application
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public static class QueryBuilder
    {
        public static Exceptional<Query> BuildQuery<T>(IDictionary<string, string> parameters) where T : ViewProjection
        {
            return Try<Query>(() =>
            {
                if (typeof(T) == typeof(TaskViewProjection))
                {
                    return BuildGetTaskQuery(parameters);
                }
                else
                {
                    throw new Exception("Type de projection non pris en charge");
                }
            }).Run();
        }

        private static GetTaskQuery BuildGetTaskQuery(IDictionary<string, string> parameters)
        {
            var query = new GetTaskQuery();

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
