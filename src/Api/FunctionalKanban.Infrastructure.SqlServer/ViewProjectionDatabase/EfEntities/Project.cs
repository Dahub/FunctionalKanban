namespace FunctionalKanban.Infrastructure.SqlServer.ViewProjectionDatabase.EfEntities
{
    using System;

    internal class Project
    {
        public Project() => Name = string.Empty;

        public Guid Id { get; set; }

        public bool IsDeleted { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public int TotalRemaningWork { get; set; }
    }
}
