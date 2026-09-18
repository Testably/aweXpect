namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsNotFalse
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenFalse_ShouldFail()
				{
					bool? subject = false;

					async Task Act()
						=> await That(subject).IsNotFalse().Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not False, because we want to test the failure,
						             but it was
						             """);
				}

				[Fact]
				public async Task WhenTaskFails_ShouldFailWithExceptionMessage()
				{
					Task<bool?> subject = Task.FromException<bool?>(
						new NotSupportedException("When Task throws an exception"));

					async Task Act()
						=> await That(subject).IsNotFalse().Because("the exception should be logged");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not False, because the exception should be logged,
						             but it did throw a NotSupportedException:
						               When Task throws an exception
						             """);
				}

				[Theory]
				[InlineData(true)]
				[InlineData(null)]
				public async Task WhenTrueOrNull_ShouldSucceed(bool? subject)
				{
					async Task Act()
						=> await That(subject).IsNotFalse();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
