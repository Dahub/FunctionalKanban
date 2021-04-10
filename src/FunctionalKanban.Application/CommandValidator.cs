namespace FunctionalKanban.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Application.Validators;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public static class CommandValidator
    {
        private static readonly IEnumerable<IValidator> _validators = new List<IValidator>()
        {
            new AllCommandValidator(),
            new CreateTaskValidator()
        };

        public static Validation<Command> Validate<T>(this T command) where T : Command => HarvestErrors(_validators, command);

        private static Func<IEnumerable<IValidator>, Command, Validation<Command>> HarvestErrors => (validators, command) =>
        {
            var errors = 
                validators.
                Where(v => v.CanValidate(command)).
                Map(v => v.Validate(command)).
                Bind(v => v.Match(
                    Invalid:    (errors)    => Some(errors),
                    Valid:      (_)         => None));

            return errors.Any()
               ? Invalid(errors.Flatten())
               : Valid(command);
        };
    }
}
