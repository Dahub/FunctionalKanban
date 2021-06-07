namespace FunctionalKanban.Core.Domain.Task.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FunctionalKanban.Core.Domain.Common;
    using FunctionalKanban.Core.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetTaskQuery : Query
    {
        protected override Expression<Func<ViewProjection, bool>> Predicate { get; init; } = (p) => p is TaskViewProjection;

        public GetTaskQuery WithMinRemaningWork(uint minRemaningWork) => this with { Predicate = PredicateBuilder.And(Predicate, (p) => ((TaskViewProjection)p).RemaningWork >= minRemaningWork) };

        public GetTaskQuery WithMaxRemaningWork(uint maxRemaningWork) => this with { Predicate = PredicateBuilder.And(Predicate, (p) => ((TaskViewProjection)p).RemaningWork < maxRemaningWork) };

        public GetTaskQuery WithTaskStatus(TaskStatus taskStatus) => this with { Predicate = PredicateBuilder.And(Predicate, (p) => ((TaskViewProjection)p).Status == taskStatus) };

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetTaskQuery, uint>(parameters, "minRemaningWork", WithMinRemaningWork).Bind(q => q.
            WithParameterValue<GetTaskQuery, uint>(parameters, "maxRemaningWork", q.WithMaxRemaningWork)).Bind(q => q.
            WithParameterValue<GetTaskQuery, TaskStatus>(parameters, "taskStatus", q.WithTaskStatus)).
            ToExceptional();     
    }
}
