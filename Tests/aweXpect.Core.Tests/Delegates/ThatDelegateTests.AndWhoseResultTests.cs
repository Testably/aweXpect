using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class AndWhoseResultTests
	{
		[Fact]
		public async Task DoesNotThrow_WhenResultDiffers_ShouldIntroduceTheResult()
		{
			Func<int> @delegate = () => 4;

			async Task Act()
				=> await That(@delegate).DoesNotThrow().AndWhoseResult.IsEqualTo(5);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             does not throw any exception and its result is equal to 5,
				             but the result was 4 which differs by -1
				             """);
		}

		[Fact]
		public async Task DoesNotThrowOfType_WhenResultDiffers_ShouldIntroduceTheResult()
		{
			Func<int> @delegate = () => 4;

			async Task Act()
				=> await That(@delegate).DoesNotThrow<MyException>().AndWhoseResult.IsEqualTo(5);

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             does not throw a MyException and its result is equal to 5,
				             but the result was 4 which differs by -1
				             """);
		}
	}
}
