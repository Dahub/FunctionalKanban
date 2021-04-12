namespace FunctionalKanban.Application.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Application.Dtos;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using static QueryBuilder;

    public class QueryHandler
    {
        private readonly Func<Type, Func<ViewProjection, bool>, Exceptional<IEnumerable<ViewProjection>>> _findProjections;

        public QueryHandler(Func<Type, Func<ViewProjection, bool>, Exceptional<IEnumerable<ViewProjection>>> findProjections) =>
            _findProjections = findProjections;

        public Exceptional<IEnumerable<TDto>> Handle<TQuery, TDto>(Dictionary<string, string> parameters)
                where TQuery : Query, new()
                where TDto : Dto =>
            BuildQuery<TQuery>(parameters).
            Bind(LoadProjections).
            Bind(ConvertToDto<TDto>);

        private Exceptional<IEnumerable<TDto>> ConvertToDto<TDto>(IEnumerable<ViewProjection> projections)
                where TDto : Dto =>
            projections.Map(r => (TaskViewProjection)r).ToDto<TaskViewProjection, TDto>();

        private Exceptional<IEnumerable<ViewProjection>> LoadProjections(Query query) => 
            query switch
            {
                GetTaskQuery q => GetViewProjections<TaskViewProjection>(q),
                GetTaskByIdQuery q => GetViewProjections<TaskViewProjection>(q),
                _ => new Exception("Type de requête non pris en charge")
            };

        private Exceptional<IEnumerable<ViewProjection>> GetViewProjections<TProjection>(Query q)
                where TProjection : ViewProjection => _findProjections(typeof(TProjection), q.BuildPredicate());
    }
}
