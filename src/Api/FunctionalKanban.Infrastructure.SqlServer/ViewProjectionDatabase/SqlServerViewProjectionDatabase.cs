namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using FunctionalKanban.Infrastructure.Abstraction;
    using LaYumba.Functional;
    using Microsoft.EntityFrameworkCore;
    using static LaYumba.Functional.F;
    using Unit = System.ValueTuple;

    public class SqlServerViewProjectionDatabase : IViewProjectionDataBase
    {
        private readonly ViewProjectionDbContext _context;

        public SqlServerViewProjectionDatabase(ViewProjectionDbContext dbContext) => _context = dbContext;

        public Exceptional<Unit> Delete<T>(T viewProjection) where T : ViewProjection =>
            Try(() =>
            {
                _context.Set<T>().Remove(viewProjection);
                _context.SaveChanges();
                return Unit.Create();
            }).Run();

        public Exceptional<IEnumerable<T>> Projections<T>() where T : ViewProjection =>
            Try(() => _context.Set<T>().AsEnumerable()).Run();

        public Exceptional<IEnumerable<ViewProjection>> Projections(Type type, Expression<Func<ViewProjection, bool>> predicate) => 
            Try(() => _context.Set<TaskViewProjection>().Where(predicate).AsEnumerable()).Run();

        public Exceptional<Unit> Upsert<T>(T viewProjection) where T : ViewProjection =>
            Try(() =>
                FindEntity(_context, viewProjection).Match(
                    None: ()    => AddEntity(_context, viewProjection),
                    Some: (e)   => UpdateEntity(_context, e, viewProjection))
            ).Run();

        private static Unit UpdateEntity<T>(ViewProjectionDbContext context, T existingEntity, T viewProjection) where T : ViewProjection
        {
            context.Entry(existingEntity).State = EntityState.Detached;
            context.Attach(viewProjection);
            context.Update(viewProjection);
            context.SaveChanges();
            return Unit.Create();
        }

        private static Unit AddEntity<T>(ViewProjectionDbContext context, T viewProjection) where T : ViewProjection
        {
            context.Add(viewProjection);
            context.SaveChanges();
            return Unit.Create();
        }

        private static Option<T> FindEntity<T>(ViewProjectionDbContext context, T viewProjection) where T : ViewProjection =>
            context.Find(viewProjection.GetType(), viewProjection.Id) is T entity
            ? Some(entity)
            : None;
    }
}
