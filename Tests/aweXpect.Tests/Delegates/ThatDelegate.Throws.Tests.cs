namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAwaited_ShouldReturnThrownException()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				Exception result = await That(action).Throws();

				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenChained_ShouldOnlyDisplayInformationAboutNotThrownException()
			{
				Action action = () => { };

				async Task<Exception> Act()
					=> await That(action).Throws().WithMessage("foo");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws an exception with message equal to "foo",
					             but it did not throw any exception
					             """);
			}

			[Fact]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<Task> @delegate = () => null!;

				async Task<Exception> Act()
					=> await That(@delegate).Throws();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             throws an exception,
					             but it returned <null> instead of a task
					             """).And
					.Whose(e => e.InnerException, i => i.IsNull())
					.Because("a null task is not an exception thrown by the delegate");
			}

			[Fact]
			public async Task WhenExceptionIsThrown_ShouldSucceed()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).Throws();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_ShouldFail()
			{
				Action action = () => { };

				async Task<Exception> Act()
					=> await That(action).Throws().Because("it should throw");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws an exception, because it should throw,
					             but it did not throw any exception
					             """);
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_WithOr_ShouldFail()
			{
				Action action = () => { };

				async Task<Exception> Act()
					=> await That(action).Throws().WithMessage("x").Or.WithMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws an exception with message equal to "x" or with message equal to "y",
					             but it did not throw any exception
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).Throws();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             throws an exception,
					             but it was <null>
					             """);
			}
		}

		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldSupportChainedConstraints()
			{
				Action action = () => { };

				async Task Act()
					=> await That(action).Throws<ArgumentException>().WithMessage("foo");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws an ArgumentException with message equal to "foo",
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnThrownException(string value)
			{
				Exception exception = new CustomException
				{
					Value = value,
				};
				Action action = () => throw exception;

				CustomException result =
					await That(action).Throws<CustomException>();

				await That(result.Value).IsEqualTo(value);
				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenDelegateReturnsNullTask_ShouldFail()
			{
				Func<System.Threading.CancellationToken, Task<int>> @delegate = _ => null!;

				async Task<NullReferenceException> Act()
					=> await That(@delegate).Throws<NullReferenceException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that @delegate
					             throws a NullReferenceException,
					             but it returned <null> instead of a task
					             """)
					.Because("a null task is not an exception thrown by the delegate");
			}

			[Fact]
			public async Task WhenExactExceptionTypeIsThrown_ShouldSucceed()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).Throws<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_ShouldFail()
			{
				Action action = () => { };

				async Task<Exception> Act()
					=> await That(action).Throws<Exception>();

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws an exception,
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenOtherExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new OtherException(message);
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).Throws<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenOtherExceptionIsThrown_ShouldForwardExceptionAsInnerException(string message)
			{
				Exception exception = new OtherException(message);
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).Throws<CustomException>();

				await That(Act).Throws()
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenOtherExceptionIsThrown_WithOr_ShouldFail()
			{
				Exception exception = new OtherException("y");
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).Throws<CustomException>().WithMessage("x").Or.WithMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws a ThatDelegate.CustomException with message equal to "x" or with message equal to "y",
					             but it did throw a ThatDelegate.OtherException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenOtherExceptionIsThrown_WithWhichAndOr_ShouldFail()
			{
				Exception exception = new OtherException("y");
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).Throws<CustomException>().Which.HasMessage("x").Or.HasMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws a ThatDelegate.CustomException that has message equal to "x" or has message equal to "y",
					             but it did throw a ThatDelegate.OtherException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_ShouldSucceed()
			{
				Exception exception = new SubCustomException();
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).Throws<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).Throws<CustomException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             throws a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSupertypeExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Action action = () => throw exception;

				async Task<SubCustomException> Act()
					=> await That(action).Throws<SubCustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws a ThatDelegate.SubCustomException,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class TypeTests
		{
			[Fact]
			public async Task ShouldSupportChainedConstraints()
			{
				Action action = () => { };

				async Task Act()
					=> await That(action).Throws(typeof(ArgumentException)).WithMessage("foo");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws an ArgumentException with message equal to "foo",
					             but it did not throw any exception
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenAwaited_ShouldReturnThrownException(string value)
			{
				Exception exception = new CustomException
				{
					Value = value,
				};
				Action action = () => throw exception;

				Exception result =
					await That(action).Throws(typeof(CustomException));

				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenExactExceptionTypeIsThrown_ShouldSucceed()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).Throws(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_ShouldFail()
			{
				Action action = () => { };

				async Task<Exception> Act()
					=> await That(action).Throws(typeof(Exception));

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws an exception,
					             but it did not throw any exception
					             """);
			}

			[Fact]
			public async Task WhenOpenGenericTypeDoesNotMatch_ShouldFail()
			{
				Action action = () => throw new OtherException("foo");

				async Task Act()
					=> await That(action).Throws(typeof(GenericException<>));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws a ThatDelegate.GenericException<>,
					             but it did throw a ThatDelegate.OtherException:
					               foo
					             """);
			}

			[Fact]
			public async Task WhenOpenGenericTypeIsABaseType_ShouldSucceed()
			{
				Action action = () => throw new SubGenericException();

				async Task Act()
					=> await That(action).Throws(typeof(GenericException<>));

				await That(Act).DoesNotThrow()
					.Because("the base type GenericException<int> is constructed from the open generic type");
			}

			[Fact]
			public async Task WhenOpenGenericTypeIsTheDefinition_ShouldSucceed()
			{
				Action action = () => throw new GenericException<int>();

				async Task Act()
					=> await That(action).Throws(typeof(GenericException<>));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenOtherExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new OtherException(message);
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).Throws(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WhenOtherExceptionIsThrown_WithOr_ShouldFail()
			{
				Exception exception = new OtherException("y");
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).Throws(typeof(CustomException)).WithMessage("x").Or.WithMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws a ThatDelegate.CustomException with message equal to "x" or with message equal to "y",
					             but it did throw a ThatDelegate.OtherException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenOtherExceptionIsThrown_WithWhichAndOr_ShouldFail()
			{
				Exception exception = new OtherException("y");
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).Throws(typeof(CustomException)).Which.HasMessage("x").Or.HasMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws a ThatDelegate.CustomException that has message equal to "x" or has message equal to "y",
					             but it did throw a ThatDelegate.OtherException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_ShouldSucceed()
			{
				Exception exception = new SubCustomException();
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).Throws(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).Throws(typeof(CustomException));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             throws a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSupertypeExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new CustomException(message);
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).Throws(typeof(SubCustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws a ThatDelegate.SubCustomException,
					              but it did throw a ThatDelegate.CustomException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNotAnException_ShouldThrowArgumentException()
			{
				Action action = () => throw new CustomException();

				async Task Act()
					=> await That(action).Throws(typeof(string));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("type").And
					.WithMessage("The 'type' must be an exception type, but string is not.").AsPrefix()
					.Because("no exception could ever be a string");
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				Action action = () => throw new CustomException();

				async Task Act()
					=> await That(action).Throws((Type)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}
		}
#pragma warning restore CA2263
	}
}
