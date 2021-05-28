namespace FunctionalKanban.Application.Commands.Validators
{
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;

    internal class AllCommandValidator : Validator<Command>
    {
        protected override IEnumerable<Error> GetErrors(Command c)
        {
            if (c.EntityId == default)
            {
                yield return "L'id d'aggregat doit être défini";
            }

            if (c.TimeStamp == default)
            {
                yield return "Le time stamp doit être défini";
            }

            yield break;
        }
    }
}
