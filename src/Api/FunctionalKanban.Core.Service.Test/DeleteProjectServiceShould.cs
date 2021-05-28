namespace FunctionalKanban.Service.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Domain.Project.Events;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;
    using Xunit;
    using static LaYumba.Functional.F;

    public class DeleteProjectServiceShould
    {
        [Fact]
        public void ReturnExceptionalWhenHandleDeleteProjectCommandWithExceptionialEntity()
        {
            var projectId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            var deleteProject = new DeleteProject() { EntityId = projectId };

            var eventsAndSates = DeleteProjectService.HandleDeleteProjectCommand(
               deleteProject,
               (id) => id.Equals(projectId) ?
                           Exceptional(Valid<State>(new ProjectEntityState()
                           {
                               ProjectId = projectId,
                               AssociatedTaskIds = new List<Guid>() { taskId }
                           })) :
                           new Exception("ex"));

            eventsAndSates.Exception.Should().BeTrue();
        }

        [Fact]
        public void GenerateRightEventsWhenHandleLinkToProjectCommandWithDeleteChildrenFalse()
        {
            var projectId = Guid.NewGuid();
            var firstTaskId = Guid.NewGuid();
            var secondTaskId = Guid.NewGuid();

            var deleteProject = new DeleteProject() { EntityId = projectId, DeleteChildrenTasks = false };

            var expectedEvents = new List<Event>()
                {
                    new ProjectDeleted()
                    {
                        DeleteChildrenTasks = false,
                        EntityId = projectId,
                        EntityName = typeof(ProjectEntityState).FullName,
                        EntityVersion = 1,
                        IsDeleted = true,
                        TimeStamp = deleteProject.TimeStamp
                    },
                    new TaskRemovedFromProject()
                    {
                        EntityId = firstTaskId,
                        EntityName = typeof(TaskEntityState).FullName,
                        EntityVersion = 1,
                        RemaningWork = 0,
                        OldProjectId = projectId,
                        ProjectId = None,
                        TimeStamp = deleteProject.TimeStamp
                    },
                    new TaskRemovedFromProject()
                    {
                        EntityId = secondTaskId,
                        EntityName = typeof(TaskEntityState).FullName,
                        EntityVersion = 1,
                        RemaningWork = 0,
                        OldProjectId = projectId,
                        ProjectId = None,
                        TimeStamp = deleteProject.TimeStamp
                    }
                };

            var eventsAndSates = DeleteProjectService.HandleDeleteProjectCommand(
                deleteProject,
                (id) =>
                {
                    if (id == projectId)
                    {
                        return Exceptional(Valid<State>(new ProjectEntityState()
                        {
                            ProjectId = projectId,
                            AssociatedTaskIds = new List<Guid>() { firstTaskId, secondTaskId }
                        }));
                    }
                    else if (id == firstTaskId)
                    {
                        return Exceptional(Valid<State>(new TaskEntityState()
                        {
                            ProjectId = projectId,
                            TaskId = firstTaskId
                        }));
                    }
                    else
                    {
                        return Exceptional(Valid<State>(new TaskEntityState()
                        {
                            ProjectId = projectId,
                            TaskId = secondTaskId
                        }));
                    }
                });

            eventsAndSates.Exception.Should().BeFalse();

            eventsAndSates.ForEach(e => e.IsValid.Should().BeTrue());

            eventsAndSates.ForEach(e => e.ForEach(
                eas => eas.Should().HaveCount(3)));

            eventsAndSates.ForEach(e => e.ForEach(
                eas => AreEquals(eas, expectedEvents).Should().BeTrue()));
        }

        [Fact]
        public void GenerateRightEventsWhenHandleLinkToProjectCommandWithDeleteChildrenTrue()
        {
            var projectId = Guid.NewGuid();
            var firstTaskId = Guid.NewGuid();
            var secondTaskId = Guid.NewGuid();

            var deleteProject = new DeleteProject() { EntityId = projectId, DeleteChildrenTasks = true };

            var expectedEvents = new List<Event>()
                {
                    new ProjectDeleted()
                    {
                        DeleteChildrenTasks = true,
                        EntityId = projectId,
                        EntityName = typeof(ProjectEntityState).FullName,
                        EntityVersion = 1,
                        IsDeleted = true,
                        TimeStamp = deleteProject.TimeStamp
                    },
                    new TaskDeleted()
                    {
                        EntityId = firstTaskId,
                        EntityName = typeof(TaskEntityState).FullName,
                        EntityVersion = 1,
                        IsDeleted = true,
                        OldRemaningWork = 0,
                        RemaningWork = 0,
                        ProjectId = None,
                        TimeStamp = deleteProject.TimeStamp
                    },
                    new TaskDeleted()
                    {
                        EntityId = secondTaskId,
                        EntityName = typeof(TaskEntityState).FullName,
                        EntityVersion = 1,
                        IsDeleted = true,
                        OldRemaningWork = 0,
                        RemaningWork = 0,
                        ProjectId = None,
                        TimeStamp = deleteProject.TimeStamp
                    }
                };

            var eventsAndSates = DeleteProjectService.HandleDeleteProjectCommand(
                deleteProject,
                (id) =>
                {
                    if (id == projectId)
                    {
                        return Exceptional(Valid<State>(new ProjectEntityState()
                        {
                            ProjectId = projectId,
                            AssociatedTaskIds = new List<Guid>() { firstTaskId, secondTaskId }
                        }));
                    }
                    else if(id == firstTaskId)
                    {
                        return Exceptional(Valid<State>(new TaskEntityState()
                        {
                            ProjectId = projectId,
                            TaskId = firstTaskId
                        }));
                    }
                    else
                    {
                        return Exceptional(Valid<State>(new TaskEntityState()
                        {
                            ProjectId = projectId,
                            TaskId = secondTaskId
                        }));
                    }
                });

            eventsAndSates.Exception.Should().BeFalse();

            eventsAndSates.ForEach(e => e.IsValid.Should().BeTrue());

            eventsAndSates.ForEach(e => e.ForEach(
                eas => eas.Should().HaveCount(3)));

            eventsAndSates.ForEach(e => e.ForEach(
                eas => AreEquals(eas, expectedEvents)
                .Should().BeTrue()));
        }

        private static bool AreEquals(IEnumerable<Event> firsts, IEnumerable<Event> seconds) => 
            firsts.Count() == seconds.Count()
            && !firsts.Any(f => !seconds.Where(s => AreEquals(f, s)).Any());

        private static bool AreEquals(Event first, Event second) => 
            first.Equals(second);
    }
}
