namespace FunctionalKanban.Domain.Task
{
    public record TaskState(
        string TaskName,
        TaskStatus TaskStatus);
}
