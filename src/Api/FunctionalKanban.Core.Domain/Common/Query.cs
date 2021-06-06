namespace FunctionalKanban.Core.Domain.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using FunctionalKanban.Core.Shared;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public abstract record Query
    {
        public abstract Expression<Func<ViewProjection, bool>> BuildPredicate();

        public abstract Exceptional<Query> WithParameters(IDictionary<string, string> parameters);
    }

    internal static class QueryExt
    {
        public static bool MoreOrEqualThan(this uint valueToCompare, Option<uint> value) => 
            value.Match(
                None: () => true,
                Some: (v) => valueToCompare >= v);

        public static bool StrictlyLessThan(this uint valueToCompare, Option<uint> value) => 
            value.Match(
                None: () => true,
                Some: (v) => valueToCompare < v);

        public static bool MoreOrEqualThan(this int valueToCompare, Option<uint> value) =>
           value.Match(
               None: () => true,
               Some: (v) => valueToCompare >= v);

        public static bool StrictlyLessThan(this int valueToCompare, Option<uint> value) =>
            value.Match(
                None: () => true,
                Some: (v) => valueToCompare < v);

        public static bool EqualTo<TValue>(this TValue valueToCompare, Option<TValue> value) where TValue : notnull =>
            value.Match(
                None: () => true,
                Some: (v) => valueToCompare.Equals(v));

        public static bool DifferentFrom<TValue>(this TValue valueToCompare, Option<TValue> value) where TValue : notnull =>
            value.Match(
                None: () => true,
                Some: (v) => !valueToCompare.Equals(v));

        public static Validation<TQuery> WithParameterValue<TQuery, TParam>(
             this TQuery query,
             IDictionary<string, string> parameters,
             string key,
             Func<TParam, TQuery> f)
                 where TQuery : Query
                 where TParam : notnull =>
         parameters != null && parameters.ContainsKey(key)
             ? ParseParameterAndExecute(parameters, key, f)
             : Valid(query);

        public static Exceptional<Query> ToExceptional<TQuery>(this Validation<TQuery> query) where TQuery : Query =>
              query.Match(
                  Valid: (q) => Exceptional(q as Query),
                  Invalid: (errors) => new ArgumentException(string.Join(" - ", errors)));

        private static Validation<TQuery> ParseParameterAndExecute<TQuery, TParam>(
                IDictionary<string, string> parameters,
                string key,
                Func<TParam, TQuery> f)
                    where TQuery : Query
                    where TParam : notnull =>
            parameters[key].Parse<TParam>().Match(
                Some: (value) => Valid(f(value)),
                None: () => Invalid($"Paramètre incorrect {key} : type attendu {typeof(TParam).Name}"));

        private static Option<T> Parse<T>(this string input) where T : notnull =>
            GetConverter<T>(input).Bind((tc) => Some((T)tc.ConvertFromString(input)));

        private static Option<TypeConverter> GetConverter<T>(string input) =>
            TypeDescriptor.GetConverter(typeof(T)).
                ToOption().
                Bind(c => c.IsValid(input)
                    ? Some(c)
                    : None);
    }
}
