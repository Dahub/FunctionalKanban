namespace FunctionalKanban.Application.Commands
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Project;
    using FunctionalKanban.Domain.Project.Commands;
    using FunctionalKanban.Domain.Task;
    using FunctionalKanban.Domain.Task.Commands;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;
    using FunctionalKanban.Service;

    public class CommandHandler
    {
        private readonly Func<Guid, Exceptional<Option<State>>> _getEntity;

        private readonly Func<Event, Exceptional<Unit>> _publishEvent;

        public CommandHandler(
            Func<Guid, Exceptional<Option<State>>> getEntity,
            Func<Event, Exceptional<Unit>> publishEvent)
        {
            _getEntity      = getEntity;
            _publishEvent   = publishEvent;
        }

        public Validation<Exceptional<Unit>> Handle(Command command) =>
            command.Validate().Bind(command => 
            (command) switch
            {
                CreateTask c            => TaskEntity.Create(c).PublishEvent(_publishEvent),
                ChangeTaskStatus c      => Handle<TaskEntityState>(c, _getEntity, (e) => e.ChangeStatus(c)),
                DeleteTask c            => Handle<TaskEntityState>(c, _getEntity, (e) => e.Delete(c)),
                ChangeRemaningWork c    => Handle<TaskEntityState>(c, _getEntity, (e) => e.ChangeRemaningWork(c)),
                LinkToProject c         => Handle(TaskAndProjectLinkService.HandleLinkToProjectCommand(c, LoadEntity)),
                CreateProject c         => ProjectEntity.Create(c).PublishEvent(_publishEvent),
                _                       => Invalid("Commande non prise en charge")
            });

        private Exceptional<Validation<State>> LoadEntity(Guid entityId) =>
            _getEntity(entityId).Map(
            (entity)  => entity.Match(
                        None: ()    => Invalid($"Entité d'id {entityId} introuvable"),
                        Some: (x)   => Valid(x)));

        private Validation<Exceptional<Unit>> Handle<T>(
            Command command,
            Func<Guid, Exceptional<Option<State>>> getEntity,
            Func<T, Option<Validation<EventAndState>>> f) where T : State =>
                getEntity(command.EntityId).Match
                (
                    Exception:  (ex)        => (Exceptional<Unit>)ex,
                    Success:    (entity)    => entity.
                        CastTo<T>().
                        Bind(f).
                        Match(
                            None: ()    => Invalid($"Entité d'id {command.EntityId} introuvable"),
                            Some: (x)   => x.PublishEvent(_publishEvent))
                );

        private Validation<Exceptional<Unit>> Handle(Exceptional<Validation<IEnumerable<Event>>> events) =>
              events.Match(
                Exception: (ex) => (Exceptional<Unit>)ex,
                Success: (events) => events.PublishEvents(_publishEvent));
    }

    internal static class CommandHandlerExt
    {
        public static Validation<Exceptional<Unit>> PublishEvent(
                            this Validation<EventAndState> v,
                            Func<Event, Exceptional<Unit>> publishEvent) => 
            v.Bind<EventAndState, Exceptional<Unit>>((x) => publishEvent(x.Event));

        public static Validation<Exceptional<Unit>> PublishEvents(
                            this Validation<IEnumerable<Event>> events,
                            Func<Event, Exceptional<Unit>> publishEvent) =>
            events.Bind<IEnumerable<Event>, Exceptional<Unit>>((evts) => evts.Aggregate(
                        seed: new Exceptional<Unit>(),
                        func: (ex, next) => ex.Bind(_ => publishEvent(next))));

        public static Option<T> CastTo<T>(this Option<State> value) where T : State =>
            value.Bind<State, T>((state) => state is T t ? t : None);
    }
}
