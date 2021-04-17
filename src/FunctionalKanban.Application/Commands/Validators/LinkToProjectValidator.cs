namespace FunctionalKanban.Application.Commands.Validators
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Task.Commands;
    using LaYumba.Functional;

    internal class LinkToProjectValidator : ValidatorBase<LinkToProject>
    {
        protected override IEnumerable<Error> GetErrors(LinkToProject c)
        {
            if (c.ProjectId.Equals(default))
            {
                yield return "L'id de projet doit être défini";
            }

            yield break;
        }
    }
}
