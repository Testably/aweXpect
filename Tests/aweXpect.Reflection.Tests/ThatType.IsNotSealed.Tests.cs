namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsNotSealed_ShouldSucceed()
			{
				Type subject = typeof(MyInstanceType);

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsSealed_ShouldFail()
			{
				Type subject = typeof(MySealedType);

				async Task Act()
					=> await That(subject).IsNotSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was sealed MySealedType
					             """);
			}
		}

		private sealed class MySealedType;

		private class MyInstanceType;
	}
}
