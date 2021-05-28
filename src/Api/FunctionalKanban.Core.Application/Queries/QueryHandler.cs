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
    using ExceptionalViewProjections = LaYumba.Functional.Exceptional<System.Collections.Generic.IEnumerable<Domain.Common.ViewProjection>>;
    using ExceptionalDtos = LaYumba.Functional.Exceptional<System.Collections.Generic.IEnumerable<Dtos.Dto>>;

    public class QueryHandler
    {
        private readonly Func<Type, Predicate, ExceptionalViewProjections> _findProjections;

        public QueryHandler(
                Func<Type, Predicate, ExceptionalViewProjections> findProjections) =>
            _findProjections = findProjections;

        public ExceptionalDtos Handle<TQuery>(Dictionary<string, string> parameters)
                where TQuery : Query, new() =>
            new TQuery().WithParameters(parameters).
            Bind(GetProjections);

        private ExceptionalDtos GetProjections(Query query) =>
            query switch
            {
                GetTaskQuery q          => ApplyQueryToViewProjections<TaskViewProjection, TaskDto>(q),
                GetTaskByIdQuery q      => ApplyQueryToViewProjections<TaskViewProjection, TaskDto>(q),
                GetProjectByIdQuery q   => ApplyQueryToViewProjections<ProjectViewProjection, ProjectDto>(q),
                _                       => new Exception("Type de requête non pris en charge")
            };

        private ExceptionalDtos ApplyQueryToViewProjections<TProjection, TDto>(Query q)
                where TProjection : ViewProjection
                where TDto : Dto =>
            _findProjections(typeof(TProjection), q.BuildPredicate())
            .Bind(ConvertToDto<TProjection, TDto>);

        private static ExceptionalDtos ConvertToDto<TProjection, TDto>(IEnumerable<ViewProjection> projections)
                where TProjection : ViewProjection    
                where TDto : Dto =>
            projections.
            Map(r => (TProjection)r).
            ToDto<TProjection, TDto>().
            Bind(Convert);

         private static ExceptionalDtos Convert<TDto>(IEnumerable<TDto> dtos) where TDto : Dto =>
            Try(() => dtos.Map(d => (Dto)d)).Run();
    }
}
