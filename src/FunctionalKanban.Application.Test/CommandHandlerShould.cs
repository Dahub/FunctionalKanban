namespace FunctionalKanban.Application.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Application.Commands;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;
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
            var expectedAggregateName = typeof(TaskEntityState).FullName;

            TaskCreated lastPublishedEvent = null;

            var command = new CreateTask()
            {
                AggregateId =   expectedAggregateId,
                Name =          expectedEntityName,
                RemaningWork =  (uint)expectedRemaningWork
            };

            var commandHandler = new CommandHandler(
                getEntity:      (id)    => Some((State)new TaskEntityState()),
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
                getEntity:      (id) => Some((State)new TaskEntityState()
                                {
                                    Version =       1,
                                    TaskStatus =    TaskStatus.Todo,
                                    RemaningWork =  10,
                                    TaskName =      Guid.NewGuid().ToString()
                                }),
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
                AggregateId = Guid.NewGuid(),
                TaskStatus = TaskStatus.InProgress
            };

            var commandHandler = new CommandHandler(
               getEntity:       (id)    => (Option<State>)None,
               publishEvent:    (evt)   => Unit.Create());

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ReturnExceptionalWhenHandleChangeTaskStatusCommandWithException()
        {
            var command = new ChangeTaskStatus()
            {
                AggregateId =   Guid.NewGuid(),
                TaskStatus =    TaskStatus.InProgress
            };

            var commandHandler = new CommandHandler(
               getEntity:       (id) =>  new Exception(),
               publishEvent:    (evt) => Unit.Create());

            var validationResult = commandHandler.Handle(command);

            validationResult
                .Match(
                    Valid:      (v) => v.Match(
                        Exception:  (_) => true,
                        Success:    (_) => false),
                    Invalid:    (_) => false)
                .Should().BeTrue();
        }
    }
}
