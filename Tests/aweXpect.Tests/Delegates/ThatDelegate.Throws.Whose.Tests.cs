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
						              throws an exception whose .HResult is equal to {hResult} and with HResult {otherHResult},
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
						              throws an exception whose .HResult is equal to {expectedHResult} and with HResult {hResult},
						              but .HResult was {hResult} which differs by -1
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
						              throws a HResultException whose .HResult is equal to {hResult} and with HResult {otherHResult},
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
						              throws a HResultException whose .HResult is equal to {expectedHResult} and with HResult {hResult},
						              but .HResult was {hResult} which differs by -1
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
			}

			private sealed class MyException(Base payload) : Exception
			{
				public Base Payload { get; } = payload;
			}
		}
	}
}
