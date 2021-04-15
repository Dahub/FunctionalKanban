namespace FunctionalKanban.Application.Commands.Validators
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Functional;

    internal class CreateTaskValidator : ValidatorBase<CreateTask>
    {
        protected override IEnumerable<Error> GetErrors(CreateTask c)
        {
            if (string.IsNullOrWhiteSpace(c.Name))
            {
                yield return "La tâche dois avoir un nom";
            }

            yield break;
        }
    }
}
