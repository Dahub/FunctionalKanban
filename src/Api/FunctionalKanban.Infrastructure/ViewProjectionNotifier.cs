namespace FunctionalKanban.Infrastructure
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using Unit = System.ValueTuple;

    public class ViewProjectionNotifier : INotifier
    {
        private readonly IViewProjectionRepository _repo;

        public ViewProjectionNotifier(IViewProjectionRepository viewProjectionRepository) => _repo = viewProjectionRepository;

        public Exceptional<Unit> Notify(Event @event) =>
            Notify<TaskViewProjection>(_repo, @event).Bind(_ => 
            Notify<ProjectViewProjection>(_repo, @event, ProjectViewProjection.HandleWithId)).Bind(_ => 
            Notify<DeletedTaskViewProjection>(_repo, @event));

        private static Exceptional<Unit> Notify<T>(
                     IViewProjectionRepository repository,
                     Event @event) where T : ViewProjection, new() =>
            Notify<T>(repository, @event, (e) => e.EntityId);

        private static Exceptional<Unit> Notify<T>(
                     IViewProjectionRepository repository,
                     Event @event,
                     Func<Event, Option<Guid>> getProjectionIdFromEvent) where T : ViewProjection, new() =>
             getProjectionIdFromEvent(@event).Match(
                 None: ()   => Unit.Create(),
                 Some: (id) => HandleEvent<T>(repository, id, @event));

        private static Exceptional<Unit> HandleEvent<T>(
                    IViewProjectionRepository repository,
                    Guid projectionId, 
                    Event @event) where T : ViewProjection, new() =>
            GetProjection<T>(repository, projectionId).
            Bind((p) => p.With(@event).Match(
                Some: (value)   => repository.Upsert(value),
                None: ()        => repository.Delete(p)));

        private static Exceptional<T> GetProjection<T>(
                    IViewProjectionRepository repository,
                    Guid id) where T : ViewProjection, new() =>
            repository.GetById<T>(id).Match(
                Exception:  (e) => Exceptional.Of<T>(e),
                Success:    (p) => p.Match(
                        None: ()    => new T(),
                        Some: (p)   => p));
    }
}
