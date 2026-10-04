using aweXpect.Core.Constraints;
using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Delegates;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class ExpectationBuilderTests
	{
		[Fact]
		public async Task Eventually_ShouldExposeTheExpectationBuilderThroughIExpectThat()
		{
			EventuallySubject<int> subject = That(() => 0).Eventually().Within(TimeSpan.Zero);

			async Task Act()
				=> await new ExpectationResult(((IExpectThat<int>)subject).ExpectationBuilder
					.AddConstraint((_, _) => FailingConstraint<int>()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => 0
				             eventually is reached through IExpectThat within 0:00,
				             but it was not
				             """)
				.Because("the builder is no longer public on the subject, but still reachable for extension authors");
		}

		[Fact]
		public async Task WithoutValue_ShouldExposeTheExpectationBuilderThroughIExpectThat()
		{
			ThatDelegate.WithoutValue subject = That(() => { });

			async Task Act()
				=> await new ExpectationResult(((IExpectThat<ThatDelegate.WithoutValue>)subject).ExpectationBuilder
					.AddConstraint((_, _) => FailingConstraint<DelegateValue>()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => { }
				             is reached through IExpectThat,
				             but it was not
				             """)
				.Because("the builder is no longer public on the subject, but still reachable for extension authors");
		}

		[Fact]
		public async Task WithValue_ShouldExposeTheExpectationBuilderThroughIExpectThat()
		{
			ThatDelegate.WithValue<int> subject = That(() => 0);

			async Task Act()
				=> await new ExpectationResult(((IExpectThat<ThatDelegate.WithValue<int>>)subject).ExpectationBuilder
					.AddConstraint((_, _) => FailingConstraint<DelegateValue<int>>()));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => 0
				             is reached through IExpectThat,
				             but it was not
				             """)
				.Because("the builder is no longer public on the subject, but still reachable for extension authors");
		}

		private static DummyValueConstraint<T> FailingConstraint<T>()
			=> new(_ => new DummyConstraintResult(Outcome.Failure, "is reached through IExpectThat", "it was not"));
	}
}
