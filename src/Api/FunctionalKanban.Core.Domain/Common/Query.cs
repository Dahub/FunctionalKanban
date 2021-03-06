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
        public Expression<Func<ViewProjection, bool>> GetPredicate() => Predicate;

        public abstract Exceptional<Query> WithParameters(IDictionary<string, string> parameters);

        protected abstract Expression<Func<ViewProjection, bool>> Predicate { get; init; } 
    }

    internal static class QueryExt
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            var p = a.Parameters[0];
            var visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;
            var body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        private class SubstExpressionVisitor : ExpressionVisitor
        {
            public Dictionary<Expression, Expression> subst = new();

            protected override Expression VisitParameter(ParameterExpression node) => 
                subst.TryGetValue(node, out var newValue) ? newValue : node;
        }

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
                  Valid: (q)        => Exceptional(q as Query),
                  Invalid: (errors) => new ArgumentException(string.Join(" - ", errors)));

        private static Validation<TQuery> ParseParameterAndExecute<TQuery, TParam>(
                IDictionary<string, string> parameters,
                string key,
                Func<TParam, TQuery> f)
                    where TQuery : Query
                    where TParam : notnull =>
            parameters[key].Parse<TParam>().Match(
                Some: (value)   => Valid(f(value)),
                None: ()        => Invalid($"Paramètre incorrect {key} : type attendu {typeof(TParam).Name}"));

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
