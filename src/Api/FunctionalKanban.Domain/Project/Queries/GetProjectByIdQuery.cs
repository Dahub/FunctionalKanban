﻿namespace FunctionalKanban.Domain.Project.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetProjectByIdQuery : Query
    {
        public Guid Id { get; private set; }

        public GetProjectByIdQuery WithId(Guid id) => this with { Id = id };

        public override Func<ViewProjection, bool> BuildPredicate() => (p) =>
             ((ProjectViewProjection)p).Id.Equals(Id)
             && ((ProjectViewProjection)p).IsDeleted.EqualTo(false);

        public override Exceptional<Query> Build(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetProjectByIdQuery, Guid>(parameters, "id", WithId).
            ToExceptional();
    }
}