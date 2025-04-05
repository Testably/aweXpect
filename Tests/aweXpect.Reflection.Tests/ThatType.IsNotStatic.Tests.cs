namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsStatic_ShouldFail()
			{
				Type subject = typeof(MyStaticType);

				async Task Act()
					=> await That(subject).IsNotStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not static,
					             but it was
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotStatic_ShouldSucceed()
			{
				Type subject = typeof(MyInstanceType);

				async Task Act()
					=> await That(subject).IsNotStatic();

				await That(Act).DoesNotThrow();
			}
		}

		private static class MyStaticType;

		private class MyInstanceType;
	}
}
