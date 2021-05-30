namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase.EfEntities
{
    using System;

    internal class Task
    {
        public Task() => Name = string.Empty;

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public Guid? ProjectId { get; set; }

        public int RemaningWork { get; set; }
    }
}
