namespace FunctionalKanban.Domain.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using LaYumba.Functional;
    using Xunit;
    using static LaYumba.Functional.F;

    public class TaskShould
    {
        [Fact]
        public void ReturnTaskStateWithProjectIdWhenLinkedProject()
        {
            var expectedProjectId = Guid.NewGuid();
            var entityId = Guid.NewGuid();

            var linkToProject = new LinkToProject()
            {
                EntityId = entityId,
                ProjectId = expectedProjectId
            };

            var eventAndState = BuildNewTask(entityId).Bind((x) => ((TaskEntityState)x.State).LinkToProject(linkToProject));

            eventAndState.Match(
            Invalid: (errors) => default,
            Valid: (eas) => ((TaskEntityState)eas.State).ProjectId).Should().Be(Some(expectedProjectId));
        }

        [Fact]
        public void ReturnTaskStateWithProjectIdSetToNoneWhenLinkedProjectWithoutId()
        {
            var entityId = Guid.NewGuid();

            var linkToProject = new LinkToProject()
            {
                EntityId = entityId
            };

            var eventAndState = BuildNewTask(entityId).Bind((x) => ((TaskEntityState)x.State).LinkToProject(linkToProject));

            eventAndState.Match(
            Invalid: (errors) => default,
            Valid: (eas) => ((TaskEntityState)eas.State).ProjectId).Should().Be(new Option<Guid>());
        }

        [Fact]
        public void ReturnTaskStateWithNameAndStatusWhenCreated()
        {
            var expectedTaskName = Guid.NewGuid().ToString();
            var expectedTaskStatus = TaskStatus.Todo;
            var entityId = Guid.NewGuid();

            var eventAndTask = BuildNewTask(entityId, expectedTaskName);

            bool CheckEquality(TaskEntityState s) =>
                s.TaskName.Equals(expectedTaskName)
                && s.TaskStatus.Equals(expectedTaskStatus)
                && s.IsDeleted.Equals(false);                

            eventAndTask.Match(
                Invalid: (errors) => false,
                Valid: (eas) => CheckEquality((TaskEntityState)eas.State)).Should().BeTrue();
        }

        [Fact]
        public void ChangeRemaningWorkWhenRemaningWorkChange()
        {
            var expectedRemaningWork = 5u;
            var entityId = Guid.NewGuid();
            var changeRemaningWork = new ChangeRemaningWork()
            {
                EntityId = entityId,
                RemaningWork = expectedRemaningWork
            };

            var eventAndState = BuildNewTask(entityId).Bind((x) => ((TaskEntityState)x.State).ChangeRemaningWork(changeRemaningWork));

            eventAndState.Match(
            Invalid: (errors) => 0u,
            Valid: (eas) => ((TaskEntityState)eas.State).RemaningWork).Should().Be(expectedRemaningWork);
        }

        [Fact]
        public void ChangeTaskStateStatusWhenStatusChange()
        {
            var expectedTaskStatus = TaskStatus.InProgress;
            var entityId = Guid.NewGuid();
            var changeTaskStatus = new ChangeTaskStatus()
            {
                EntityId = entityId,
                TaskStatus = expectedTaskStatus
            };

            var eventAndState = BuildNewTask(entityId).Bind((x) => ((TaskEntityState)x.State).ChangeStatus(changeTaskStatus));

            eventAndState.Match(
                Invalid:    (errors)    => false,
                Valid:      (eas)       => ((TaskEntityState)eas.State).TaskStatus.Equals(expectedTaskStatus)).Should().BeTrue();
        }

        [Fact]
        public void SetIsDeletedToTrueWhenDeleted()
        {
            var expectedIsDeletedValue = true;
            var entityId = Guid.NewGuid();

            var deleteTask = new DeleteTask()
            {
                EntityId =  entityId
            };

            var eventAndState = BuildNewTask(entityId).Bind((x) => ((TaskEntityState)x.State).Delete(deleteTask));

            eventAndState.Match(
                Invalid:    (errors)    => false,
                Valid:      (eas)       => ((TaskEntityState)eas.State).IsDeleted.Equals(expectedIsDeletedValue)).Should().BeTrue();
        }

        [Fact]
        public void SetRemaningWorkToZeroWhenDeleted()
        {
            var entityId = Guid.NewGuid();
            var remaningWork = 10u;

            var deleteTask = new DeleteTask()
            {
                EntityId = entityId
            };

            var eventAndState = BuildNewTask(entityId, remaningWork: remaningWork).
                Bind((x) => ((TaskEntityState)x.State).Delete(deleteTask));

            eventAndState.Match(
               Invalid: (errors) => remaningWork,
               Valid: (eas) => ((TaskEntityState)eas.State).RemaningWork).Should().Be(0);
        }

        [Fact]
        public void SetProjectIdToNoneWhenDeleted()
        {
            var entityId = Guid.NewGuid();

            var deleteTask = new DeleteTask()
            {
                EntityId = entityId
            };

            var eventAndState = BuildNewTask(entityId).Bind((x) => ((TaskEntityState)x.State).Delete(deleteTask));

            eventAndState.Match(
                Invalid: (errors) => false,
                Valid: (eas) => ((TaskEntityState)eas.State).ProjectId.Equals(None)).Should().BeTrue();
        }

        [Fact]
        public void ReturnInvalidWhenChangeStatusOfDeletedTask()
        {
            var taskState = new TaskEntityState()
            {
                IsDeleted = true,
                RemaningWork = 5,
                TaskName = "test task",
                TaskStatus = TaskStatus.Archived,
                Version = 5
            };

            var changeTaskStatus = new ChangeTaskStatus()
            {
                EntityId = Guid.NewGuid(),
                TaskStatus = TaskStatus.Canceled
            };

            TaskEntity.ChangeStatus(taskState, changeTaskStatus)
                .Match(
                    Invalid:    (_) => true,
                    Valid:      (_) => false).Should().BeTrue();
        }

        [Fact]
        public void BeNoneWhenHydrateWithoutEvents()
        {
            var optionTask = new TaskEntityState().From(Enumerable.Empty<Event>());

            optionTask.Should().Equals(None);
        }

        [Fact]
        public void BeNoneWhenHydratedWithoutCreatedEvent()
        {
            var entityId = Guid.NewGuid();
            var entityName = typeof(TaskEntityState).FullName;
            var changedStatus = TaskStatus.InProgress;
            var lastStatus = TaskStatus.Done;

            var events = new List<Event>()
            {
                new TaskStatusChanged()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 1,
                    NewStatus       = changedStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 2,
                    NewStatus       = lastStatus,
                    TimeStamp       = DateTime.Now
                }
            };

            var optionTask = new TaskEntityState().From(events);

            optionTask.Should().Equals(None);
        }

        [Fact]
        public void BeSomeWhenHydrateWithConsecutivesEvents()
        {
            var entityId = Guid.NewGuid();
            var entityName = typeof(TaskEntityState).FullName;
            var name = Guid.NewGuid().ToString();
            var remaningWork = 10u;
            var initialStatus = TaskStatus.Todo;
            var changedStatus = TaskStatus.InProgress;
            var lastStatus = TaskStatus.Done;

            var events = new List<Event>()
            {
                new TaskCreated()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 1,
                    Name            = name,
                    RemaningWork    = remaningWork,
                    Status          = initialStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 2,
                    NewStatus       = changedStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 3,
                    NewStatus       = lastStatus,
                    TimeStamp       = DateTime.Now
                }
            };

            var optionTask = new TaskEntityState().From(events);

            optionTask.Match(
                None: () => false,
                Some: (_) => true).Should().BeTrue();
        }

        [Fact]
        public void BeSomeWhenHydrateWithNonOrderedConsecutivesEvents()
        {
            var entityId = Guid.NewGuid();
            var entityName = typeof(TaskEntityState).FullName;
            var name = Guid.NewGuid().ToString();
            var remaningWork = 10u;
            var initialStatus = TaskStatus.Todo;
            var changedStatus = TaskStatus.InProgress;
            var lastStatus = TaskStatus.Done;

            var events = new List<Event>()
            {
                new TaskStatusChanged()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 2,
                    NewStatus       = changedStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 3,
                    NewStatus       = lastStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskCreated()
                {
                    EntityId     = entityId,
                    EntityName   = entityName,
                    EntityVersion   = 1,
                    Name            = name,
                    RemaningWork    = remaningWork,
                    Status          = initialStatus,
                    TimeStamp       = DateTime.Now
                }
            };

            var optionTask = new TaskEntityState().From(events);

            optionTask.Match(
                None: () => false,
                Some: (_) => true).Should().BeTrue();
        }

        private static Validation<EventAndState> BuildNewTask(
                Guid entityId, 
                string taskName = "fake task",
                uint remaningWork = 10) =>
            TaskEntity.Create(BuildCreateTaskCommand(entityId, taskName, remaningWork));

        private static CreateTask BuildCreateTaskCommand(Guid entityId, string taskName, uint remaningWork) =>
            new()
            {
                EntityId     = entityId,
                Name            = taskName,
                RemaningWork    = remaningWork,
                ProjectId       = Guid.NewGuid()
            };
    }
}
