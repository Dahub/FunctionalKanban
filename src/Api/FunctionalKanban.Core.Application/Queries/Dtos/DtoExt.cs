namespace FunctionalKanban.Application.Queries.Dtos
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public static class DtoExt
    {
        public static Exceptional<IEnumerable<TDto>> ToDto<TProjection, TDto>(this IEnumerable<TProjection> projections)
                where TProjection : ViewProjection
                where TDto : Dto =>
            Try(() => projections.Map(p => (TDto)Convert(p))).Run();

        private static Dto Convert<TProjection>(TProjection projection) where TProjection : ViewProjection =>
            projection switch
            {
                TaskViewProjection p    => TaskDto.FromProjection(p),
                ProjectViewProjection p => ProjectDto.FromProjection(p),
                _                       => throw new Exception($"Type de projection {projection.GetType().Name} non pris en charge")
            };
    }
}
