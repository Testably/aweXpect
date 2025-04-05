namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotGeneric
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsGeneric_ShouldFail()
			{
				Type subject = typeof(MyGenericType<>);

				async Task Act()
					=> await That(subject).IsNotGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was generic MyGenericType<T>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotGeneric_ShouldSucceed()
			{
				Type subject = typeof(MyInstanceType);

				async Task Act()
					=> await That(subject).IsNotGeneric();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotGeneric();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not generic,
					             but it was <null>
					             """);
			}
		}

		private class MyGenericType<T>;

		private class MyInstanceType;
	}
}
