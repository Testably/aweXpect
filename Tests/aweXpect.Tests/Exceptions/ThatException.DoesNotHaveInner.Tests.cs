namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed class DoesNotHaveInner
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenInnerExceptionIsNotOfTheType_ShouldSucceed()
			{
				Exception subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInnerExceptionIsOfTheType_ShouldFail()
			{
				Exception subject = new("outer", new CustomException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.CustomException,
					             but it had an inner ThatException.CustomException:
					               inner
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.CustomException,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsOfTheInnerExceptionType_ShouldCheckTheInnerExceptionType()
			{
				ArgumentException subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner<ArgumentException>();

				await That(Act).DoesNotThrow()
					.Because("the type argument names the inner exception type, even when the subject has the same type");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenInnerExceptionIsNotSet_ShouldFail()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveInner());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an inner exception,
					             but it had no inner exception
					             """);
			}

			[Test]
			public async Task WhenInnerExceptionIsSet_ShouldSucceed()
			{
				Exception subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveInner());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenChainedAfterExpectationsOnTheSubjectType_ShouldApplyAll()
			{
				ArgumentException subject = new("outer", "paramName");

				async Task Act()
					=> await That(subject).HasMessage().StartingWith("outer")
						.And.HasParamName("paramName")
						.And.DoesNotHaveInner();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotHaveInner();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInnerExceptionIsSet_ShouldFail()
			{
				Exception subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner exception,
					             but it had an inner InvalidOperationException:
					               inner
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveInner();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner exception,
					             but it was <null>
					             """);
			}
		}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
		public sealed class TypeTests
		{
			[Test]
			public async Task WhenAwaited_ShouldReturnTheSubject()
			{
				ArgumentException subject = new("outer", "paramName", new InvalidOperationException("inner"));

				ArgumentException result = await That(subject).DoesNotHaveInner(typeof(CustomException));

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenChainedWithHasParamName_ShouldApplyBoth()
			{
				ArgumentException subject = new("outer", "paramName", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(CustomException)).And.HasParamName("other");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.CustomException and has param name equal to "other",
					             but it had param name "paramName", which differs at index 0:
					                ↓ (actual)
					               "paramName"
					               "other"
					                ↑ (expected)

					             Param name:
					             paramName
					             """)
					.Because("the chain continues on the ArgumentException subject");
			}

			[Test]
			public async Task WhenInnerExceptionIsNotOfTheType_ShouldSucceed()
			{
				Exception subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInnerExceptionIsOfADerivedType_ShouldFail()
			{
				Exception subject = new("outer", new TaskCanceledException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(OperationCanceledException));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner OperationCanceledException,
					             but it had an inner TaskCanceledException:
					               inner
					             """);
			}

			[Test]
			public async Task WhenInnerExceptionIsOfAnOpenGenericType_ShouldFail()
			{
				Exception subject = new("outer", new GenericException<int>("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(GenericException<>));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.GenericException<>,
					             but it had an inner ThatException.GenericException<int>:
					               inner
					             """);
			}

			[Test]
			public async Task WhenInnerExceptionIsOfTheType_ShouldFail()
			{
				Exception subject = new("outer", new CustomException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(CustomException));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.CustomException,
					             but it had an inner ThatException.CustomException:
					               inner
					             """);
			}

			[Test]
			public async Task WhenTypeIsNotAnException_ShouldThrowArgumentException()
			{
				Exception subject = new("outer", new CustomException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(string));

				await That(Act).Throws<ArgumentException>()
					.WithParamName("type").And
					.WithMessage("The 'type' must be an exception type, but string is not.").AsPrefix()
					.Because("no exception could ever be a string");
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				Exception subject = new("outer", new CustomException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}
		}
#pragma warning restore CA2263
	}
}
