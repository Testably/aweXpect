namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public sealed class DoesNotHaveInner
	{
		public sealed class GenericTests
		{
			[Fact]
			public async Task WhenInnerExceptionIsNotOfTheType_ShouldSucceed()
			{
				Exception subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionIsOfTheType_ShouldFail()
			{
				Exception subject = new("outer", new CustomException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.CustomException,
					             but it had an inner ThatException.CustomException:
					               inner
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveInner<CustomException>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.CustomException,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenInnerExceptionIsNotSet_ShouldFail()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveInner());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an inner exception,
					             but it had no inner exception
					             """);
			}

			[Fact]
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
			[Fact]
			public async Task WhenChainedAfterExpectationsOnTheSubjectType_ShouldApplyAll()
			{
				ArgumentException subject = new("outer", "paramName");

				async Task Act()
					=> await That(subject).HasMessage().StartingWith("outer")
						.And.HasParamName("paramName")
						.And.DoesNotHaveInner();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotHaveInner();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionIsSet_ShouldFail()
			{
				Exception subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner exception,
					             but it had an inner InvalidOperationException:
					               inner
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveInner();

				await That(Act).Throws<XunitException>()
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
			[Fact]
			public async Task WhenInnerExceptionIsNotOfTheType_ShouldSucceed()
			{
				Exception subject = new("outer", new InvalidOperationException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionIsNotSet_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(CustomException));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionIsOfADerivedType_ShouldFail()
			{
				Exception subject = new("outer", new TaskCanceledException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(OperationCanceledException));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner OperationCanceledException,
					             but it had an inner TaskCanceledException:
					               inner
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionIsOfTheType_ShouldFail()
			{
				Exception subject = new("outer", new CustomException("inner"));

				async Task Act()
					=> await That(subject).DoesNotHaveInner(typeof(CustomException));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have an inner ThatException.CustomException,
					             but it had an inner ThatException.CustomException:
					               inner
					             """);
			}
		}
#pragma warning restore CA2263
	}
}
