namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsNull
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenNull_ShouldSucceed()
				{
					bool? subject = null;

					async Task Act()
						=> await That(subject).IsNull();

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenTrueOrFalse_ShouldFail(bool? subject)
				{
					async Task Act()
						=> await That(subject).IsNull().Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is null, because we want to test the failure,
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}
		}
	}
}
