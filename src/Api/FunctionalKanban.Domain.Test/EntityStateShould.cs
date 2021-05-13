namespace FunctionalKanban.Domain.Test
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;
    using Xunit;
    using static LaYumba.Functional.F;

    public class EntityStateShould
    {
        [Fact]
        public void BeMutatedWhenHydrated()
        {
            var taskId = Guid.NewGuid();
            var taskName = Guid.NewGuid().ToString();
            var initialRemaningWork = 15u;
            var expectedStatus = TaskStatus.InProgress;
            var expectedRemaningWork = 10u;
            var expectedProjectId = Guid.NewGuid();

            var events = new List<Event>()
            {
                new TaskCreated()
                {
                    EntityId = taskId,
                    EntityName = typeof(TaskEntityState).FullName,
                    EntityVersion = 1,
                    IsDeleted = false,
                    Name = taskName,
                    ProjectId = None,
                    RemaningWork = initialRemaningWork,
                    Status = TaskStatus.Todo,
                    TimeStamp = DateTime.Now
                },
                new TaskStatusChanged()
                {
                    EntityId = taskId,
                    EntityName = typeof(TaskEntityState).FullName,
                    EntityVersion = 2,
                    NewStatus = expectedStatus,
                    RemaningWork = initialRemaningWork,
                    TimeStamp = DateTime.Now
                },
                new TaskLinkedToProject()
                {
                    EntityId = taskId,
                    EntityName = typeof(TaskEntityState).FullName,
                    EntityVersion = 3,
                    ProjectId = expectedProjectId,
                    RemaningWork = initialRemaningWork,
                    TimeStamp = DateTime.Now
                },
                new TaskRemaningWorkChanged()
                {
                    EntityId = taskId,
                    EntityName = typeof(TaskEntityState).FullName,
                    EntityVersion = 4,
                    OldRemaningWork = initialRemaningWork,
                    RemaningWork = expectedRemaningWork,
                    ProjectId = expectedProjectId,
                    TimeStamp = DateTime.Now
                }
            };

            var taskEntityState = new TaskEntityState().From(events);

            _ = taskEntityState.Match(
                None: () => false,
                Some: (t) => CheckTask(t as TaskEntityState)).Should().BeTrue();


            bool CheckTask(TaskEntityState t) =>
                t != null
                && t.TaskName == taskName
                && t.TaskId == taskId
                && t.ProjectId == expectedProjectId
                && t.IsDeleted == false
                && t.RemaningWork == expectedRemaningWork
                && t.TaskStatus == expectedStatus
                && t.Version == events.Count();
        }
    }
}
