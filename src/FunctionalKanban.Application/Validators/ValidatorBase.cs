namespace FunctionalKanban.Application.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    internal abstract class ValidatorBase<T> : IValidator where T : Command
    {
        public bool CanValidate(Command command) => command is T;

        public Validation<Command> Validate(Command command) => ToValidation(GetErrors((T)command), command);

        protected abstract IEnumerable<Error> GetErrors(T command);

        private static Func<IEnumerable<Error>, Command, Validation<Command>> ToValidation => (errors, command) =>
            errors.Any() ? Invalid(errors) : Valid(command);
    }

    internal interface IValidator
    {
        bool CanValidate(Command command);

        Validation<Command> Validate(Command command);
    }
}


