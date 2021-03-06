namespace FunctionalKanban.Core.Application.Commands.Validators
{
    using System.Collections.Generic;
    using FunctionalKanban.Core.Domain.Task.Commands;
    using LaYumba.Functional;

    internal class LinkToProjectValidator : Validator<LinkToProject>
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
