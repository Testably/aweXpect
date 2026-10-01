namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ThrowsExactly
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task ShouldSupportChainedConstraints()
			{
				Action action = () => { };

				async Task Act()
					=> await That(action).ThrowsExactly<Exception>().WithMessage("foo");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly an Exception with message equal to "foo",
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
					await That(action).ThrowsExactly<CustomException>();

				await That(result.Value).IsEqualTo(value);
				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenCorrectExceptionTypeIsThrown_ShouldSucceed()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_ShouldFail()
			{
				Action action = () => { };

				async Task<CustomException> Act()
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException,
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
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubCustomExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Action action = () => throw exception;

				async Task<CustomException> Act()
					=> await That(action).ThrowsExactly<CustomException>();

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_WithOr_ShouldFail()
			{
				Exception exception = new SubCustomException("y");
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).ThrowsExactly<CustomException>().WithMessage("x").Or.WithMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException with message equal to "x" or with message equal to "y",
					             but it did throw a ThatDelegate.SubCustomException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_WithOrWithInner_ShouldFail()
			{
				Exception exception = new SubCustomException("y", new OtherException("z"));
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).ThrowsExactly<CustomException>().WithMessage("x").Or.WithInner<OtherException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException with message equal to "x" or with an inner ThatDelegate.OtherException,
					             but it did throw a ThatDelegate.SubCustomException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_WithWhichAndOr_ShouldFail()
			{
				Exception exception = new SubCustomException("y");
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).ThrowsExactly<CustomException>().Which.HasMessage("x").Or.HasMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException that has message equal to "x" or has message equal to "y",
					             but it did throw a ThatDelegate.SubCustomException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).ThrowsExactly<CustomException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             throws exactly a ThatDelegate.CustomException,
					             but it was <null>
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
					=> await That(action).ThrowsExactly(typeof(Exception)).WithMessage("foo");

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly an Exception with message equal to "foo",
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
					await That(action).ThrowsExactly(typeof(CustomException));

				await That(result).IsSameAs(exception);
			}

			[Fact]
			public async Task WhenCorrectExceptionTypeIsThrown_ShouldSucceed()
			{
				Exception exception = new CustomException();
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoExceptionIsThrown_ShouldFail()
			{
				Action action = () => { };

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException,
					             but it did not throw any exception
					             """);
			}

			[Fact]
			public async Task WhenOpenGenericTypeIsABaseType_ShouldFail()
			{
				Action action = () => throw new SubGenericException("foo");

				async Task Act()
					=> await That(action).ThrowsExactly(typeof(GenericException<>));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.GenericException<>,
					             but it did throw a ThatDelegate.SubGenericException:
					               foo
					             """)
					.Because("only an exception constructed directly from the open generic type is exactly of that type");
			}

			[Fact]
			public async Task WhenOpenGenericTypeIsTheDefinition_ShouldSucceed()
			{
				Action action = () => throw new GenericException<int>();

				async Task Act()
					=> await That(action).ThrowsExactly(typeof(GenericException<>));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenOtherExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new OtherException(message);
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.OtherException:
					                {message}
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenSubCustomExceptionIsThrown_ShouldFail(string message)
			{
				Exception exception = new SubCustomException(message);
				Action action = () => throw exception;

				async Task<Exception> Act()
					=> await That(action).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that action
					              throws exactly a ThatDelegate.CustomException,
					              but it did throw a ThatDelegate.SubCustomException:
					                {message}
					              """);
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_WithOr_ShouldFail()
			{
				Exception exception = new SubCustomException("y");
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).ThrowsExactly(typeof(CustomException)).WithMessage("x").Or.WithMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException with message equal to "x" or with message equal to "y",
					             but it did throw a ThatDelegate.SubCustomException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_WithOrWithInner_ShouldFail()
			{
				Exception exception = new SubCustomException("y", new OtherException("z"));
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).ThrowsExactly(typeof(CustomException)).WithMessage("x").Or.WithInner<OtherException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException with message equal to "x" or with an inner ThatDelegate.OtherException,
					             but it did throw a ThatDelegate.SubCustomException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubCustomExceptionIsThrown_WithWhichAndOr_ShouldFail()
			{
				Exception exception = new SubCustomException("y");
				Action action = () => throw exception;

				async Task Act()
					=> await That(action).ThrowsExactly(typeof(CustomException)).Which.HasMessage("x").Or.HasMessage("y");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that action
					             throws exactly a ThatDelegate.CustomException that has message equal to "x" or has message equal to "y",
					             but it did throw a ThatDelegate.SubCustomException:
					               y
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Action? subject = null;

				async Task Act()
					=> await That(subject!).ThrowsExactly(typeof(CustomException));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             throws exactly a ThatDelegate.CustomException,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsNotAnException_ShouldThrowArgumentException()
			{
				Action action = () => throw new CustomException();

				async Task Act()
					=> await That(action).ThrowsExactly(typeof(string));

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
					=> await That(action).ThrowsExactly((Type)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}
		}
#pragma warning restore CA2263
	}
}
