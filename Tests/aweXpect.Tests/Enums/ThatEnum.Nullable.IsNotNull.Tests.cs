namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed partial class Nullable
	{
		public sealed class IsNotNull
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData(MyColors.Blue)]
				[InlineData((MyColors)42)]
				public async Task WhenSubjectIsNotNull_ShouldSucceed(MyColors? subject)
				{
					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					MyColors? subject = null;

					async Task Act()
						=> await That(subject).IsNotNull();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not null,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
