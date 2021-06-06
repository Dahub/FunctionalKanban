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
        public Option<uint> MinRemaningWork { get; private set; }

        public Option<uint> MaxRemaningWork { get; private set; }

        public Option<TaskStatus> TaskStatus { get; private set; }

        public GetTaskQuery WithMinRemaningWork(uint minRemaningWork) => this with { MinRemaningWork = minRemaningWork };

        public GetTaskQuery WithMaxRemaningWork(uint maxRemaningWork) => this with { MaxRemaningWork = maxRemaningWork };

        public GetTaskQuery WithTaskStatus(TaskStatus taskStatus) => this with { TaskStatus = taskStatus };

        public override Expression<Func<ViewProjection, bool>> BuildPredicate() => (p) =>            
            ((TaskViewProjection)p).RemaningWork.MoreOrEqualThan(MinRemaningWork)
            && ((TaskViewProjection)p).RemaningWork.StrictlyLessThan(MaxRemaningWork)
            && ((TaskViewProjection)p).Status.EqualTo(TaskStatus)
            && ((TaskViewProjection)p).Status.DifferentFrom(Task.TaskStatus.Archived);

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetTaskQuery, uint>(parameters, "minRemaningWork", WithMinRemaningWork).Bind(q => q.
            WithParameterValue<GetTaskQuery, uint>(parameters, "maxRemaningWork", q.WithMaxRemaningWork)).Bind(q => q.
            WithParameterValue<GetTaskQuery, TaskStatus>(parameters, "taskStatus", q.WithTaskStatus)).
            ToExceptional();
    }
}
