using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed partial class Nullable
	{
		public sealed class IsOneOf
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					Guid? subject = FixedGuid();
					Guid[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					Guid? subject = FixedGuid();
					Guid[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenExpectedOnlyContainsNull_ShouldFail()
				{
					Guid? subject = FixedGuid();
					IEnumerable<Guid?> expected = [null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is one of expected,
						              but it was {Formatter.Format(subject)}

						              Expected values:
						              [<null>]
						              """);
				}

				[Test]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					Guid? subject = FixedGuid();
					Guid?[] expected = [];

					object Act()
						=> That(subject).IsOneOf(expected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Test]
				public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					Guid? subject = FixedGuid();
					Guid?[]? expected = null;

					async Task Act()
						=> await That(subject).IsOneOf(expected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expected").And
						.WithMessage("The 'expected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsContained_ShouldSucceed()
				{
					Guid? subject = FixedGuid();
					IEnumerable<Guid?> expected = [OtherGuid(), subject, OtherGuid(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsDifferent_ShouldFail()
				{
					Guid? subject = FixedGuid();
					Guid?[] expected = [OtherGuid(), OtherGuid(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is one of {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
				{
					Guid? subject = null;
					IEnumerable<Guid?> expected = [OtherGuid(), null,];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNullAndExpectedDoesNotContainNull_ShouldFail()
				{
					Guid? subject = null;
					IEnumerable<Guid?> expected = [OtherGuid(),];

					async Task Act()
						=> await That(subject).IsOneOf(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is one of expected,
						              but it was <null>

						              Expected values:
						              {Formatter.Format(expected)}
						              """);
				}
			}
		}
	}
}
