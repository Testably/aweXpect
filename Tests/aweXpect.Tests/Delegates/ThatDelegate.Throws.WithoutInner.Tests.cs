namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithoutInner
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenInnerExceptionIsNotOfTheType_ShouldSucceed()
				{
					Action action = () => throw new OuterException(innerException: new OtherException());

					async Task Act()
						=> await That(action).Throws().WithoutInner<CustomException>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithoutInner<CustomException>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsOfADerivedType_ShouldFail()
				{
					Action action = () => throw new OuterException(innerException: new SubCustomException("inner"));

					async Task Act()
						=> await That(action).Throws().WithoutInner<CustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception without an inner ThatDelegate.CustomException,
						             but it had an inner ThatDelegate.SubCustomException:
						               inner
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsOfTheType_ShouldFail()
				{
					Action action = () => throw new OuterException(innerException: new CustomException("inner"));

					async Task Act()
						=> await That(action).Throws().WithoutInner<CustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception without an inner ThatDelegate.CustomException,
						             but it had an inner ThatDelegate.CustomException:
						               inner
						             """);
				}

				[Fact]
				public async Task WhenNoExceptionIsThrown_ShouldFail()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).Throws().WithoutInner<CustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception without an inner ThatDelegate.CustomException,
						             but it did not throw any exception
						             """);
				}
			}

			public sealed class Tests
			{
				[Fact]
				public async Task WhenAwaited_ShouldReturnThrownException()
				{
					Exception exception = new OuterException();
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithoutInner();

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenChainedWithOtherExpectations_ShouldApplyAll()
				{
					Action action = () => throw new ArgumentException("outer", "paramName");

					async Task Act()
						=> await That(action).Throws<ArgumentException>()
							.WithoutInner().And
							.WithParamName("otherParamName");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an ArgumentException without an inner exception and with param name equal to "otherParamName",
						             but it had param name "paramName", which differs at index 0:
						                ↓ (actual)
						               "paramName"
						               "otherParamName"
						                ↑ (expected)

						             Param name:
						             paramName
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithoutInner();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsSet_ShouldFail()
				{
					Action action = () => throw new OuterException(innerException: new OtherException("inner"));

					async Task Act()
						=> await That(action).Throws().WithoutInner();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception without an inner exception,
						             but it had an inner ThatDelegate.OtherException:
						               inner
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatch_WithThrowsType_ShouldOnlyReportTheType()
				{
					Action action = () => throw new OtherException("bar", new CustomException("inner"));

					async Task Act()
						=> await That(action).Throws<CustomException>().WithoutInner();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws a ThatDelegate.CustomException without an inner exception,
						             but it did throw a ThatDelegate.OtherException:
						               bar
						             """)
						.Because("the inner exception of an exception of another type is irrelevant");
				}
			}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
			public sealed class TypeTests
			{
				[Fact]
				public async Task WhenInnerExceptionIsNotOfTheType_ShouldSucceed()
				{
					Action action = () => throw new OuterException(innerException: new OtherException());

					async Task Act()
						=> await That(action).Throws().WithoutInner(typeof(CustomException));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithoutInner(typeof(CustomException));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsOfADerivedType_ShouldFail()
				{
					Action action = () => throw new OuterException(innerException: new SubCustomException("inner"));

					async Task Act()
						=> await That(action).Throws().WithoutInner(typeof(CustomException));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception without an inner ThatDelegate.CustomException,
						             but it had an inner ThatDelegate.SubCustomException:
						               inner
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsOfTheType_ShouldFail()
				{
					Action action = () => throw new OuterException(innerException: new CustomException("inner"));

					async Task Act()
						=> await That(action).Throws().WithoutInner(typeof(CustomException));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception without an inner ThatDelegate.CustomException,
						             but it had an inner ThatDelegate.CustomException:
						               inner
						             """);
				}
			}
#pragma warning restore CA2263
		}
	}
}
