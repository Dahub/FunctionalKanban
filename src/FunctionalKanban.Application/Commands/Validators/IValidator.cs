namespace FunctionalKanban.Application.Commands.Validators
{
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;

    internal interface IValidator
    {
        bool CanValidate(Command command);

        Validation<Command> Validate(Command command);
    }
}


