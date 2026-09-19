namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithRecursiveInnerExceptionsTests
		{
			[Theory]
			[InlineData(1, false)]
			[InlineData(2, true)]
			public async Task WhenAnyInnerExceptionDoesMatch_ShouldSucceed(int minimum,
				bool shouldThrow)
			{
				Action action = () => throw new OuterException(
					innerException: new OtherException(
						innerException: new AggregateException(
							new OtherException(),
							new OtherException(
								innerException: new CustomException()))));

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.AtLeast(minimum).Are<CustomException>());

				await That(Act).Throws<XunitException>().OnlyIf(shouldThrow)
					.WithMessage($"""
					              Expected that action
					              throws an exception with recursive inner exceptions of which at least {minimum} are of type ThatDelegate.CustomException,
					              but only 1 of 5 were

					              Collection:
					              [
					                ThatDelegate.OtherException: WhenAnyInnerExceptionDoesMatch_ShouldSucceed*,
					                AggregateException: *,
					                ThatDelegate.OtherException: WhenAnyInnerExceptionDoesMatch_ShouldSucceed*,
					                ThatDelegate.OtherException: WhenAnyInnerExceptionDoesMatch_ShouldSucceed*,
					                ThatDelegate.CustomException: WhenAnyInnerExceptionDoesMatch_ShouldSucceed*
					              ]
					              """).AsWildcard();
			}

			[Fact]
			public async Task WhenAwaited_WithExpectations_ShouldReturnThrownException()
			{
				Exception exception = new OuterException(innerException: new CustomException());
				void Delegate() => throw exception;

				Exception? result = await That(Delegate)
					.Throws().WithRecursiveInnerExceptions(e => e.All().Satisfy(_ => true));

				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenExpectationsAreEmpty_ShouldThrowArgumentException()
			{
				Action action = () => throw new OuterException(innerException: new CustomException());

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(_ => { });

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
					.And.WithParamName("expectations");
			}

			[Fact]
			public async Task WhenExpectingInnerExceptionsToBeEmpty_ShouldFail()
			{
				Action action = () => throw new OuterException(innerException: new CustomException());

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(innerExceptions => innerExceptions.IsEmpty());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions which are empty,
					             but recursive inner exceptions were [
					               ThatDelegate.CustomException: WhenExpectingInnerExceptionsToBeEmpty_ShouldFail
					             ]
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionDoesNotMatch_ShouldFail()
			{
				Action action = () => throw new OuterException(innerException: new CustomException());

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.All().Satisfy(_ => false));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions of which all satisfy _ => false,
					             but none of at least 1 did

					             Not matching items:
					             [
					               ThatDelegate.CustomException: WhenInnerExceptionDoesNotMatch_ShouldFail,
					               (… and maybe others)
					             ]

					             Collection:
					             [
					               ThatDelegate.CustomException: WhenInnerExceptionDoesNotMatch_ShouldFail
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_ForAll_ShouldFail()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.All().Satisfy(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions of which all satisfy _ => true,
					             but none of 0 did

					             Collection:
					             []
					             """)
					.Because("an expectation on all inner exceptions requires at least one of them");
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_ForAtMost_ShouldSucceed()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.AtMost(2).Satisfy(_ => true));

				await That(Act).DoesNotThrow()
					.Because("at most 2 inner exceptions is what an exception without any inner exception has");
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_ForNone_ShouldSucceed()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.None().Satisfy(_ => true));

				await That(Act).DoesNotThrow()
					.Because("no inner exception matches when there is no inner exception");
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_WhenExpectingInnerExceptionsToBeEmpty_ShouldSucceed()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws()
						.WithRecursiveInnerExceptions(innerExceptions => innerExceptions.IsEmpty());

				await That(Act).DoesNotThrow()
					.Because("an expectation that is only about the absence of items stays satisfiable");
			}
		}
	}
}
