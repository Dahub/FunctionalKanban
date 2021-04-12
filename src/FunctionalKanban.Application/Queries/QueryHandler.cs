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

        public Exceptional<IEnumerable<TDto>> Handle<TQuery, TDto>(Dictionary<string, string> parameters)
                where TQuery : Query, new()
                where TDto : Dto =>
            BuildQuery<TQuery>(parameters).
            Bind(LoadProjections<TDto>).
            Bind(d => Convert<TDto>(d));

        private Exceptional<IEnumerable<Dto>> LoadProjections<TDto>(Query query)
            where TDto : Dto =>
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

        private Exceptional<IEnumerable<Dto>> ConvertToDto<TProjection, TDto>(IEnumerable<ViewProjection> projections)
                where TProjection : ViewProjection    
                where TDto : Dto =>
            projections.
            Map(r => (TProjection)r).
            ToDto<TProjection, TDto>().
            Bind(Convert);

         private static Exceptional<IEnumerable<Dto>> Convert<TDto>(IEnumerable<TDto> dtos) where TDto : Dto =>
            Try(() => dtos.Map(d => (Dto)d)).Run();

        private static Exceptional<IEnumerable<TDto>> Convert<TDto>(IEnumerable<Dto> dtos) where TDto : Dto =>
            Try(() => dtos.Map(d => (TDto)d)).Run();
    }
}
