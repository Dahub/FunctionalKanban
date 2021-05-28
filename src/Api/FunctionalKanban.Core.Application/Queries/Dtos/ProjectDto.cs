namespace FunctionalKanban.Core.Application.Queries.Dtos
{
    using FunctionalKanban.Core.Domain.Project;
    using FunctionalKanban.Core.Domain.ViewProjections;

    public record ProjectDto : Dto
    {
        public string? Name { get; init; }

        public ProjectStatus Status { get; init; }

        public bool IsDeleted { get; init; }

        public static Dto FromProjection(ProjectViewProjection projection) =>
            new ProjectDto()
            {
                Id = projection.Id,
                Name = projection.Name,
                Status = projection.Status,
                IsDeleted = projection.IsDeleted
            };
    }
}
