namespace FunctionalKanban.Application.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using Xunit;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    public class CommandHandlerShould
    {
        [Fact]
        public void PublishTaskCreatedWhenHandleCreateTaskCommand()
        {
            var expectedEntityId = Guid.NewGuid();
            var expectedEntityName = Guid.NewGuid().ToString();
            var expectedRemaningWork = 10;
            var expectedTimeStamp = DateTime.Now;

            TaskCreated lastPublishedEvent = null;

            var command = new CreateTask()
            {
                EntityId = expectedEntityId,
                Name = expectedEntityName,
                RemaningWork = (uint)expectedRemaningWork,
                TimeStamp = expectedTimeStamp
            };

            var commandHandler = new CommandHandler(
                (id) => new TaskState(),
                (evt) => { lastPublishedEvent = evt as TaskCreated; return Unit.Create(); });

            commandHandler.Handle(command);

            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.EntityId.Should().Equals(expectedEntityId);
            lastPublishedEvent.EntityVersion.Should().Equals(1);
            lastPublishedEvent.Name.Should().Equals(expectedEntityName);
            lastPublishedEvent.RemaningWork.Should().Equals(expectedRemaningWork);
            lastPublishedEvent.TimeStamp.Should().Equals(expectedTimeStamp);
        }

        [Fact]
        public void PublishTaskStatusChangedWhenHandleChangeTaskStatusCommandOnExistingEntity()
        {            
            var entityId = Guid.NewGuid();
            var expectedTimeStamp = DateTime.Now;
            var expectedTaskStatus = TaskStatus.InProgress;
            var expectedEntityVersion = 2;

            TaskStatusChanged lastPublishedEvent = null;

            var command = new ChangeTaskStatus()
            {
                EntityId = entityId,
                TimeStamp = expectedTimeStamp,
                TaskStatus = expectedTaskStatus
            };

            var commandHandler = new CommandHandler(
                (id) => new TaskState()
                {
                    Version = 1,
                    TaskStatus = TaskStatus.Todo,
                    RemaningWork = 10,
                    TaskName = Guid.NewGuid().ToString()
                },
                (evt) => { lastPublishedEvent = evt as TaskStatusChanged; return Unit.Create(); });

            commandHandler.Handle(command);

            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.TimeStamp.Should().Equals(expectedTimeStamp);
            lastPublishedEvent.EntityVersion.Should().Equals(expectedEntityVersion);
            lastPublishedEvent.NewStatus.Should().Equals(expectedTaskStatus);
        }

        [Fact]
        public void ReturnValidationErrorWhenHandleChangeTaskStatusCommandOnMissingEntity()
        {
            var command = new ChangeTaskStatus()
            {
                EntityId = Guid.NewGuid(),
                TimeStamp = DateTime.Now,
                TaskStatus = TaskStatus.InProgress
            };

            var commandHandler = new CommandHandler(
               (id) => None,
               (evt) => Unit.Create());

            var validationResult = commandHandler.Handle(command);

            validationResult.Match(
                Invalid: (errors) => true,
                Valid: (x) => false).Should().BeTrue();
        }
    }
}
