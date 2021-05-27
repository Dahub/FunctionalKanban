namespace FunctionalKanban.Application.Commands.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    internal abstract class Validator<T> : IValidator where T : Command
    {
        public bool CanValidate(Command command) => command is T;

        public Validation<Command> Validate(Command command) => ToValidation(GetErrors((T)command), command);

        protected abstract IEnumerable<Error> GetErrors(T command);

        private static Func<IEnumerable<Error>, Command, Validation<Command>> ToValidation => (errors, command) =>
            errors.Any() ? Invalid(errors) : Valid(command);
    }
}


