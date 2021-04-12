namespace FunctionalKanban.Application.Dtos
{
    using System;

    public abstract record Dto
    {
        public Guid Id { get; init; }
    }
}
