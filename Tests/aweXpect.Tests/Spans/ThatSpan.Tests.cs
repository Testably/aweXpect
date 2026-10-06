#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatSpan
{
	public sealed class Tests
	{
		[Test]
		public async Task ShouldSupportIsEmpty()
		{
			int[] subject = new[]
			{
				1, 2, 3,
			};

			async Task Act()
				=> await That(subject.AsSpan()).IsEmpty();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject.AsSpan()
				             is empty,
				             but it was [
				               1,
				               2,
				               3
				             ]
				             """);
		}

		[Test]
		public async Task ShouldSupportIsInAscendingOrder()
		{
			async Task Act()
				=> await That(new[]
				{
					1, 2, 3,
				}.AsSpan()).IsInAscendingOrder();

			await That(Act).DoesNotThrow();
		}
	}
}
#endif
