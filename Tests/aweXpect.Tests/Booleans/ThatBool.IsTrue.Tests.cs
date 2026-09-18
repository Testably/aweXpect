namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed class IsTrue
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFalse_ShouldFail()
			{
				bool subject = false;

				async Task Act()
					=> await That(subject).IsTrue();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is True,
					             but it was False
					             """);
			}

			[Fact]
			public async Task WhenFalse_ShouldFailWithDescriptiveMessage()
			{
				bool subject = false;

				async Task Act()
					=> await That(subject).IsTrue().Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is True, because we want to test the failure,
					             but it was False
					             """);
			}

			[Fact]
			public async Task WhenTaskFails_ShouldFailWithExceptionMessage()
			{
				Task<bool> subject = Task.FromException<bool>(
					new NotSupportedException("When Task throws an exception"));

				async Task Act()
					=> await That(subject).IsTrue().Because("the exception should be logged");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is True, because the exception should be logged,
					             but it did throw a NotSupportedException:
					               When Task throws an exception
					             """);
			}

			[Fact]
			public async Task WhenTrue_ShouldSucceed()
			{
				bool subject = true;

				async Task Act()
					=> await That(subject).IsTrue();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
