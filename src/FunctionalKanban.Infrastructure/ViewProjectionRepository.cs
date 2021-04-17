namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public class ViewProjectionRepository : IViewProjectionRepository
    {
        private readonly IViewProjectionDataBase _dataBase;

        public ViewProjectionRepository(IViewProjectionDataBase dataBase) => _dataBase = dataBase;

        public Exceptional<IEnumerable<ViewProjection>> Get(Type projectionType, Func<ViewProjection, bool> predicate) =>
            _dataBase.Projections(projectionType).Bind(ps => GetByPredicate(predicate, ps));

        public Exceptional<Option<T>> GetById<T>(Guid id) where T : ViewProjection =>
            Try(() => 
                _dataBase.Projections<T>().
                Bind(ps => GetByPredicate((p) => p.Id.Equals(id), ps)).
                Match(
                    Exception: (ex) => throw ex,
                    Success: (ps) => ps.Any()?Some(ps.Single()):None)).
            Run();

        public Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection => _dataBase.Upsert(viewProjection);

        public Exceptional<Unit> Delete<T>(Guid id) where T : ViewProjection => _dataBase.Delete<T>(id);

        private static Exceptional<IEnumerable<T>> GetByPredicate<T>(
                Func<T, bool> predicate,
                IEnumerable<T> ps) where T : ViewProjection =>
            Try(() => ps.Where(predicate)).Run();
    }
}

