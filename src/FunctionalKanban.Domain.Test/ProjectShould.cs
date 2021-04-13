namespace FunctionalKanban.Domain.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Functional;
    using Xunit;

    public class ProjectShould
    {
        [Fact]
        public void ReturnProjectStateWithNameAndStatusWhenCreated()
        {
            var expectedTaskName = Guid.NewGuid().ToString();
            var expectedTaskStatus = ProjectStatus.New;
            var aggregateId = Guid.NewGuid();

            var eventAndTask = BuildNewProject(aggregateId, expectedTaskName);

            bool CheckEquality(ProjectEntityState s) =>
                s.ProjectName.Equals(expectedTaskName)
                && s.ProjectStatus.Equals(expectedTaskStatus)
                && s.IsDeleted.Equals(false);

            eventAndTask.Match(
                Invalid: (errors) => false,
                Valid: (eas) => CheckEquality((ProjectEntityState)eas.State)).Should().BeTrue();
        }

        private static Validation<EventAndState> BuildNewProject(Guid aggregateId, string projectName = "fake task") =>
            ProjectEntity.Create(BuildCreateProjectCommand(aggregateId, projectName));

        private static CreateProject BuildCreateProjectCommand(Guid aggregateId, string projectName) =>
            new CreateProject()
            {
                AggregateId = aggregateId,
                Name = projectName
            };
    }
}
