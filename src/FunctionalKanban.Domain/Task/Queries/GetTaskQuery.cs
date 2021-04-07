namespace FunctionalKanban.Domain.Task.Queries
{
    using System;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.ViewProjections;
    using FunctionalKanban.Functional;

    public record GetTaskQuery : Query
    {
        public Option<uint> MinRemaningWork { get; init; }

        public Option<uint> MaxRemaningWork { get; init; }

        public Option<TaskStatus> TaskStatus { get; init; }

        public GetTaskQuery WithMinRemaningWork(uint minRemaningWork) => this with { MinRemaningWork = minRemaningWork };

        public GetTaskQuery WithMaxRemaningWork(uint maxRemaningWork) => this with { MaxRemaningWork = maxRemaningWork };

        public GetTaskQuery WithTaskStatus(TaskStatus taskStatus) => this with { TaskStatus = taskStatus };

        public override Func<ViewProjection, bool> BuildPredicate() => (p) => 
            MoreOrEqualThanValue(((TaskViewProjection)p).RemaningWork, MinRemaningWork)
            && StrictlyLessThanValue(((TaskViewProjection) p).RemaningWork, MaxRemaningWork)
            && EqualToValue(((TaskViewProjection)p).Status, TaskStatus);
    }
}
