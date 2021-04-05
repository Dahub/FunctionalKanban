namespace FunctionalKanban.Domain.Task.Queries
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;

    public record GetTaskQuery : Query<TaskViewProjection>
    {
        public Option<uint> MinRemaningWork { get; init; }

        public Option<uint> MaxRemaningWork { get; init; }

        public Option<TaskStatus> TaskStatus { get; init; }

        public GetTaskQuery WithMinRemaningWork(uint minRemaningWork) => this with { MinRemaningWork = minRemaningWork };

        public GetTaskQuery WithMaxRemaningWork(uint maxRemaningWork) => this with { MaxRemaningWork = maxRemaningWork };

        public GetTaskQuery WithTaskStatus(TaskStatus taskStatus) => this with { TaskStatus = taskStatus };

        public override Func<TaskViewProjection, bool> BuildPredicate()
            => (p) =>
            MoreOrEqualThanValue(p.RemaningWork, MinRemaningWork)
            && StrictlyLessThanValue(p.RemaningWork, MaxRemaningWork)
            && EqualToValue(p.Status, TaskStatus);
    }
}
