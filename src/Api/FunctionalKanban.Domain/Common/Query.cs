namespace FunctionalKanban.Domain.Common
{
    using System;
    using LaYumba.Functional;

    public abstract record Query
    {
        public abstract Func<ViewProjection, bool> BuildPredicate();
    }

    internal static class QueryExt
    {
        public static bool MoreOrEqualThan(this uint valueToCompare, Option<uint> value) => value.Match(
                    None: () => true,
                    Some: (v) => valueToCompare >= v);

        public static bool StrictlyLessThan(this uint valueToCompare, Option<uint> value) => value.Match(
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
    }
}
