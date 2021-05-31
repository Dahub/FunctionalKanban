namespace FunctionalKanban.Core.Application.Queries.Dtos
{
    using System;
    using FunctionalKanban.Core.Domain.Task;
    using FunctionalKanban.Core.Domain.ViewProjections;

    public record TaskDto : Dto
    {
        public string? Name { get; init; }

        public int RemaningWork { get; init; }

        public TaskStatus Status { get; init; }

        public Guid? ProjectId { get; init; }

        public static Dto FromProjection(TaskViewProjection projection) =>
            new TaskDto()
            {
                Id = projection.Id,
                Name = projection.Name,
                RemaningWork = projection.RemaningWork,
                Status = projection.Status,
                ProjectId = projection.ProjectId.Match(None: () => (Guid?)null, Some: (id) => id)
            };
    }
}
