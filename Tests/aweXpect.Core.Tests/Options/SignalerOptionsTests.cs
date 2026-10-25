using System.Threading;
using aweXpect.Chronology;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed class SignalerOptionsTests
{
	public sealed class Tests
	{
		[Test]
		public async Task ToString_Empty_ShouldReturnEmptyString()
		{
			SignalerOptions sut = new();

			string result = sut.ToString();

			await That(result).IsEmpty();
		}

		[Test]
		public async Task ToString_WithInfiniteTimeout_ShouldReturnEmptyString()
		{
			SignalerOptions sut = new();
			sut.Within(Timeout.InfiniteTimeSpan);

			string result = sut.ToString();

			await That(result).IsEmpty()
				.Because("an infinite timeout adds no information to the expectation");
		}

		[Test]
		public async Task ToString_WithTimeout_ShouldIncludeTimeout()
		{
			SignalerOptions sut = new();
			sut.Within(5.Seconds());

			string result = sut.ToString();

			await That(result).IsEqualTo(" within 0:05");
		}

		[Test]
		public async Task Within_ShouldReturnTheSameOptions()
		{
			SignalerOptions sut = new();

			SignalerOptions result = sut.Within(5.Seconds());

			await That(result).IsSameAs(sut);
		}

		[Test]
		public async Task Within_WhenSpecifiedTwice_ShouldUseTheLastTimeout()
		{
			SignalerOptions sut = new();
			sut.Within(5.Seconds());

			sut.Within(6.Seconds());

			await That(sut.Timeout).IsEqualTo(6.Seconds());
			await That(sut.ToString()).IsEqualTo(" within 0:06");
		}
	}

	public sealed class WithParameterTests
	{
		[Test]
		public async Task ToString_Empty_ShouldReturnEmptyString()
		{
			SignalerOptions<int> sut = new();

			string result = sut.ToString();

			await That(result).IsEmpty();
		}

		[Test]
		public async Task ToString_WhenPredicateIsAddedAfterFirstCall_ShouldAppendTimeoutLast()
		{
			SignalerOptions<int> sut = new();
			sut.Within(75.Seconds());
			sut.WithPredicate(_ => true, "my predicate");
			_ = sut.ToString();
			sut.WithPredicate(_ => true, "my other predicate");

			string result = sut.ToString();

			await That(result).IsEqualTo(" with my predicate and with my other predicate within 1:15");
		}

		[Test]
		public async Task ToString_WithPredicate_ShouldIncludePredicateExpression()
		{
			SignalerOptions<int> sut = new();
			sut.WithPredicate(_ => true, "my predicate");

			string result = sut.ToString();

			await That(result).IsEqualTo(" with my predicate");
		}

		[Test]
		public async Task ToString_WithTimeout_ShouldIncludeTimeout()
		{
			SignalerOptions<int> sut = new();
			sut.Within(5.Seconds());

			string result = sut.ToString();

			await That(result).IsEqualTo(" within 0:05");
		}

		[Test]
		public async Task ToString_WithTimeoutAndPredicate_ShouldIncludePredicateExpression()
		{
			SignalerOptions<int> sut = new();
			sut.Within(75.Seconds());
			sut.WithPredicate(_ => true, "my predicate");

			string result = sut.ToString();

			await That(result).IsEqualTo(" with my predicate within 1:15");
		}

		[Test]
		public async Task ToString_WithTimeoutAndPredicate_WhenCalledTwice_ShouldReturnSameText()
		{
			SignalerOptions<int> sut = new();
			sut.Within(75.Seconds());
			sut.WithPredicate(_ => true, "my predicate");

			_ = sut.ToString();
			string result = sut.ToString();

			await That(result).IsEqualTo(" with my predicate within 1:15");
		}

		[Test]
		public async Task WithPredicate_ShouldReturnTheSameOptions()
		{
			SignalerOptions<int> sut = new();

			SignalerOptions<int> result = sut.WithPredicate(_ => true, "my predicate");

			await That(result).IsSameAs(sut);
		}
	}
}
