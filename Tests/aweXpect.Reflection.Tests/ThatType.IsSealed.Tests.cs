namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsNotSealed_ShouldFail()
			{
				Type subject = typeof(MyInstanceType);

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was non-sealed MyInstanceType
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsSealed_ShouldSucceed()
			{
				Type subject = typeof(MySealedType);

				async Task Act()
					=> await That(subject).IsSealed();

				await That(Act).DoesNotThrow();
			}
		}

		private sealed class MySealedType;

		private class MyInstanceType;
	}
}
