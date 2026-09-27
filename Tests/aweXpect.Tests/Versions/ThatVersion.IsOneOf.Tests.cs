using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?>? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> expected = [new Version(1, 3), new Version(1, 2),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(1, 2);

				async Task Act()
					=> await That(subject).IsOneOf(new Version(1, 3), new Version(2, 0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [1.3, 2.0],
					             but it was 1.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
			{
				Version? subject = null;
				IEnumerable<Version?> expected = [new Version(1, 3), null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNullAndExpectedDoesNotContainNull_ShouldFail()
			{
				Version? subject = null;
				IEnumerable<Version?> expected = [new Version(1, 3),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was <null>

					             Expected values:
					             [1.3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectOmitsAComponentOfExpected_ShouldFail()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> expected = [new Version(1, 2, 0), new Version(1, 2, 0, 0),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was 1.2

					             Expected values:
					             [1.2.0, 1.2.0.0]
					             """)
					.Because("an unset component is not treated as 0");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> expected = [new Version(1, 3), new Version(1, 2),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of expected,
					             but it was 1.2

					             Expected values:
					             [1.3, 1.2]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				IEnumerable<Version?> expected = [new Version(1, 3),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(expected));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
