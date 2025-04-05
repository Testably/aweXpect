namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsGeneric_ShouldSucceed()
			{
				Type subject = typeof(MyGenericType<int>);

				async Task Act()
					=> await That(subject).IsGeneric();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotGeneric_ShouldFail()
			{
				Type subject = typeof(MyInstanceType);

				async Task Act()
					=> await That(subject).IsGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was non-generic MyInstanceType
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is generic,
					             but it was <null>
					             """);
			}
		}

		private class MyGenericType<T>;

		private class MyInstanceType;
	}
}
