namespace FunctionalKanban.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Commands;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    public static class CommandValidator
    {
        public static Validation<Command> Validate<T>(T command) where T : Command =>
            (command) switch
            { 
                CreateTask c        => ValidateCommandObject(c).Bind(ValidateCreateTask),
                ChangeTaskStatus c  => ValidateCommandObject(c).Bind(ValidateChangeTaskStatus),
                _                   => command
            };

        private static Validation<T> ValidateCommandObject<T>(T c) where T : Command =>
#pragma warning disable IDE0029 // coalesce cannot be use with T or Error
            c != null ? c : Error("La commande ne peut être null");
#pragma warning restore IDE0029

        private static Validation<Command> ValidateCreateTask(CreateTask c) => 
            c.GetErrorsCommand().Union(c.GetErrorsCreateTask()).ToValidation(c);

        private static Validation<Command> ValidateChangeTaskStatus(ChangeTaskStatus c) =>
            c.GetErrorsCommand().ToValidation(c);

        private static Validation<Command> ToValidation(this IEnumerable<Error> errors, Command c) =>
            errors.Any() ? Invalid(errors) : Valid(c);

        private static IEnumerable<Error> GetErrorsCommand(this Command c)
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

        private static IEnumerable<Error> GetErrorsCreateTask(this CreateTask c)
        {

            if(string.IsNullOrWhiteSpace(c.Name))
            {
                yield return "La tâche dans avoir un nom";
            }

            yield break;
        }
    }
}   
