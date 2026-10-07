using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed partial class Nullable
	{
		public sealed class IsNotOneOf
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
				{
					Guid? subject = FixedGuid();
					Guid[] unexpected = [];

					object Act()
						=> That(subject).IsNotOneOf(unexpected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
				{
					Guid? subject = FixedGuid();
					Guid[]? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenExpectedOnlyContainsNull_ShouldSucceed()
				{
					Guid? subject = FixedGuid();
					IEnumerable<Guid?> unexpected = [null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
				{
					Guid? subject = FixedGuid();
					Guid?[] unexpected = [];

					object Act()
						=> That(subject).IsNotOneOf(unexpected);

					await That(Act).Throws<ArgumentException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
						.Because("an empty set is rejected when the expectation is built, before it is awaited");
				}

				[Test]
				public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
				{
					Guid? subject = FixedGuid();
					Guid?[]? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("unexpected").And
						.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenParamsValuesContainTheSubject_ShouldFail()
				{
					Guid? subject = FixedGuid();
					Guid? other = OtherGuid();

					async Task Act()
						=> await That(subject).IsNotOneOf(other, subject);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of {Formatter.Format(new[] { other, subject, })},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsContained_ShouldFail()
				{
					Guid? subject = FixedGuid();
					IEnumerable<Guid?> unexpected = [OtherGuid(), subject, OtherGuid(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of unexpected,
						              but it was {Formatter.Format(subject)}

						              Unexpected values:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsDifferent_ShouldSucceed()
				{
					Guid? subject = FixedGuid();
					Guid?[] unexpected = [OtherGuid(), OtherGuid(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
				{
					Guid? subject = null;
					IEnumerable<Guid?> unexpected = [OtherGuid(), null,];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not one of unexpected,
						              but it was <null>

						              Unexpected values:
						              {Formatter.Format(unexpected)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNullAndUnexpectedDoesNotContainNull_ShouldSucceed()
				{
					Guid? subject = null;
					IEnumerable<Guid?> unexpected = [OtherGuid(),];

					async Task Act()
						=> await That(subject).IsNotOneOf(unexpected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
