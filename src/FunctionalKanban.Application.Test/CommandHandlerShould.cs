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
            var expectedAggregateId = Guid.NewGuid();
            var expectedEntityName = Guid.NewGuid().ToString();
            var expectedRemaningWork = 10;
            var expectedTimeStamp = DateTime.Now;
            var expectedAggregateName = typeof(TaskEntity).FullName;

            TaskCreated lastPublishedEvent = null;

            var command = new CreateTask()
            {
                AggregateId =   expectedAggregateId,
                Name =          expectedEntityName,
                RemaningWork =  (uint)expectedRemaningWork
            };

            var commandHandler = new CommandHandler(
                getEntity:      (id)    => new TaskState(),
                publishEvent:   (evt)   => { lastPublishedEvent = evt as TaskCreated; return Unit.Create(); });

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeTrue();
            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.AggregateId.Should().Equals(expectedAggregateId);
            lastPublishedEvent.EntityVersion.Should().Equals(1);
            lastPublishedEvent.Name.Should().Equals(expectedEntityName);
            lastPublishedEvent.RemaningWork.Should().Equals(expectedRemaningWork);
            lastPublishedEvent.TimeStamp.Should().Equals(expectedTimeStamp);
            lastPublishedEvent.AggregateName.Should().Be(expectedAggregateName);
        }

        [Fact]
        public void PublishTaskStatusChangedWhenHandleChangeTaskStatusCommandOnExistingEntity()
        {            
            var aggregateId = Guid.NewGuid();
            var expectedTaskStatus = TaskStatus.InProgress;
            var expectedEntityVersion = 2;

            TaskStatusChanged lastPublishedEvent = null;

            var command = new ChangeTaskStatus()
            {
                AggregateId =   aggregateId,
                TaskStatus =    expectedTaskStatus
            };

            var commandHandler = new CommandHandler(
                getEntity:      (id) => new TaskState()
                                {
                                    Version =       1,
                                    TaskStatus =    TaskStatus.Todo,
                                    RemaningWork =  10,
                                    TaskName =      Guid.NewGuid().ToString()
                                },
                publishEvent:   (evt) => { lastPublishedEvent = evt as TaskStatusChanged; return Unit.Create(); });

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeTrue();
            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.EntityVersion.Should().Equals(expectedEntityVersion);
            lastPublishedEvent.NewStatus.Should().Equals(expectedTaskStatus);
        }

        [Fact]
        public void ReturnValidationErrorWhenHandleChangeTaskStatusCommandOnMissingEntity()
        {
            var command = new ChangeTaskStatus()
            {
                AggregateId =   Guid.NewGuid(),
                TaskStatus =    TaskStatus.InProgress
            };

            var commandHandler = new CommandHandler(
               getEntity:       (id) =>  None,
               publishEvent:    (evt) => Unit.Create());

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeFalse();
        }
    }
}
