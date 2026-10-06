namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed partial class Nullable
	{
		public sealed class IsNull
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectIsEmpty_ShouldFail()
				{
					Guid? subject = Guid.Empty;

					async Task Act()
						=> await That(subject).IsNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is null,
						             but it was 00000000-0000-0000-0000-000000000000
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNotNull_ShouldFail()
				{
					Guid? subject = OtherGuid();

					async Task Act()
						=> await That(subject).IsNull();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is null,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldSucceed()
				{
					Guid? subject = null;

					async Task Act()
						=> await That(subject).IsNull();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
