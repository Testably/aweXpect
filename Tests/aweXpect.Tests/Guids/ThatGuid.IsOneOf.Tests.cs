using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				Guid subject = FixedGuid();
				Guid[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Guid subject = FixedGuid();
				Guid[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedOnlyContainsNull_ShouldFail()
			{
				Guid subject = FixedGuid();
				IEnumerable<Guid?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				Guid subject = FixedGuid();
				Guid?[] expected = [];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Guid subject = FixedGuid();
				Guid?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The expected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsContained_ShouldSucceed()
			{
				Guid subject = FixedGuid();
				IEnumerable<Guid> expected = [OtherGuid(), subject, OtherGuid(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				Guid subject = FixedGuid();
				Guid[] expected = [OtherGuid(), OtherGuid(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of {Formatter.Format(expected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
