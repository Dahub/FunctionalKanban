namespace FunctionalKanban.Domain.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Functional;
    using Xunit;

    public class TaskShould
    {
        [Fact]
        public bool ReturnTaskStateWithNameAndStatusWhenCreated()
        {
            var expectedTaskName = Guid.NewGuid().ToString();
            var expectedTaskStatus = TaskStatus.Todo;

            var taskState = BuildNewTask(expectedTaskName);

            taskState.Match(
                Invalid: (errors) => false,
                Valid: (tuple) => tuple.state.TaskStatus.Equals(expectedTaskStatus)).Should().BeTrue();
        }

        //[Fact]
        //public void ChangeTaskStateStatusWhenStatusChange()
        //{
        //    var expectedTaskStatus = TaskStatus.InProgress;

        //    var changeTaskStatus = new ChangeTaskStatus()
        //    {
        //        TaskId = Guid.NewGuid(),
        //        TaskStatus = expectedTaskStatus,
        //        TimeStamp = DateTime.Now
        //    };

        //    var eventAndState = BuildNewTask().Bind<Event, TaskState>((evt, state) => state.ChangeStatus(changeTaskStatus);

        //    eventAndState.Match(
        //        Invalid: (errors) => false,
        //        Valid: (tuple) => tuple.state.TaskStatus.Equals(expectedTaskStatus)).Should().BeTrue();
        //}

        private static Validation<(Event,TaskState)> BuildNewTask(
                string taskName = "fake task") =>
            TaskEntity.Create(BuildCreateTaskCommand(taskName));

        private static CreateTask BuildCreateTaskCommand(string taskName) =>
            new CreateTask()
            {
                EntityId = Guid.NewGuid(),
                Name = taskName,
                TimeStamp = DateTime.Now,
                RemaningWork = 10
            };
    }
}
