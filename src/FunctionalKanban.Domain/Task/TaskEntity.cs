﻿namespace FunctionalKanban.Domain.Task
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Domain.Task.Events;
    using FunctionalKanban.Functional;

    public static class TaskEntity
    {
        private static readonly string _aggregateName = typeof(TaskEntityState).FullName;

        public static Validation<EventAndState> Create(CreateTask cmd)
        {
            var @event = new TaskCreated()
            {
                AggregateId     = cmd.AggregateId,
                AggregateName   = _aggregateName,
                Name            = cmd.Name,
                RemaningWork    = cmd.RemaningWork,
                TimeStamp       = cmd.TimeStamp,
                Status          = TaskStatus.Todo,
                EntityVersion   = 1
            };

            return new TaskEntityState().ApplyEvent(@event);
        }

        public static Validation<EventAndState> ChangeStatus(
                this TaskEntityState state,
                ChangeTaskStatus cmd)
        {
            var @event = new TaskStatusChanged()
            {
                AggregateId     = cmd.AggregateId,
                AggregateName   = _aggregateName,
                EntityVersion   = state.Version + 1,
                NewStatus       = cmd.TaskStatus,
                TimeStamp       = cmd.TimeStamp,
                RemaningWork    = cmd.TaskStatus.Equals(
                                    TaskStatus.Done | 
                                    TaskStatus.Canceled |
                                    TaskStatus.Archived) ? 0 : state.RemaningWork
            };

            return state.ApplyEvent(@event);
        }
    }
}
