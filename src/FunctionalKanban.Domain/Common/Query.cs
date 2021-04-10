namespace FunctionalKanban.Domain.Common
{
    using System;
    using FunctionalKanban.Functional;

    public abstract record Query
    {

        protected static bool MoreOrEqualThanValue(uint valueToCompare, Option<uint> value) => value.Match(
                        None: ()    => true,
                        Some: (v)   => valueToCompare >= v);

        protected static bool StrictlyLessThanValue(uint valueToCompare, Option<uint> value) => value.Match(
                       None: ()     => true,
                       Some: (v)    => valueToCompare < v);

        protected static bool EqualToValue<T>(T valueToCompare, Option<T> value) => value.Match(
                       None: ()     => true,
                       Some: (v)    => valueToCompare.Equals(v));

        protected static bool NotEqualToValue<T>(T valueToCompare, Option<T> value) => value.Match(
                       None: ()     => true,
                       Some: (v)    => !valueToCompare.Equals(v));

        public abstract Func<ViewProjection, bool> BuildPredicate();
    }
}
