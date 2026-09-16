namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithHResult
		{
			public sealed class ContinuationTests
			{
				[Theory]
				[AutoData]
				public async Task ShouldContinueOnTheDelegateChain(int hResult)
				{
					Exception exception = new HResultException(hResult, "foo");
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithHResult().GreaterThan(hResult - 1).And.WithMessage("bar");

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception with HResult greater than {hResult - 1} and with Message equal to "bar",
						              but it was "foo" which differs at index 0:
						                 ↓ (actual)
						                "foo"
						                "bar"
						                 ↑ (expected)

						              Message:
						              foo
						              """);
				}

				[Theory]
				[AutoData]
				public async Task ShouldIncludeExceptionType(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<HResultException>().WithHResult().LessThan(hResult);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws a HResultException with HResult less than {hResult},
						              but it had HResult {hResult}
						              """);
				}

				[Theory]
				[AutoData]
				public async Task ShouldSupportTheComparisonVocabulary(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws().WithHResult().GreaterThan(hResult - 1);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[AutoData]
				public async Task WhenAwaited_ShouldReturnThrownException(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					Exception result = await That(Delegate).Throws().WithHResult().EqualTo(hResult);

					await That(result).IsSameAs(exception);
				}

				[Theory]
				[AutoData]
				public async Task WhenHResultIsDifferent_ShouldRenderLikeTheShorthand(int hResult)
				{
					int expectedHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws().WithHResult().EqualTo(expectedHResult);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception with HResult equal to {expectedHResult},
						              but it had HResult {hResult}
						              """)
						.Because("the continuation renders exactly like the WithHResult(expected) shorthand");
				}
			}

			public sealed class Tests
			{
				[Theory]
				[AutoData]
				public async Task WhenHResultIsDifferent_ShouldFail(int hResult)
				{
					int expectedHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws().WithHResult(expectedHResult);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception with HResult equal to {expectedHResult},
						              but it had HResult {hResult}
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenHResultMatchesExpected_ShouldSucceed(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws().WithHResult(hResult);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
