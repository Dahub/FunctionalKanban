namespace FunctionalKanban.Application.Commands.Validators
{
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Project.Commands;
    using LaYumba.Functional;

    internal class CreateProjectValidator : Validator<CreateProject>
    {
        protected override IEnumerable<Error> GetErrors(CreateProject c)
        {
            if (string.IsNullOrWhiteSpace(c.Name))
            {
                yield return "Le projet dans avoir un nom";
            }

            yield break;
        }
    }
}
