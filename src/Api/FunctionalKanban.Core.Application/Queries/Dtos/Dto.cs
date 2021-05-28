namespace FunctionalKanban.Core.Application.Queries.Dtos
{
    using System;

    public abstract record Dto
    {
        public Guid Id { get; init; }
    }
}
