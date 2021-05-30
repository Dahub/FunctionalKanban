namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase.EfEntities
{
    using System;

    internal class DeletedTask
    {
        public Guid Id { get; set; }

        public DateTime DeletedAt { get; set; }
    }
}
