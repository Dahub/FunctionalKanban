namespace FunctionalKanban.Shared
{
    using System.Collections.Generic;
    using System.Linq;
    using LaYumba.Functional;
    using static LaYumba.Functional.F;

    public static class FunctionalExtension
    {
        public static Option<T> CastTo<R, T>(this Option<R> value) => value.Bind<R, T>((x) => x is T t ? t : None);

        public static Exceptional<IEnumerable<T>> ToMonadOfList<T>(this IEnumerable<Exceptional<T>> exceptionals) =>
          exceptionals.Aggregate(
              seed: Exceptional(Enumerable.Empty<T>()),
              func: (list, next) => next.Bind(value => list.Bind(a => Exceptional(a.Append(value)))));

        public static Validation<IEnumerable<T>> ToMonadOfList<T>(this IEnumerable<Validation<T>> validations) =>
            validations.Aggregate(
                seed: Valid(Enumerable.Empty<T>()),
                func: (list, next) => next.Bind(value => list.Bind(a => Valid(a.Append(value)))));
    }
}
