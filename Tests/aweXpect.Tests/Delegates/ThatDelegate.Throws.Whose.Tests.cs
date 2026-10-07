namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class Whose
		{
			public sealed class Tests
			{
				[Test]
				[AutoArguments]
				public async Task ShouldResetItAfterWhichClause(int hResult)
				{
					int otherHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult)).And
							.WithHResult(otherHResult);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception whose HResult is equal to {hResult} and with HResult equal to {otherHResult},
						              but it had HResult {hResult}
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenMemberIsDifferent_ShouldFail(int hResult)
				{
					int expectedHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Whose(e => e.HResult, h => h.IsEqualTo(expectedHResult)).And
							.WithHResult(hResult);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception whose HResult is equal to {expectedHResult} and with HResult equal to {hResult},
						              but HResult was {hResult}, which differs by -1
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenMemberMatchesExpected_ShouldSucceed(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeDoesNotMatch_WithThrowsType_ShouldOnlyReportTheType()
				{
					Action action = () => throw new OtherException("bar");

					async Task Act()
						=> await That(action).Throws(typeof(CustomException))
							.Whose(e => e.HResult, h => h.IsEqualTo(42));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that action
						             throws a ThatDelegate.CustomException whose HResult is equal to 42,
						             but it did throw a ThatDelegate.OtherException:
						               bar
						             """)
						.Because("the members of an exception of another type are irrelevant");
				}

				[Test]
				[AutoArguments]
				public async Task WithNamedMemberAccessor_ShouldSucceed(int hResult)
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
				[Test]
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

				[Test]
				[AutoArguments]
				public async Task ShouldResetItAfterWhichClause(int hResult)
				{
					int otherHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<HResultException>()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult)).And
							.WithHResult(otherHResult);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an HResultException whose HResult is equal to {hResult} and with HResult equal to {otherHResult},
						              but it had HResult {hResult}
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenAsyncMemberIsDifferent_ShouldFail(int value)
				{
					int expectedValue = value + 1;
					void Delegate() => throw new AsyncException(value);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(expectedValue));

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws a ThatDelegate.Throws.Whose.AsyncException whose GetValueAsync() is equal to {expectedValue},
						              but GetValueAsync() was {value}, which differs by -1
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenAsyncMemberMatchesExpected_ShouldSucceed(int value)
				{
					void Delegate() => throw new AsyncException(value);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.GetValueAsync(), v => v.IsEqualTo(value));

					await That(Act).DoesNotThrow();
				}

				[Test]
				[AutoArguments]
				public async Task WhenMemberIsDifferent_ShouldFail(int hResult)
				{
					int expectedHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<HResultException>()
							.Whose(e => e.HResult, h => h.IsEqualTo(expectedHResult)).And
							.WithHResult(hResult);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an HResultException whose HResult is equal to {expectedHResult} and with HResult equal to {hResult},
						              but HResult was {hResult}, which differs by -1
						              """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenMemberMatchesExpected_ShouldSucceed(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<HResultException>()
							.Whose(e => e.HResult, h => h.IsEqualTo(hResult));

					await That(Act).DoesNotThrow();
				}

				[Test]
				[AutoArguments]
				public async Task WhenValueTaskMemberIsDifferent_ShouldFail(int value)
				{
					int expectedValue = value + 1;
					void Delegate() => throw new AsyncException(value);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.GetValueAsValueTaskAsync(), v => v.IsEqualTo(expectedValue));

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws a ThatDelegate.Throws.Whose.AsyncException whose GetValueAsValueTaskAsync() is equal to {expectedValue},
						              but GetValueAsValueTaskAsync() was {value}, which differs by -1
						              """);
				}

				[Test]
				public async Task WithInner_AllowsNestedIs()
				{
					void Throwing()
						=> throw new InvalidOperationException(
							"outer",
							new InvalidCastException("inner"));

					async Task Act()
						=> await That(Throwing).Throws<InvalidOperationException>()
							.WithInner(it => it.Is<InvalidCastException>()
								.Whose(e => e!.Message, it => it.IsEqualTo("inner")));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class AsyncMemberFaultTests
			{
				[Test]
				public async Task WhenAsyncMemberFaults_ShouldFail()
				{
					void Delegate() => throw new AsyncException(1);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.FaultedAsync(), v => v.IsEqualTo(1));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a ThatDelegate.Throws.Whose.AsyncException whose FaultedAsync() is equal to 1,
						             but FaultedAsync() did throw an InvalidOperationException:
						               async member failed
						             """)
						.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
				}

				[Test]
				public async Task WhenValueTaskMemberFaults_ShouldFail()
				{
					void Delegate() => throw new AsyncException(1);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.FaultedValueTaskAsync(), v => v.IsEqualTo(1));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a ThatDelegate.Throws.Whose.AsyncException whose FaultedValueTaskAsync() is equal to 1,
						             but FaultedValueTaskAsync() did throw an InvalidOperationException:
						               async member failed
						             """)
						.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
				}
			}

			public sealed class MemberExpectationThrowsOnDefaultTests
			{
				[Test]
				public async Task WhenAsyncMemberFaults_ShouldFail()
				{
					void Delegate() => throw new AsyncException(1);

					async Task Act()
						=> await That(Delegate).Throws<AsyncException>()
							.Whose(e => e.FaultedAsync(), v => v.Satisfies(x => 10 / x > 1));

					await That(Act).Throws<FailException>()
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
				public Task<int> GetValueAsync() => Task.FromResult(value);

				public ValueTask<int> GetValueAsValueTaskAsync() => new(value);
#pragma warning disable CA1822 // the tests access these members through the subject
				public async Task<int> FaultedAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}

				public async ValueTask<int> FaultedValueTaskAsync()
				{
					await Task.Yield();
					throw new InvalidOperationException("async member failed");
				}
#pragma warning restore CA1822
			}

			private sealed class MyException(Base payload) : Exception
			{
				public Base Payload { get; } = payload;
			}
		}
	}
}
