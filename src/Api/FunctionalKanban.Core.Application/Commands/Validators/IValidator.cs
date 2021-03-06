namespace FunctionalKanban.Core.Application.Commands.Validators
{
    using FunctionalKanban.Core.Domain.Common;
    using LaYumba.Functional;

    internal interface IValidator
    {
        bool CanValidate(Command command);

        Validation<Command> Validate(Command command);
    }
}


