namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed partial class Nullable
	{
		public sealed class IsNotNullOrEmpty
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectIsEmpty_ShouldFail()
				{
					Guid? subject = Guid.Empty;

					async Task Act()
						=> await That(subject).IsNotNullOrEmpty();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not null or empty,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNotEmpty_ShouldSucceed()
				{
					Guid? subject = OtherGuid();

					async Task Act()
						=> await That(subject).IsNotNullOrEmpty();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Guid? subject = null;

					async Task Act()
						=> await That(subject).IsNotNullOrEmpty();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null or empty,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
