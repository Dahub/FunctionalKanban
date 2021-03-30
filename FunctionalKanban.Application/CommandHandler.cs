namespace FunctionalKanban.Application
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Functional;
    using Unit = System.ValueTuple;
    using static FunctionalKanban.Functional.F;
    using FunctionalKanban.Domain.Task;

    public class CommandHandler
    {
        private readonly Func<Guid, Option<State>> _getEntity;

        private readonly Func<Event, Unit> _publishEvent;

        public CommandHandler(
            Func<Guid, Option<State>> GetEntity,
            Func<Event, Unit> PublishEvent)
        {
            _getEntity = GetEntity;
            _publishEvent = PublishEvent;
        }

        public Validation<Unit> Handle(Command command) =>
            (command) switch
            {
                CreateTask createTaskCommand => TaskEntity.Create(createTaskCommand).Bind<(Event evt, TaskState state), Unit>((tuple) => _publishEvent(tuple.evt)),
                ChangeTaskStatus changeTaskStatusCommand => _getEntity(changeTaskStatusCommand.EntityId)
                                                                .Bind(CastToEntityState<TaskState>)
                                                                .Bind((e) => e.ChangeStatus(changeTaskStatusCommand))
                
                _ => Invalid("Commande non prise en charge")
            };


        private Option<T> CastToEntityState<T>(State state) where T : State => state is T ? (T)state : None;
    }
}
