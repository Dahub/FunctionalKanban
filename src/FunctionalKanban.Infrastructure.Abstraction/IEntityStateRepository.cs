namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;

    public interface IEntityStateRepository
    {
        Exceptional<Option<State>> GetById(Guid id);
    }
}
