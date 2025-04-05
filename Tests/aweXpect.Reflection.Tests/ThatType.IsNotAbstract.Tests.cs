namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAbstract_ShouldFail()
			{
				Type subject = typeof(MyAbstractType);

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was abstract MyAbstractType
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotAbstract_ShouldSucceed()
			{
				Type subject = typeof(MyInstanceType);

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was <null>
					             """);
			}
		}

		private abstract class MyAbstractType;

		private class MyInstanceType;
	}
}
