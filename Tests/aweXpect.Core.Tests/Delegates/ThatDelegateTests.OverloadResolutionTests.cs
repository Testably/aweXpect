using System.Threading;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class OverloadResolutionTests
	{
		[Fact]
		public async Task AsyncBlockLambda_DoesNotThrow_ShouldAwaitTheDelegate()
		{
			Sut sut = new();

			await That(async () => { await sut.RunAsync(); }).DoesNotThrow();

			await That(sut.IsCompleted).IsTrue()
				.Because("the async lambda must be awaited");
		}

		[Fact]
		public async Task AsyncBlockLambda_Throws_ShouldAwaitTheDelegate()
		{
			Sut sut = new(new MyException("async block lambda"));

			await That(async () => { await sut.RunAsync(); }).Throws<MyException>()
				.WithMessage("async block lambda");
		}

		[Fact]
		public async Task AsyncLambda_DoesNotThrow_ShouldAwaitTheDelegate()
		{
			Sut sut = new();

			await That(async () => await sut.RunAsync()).DoesNotThrow();

			await That(sut.IsCompleted).IsTrue()
				.Because("the async lambda must be awaited");
		}

		[Fact]
		public async Task AsyncLambda_Throws_ShouldAwaitTheDelegate()
		{
			Sut sut = new(new MyException("async lambda"));

			await That(async () => await sut.RunAsync()).Throws<MyException>()
				.WithMessage("async lambda");
		}

		[Fact]
		public async Task AsyncLambdaWithCancellationToken_DoesNotThrow_ShouldAwaitTheDelegate()
		{
			Sut sut = new();

			await That(async ct => await sut.RunAsync(ct)).DoesNotThrow();

			await That(sut.IsCompleted).IsTrue()
				.Because("the async lambda must be awaited");
		}

		[Fact]
		public async Task AsyncLambdaWithCancellationToken_Throws_ShouldAwaitTheDelegate()
		{
			Sut sut = new(new MyException("async lambda with cancellation token"));

			await That(async ct => await sut.RunAsync(ct)).Throws<MyException>()
				.WithMessage("async lambda with cancellation token");
		}

		[Fact]
		public async Task AsyncLambdaWithCancellationTokenAndValue_DoesNotThrow_ShouldAwaitTheDelegate()
		{
			Sut sut = new();

			int result = await That(async ct => await sut.GetAsync(ct)).DoesNotThrow();

			await That(result).IsEqualTo(42)
				.Because("the async lambda must be awaited and its result must become the result");
		}

		[Fact]
		public async Task AsyncLambdaWithCancellationTokenAndValue_Throws_ShouldAwaitTheDelegate()
		{
			Sut sut = new(new MyException("async lambda with cancellation token and value"));

			await That(async ct => await sut.GetAsync(ct)).Throws<MyException>()
				.WithMessage("async lambda with cancellation token and value");
		}

		[Fact]
		public async Task AsyncLambdaWithValue_DoesNotThrow_ShouldAwaitTheDelegate()
		{
			Sut sut = new();

			int result = await That(async () => await sut.GetAsync()).DoesNotThrow();

			await That(result).IsEqualTo(42)
				.Because("the async lambda must be awaited and its result must become the result");
		}

		[Fact]
		public async Task AsyncLambdaWithValue_Throws_ShouldAwaitTheDelegate()
		{
			Sut sut = new(new MyException("async lambda with value"));

			await That(async () => await sut.GetAsync()).Throws<MyException>()
				.WithMessage("async lambda with value");
		}

		[Fact]
		public async Task ThrowLambda_DoesNotThrow_ShouldFail()
		{
			async Task Act()
				=> await That(() => throw new MyException("throw lambda")).DoesNotThrow();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => throw new MyException("throw lambda")
				             does not throw any exception,
				             but it did throw a MyException:
				               throw lambda
				             """);
		}

		[Fact]
		public async Task ThrowLambda_Throws_ShouldSucceed()
		{
			await That(() => throw new MyException("throw lambda")).Throws<MyException>()
				.WithMessage("throw lambda");
		}

		[Fact]
		public async Task ThrowLambdaWithCancellationToken_DoesNotThrow_ShouldFail()
		{
			async Task Act()
				=> await That(ct => throw new MyException("throw lambda with cancellation token")).DoesNotThrow();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that ct => throw new MyException("throw lambda with cancellation token")
				             does not throw any exception,
				             but it did throw a MyException:
				               throw lambda with cancellation token
				             """);
		}

		[Fact]
		public async Task ThrowLambdaWithCancellationToken_Throws_ShouldSucceed()
		{
			await That(ct => throw new MyException("throw lambda with cancellation token")).Throws<MyException>()
				.WithMessage("throw lambda with cancellation token");
		}

#if NET8_0_OR_GREATER
		[Fact]
		public async Task ValueTaskLambda_ShouldBeAwaited()
		{
			Sut sut = new();

			await That(() => sut.RunValueTaskAsync()).DoesNotThrow();

			await That(sut.IsCompleted).IsTrue()
				.Because("a lambda returning a ValueTask must still bind to the ValueTask overload");
		}
#endif

#if NET8_0_OR_GREATER
		[Fact]
		public async Task ValueTaskLambdaWithCancellationToken_ShouldBeAwaited()
		{
			Sut sut = new();

			await That(ct => new ValueTask(sut.RunAsync(ct))).DoesNotThrow();

			await That(sut.IsCompleted).IsTrue()
				.Because("a lambda returning a ValueTask must still bind to the ValueTask overload");
		}
#endif

#if NET8_0_OR_GREATER
		[Fact]
		public async Task ValueTaskLambdaWithCancellationTokenAndValue_ShouldBeAwaited()
		{
			Sut sut = new();

			int result = await That(ct => new ValueTask<int>(sut.GetAsync(ct))).DoesNotThrow();

			await That(result).IsEqualTo(42)
				.Because("a lambda returning a ValueTask<int> must still bind to the ValueTask<TValue> overload");
		}
#endif

#if NET8_0_OR_GREATER
		[Fact]
		public async Task ValueTaskLambdaWithValue_ShouldBeAwaited()
		{
			Sut sut = new();

			int result = await That(() => sut.GetValueTaskAsync()).DoesNotThrow();

			await That(result).IsEqualTo(42)
				.Because("a lambda returning a ValueTask<int> must still bind to the ValueTask<TValue> overload");
		}
#endif

#if NET8_0_OR_GREATER
		[Fact]
		public async Task ValueTaskMethodGroup_ShouldBeAwaited()
		{
			Sut sut = new();

			await That(sut.RunValueTaskAsync).DoesNotThrow();

			await That(sut.IsCompleted).IsTrue()
				.Because("a method group returning a ValueTask must still bind to the ValueTask overload");
		}
#endif

#if NET8_0_OR_GREATER
		[Fact]
		public async Task ValueTaskMethodGroupWithValue_ShouldBeAwaited()
		{
			Sut sut = new();

			int result = await That(sut.GetValueTaskAsync).DoesNotThrow();

			await That(result).IsEqualTo(42)
				.Because("a method group returning a ValueTask<int> must still bind to the ValueTask<TValue> overload");
		}
#endif

		private sealed class Sut(Exception? exception = null)
		{
			public bool IsCompleted { get; private set; }

			public async Task<int> GetAsync(CancellationToken cancellationToken = default)
			{
				await RunAsync(cancellationToken);
				return 42;
			}

			public async Task RunAsync(CancellationToken cancellationToken = default)
			{
				await Task.Delay(10, cancellationToken);
				if (exception is not null)
				{
					throw exception;
				}

				IsCompleted = true;
			}

#if NET8_0_OR_GREATER
			public async ValueTask<int> GetValueTaskAsync()
			{
				await RunAsync();
				return 42;
			}

			public async ValueTask RunValueTaskAsync()
				=> await RunAsync();
#endif
		}
	}
}
