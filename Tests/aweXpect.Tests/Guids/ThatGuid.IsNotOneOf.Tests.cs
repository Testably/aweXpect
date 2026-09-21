using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				Guid subject = FixedGuid();
				Guid[] unexpected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Guid subject = FixedGuid();
				Guid[]? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
			{
				Guid subject = FixedGuid();
				IEnumerable<Guid?> unexpected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				Guid subject = FixedGuid();
				Guid?[] unexpected = [];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You have to provide at least one expected value!");
			}

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Guid subject = FixedGuid();
				Guid?[]? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The unexpected cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsContained_ShouldFail()
			{
				Guid subject = FixedGuid();
				IEnumerable<Guid> unexpected = [OtherGuid(), subject, OtherGuid(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not one of {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				Guid subject = FixedGuid();
				Guid[] unexpected = [OtherGuid(), OtherGuid(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
