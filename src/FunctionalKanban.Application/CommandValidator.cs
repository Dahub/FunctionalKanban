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
                CreateTask c => ValidateCommandObject(c).Bind(ValidateCreateTask),
                _ => command
            };

        private static Validation<Command> ValidateCreateTask(CreateTask c) => 
            c.GetErrors().ToValidation(c);

        private static Validation<Command> ToValidation(this IEnumerable<Error> errors, Command c) =>
            errors == null ? Valid(c) : Invalid(errors);

        private static IEnumerable<Error> GetErrors(this CreateTask c)
        {
            if (c.EntityId == Guid.Empty)
            {
                yield return "L'id d'entity doit être défini";
            }

            if(string.IsNullOrWhiteSpace(c.Name))
            {
                yield return "La tâche dans avoir un nom";
            }

            if(c.TimeStamp == new DateTime())
            {
                yield return "Le time stamp doit être défini";
            }
        }
        private static Validation<T> ValidateCommandObject<T>(T c) where T : Command =>
            c != null ? c : Error("La commande ne peut être null");  
    }
}   
