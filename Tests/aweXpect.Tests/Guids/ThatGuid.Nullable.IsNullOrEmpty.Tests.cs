namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed partial class Nullable
	{
		public sealed class IsNullOrEmpty
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectIsEmpty_ShouldSucceed()
				{
					Guid? subject = Guid.Empty;

					async Task Act()
						=> await That(subject).IsNullOrEmpty();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNotEmpty_ShouldFail()
				{
					Guid? subject = OtherGuid();

					async Task Act()
						=> await That(subject).IsNullOrEmpty();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is null or empty,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					Guid? subject = null;

					async Task Act()
						=> await That(subject).IsNullOrEmpty();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
