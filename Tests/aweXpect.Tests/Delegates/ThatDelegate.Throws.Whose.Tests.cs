namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class Whose
		{
			public sealed class Tests
			{
				[Theory]
				[AutoData]
				public async Task ShouldResetItAfterWhichClause(int hResult)
				{
					int otherHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult)).And
							.WithHResult(otherHResult);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception whose HResult is equal to {hResult} and with HResult equal to {otherHResult},
						              but it had HResult {hResult}
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenMemberIsDifferent_ShouldFail(int hResult)
				{
					int expectedHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Whose(e => e.HResult, h => h.IsEqualTo(expectedHResult)).And
							.WithHResult(hResult);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception whose HResult is equal to {expectedHResult} and with HResult equal to {hResult},
						              but HResult was {hResult} which differs by -1
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenMemberMatchesExpected_ShouldSucceed(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GenericTests
			{
				[Fact]
				public async Task AllowsNestedIs()
				{
					void Throwing()
						=> throw new MyException(new Derived
						{
							Name = "foo",
						});

					async Task Act()
						=> await That(Throwing).Throws<MyException>()
							.Whose(e => e.Payload, it => it.Is<Derived>()
								.Whose(d => d.Name, it => it.IsEqualTo("foo")));

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[AutoData]
				public async Task ShouldResetItAfterWhichClause(int hResult)
				{
					int otherHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<HResultException>()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult)).And
							.WithHResult(otherHResult);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an HResultException whose HResult is equal to {hResult} and with HResult equal to {otherHResult},
						              but it had HResult {hResult}
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenMemberIsDifferent_ShouldFail(int hResult)
				{
					int expectedHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<HResultException>()
							.Whose(e => e.HResult, h => h.IsEqualTo(expectedHResult)).And
							.WithHResult(hResult);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an HResultException whose HResult is equal to {expectedHResult} and with HResult equal to {hResult},
						              but HResult was {hResult} which differs by -1
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenMemberMatchesExpected_ShouldSucceed(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<HResultException>()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WithInnerException_AllowsNestedIs()
				{
					void Throwing()
						=> throw new InvalidOperationException(
							"outer",
							new InvalidCastException("inner"));

					async Task Act()
						=> await That(Throwing).Throws<InvalidOperationException>()
							.WithInnerException(it => it.Is<InvalidCastException>()
								.Whose(e => e!.Message, it => it.IsEqualTo("inner")));

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[AutoData]
				public async Task WhenAsyncMemberIsDifferent_ShouldFail(int value)
				{
					int expectedValue = value + 1;
					void Delegate() => throw new AsyncException(value);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(expectedValue));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws a ThatDelegate.Throws.Whose.AsyncException whose GetValueAsync() is equal to {expectedValue},
						              but GetValueAsync() was {value} which differs by -1
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenAsyncMemberMatchesExpected_ShouldSucceed(int value)
				{
					void Delegate() => throw new AsyncException(value);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(value));

					await That(Act).DoesNotThrow();
				}

#if NET8_0_OR_GREATER
				[Theory]
				[AutoData]
				public async Task WhenValueTaskMemberIsDifferent_ShouldFail(int value)
				{
					int expectedValue = value + 1;
					void Delegate() => throw new AsyncException(value);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.GetValueAsValueTaskAsync(), v => v.IsEqualTo(expectedValue));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws a ThatDelegate.Throws.Whose.AsyncException whose GetValueAsValueTaskAsync() is equal to {expectedValue},
						              but GetValueAsValueTaskAsync() was {value} which differs by -1
						              """);
				}
#endif
			}

			public sealed class AsyncMemberFaultTests
			{
				[Fact]
				public async Task WhenAsyncMemberFaults_ShouldFail()
				{
					void Delegate() => throw new AsyncException(1);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.FaultedAsync(), v => v.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a ThatDelegate.Throws.Whose.AsyncException whose FaultedAsync() is equal to 1,
						             but FaultedAsync() did throw an InvalidOperationException:
						               async member failed
						             """)
						.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
				}

#if NET8_0_OR_GREATER
				[Fact]
				public async Task WhenValueTaskMemberFaults_ShouldFail()
				{
					void Delegate() => throw new AsyncException(1);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.FaultedValueTaskAsync(), v => v.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a ThatDelegate.Throws.Whose.AsyncException whose FaultedValueTaskAsync() is equal to 1,
						             but FaultedValueTaskAsync() did throw an InvalidOperationException:
						               async member failed
						             """)
						.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
				}
#endif
			}

			public sealed class MemberExpectationThrowsOnDefaultTests
			{
				[Fact]
				public async Task WhenAsyncMemberFaults_ShouldFail()
				{
					void Delegate() => throw new AsyncException(1);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.FaultedAsync(), v => v.Satisfies(x => 10 / x > 1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a ThatDelegate.Throws.Whose.AsyncException whose FaultedAsync() satisfies x => 10 / x > 1,
						             but FaultedAsync() did throw an InvalidOperationException:
						               async member failed
						             """)
						.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
				}
			}

			private sealed class AsyncException(int value) : Exception
			{
				public async Task<int> FaultedAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}

#if NET8_0_OR_GREATER
				public async ValueTask<int> FaultedValueTaskAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}
#endif

				public Task<int> GetValueAsync() => Task.FromResult(value);

#if NET8_0_OR_GREATER
				public ValueTask<int> GetValueAsValueTaskAsync() => new(value);
#endif
			}

			private sealed class MyException(Base payload) : Exception
			{
				public Base Payload { get; } = payload;
			}
		}
	}
}
