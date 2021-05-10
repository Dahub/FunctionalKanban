namespace FunctionalKanban.Domain.Task.Queries
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.ViewProjections;
    using LaYumba.Functional;

    public record GetTaskQuery : Query
    {
        public Option<uint> MinRemaningWork { get; private set; }

        public Option<uint> MaxRemaningWork { get; private set; }

        public Option<TaskStatus> TaskStatus { get; private set; }

        public GetTaskQuery WithMinRemaningWork(uint minRemaningWork) => this with { MinRemaningWork = minRemaningWork };

        public GetTaskQuery WithMaxRemaningWork(uint maxRemaningWork) => this with { MaxRemaningWork = maxRemaningWork };

        public GetTaskQuery WithTaskStatus(TaskStatus taskStatus) => this with { TaskStatus = taskStatus };

        public override Func<ViewProjection, bool> BuildPredicate() => (p) => BuildPredicate((TaskViewProjection)p);

        public override Exceptional<Query> WithParameters(IDictionary<string, string> parameters) => this.
            WithParameterValue<GetTaskQuery, uint>(parameters, "minRemaningWork", WithMinRemaningWork).Bind(q => q.
            WithParameterValue<GetTaskQuery, uint>(parameters, "maxRemaningWork", q.WithMaxRemaningWork)).Bind(q => q.
            WithParameterValue<GetTaskQuery, TaskStatus>(parameters, "taskStatus", q.WithTaskStatus)).
            ToExceptional();

        private bool BuildPredicate(TaskViewProjection p) =>
            p.RemaningWork.MoreOrEqualThan(MinRemaningWork)
            && p.RemaningWork.StrictlyLessThan(MaxRemaningWork)
            && p.Status.EqualTo(TaskStatus)
            && p.Status.DifferentFrom(Task.TaskStatus.Archived);
    }
}
