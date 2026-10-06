namespace aweXpect.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class ThatBool
{
	public sealed class Tests
	{
		[Test]
		public async Task WhenFalse_ShouldFail()
		{
			bool subject = false;

			async Task Act()
				=> await That(subject);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is True,
				             but it was False
				             """);
		}

		[Test]
		public async Task WhenFalse_ShouldFailWithDescriptiveMessage()
		{
			bool subject = false;

			async Task Act()
				=> await That(subject).Because("we want to test the failure");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is True, because we want to test the failure,
				             but it was False
				             """);
		}

		[Test]
		public async Task WhenTrue_ShouldSucceed()
		{
			bool subject = true;

			async Task Act()
				=> await That(subject);

			await That(Act).DoesNotThrow();
		}
	}
}
