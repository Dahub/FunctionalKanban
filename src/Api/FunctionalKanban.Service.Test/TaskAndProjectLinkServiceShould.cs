namespace FunctionalKanban.Service.Test
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Events;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;
    using Xunit;
    using static LaYumba.Functional.F;

    public class TaskAndProjectLinkServiceShould
    {
        [Fact]
        public void ReturnExceptionalWhenHandleLinkToProjectCommandWithExceptionialEntity()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var linkToProject = new LinkToProject() { EntityId = taskId, ProjectId = projectId };

            var eventsAndSates = TaskAndProjectLinkService.HandleLinkToProjectCommand(
               linkToProject,
               (id) => id.Equals(projectId) ?
                           Exceptional(Valid<State>(new ProjectEntityState() { ProjectId = projectId })) :
                           new Exception("ex"));

            eventsAndSates.Exception.Should().BeTrue();
        }

        [Fact]
        public void ReturnInvalidWhenHandleLinkToProjectCommandWithInvalidEntity()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var linkToProject = new LinkToProject() { EntityId = taskId, ProjectId = projectId };

            var eventsAndSates = TaskAndProjectLinkService.HandleLinkToProjectCommand(
                linkToProject,
                (id) => Exceptional(Valid<State>(id.Equals(projectId) ?
                            new ProjectEntityState() { ProjectId = projectId } :
                            new TaskEntityState() { TaskId = taskId, IsDeleted = true })));

            eventsAndSates.Exception.Should().BeFalse();

            eventsAndSates.ForEach(e => e.IsValid.Should().BeFalse());
        }

        [Fact]
        public void GenerateRightEventsWhenHandleLinkToProjectCommand()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var linkToProject = new LinkToProject() { EntityId = taskId, ProjectId = projectId };

            var expectedTaskLinkedToProjectEvent = new TaskLinkedToProject()
            {
                EntityId = taskId,
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                ProjectId = projectId,
                RemaningWork = 0,
                TimeStamp = linkToProject.TimeStamp
            };

            var expectedProjectNewTaskLinked = new ProjectNewTaskLinked()
            {
                EntityId = projectId,
                EntityName = typeof(ProjectEntityState).FullName,
                EntityVersion = 1,
                TaskId = taskId,
                TimeStamp = linkToProject.TimeStamp
            };

            var eventsAndSates = TaskAndProjectLinkService.HandleLinkToProjectCommand(
                linkToProject,
                (id) => Exceptional(Valid<State>(id.Equals(projectId) ? 
                            new ProjectEntityState() { ProjectId = projectId } : 
                            new TaskEntityState() { TaskId = taskId })));

            eventsAndSates.Exception.Should().BeFalse();

            eventsAndSates.ForEach(e => e.IsValid.Should().BeTrue());

            eventsAndSates.ForEach(e => e.ForEach(
                eas => eas.Should().HaveCount(2)));

            eventsAndSates.ForEach(e => e.ForEach(
                eas => eas.Any(e => e.Equals(expectedTaskLinkedToProjectEvent)).Should().BeTrue()));

            eventsAndSates.ForEach(e => e.ForEach(
                eas => eas.Any(e => e.Equals(expectedProjectNewTaskLinked)).Should().BeTrue()));
        }
    }
}