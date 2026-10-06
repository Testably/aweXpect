namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsFalse
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenFalse_ShouldSucceed()
				{
					bool? subject = false;

					async Task Act()
						=> await That(subject).IsFalse();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTaskFails_ShouldFailWithExceptionMessage()
				{
					Task<bool?> subject = Task.FromException<bool?>(
						new NotSupportedException("When Task throws an exception"));

					async Task Act()
						=> await That(subject).IsFalse().Because("the exception should be logged");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is False, because the exception should be logged,
						             but it did throw a NotSupportedException:
						               When Task throws an exception
						             """);
				}

				[Test]
				[Arguments(true)]
				[Arguments(null)]
				public async Task WhenTrueOrNull_ShouldFail(bool? subject)
				{
					async Task Act()
						=> await That(subject).IsFalse().Because("we want to test the failure");

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is False, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}
		}
	}
}
