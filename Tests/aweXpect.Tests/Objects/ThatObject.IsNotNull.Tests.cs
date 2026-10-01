namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed class IsNotNull
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsFaultedTask_ShouldFail()
			{
				Task subject = Task.FromException(new InvalidOperationException("boom"));

#pragma warning disable aweXpect0004
				async Task Act()
					=> await That(subject).IsNotNull();
#pragma warning restore aweXpect0004

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null,
					             but it did throw an InvalidOperationException:
					               boom
					             """).And
					.WithInner<InvalidOperationException>(inner => inner.HasMessage("boom"))
					.Because("a faulted task must be reported as a failed expectation, not as an internal exception");
			}

			[Fact]
			public async Task WhenSubjectIsFaultedValueTask_ShouldFail()
			{
				ValueTask subject = new(Task.FromException(new InvalidOperationException("boom")));

#pragma warning disable aweXpect0004
				async Task Act()
					=> await That(subject).IsNotNull();
#pragma warning restore aweXpect0004

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null,
					             but it did throw an InvalidOperationException:
					               boom
					             """).And
					.WithInner<InvalidOperationException>(inner => inner.HasMessage("boom"))
					.Because("a faulted value task must be reported as a failed expectation, not as an internal exception");
			}

			[Fact]
			public async Task WhenSubjectIsNull_AndChainedWithSatisfies_ShouldNotEvaluateThePredicate()
			{
				InnerClass? subject = null;
				bool isEvaluated = false;

				async Task Act()
					=> await That(subject).IsNotNull().And.Satisfies(x => (isEvaluated = true) && x!.IntValue == 1);

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null and satisfies x => (isEvaluated = true) && x!.IntValue == 1,
					             but it was <null>
					             """);
				await That(exception.InnerException).IsNull()
					.Because("the predicate must not throw on a subject that already failed the null check");
				await That(isEvaluated).IsFalse();
			}

			[Fact]
			public async Task WhenSubjectIsNull_AndChainedWithWhose_ShouldReportNullOnce()
			{
				InnerClass? subject = null;

				async Task Act()
					=> await That(subject).IsNotNull().And.Whose(x => x.IntValue, v => v.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null and whose IntValue is equal to 1,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				object? subject = null;

				async Task Act()
					=> await That(subject).IsNotNull()
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null, because we want to test the failure,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsObject_ShouldSucceed()
			{
				object subject = new MyClass();

				async Task Act()
					=> await That(subject).IsNotNull();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class StructTests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).IsNotNull()
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not null, because we want to test the failure,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsObject_ShouldSucceed()
			{
				int? subject = 1;

				async Task Act()
					=> await That(subject).IsNotNull();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
