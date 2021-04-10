namespace FunctionalKanban.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using FunctionalKanban.Infrastructure.Abstraction;
    using static FunctionalKanban.Functional.F;
    using Unit = System.ValueTuple;

    public class ViewProjectionRepository<T> : IViewProjectionRepository<T> where T : ViewProjection
    {
        private readonly IViewProjectionDataBase _dataBase;

        public ViewProjectionRepository(IViewProjectionDataBase dataBase) => _dataBase = dataBase;

        public Exceptional<IEnumerable<T>> Get(Func<T, bool> predicate) =>
            _dataBase.Projections<T>().Bind(ps => GetByPredicate(predicate, ps));

        public Exceptional<Option<T>> GetById(Guid id) =>
            Try(() => 
                _dataBase.Projections<T>().
                Bind(ps => GetByPredicate((p) => p.Id.Equals(id), ps)).
                Match(
                    Exception: (ex) => throw ex,
                    Success: (ps) => ps.Any()?Some(ps.Single()):None)).
            Run();

        public Exceptional<Unit> Upsert(T viewProjection) => _dataBase.Upsert(viewProjection);

        private static Exceptional<IEnumerable<T>> GetByPredicate(
                Func<T, bool> predicate,
                IEnumerable<T> ps) =>
            Try(() => ps.Where(predicate)).Run();
    }
}

