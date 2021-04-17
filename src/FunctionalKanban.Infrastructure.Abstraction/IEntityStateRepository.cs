namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;

    public interface IEntityStateRepository
    {
        Exceptional<Option<State>> GetById(Guid id);
    }
}
