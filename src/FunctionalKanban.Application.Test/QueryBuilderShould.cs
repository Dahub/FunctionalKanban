namespace FunctionalKanban.Application.Test
{
    using System;
    using FluentAssertions;
    using FunctionalKanban.Domain.Common;
    using FunctionalKanban.Domain.Task.Queries;
    using Xunit;

    public class QueryBuilderShould
    {
        [Fact]
        public void ReturnExceptionWhenBuildUnknowQuery()
        {
            var error = QueryBuilder.BuildQuery<UnknowQuery>(null)
                .Match(
                    Exception:  (ex)    => ex.Message,
                    Success:    (_)     => string.Empty
                );

            error.Should().Be("Type de requête non pris en charge");
        }

        [Fact]
        public void ReturnQueryWhenBuildKnowQueryWithNullParameters()
        {
            var query = QueryBuilder.BuildQuery<GetTaskQuery>(null);

            query.Match(
                Exception: (_) => false,
                Success: (_) => true).Should().BeTrue();
        }

        internal record UnknowQuery : Query
        {
            public override Func<ViewProjection, bool> BuildPredicate() => (_) => true;
        }
    }
}
