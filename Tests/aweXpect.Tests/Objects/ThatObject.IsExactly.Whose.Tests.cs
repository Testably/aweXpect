namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsExactly
	{
		public sealed class WhoseTests
		{
			[Fact]
			public async Task WhenPropertyDoesNotMatch_ShouldSucceed()
			{
				object subject = new MyClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).IsExactly<MyClass>()
						.Whose(x => x.Value, x => x.IsLessThan(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is exactly type ThatObject.MyClass whose Value is less than 42,
					             but Value was 42
					             """);
			}

			[Fact]
			public async Task WhenPropertyMatches_ShouldSucceed()
			{
				object subject = new MyClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).IsExactly<MyClass>().Whose(x => x.Value, x => x.IsEqualTo(42));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
