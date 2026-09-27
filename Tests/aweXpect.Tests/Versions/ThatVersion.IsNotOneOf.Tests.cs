using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> unexpected = [new Version(1, 3), new Version(1, 2),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected, because we want to test the failure,
					             but it was 1.2

					             Unexpected values:
					             [1.3, 1.2]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(1, 2);

				async Task Act()
					=> await That(subject).IsNotOneOf(new Version(1, 3), new Version(2, 0));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(new Version(1, 3), null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [1.3, <null>],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectOmitsAComponentOfUnexpected_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> unexpected = [new Version(1, 2, 0), new Version(1, 2, 0, 0),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow()
					.Because("an unset component is not treated as 0");
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> unexpected = [];

				object Act()
					=> That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> unexpected = [new Version(1, 2),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotOneOf(unexpected));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> unexpected = [new Version(1, 3),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of unexpected,
					             but it was 1.2

					             Unexpected values:
					             [1.3]
					             """);
			}
		}
	}
}
