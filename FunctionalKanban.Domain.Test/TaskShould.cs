namespace FunctionalKanban.Domain.Test
{
    using System;
    using Xunit;
    using FluentAssertions;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Domain.Task;

    public class TaskShould
    {
        [Fact]
        public void ReturnTaskStateWithNameAndStatusWhenCreated()
        {
            var expectedTaskName = Guid.NewGuid().ToString();
            var expectedTaskStatus = TaskStatus.Todo;

            var taskState = BuildNewTask(expectedTaskName, expectedTaskStatus);

            taskState.TaskName.Should().Equals(expectedTaskName);
            taskState.TaskStatus.Should().Equals(expectedTaskStatus);
        }

        [Fact]
        public void ChangeTaskStateStatusWhenStatusChange()
        {
            var expectedTaskStatus = TaskStatus.InProgress;

            var taskStatusChangedEvent = new TaskStatusChanged()
            {
                EntityId = Guid.NewGuid(),
                EntityVersion = 2,
                TimeStamp = DateTime.Now,
                NewStatus = expectedTaskStatus
            };

            var taskState = BuildNewTask().Apply(taskStatusChangedEvent);

            taskState.TaskStatus.Should().Equals(expectedTaskStatus);
        }

        private static TaskState BuildNewTask(
                string taskName = "fake task", 
                TaskStatus taskStatus = TaskStatus.Todo) => 
            TaskEntity.Create(BuildTaskCreatedEvent(taskName, taskStatus));

        private static TaskCreated BuildTaskCreatedEvent(string taskName, TaskStatus taskStatus) =>
            new TaskCreated()
            {
                EntityId = Guid.NewGuid(),
                EntityVersion = 1,
                Name = taskName,
                TimeStamp = DateTime.Now,
                Status = taskStatus
            };
    }
}
