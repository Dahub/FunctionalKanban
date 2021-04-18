namespace FunctionalKanban.Domain.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Events;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;
    using Xunit;
    using static LaYumba.Functional.F;

    public class ProjectViewProjectionShould
    {
        [Fact]
        public void HandleAndPopulateWhenProjectCreatedEventIsFired()
        {
            var projectEntityId = Guid.NewGuid();
            var entityName = typeof(ProjectEntityState).FullName;
            var projectName = Guid.NewGuid().ToString();
            var projectStatus = ProjectStatus.New;
            var timeStamp = DateTime.Now;
            var isDeleted = false;
            var expectedRemaningWork = 0u;

            var projectCreated = new ProjectCreated()
            {
                EntityId = projectEntityId,
                EntityName = entityName,
                EntityVersion = 1,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TimeStamp = timeStamp
            };

            var expectedProjectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = expectedRemaningWork
            };

            ProjectViewProjection.HandleWithId(projectCreated).Should().Be(Some(projectEntityId));

            var projectProjection = new ProjectViewProjection();

            var resultProjectProjection = projectProjection.With(projectCreated);

            resultProjectProjection.Match(
                None : ()   => new ProjectViewProjection(),
                Some : (p)  => p) .Should().Be(expectedProjectProjection);
        }

        [Fact]
        public void NotHandleTaskCreatedWhenProjectIdIsNone()
        {
            var taskCreated = new TaskCreated()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                IsDeleted = false,
                Name = Guid.NewGuid().ToString(),
                ProjectId = None,
                RemaningWork = 10u,
                Status = TaskStatus.Todo,
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskCreated).Should().Be(new Option<Guid>());
        }

        [Fact]
        public void HandleTaskCreatedWhenProjectIdIsSome()
        {
            var projectEntityId = Guid.NewGuid();
            var entityName = typeof(ProjectEntityState).FullName;
            var projectName = Guid.NewGuid().ToString();
            var projectStatus = ProjectStatus.New;
            var timeStamp = DateTime.Now;
            var isDeleted = false;
            var initialRemaningWork = 0u;
            var expectedRemaningWork = 10u;

            var projectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = initialRemaningWork
            };

            var expectedProjectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = expectedRemaningWork
            };

            var taskCreated = new TaskCreated()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                IsDeleted = false,
                Name = Guid.NewGuid().ToString(),
                ProjectId = Some(projectEntityId),
                RemaningWork = 10u,
                Status = TaskStatus.Todo,
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskCreated).Should().Be(Some(projectEntityId));

            var resultProjectProjection = projectProjection.With(taskCreated);

            resultProjectProjection.Match(
                None: () => new ProjectViewProjection(),
                Some: (p) => p).Should().Be(expectedProjectProjection);
        }

        [Fact]
        public void NotHandleTaskDeletedWhenProjectIdIsNone()
        {
            var taskDeleted = new TaskDeleted()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                IsDeleted = false,
                ProjectId = None,
                RemaningWork = 3u,
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskDeleted).Should().Be(new Option<Guid>());
        }

        [Fact]
        public void HandleTaskDeletedWhenProjectIdIsSome()
        {
            var projectEntityId = Guid.NewGuid();
            var entityName = typeof(ProjectEntityState).FullName;
            var projectName = Guid.NewGuid().ToString();
            var projectStatus = ProjectStatus.New;
            var timeStamp = DateTime.Now;
            var isDeleted = false;
            var initialRemaningWork = 10u;
            var expectedRemaningWork = 7u;

            var projectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = initialRemaningWork
            };

            var expectedProjectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = expectedRemaningWork
            };

            var taskDeleted = new TaskDeleted()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                IsDeleted = false,
                ProjectId = Some(projectEntityId),
                OldRemaningWork = 3u,
                RemaningWork = 0u,
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskDeleted).Should().Be(Some(projectEntityId));

            var resultProjectProjection = projectProjection.With(taskDeleted);

            resultProjectProjection.Match(
                None: () => new ProjectViewProjection(),
                Some: (p) => p).Should().Be(expectedProjectProjection);
        }

        [Fact]
        public void NotHandleTaskRemaningWorkChangedWhenProjectIdIsNone()
        {
            var taskRemaningWorkChanged = new TaskRemaningWorkChanged()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                ProjectId = None,
                RemaningWork = 3u,
                OldRemaningWork = 10u,
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskRemaningWorkChanged).Should().Be(new Option<Guid>());
        }

        [Fact]
        public void HandleTaskRemaningWorkChangedWhenProjectIdIsSome()
        {
            var projectEntityId = Guid.NewGuid();
            var entityName = typeof(ProjectEntityState).FullName;
            var projectName = Guid.NewGuid().ToString();
            var projectStatus = ProjectStatus.New;
            var timeStamp = DateTime.Now;
            var isDeleted = false;
            var initialRemaningWork = 10u;
            var expectedRemaningWork = 2u;

            var projectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = initialRemaningWork
            };

            var expectedProjectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = expectedRemaningWork
            };

            var taskRemaningWorkChanged = new TaskRemaningWorkChanged()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                ProjectId = Some(projectEntityId),
                OldRemaningWork = 25,
                RemaningWork = 17u,
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskRemaningWorkChanged).Should().Be(Some(projectEntityId));

            var resultProjectProjection = projectProjection.With(taskRemaningWorkChanged);

            resultProjectProjection.Match(
                None: () => new ProjectViewProjection(),
                Some: (p) => p).Should().Be(expectedProjectProjection);
        }

        [Fact]
        public void NotHandleTaskLinkedToProjectWhenProjectIdIsNone()
        {
            var taskLinkedToProject = new TaskLinkedToProject()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                ProjectId = None,
                RemaningWork = 3u,                
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskLinkedToProject).Should().Be(new Option<Guid>());
        }

        [Fact]
        public void HandleTaskLinkedToProjectWhenProjectIdIsSome()
        {
            var projectEntityId = Guid.NewGuid();
            var entityName = typeof(ProjectEntityState).FullName;
            var projectName = Guid.NewGuid().ToString();
            var projectStatus = ProjectStatus.New;
            var timeStamp = DateTime.Now;
            var isDeleted = false;
            var initialRemaningWork = 10u;
            var expectedRemaningWork = 17u;

            var projectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = initialRemaningWork
            };

            var expectedProjectProjection = new ProjectViewProjection()
            {
                Id = projectEntityId,
                IsDeleted = isDeleted,
                Name = projectName,
                Status = projectStatus,
                TotalRemaningWork = expectedRemaningWork
            };

            var taskLinkedToProject = new TaskLinkedToProject()
            {
                EntityId = Guid.NewGuid(),
                EntityName = typeof(TaskEntityState).FullName,
                EntityVersion = 1,
                ProjectId = Some(projectEntityId),
                RemaningWork = 7u,
                TimeStamp = DateTime.Now
            };

            ProjectViewProjection.HandleWithId(taskLinkedToProject).Should().Be(Some(projectEntityId));

            var resultProjectProjection = projectProjection.With(taskLinkedToProject);

            resultProjectProjection.Match(
                None: () => new ProjectViewProjection(),
                Some: (p) => p).Should().Be(expectedProjectProjection);
        }
    }
}
