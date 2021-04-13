﻿namespace FunctionalKanban.Application.Dtos
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

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
                _                       => throw new Exception($"Type de projection {projection.GetType().Name} non pris en charge")
            };
    }
}
