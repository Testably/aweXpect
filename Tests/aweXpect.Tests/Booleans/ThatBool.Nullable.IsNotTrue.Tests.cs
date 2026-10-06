namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsNotTrue
		{
			public sealed class Tests
			{
				[Test]
				[Arguments(false)]
				[Arguments(null)]
				public async Task WhenFalseOrNull_ShouldSucceed(bool? subject)
				{
					async Task Act()
						=> await That(subject).IsNotTrue();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTaskFails_ShouldFailWithExceptionMessage()
				{
					Task<bool?> subject = Task.FromException<bool?>(
						new NotSupportedException("When Task throws an exception"));

					async Task Act()
						=> await That(subject).IsNotTrue().Because("the exception should be logged");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not True, because the exception should be logged,
						             but it did throw a NotSupportedException:
						               When Task throws an exception
						             """);
				}

				[Test]
				public async Task WhenTrue_ShouldFail()
				{
					bool? subject = true;

					async Task Act()
						=> await That(subject).IsNotTrue().Because("we want to test the failure");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not True, because we want to test the failure,
						             but it was True
						             """);
				}
			}
		}
	}
}
