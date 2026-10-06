namespace aweXpect.Tests;

public sealed partial class ThatGuid
{
	public sealed partial class Nullable
	{
		public sealed class IsNotNull
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectIsEmpty_ShouldSucceed()
				{
					Guid? subject = Guid.Empty;

					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNotNull_ShouldSucceed()
				{
					Guid? subject = OtherGuid();

					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Guid? subject = null;

					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not null,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
