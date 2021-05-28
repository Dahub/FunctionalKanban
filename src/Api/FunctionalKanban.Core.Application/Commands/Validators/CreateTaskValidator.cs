namespace FunctionalKanban.Core.Application.Commands.Validators
{
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Task.Commands;
    using LaYumba.Functional;

    internal class CreateTaskValidator : Validator<CreateTask>
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
