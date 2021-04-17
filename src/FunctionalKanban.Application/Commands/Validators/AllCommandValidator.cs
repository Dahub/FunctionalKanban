﻿namespace FunctionalKanban.Application.Commands.Validators
{
    using System;
    using System.Collections.Generic;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;

    internal class AllCommandValidator : ValidatorBase<Command>
    {
        protected override IEnumerable<Error> GetErrors(Command c)
        {
            if (c.EntityId == Guid.Empty)
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
