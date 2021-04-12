namespace FunctionalKanban.Application.Dtos
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public record TaskDto : Dto
    {
        public string? Name { get; init; }

        public uint RemaningWork { get; init; }

        public TaskStatus Status { get; init; }

        public bool IsDeleted { get; init; }

        public Guid? ProjectId { get; init; }

        public static Dto FromProjection(TaskViewProjection projection) =>
            new TaskDto()
            {
                Id = projection.Id,
                Name = projection.Name,
                RemaningWork = projection.RemaningWork,
                Status = projection.Status,
                IsDeleted = projection.IsDeleted,
                ProjectId = projection.ProjectId.Match(None: () => (Guid?)null, Some: (id) => id)
            };
    }
}
