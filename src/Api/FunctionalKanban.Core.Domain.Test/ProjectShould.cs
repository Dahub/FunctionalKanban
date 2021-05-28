namespace FunctionalKanban.Domain.Test
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Domain.Project.Events;
    using LaYumba.Functional;
    using Xunit;

    public class ProjectShould
    {
        [Fact]
        public void ReturnProjectStateWithIsDeletedToTrueAndFireProjectDeletedEventWhenDeleted()
        {
            var entityId = Guid.NewGuid();

            var projectEntityState = new ProjectEntityState()
            {
                ProjectId = entityId,
                IsDeleted = false,
                Version = 1
            };

            var deleteProjectCommand = new DeleteProject()
            {
                DeleteChildrenTasks = false,
                EntityId = entityId
            };

            var eventAndState = projectEntityState.Delete(deleteProjectCommand);

            var expectedEvent = new ProjectDeleted()
            {
                EntityId = entityId,
                DeleteChildrenTasks = false,
                EntityName = typeof(ProjectEntityState).FullName,
                EntityVersion = 2,
                TimeStamp = deleteProjectCommand.TimeStamp,
                IsDeleted = true
            };

            eventAndState.Match(
                Invalid:    (errors)    => false,
                Valid:      (eas)       => 
                    ((ProjectEntityState)eas.State).IsDeleted
                    && ((ProjectDeleted)eas.Event) == expectedEvent).Should().BeTrue();
        }

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

        [Fact]
        public void BeSomeWhenHydrateWithConsecutivesEvents()
        {
            var events = new List<Event>()
            {
                new ProjectCreated()
                {
                    EntityId = Guid.NewGuid(),
                    EntityName = "test",
                    EntityVersion = 1,
                    IsDeleted = false,
                    Name = "test",
                    Status = ProjectStatus.New,
                    TimeStamp = DateTime.Now
                }
            };

            var project = new ProjectEntityState().From(events);

            project.Match(
                None: () => false,
                Some: (_) => true).Should().BeTrue();
        }

        [Fact]
        public void AddTaskToProjectTasksAndFireProjectNewTaskLinkedEvent()
        {
            var entityId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var timeStamp = DateTime.Now;

            var eventAndState = BuildNewProject(entityId).
                Bind(eas => ((ProjectEntityState)eas.State).AddTaskToProject(timeStamp, taskId));

            eventAndState.Match(
                Invalid : (_)   => false,
                Valid :   (eas) => ((ProjectEntityState)eas.State).AssociatedTaskIds.ToList().Contains(taskId)).
            Should().BeTrue();
        }

        private static Validation<EventAndState> BuildNewProject(Guid entityId, string projectName = "fake project") =>
            ProjectEntity.Create(BuildCreateProjectCommand(entityId, projectName));

        private static CreateProject BuildCreateProjectCommand(Guid entityId, string projectName) =>
            new()
            {
                EntityId = entityId,
                Name = projectName
            };
    }
}
