namespace FunctionalKanban.Application.Queries.QueriesBuilders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FunctionalKanban.Domain.Common;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

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
                Func<TParam, TQuery> f) 
                    where TQuery : Query
                    where TParam : notnull, new() =>
            parameters != null && parameters.ContainsKey(key)
                ? ParseParameterAndExecute(parameters, key, f)
                : Valid(query);

        private static Validation<TQuery> ParseParameterAndExecute<TQuery, TParam>(
                IDictionary<string, string> parameters, 
                string key, 
                Func<TParam, TQuery> f) 
                    where TQuery : Query
                    where TParam : notnull, new() => 
            parameters[key].Parse<TParam>().Match(
                Some: (value)   => Valid(f(value)),
                None: ()        => Invalid($"Paramètre incorrect {key} : type attendu {typeof(TParam).Name}"));

        private static Option<T> Parse<T>(this string input) where T : notnull, new() =>
            GetConverter<T>(input).Bind((tc) => Some((T)tc.ConvertFromString(input)));

        private static Option<TypeConverter> GetConverter<T>(string input)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (converter != null && converter.IsValid(input))
                ? Some(converter)
                : None;
        }
    }
}
