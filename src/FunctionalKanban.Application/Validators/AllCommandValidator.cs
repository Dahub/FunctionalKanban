namespace FunctionalKanban.Application.Validators
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;

    internal class AllCommandValidator : ValidatorBase<Command>
    {
        protected override IEnumerable<Error> GetErrors(Command c)
        {
            if (c.AggregateId == Guid.Empty)
            {
                yield return "L'id d'aggregat doit être défini";
            }

            if (c.TimeStamp == new DateTime())
            {
                yield return "Le time stamp doit être défini";
            }

            yield break;
        }
    }
}
