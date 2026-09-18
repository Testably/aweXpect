namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WhoseTests
	{
		[Fact]
		public async Task Throws_Whose_WithAsyncMember_ShouldVerifyAwaitedValue()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetValueAsync() is equal to 2,
				             but GetValueAsync() was 1 which differs by -1
				             """);
		}

		[Fact]
		public async Task Throws_Whose_WithAsyncMember_WhenMatching_ShouldSucceed()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(1));

			await That(Act).DoesNotThrow();
		}

		private sealed class AsyncException(int value) : Exception
		{
			public Task<int> GetValueAsync() => Task.FromResult(value);
		}
	}
}
