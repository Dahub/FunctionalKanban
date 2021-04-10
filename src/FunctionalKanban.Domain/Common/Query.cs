namespace FunctionalKanban.Domain.Common
{
    using System;
    using FunctionalKanban.Functional;

    public abstract record Query
    {

        protected bool MoreOrEqualThanValue(uint valueToCompare, Option<uint> value) => value.Match(
                        None: ()    => true,
                        Some: (v)   => valueToCompare >= v);

        protected bool StrictlyLessThanValue(uint valueToCompare, Option<uint> value) => value.Match(
                       None: ()     => true,
                       Some: (v)    => valueToCompare < v);

        protected bool EqualToValue<T>(T valueToCompare, Option<T> value) => value.Match(
                       None: ()     => true,
                       Some: (v)    => valueToCompare.Equals(v));

        protected bool NotEqualToValue<T>(T valueToCompare, Option<T> value) => value.Match(
                       None: ()     => true,
                       Some: (v)    => !valueToCompare.Equals(v));

        public abstract Func<ViewProjection, bool> BuildPredicate();
    }
}
