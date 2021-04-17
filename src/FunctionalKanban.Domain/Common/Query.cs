namespace FunctionalKanban.Domain.Common
{
    using System;
    using LaYumba.Functional;

    public abstract record Query
    {

        protected static bool MoreOrEqualThanValue(uint valueToCompare, Option<uint> value) => value.Match(
                    None: () => true,
                    Some: (v) => valueToCompare >= v);

        protected static bool StrictlyLessThanValue(uint valueToCompare, Option<uint> value) => value.Match(
                    None: () => true,
                    Some: (v) => valueToCompare < v);

        protected static bool EqualToValue<TValue>(TValue valueToCompare, Option<TValue> value) where TValue : notnull =>
            value.Match(
                    None: () => true,
                    Some: (v) => valueToCompare.Equals(v));

        protected static bool NotEqualToValue<TValue>(TValue valueToCompare, Option<TValue> value) where TValue : notnull =>
            value.Match(
                    None: () => true,
                    Some: (v) => !valueToCompare.Equals(v));

        public abstract Func<ViewProjection, bool> BuildPredicate();
    }
}
