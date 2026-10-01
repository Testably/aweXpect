namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed partial class Nullable
	{
		public sealed class IsNotNull
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenNull_ShouldFail()
				{
					bool? subject = null;

					async Task Act()
						=> await That(subject).IsNotNull().Because("we want to test the failure");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not null, because we want to test the failure,
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData(true)]
				[InlineData(false)]
				public async Task WhenTrueOrFalse_ShouldSucceed(bool? subject)
				{
					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
