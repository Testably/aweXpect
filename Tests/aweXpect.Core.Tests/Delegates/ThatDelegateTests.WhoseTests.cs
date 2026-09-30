namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WhoseTests
	{
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

		[Fact]
		public async Task Throws_Whose_WhenAsyncMemberReturnsNullTask_ShouldFail()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(e => e.GetNullTask(), v => v.IsEqualTo(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetNullTask() is equal to 1,
				             but GetNullTask() returned <null> instead of a task
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull())
				.Because("a null task is not an exception thrown by the member");
		}

		[Fact]
		public async Task Throws_Whose_WithAsyncLambda_ShouldRenderMemberPath()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(async e => await e.GetValueAsync(), v => v.IsEqualTo(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetValueAsync() is equal to 2,
				             but GetValueAsync() was 1, which differs by -1
				             """);
		}

		[Fact]
		public async Task Throws_Whose_WithAsyncLambda_WithConfigureAwait_ShouldRenderMemberPath()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(async e => await e.GetValueAsync().ConfigureAwait(false), v => v.IsEqualTo(2));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose GetValueAsync() is equal to 2,
				             but GetValueAsync() was 1, which differs by -1
				             """)
				.Because("ConfigureAwait does not select a member, so it is not part of the member path");
		}

		[Fact]
		public async Task Throws_Whose_WithAsyncLambda_WithoutMemberPath_ShouldRenderExpression()
		{
			void Delegate() => throw new AsyncException(1);

			async Task Act()
				=> await That(Delegate).Throws<AsyncException>()
					.Whose(async e => await e.GetValueAsync() + 1, v => v.IsEqualTo(3));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a ThatDelegateTests.WhoseTests.AsyncException whose async e => await e.GetValueAsync() + 1 is equal to 3,
				             but async e => await e.GetValueAsync() + 1 was 2, which differs by -1
				             """)
				.Because("an expression that is not a member path must not be shortened");
		}

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
				             but GetValueAsync() was 1, which differs by -1
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
			public async Task<int> FaultedAsync()
			{
				await Task.Yield();
				throw new InvalidOperationException($"async member failed for {value}");
			}

			public Task<int> GetNullTask() => null!;

			public Task<int> GetValueAsync() => Task.FromResult(value);
		}
	}
}
