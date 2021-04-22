namespace FunctionalKanban.Application.Dtos
{
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.ViewProjections;

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
