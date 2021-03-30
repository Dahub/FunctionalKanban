namespace FunctionalKanban.Application
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

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
                CreateTask c => TaskEntity.Create(c).BindAndPublishEvent(_publishEvent),
                ChangeTaskStatus c => Handle<TaskState>(c, _getEntity, (e) => e.ChangeStatus(c)),
                _ => Invalid("Commande non prise en charge")
            };

        private Validation<Unit> Handle<T>(
            Command command,
            Func<Guid, Option<State>> getEntity,
            Func<T, Option<Validation<EventAndState>>> f) where T : State =>
                getEntity(command.EntityId)
                    .CastTo<T>()
                    .Bind(f)
                    .Match(
                        None: () => Invalid("Erreur lors de l'exécution de la commande"),
                        Some: (x) => x.BindAndPublishEvent(_publishEvent));
    }

    internal static class CommandHandlerExt
    {
        public static Validation<Unit> BindAndPublishEvent(this Validation<EventAndState> v, Func<Event, Unit> publishEvent) => 
            v.Bind<EventAndState, Unit>((x) => publishEvent(x.@event));

        public static Option<T> CastTo<T>(this Option<State> v) where T : State => v.Bind<State, T>((state) => state is T ? (T)state : None);
    }
}
