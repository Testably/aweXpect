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

		[Fact]
		public async Task Throws_Whose_WhenAsyncMemberFaults_ShouldFail()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.FaultedAsync(), v => v.IsEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose FaultedAsync() is equal to 1,
				             but FaultedAsync() did throw an InvalidOperationException:
				               async member failed for 1
				             """)
				.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed for 1"));
		}

		private sealed class AsyncException(int value) : Exception
		{
			public async Task<int> FaultedAsync()
			{
				await Task.Yield();
				throw new InvalidOperationException($"async member failed for {value}");
			}

			public Task<int> GetValueAsync() => Task.FromResult(value);
		}
	}
}
