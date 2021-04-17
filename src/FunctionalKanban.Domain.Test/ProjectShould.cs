namespace FunctionalKanban.Domain.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Commands;
    using LaYumba.Functional;
    using Xunit;

    public class ProjectShould
    {
        [Fact]
        public void ReturnProjectStateWithNameAndStatusWhenCreated()
        {
            var expectedTaskName = Guid.NewGuid().ToString();
            var expectedTaskStatus = ProjectStatus.New;
            var entityId = Guid.NewGuid();

            var eventAndTask = BuildNewProject(entityId, expectedTaskName);

            bool CheckEquality(ProjectEntityState s) =>
                s.ProjectName.Equals(expectedTaskName)
                && s.ProjectStatus.Equals(expectedTaskStatus)
                && s.IsDeleted.Equals(false);

            eventAndTask.Match(
                Invalid: (errors) => false,
                Valid: (eas) => CheckEquality((ProjectEntityState)eas.State)).Should().BeTrue();
        }

        private static Validation<EventAndState> BuildNewProject(Guid entityId, string projectName = "fake task") =>
            ProjectEntity.Create(BuildCreateProjectCommand(entityId, projectName));

        private static CreateProject BuildCreateProjectCommand(Guid entityId, string projectName) =>
            new()
            {
                EntityId = entityId,
                Name = projectName
            };
    }
}
