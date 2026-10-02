namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithRecursiveInnerExceptionsTests
		{
			[Fact]
			public async Task WhenAnyInnerExceptionDoesMatch_ShouldSucceed()
			{
				Action action = () => throw new OuterException(
					innerException: new OtherException(
						innerException: new AggregateException(
							new OtherException(),
							new OtherException(
								innerException: new CustomException()))));

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.AtLeast(1).Are<CustomException>());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAwaited_WithExpectations_ShouldReturnThrownException()
			{
				Exception exception = new OuterException(innerException: new CustomException());
				void Delegate() => throw exception;

				Exception result = await That(Delegate)
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
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				Action action = () => throw new OuterException(innerException: new CustomException());

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
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
					             throws an exception with recursive inner exceptions that are empty,
					             but recursive inner exceptions were [
					               ThatDelegate.CustomException: WhenExpectingInnerExceptionsToBeEmpty_ShouldFail
					             ]
					             """);
			}

			[Fact]
			public async Task WhenFewerInnerExceptionsMatchThanRequired_ShouldFail()
			{
				Action action = () => throw new OuterException(
					innerException: new OtherException(
						innerException: new AggregateException(
							new OtherException(),
							new OtherException(
								innerException: new CustomException()))));

				async Task Act()
					=> await That(action).Throws().WithRecursiveInnerExceptions(e => e.AtLeast(2).Are<CustomException>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with recursive inner exceptions of which at least 2 are of type ThatDelegate.CustomException,
					             but only 1 of 5 were

					             Collection (recursive inner exceptions):
					             [
					               ThatDelegate.OtherException: WhenFewerInnerExceptionsMatchThanRequired_ShouldFail*,
					               AggregateException: *,
					               ThatDelegate.OtherException: WhenFewerInnerExceptionsMatchThanRequired_ShouldFail*,
					               ThatDelegate.OtherException: WhenFewerInnerExceptionsMatchThanRequired_ShouldFail*,
					               ThatDelegate.CustomException: WhenFewerInnerExceptionsMatchThanRequired_ShouldFail*
					             ]
					             """).AsWildcard();
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

					             Not matching items (recursive inner exceptions):
					             [
					               ThatDelegate.CustomException: WhenInnerExceptionDoesNotMatch_ShouldFail,
					               (… and maybe more)
					             ]

					             Collection (recursive inner exceptions):
					             [
					               ThatDelegate.CustomException: WhenInnerExceptionDoesNotMatch_ShouldFail,
					               (… and maybe more)
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
					             throws an exception with recursive inner exceptions that are empty,
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
					             """)
					.Because("an AggregateException without inner exceptions is empty just like any other exception");
			}
		}
	}
}
