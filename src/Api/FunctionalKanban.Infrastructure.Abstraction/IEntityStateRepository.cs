namespace FunctionalKanban.Infrastructure.Abstraction
{
    using System;
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;

    public interface IEntityStateRepository
    {
        Exceptional<Option<State>> GetById(Guid id);
    }
}
