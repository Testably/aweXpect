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
					               (… and maybe more)
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
					             but it had no inner exceptions

					             Collection:
					             []
					             """)
					.Because("an expectation on the inner exceptions requires at least one of them");
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_ForAtMost_ShouldFail()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.AtMost(2).Satisfy(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions of which at most 2 satisfy _ => true,
					             but it had no inner exceptions

					             Collection:
					             []
					             """)
					.Because("the existence of an inner exception is required before any quantifier applies");
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_ForNone_ShouldFail()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.None().Satisfy(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions of which none satisfy _ => true,
					             but it had no inner exceptions

					             Collection:
					             []
					             """)
					.Because("the existence of an inner exception is required before any quantifier applies");
			}

			[Fact]
			public async Task WhenNoInnerExceptionIsPresent_WhenExpectingInnerExceptionsToBeEmpty_ShouldFail()
			{
				Action action = () => throw new OuterException();

				async Task Act()
					=> await That(action).Throws()
						.WithRecursiveInnerExceptions(innerExceptions => innerExceptions.IsEmpty());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions which are empty,
					             but it had no inner exceptions
					             """)
					.Because("the existence of an inner exception is required before the inner exceptions are inspected");
			}

			[Fact]
			public async Task WhenThrownAggregateExceptionHasNoInnerExceptions_ForAll_ShouldFail()
			{
				Action action = () => throw new AggregateException();

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.All().Satisfy(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions of which all satisfy _ => true,
					             but it had no inner exceptions

					             Collection:
					             []
					             """)
					.Because("an AggregateException without inner exceptions is empty just like any other exception");
			}

			[Fact]
			public async Task WhenThrownAggregateExceptionHasNoInnerExceptions_ForNone_ShouldFail()
			{
				Action action = () => throw new AggregateException();

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.None().Satisfy(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions of which none satisfy _ => true,
					             but it had no inner exceptions

					             Collection:
					             []
					             """)
					.Because("an AggregateException without inner exceptions is empty just like any other exception");
			}
		}
	}
}
