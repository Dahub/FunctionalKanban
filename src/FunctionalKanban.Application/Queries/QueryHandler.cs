namespace FunctionalKanban.Application.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Application.Dtos;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;
    using static QueryBuilder;

    public class QueryHandler
    {
        private readonly Func<Type, Func<ViewProjection, bool>, Exceptional<IEnumerable<ViewProjection>>> _findProjections;

        public QueryHandler(Func<Type, Func<ViewProjection, bool>, Exceptional<IEnumerable<ViewProjection>>> findProjections) =>
            _findProjections = findProjections;

        public Exceptional<IEnumerable<Dto>> Handle<TQuery>(Dictionary<string, string> parameters)
                where TQuery : Query, new() =>
            BuildQuery<TQuery>(parameters).
            Bind(LoadProjections);

        private Exceptional<IEnumerable<Dto>> LoadProjections(Query query) =>
            query switch
            {
                GetTaskQuery q => GetViewProjections<TaskViewProjection, TaskDto>(q),
                GetTaskByIdQuery q => GetViewProjections<TaskViewProjection, TaskDto>(q),
                _ => new Exception("Type de requête non pris en charge")
            };

        private Exceptional<IEnumerable<Dto>> GetViewProjections<TProjection, TDto>(Query q)
                where TProjection : ViewProjection
                where TDto : Dto =>
            _findProjections(typeof(TProjection), q.BuildPredicate())
            .Bind(ConvertToDto<TProjection, TDto>);

        private static Exceptional<IEnumerable<Dto>> ConvertToDto<TProjection, TDto>(IEnumerable<ViewProjection> projections)
                where TProjection : ViewProjection    
                where TDto : Dto =>
            projections.
            Map(r => (TProjection)r).
            ToDto<TProjection, TDto>().
            Bind(Convert);

         private static Exceptional<IEnumerable<Dto>> Convert<TDto>(IEnumerable<TDto> dtos) where TDto : Dto =>
            Try(() => dtos.Map(d => (Dto)d)).Run();
    }
}
