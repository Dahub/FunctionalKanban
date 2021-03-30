namespace FunctionalKanban.Domain.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using Xunit;

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

            var changeTaskStatus = new ChangeTaskStatus()
            {
                TaskId = Guid.NewGuid(),
                TaskStatus = expectedTaskStatus,
                TimeStamp = DateTime.Now
            };

            var eventAndState = BuildNewTask().ChangeStatus(changeTaskStatus);

            eventAndState.Match(
                Invalid: (errors) => false,
                Valid: (tuple) => tuple.state.TaskStatus.Equals(expectedTaskStatus)).Should().BeTrue();
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
                Status = taskStatus,
                RemanigWork = 10
            };
    }
}
