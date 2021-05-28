namespace FunctionalKanban.Core.Application.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Core.Application.Commands;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.Project;
    using FunctionalKanban.Core.Domain.Project.Commands;
    using FunctionalKanban.Core.Domain.Project.Events;
    using FunctionalKanban.Core.Domain.Task;
    using FunctionalKanban.Core.Domain.Task.Commands;
    using FunctionalKanban.Core.Domain.Task.Events;
    using LaYumba.Functional;
    using Xunit;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public class CommandHandlerShould
    {
        [Fact]
        public void ReturnValidationErrorWhenLinkToProjectWithoutProjectid()
        {
            var command = new LinkToProject()
            {
                EntityId = Guid.NewGuid()
            };

            var commandHandler = new CommandHandler(
              getEntity: (id) => Some((State)new TaskEntityState()),
              publishEvent: (evt) => Unit.Create());

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void ReturnValidationErrorWhenCreateProjectWithoutName()
        {
            var command = new CreateProject()
            {
                EntityId = Guid.NewGuid(),
                Name = string.Empty
            };

            var commandHandler = new CommandHandler(
              getEntity: (id) => Some((State)new ProjectEntityState()),
              publishEvent: (evt) => Unit.Create());

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void PublishProjectCreatedWhenHandleCreateProjectCommand()
        {
            var expectedEntityId = Guid.NewGuid();
            var expectedEntityName = typeof(ProjectEntityState).FullName;
            var expectedTimeStamp = DateTime.Now;
            var expectedName = Guid.NewGuid().ToString();            

            ProjectCreated lastPublishedEvent = null;

            var command = new CreateProject()
            {
                EntityId = expectedEntityId,
                Name = expectedName
            };

            var commandHandler = new CommandHandler(
               getEntity: (id) => Some((State)new ProjectEntityState()),
               publishEvent: (evt) => { lastPublishedEvent = evt as ProjectCreated; return Unit.Create(); });

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeTrue();
            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.EntityId.Should().Equals(expectedEntityId);
            lastPublishedEvent.EntityVersion.Should().Equals(1);
            lastPublishedEvent.Name.Should().Equals(expectedName);
            lastPublishedEvent.TimeStamp.Should().Equals(expectedTimeStamp);
            lastPublishedEvent.EntityName.Should().Be(expectedEntityName);
        }

        [Fact]
        public void PublishTaskDeletedWhenHandleDeleteTaskCommand()
        {
            var expectedEntityId = Guid.NewGuid();

            TaskDeleted lastPublishedEvent = null;

            var command = new DeleteTask()
            {
                EntityId = expectedEntityId
            };

            var commandHandler = new CommandHandler(
              getEntity: (id) => Some((State)new TaskEntityState()),
              publishEvent: (evt) => { lastPublishedEvent = evt as TaskDeleted; return Unit.Create(); });

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeTrue();
            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.EntityId.Should().Equals(expectedEntityId);
        }

        [Fact]
        public void PublishTaskLinkedToProjectWhenHandleLinkToProjectCommand()
        {
            var expectedEntityId = Guid.NewGuid();
            var expectedProjectId = Guid.NewGuid();

            ProjectNewTaskLinked lastPublishedEvent = null;

            var command = new LinkToProject()
            {
                EntityId = expectedEntityId,
                ProjectId = expectedProjectId
            };

            var commandHandler = new CommandHandler(
                getEntity: (id) => Some((State)(id == expectedEntityId 
                    ? new TaskEntityState() { TaskId = expectedEntityId }
                    : new ProjectEntityState() { ProjectId = expectedProjectId } )),
                publishEvent: (evt) => { lastPublishedEvent = evt as ProjectNewTaskLinked; return Unit.Create(); });

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeTrue();
            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.EntityId.Should().Be(expectedProjectId);
            lastPublishedEvent.TaskId.Should().Be(expectedEntityId);
        }

        [Fact]
        public void PublishTaskRemaningWorkChangedWhenHandleRemaningWorkChangeCommand()
        {
            var expectedEntityId = Guid.NewGuid();
            var expectedRemaningWork = 5u;

            TaskRemaningWorkChanged lastPublishedEvent = null;

            var command = new ChangeRemaningWork()
            {
                EntityId = expectedEntityId,
                RemaningWork = expectedRemaningWork
            };

            var commandHandler = new CommandHandler(
             getEntity: (id) => Some((State)new TaskEntityState()),
             publishEvent: (evt) => { lastPublishedEvent = evt as TaskRemaningWorkChanged; return Unit.Create(); });

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeTrue();
            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.EntityId.Should().Equals(expectedEntityId);
            lastPublishedEvent.RemaningWork.Should().Equals(expectedRemaningWork);
        }

        [Fact]
        public void PublishTaskCreatedWhenHandleCreateTaskCommand()
        {
            var expectedEntityId = Guid.NewGuid();
            var expectedEntityName = typeof(TaskEntityState).FullName;
            var expectedRemaningWork = 10;
            var expectedTimeStamp = DateTime.Now;
            var expectedName = Guid.NewGuid().ToString();

            TaskCreated lastPublishedEvent = null;

            var command = new CreateTask()
            {
                EntityId =   expectedEntityId,
                Name = expectedName,
                RemaningWork =  (uint)expectedRemaningWork
            };

            var commandHandler = new CommandHandler(
                getEntity:      (id)    => Some((State)new TaskEntityState()),
                publishEvent:   (evt)   => { lastPublishedEvent = evt as TaskCreated; return Unit.Create(); });

            var validationResult = commandHandler.Handle(command);

            validationResult.IsValid.Should().BeTrue();
            lastPublishedEvent.Should().NotBeNull();
            lastPublishedEvent.EntityId.Should().Equals(expectedEntityId);
            lastPublishedEvent.EntityVersion.Should().Equals(1);
            lastPublishedEvent.Name.Should().Equals(expectedName);
            lastPublishedEvent.RemaningWork.Should().Equals(expectedRemaningWork);
            lastPublishedEvent.TimeStamp.Should().Equals(expectedTimeStamp);
            lastPublishedEvent.EntityName.Should().Be(expectedEntityName);
        }

        [Fact]
        public void PublishTaskStatusChangedWhenHandleChangeTaskStatusCommandOnExistingEntity()
        {            
            var entityId = Guid.NewGuid();
            var expectedTaskStatus = TaskStatus.InProgress;
            var expectedEntityVersion = 2;

            TaskStatusChanged lastPublishedEvent = null;

            var command = new ChangeTaskStatus()
            {
                EntityId =   entityId,
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
                EntityId = Guid.NewGuid(),
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
                EntityId =   Guid.NewGuid(),
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
