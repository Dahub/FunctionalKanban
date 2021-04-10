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
    using FunctionalKanban.Functional;
    using Xunit;
    using static FunctionalKanban.Functional.F;

    public class TaskShould
    {
        [Fact]
        public void ReturnTaskStateWithNameAndStatusWhenCreated()
        {
            var expectedTaskName = Guid.NewGuid().ToString();
            var expectedTaskStatus = TaskStatus.Todo;

            var eventAndTask = BuildNewTask(expectedTaskName);

            eventAndTask.Match(
                Invalid: (errors) => false,
                Valid: (eas) => ((TaskEntityState)eas.State).TaskStatus.Equals(expectedTaskStatus)).Should().BeTrue();
        }

        [Fact]
        public void ChangeTaskStateStatusWhenStatusChange()
        {
            var expectedTaskStatus = TaskStatus.InProgress;

            var changeTaskStatus = new ChangeTaskStatus()
            {
                AggregateId = Guid.NewGuid(),
                TaskStatus = expectedTaskStatus
            };

            var eventAndState = BuildNewTask().Bind((x) => ((TaskEntityState)x.State).ChangeStatus(changeTaskStatus));

            eventAndState.Match(
                Invalid: (errors) => false,
                Valid: (eas) => ((TaskEntityState)eas.State).TaskStatus.Equals(expectedTaskStatus)).Should().BeTrue();
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
            var aggregateId = Guid.NewGuid();
            var aggregateName = typeof(TaskEntityState).FullName;
            var changedStatus = TaskStatus.InProgress;
            var lastStatus = TaskStatus.Done;

            var events = new List<Event>()
            {
                new TaskStatusChanged()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
                    EntityVersion   = 1,
                    NewStatus       = changedStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
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
            var aggregateId = Guid.NewGuid();
            var aggregateName = typeof(TaskEntityState).FullName;
            var entityName = Guid.NewGuid().ToString();
            var remaningWork = 10u;
            var initialStatus = TaskStatus.Todo;
            var changedStatus = TaskStatus.InProgress;
            var lastStatus = TaskStatus.Done;

            var events = new List<Event>()
            {
                new TaskCreated()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
                    EntityVersion   = 1,
                    Name            = entityName,
                    RemaningWork    = remaningWork,
                    Status          = initialStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
                    EntityVersion   = 2,
                    NewStatus       = changedStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
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
            var aggregateId = Guid.NewGuid();
            var aggregateName = typeof(TaskEntityState).FullName;
            var entityName = Guid.NewGuid().ToString();
            var remaningWork = 10u;
            var initialStatus = TaskStatus.Todo;
            var changedStatus = TaskStatus.InProgress;
            var lastStatus = TaskStatus.Done;

            var events = new List<Event>()
            {
                new TaskStatusChanged()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
                    EntityVersion   = 2,
                    NewStatus       = changedStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
                    EntityVersion   = 3,
                    NewStatus       = lastStatus,
                    TimeStamp       = DateTime.Now
                },
                new TaskCreated()
                {
                    AggregateId     = aggregateId,
                    AggregateName   = aggregateName,
                    EntityVersion   = 1,
                    Name            = entityName,
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

        private static Validation<EventAndState> BuildNewTask(string taskName = "fake task") =>
            TaskEntity.Create(BuildCreateTaskCommand(taskName));

        private static CreateTask BuildCreateTaskCommand(string taskName) =>
            new CreateTask()
            {
                AggregateId     = Guid.NewGuid(),
                Name            = taskName,
                RemaningWork    = 10
            };
    }
}
