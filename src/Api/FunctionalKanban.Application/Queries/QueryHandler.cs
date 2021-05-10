namespace FunctionalKanban.Application.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Application.Queries.Dtos;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project.Queries;
    using FunctionalKanban.Domain.Task.Queries;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;
    using Predicate = System.Func<Domain.Common.ViewProjection, bool>;

    public class QueryHandler
    {
        private readonly Func<Type, Predicate, Exceptional<IEnumerable<ViewProjection>>> _findProjections;

        public QueryHandler(
                Func<Type, Predicate, Exceptional<IEnumerable<ViewProjection>>> findProjections) =>
            _findProjections = findProjections;

        public Exceptional<IEnumerable<Dto>> Handle<TQuery>(Dictionary<string, string> parameters)
                where TQuery : Query, new() =>
            new TQuery().WithParameters(parameters).
            Bind(LoadProjections);

        private Exceptional<IEnumerable<Dto>> LoadProjections(Query query) =>
            query switch
            {
                GetTaskQuery q          => ApplyQueryToViewProjections<TaskViewProjection, TaskDto>(q),
                GetTaskByIdQuery q      => ApplyQueryToViewProjections<TaskViewProjection, TaskDto>(q),
                GetProjectByIdQuery q   => ApplyQueryToViewProjections<ProjectViewProjection, ProjectDto>(q),
                _                       => new Exception("Type de requête non pris en charge")
            };

        private Exceptional<IEnumerable<Dto>> ApplyQueryToViewProjections<TProjection, TDto>(Query q)
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
