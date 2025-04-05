namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsNotStatic_ShouldFail()
			{
				Type subject = typeof(MyInstanceType);

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is static,
					             but it was not
					             """);
			}

			[Fact]
			public async Task WhenTypeIsStatic_ShouldSucceed()
			{
				Type subject = typeof(MyStaticType);

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).DoesNotThrow();
			}
		}

		private static class MyStaticType;

		private class MyInstanceType;
	}
}
