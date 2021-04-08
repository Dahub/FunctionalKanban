namespace FunctionalKanban.Application.QueriesBuilders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Functional;
    using static FunctionalKanban.Functional.F;

    internal static class QueryBuilderExt
    {
        public static Exceptional<Query> ToExceptional<TQuery>(this Validation<TQuery> query) where TQuery : Query =>
              query.Match(
                  Valid: (q) => Exceptional(q as Query),
                  Invalid: (errors) => new ArgumentException(string.Join(" - ", errors)));

        public static Validation<TQuery> WithParameterValue<TQuery, TParam>(
                this TQuery query,
                IDictionary<string, string> parameters,
                string key,
                Func<TParam, TQuery> f) where TQuery : Query =>
            parameters != null && parameters.ContainsKey(key)
            ? ParseParameterAndExecute(parameters, key, f)
            : Valid(query);

        private static Validation<TQuery> ParseParameterAndExecute<TQuery, TParam>(
                IDictionary<string, string> parameters, 
                string key, 
                Func<TParam, TQuery> f) where TQuery : Query => 
            parameters[key].TryParse<TParam>(out var value)
            ? Valid(f(value))
            : Invalid($"Paramètre incorrect {key} : type attendu {typeof(TParam).Name}");

        private static bool TryParse<T>(this string input, out T value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter != null && converter.IsValid(input))
            {
                value = (T)converter.ConvertFromString(input);
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
