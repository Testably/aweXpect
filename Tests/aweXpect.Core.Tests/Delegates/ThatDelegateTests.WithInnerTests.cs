using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WithInnerTests
	{
		[Fact]
		public async Task Generic_WithEmptyExpectations_ShouldThrowArgumentException()
		{
			void Delegate() => throw new MyException(innerException: new ArgumentException());

			async Task Act()
				=> await That(Delegate).Throws<MyException>().WithInner<ArgumentException>(_ => { });

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
				.And.WithParamName("expectations");
		}

		[Fact]
		public async Task Generic_WithExpectations_WhenInnerExceptionMatches_ShouldSucceed()
		{
			void Delegate() => throw new MyException(innerException: new ArgumentException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>().WithInner<ArgumentException>(e => e.HasMessage("inner"));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task InnerException_WithEmptyExpectations_ShouldThrowArgumentException()
		{
			void Delegate() => throw new MyException(innerException: new ArgumentException());

			async Task Act()
				=> await That(Delegate).Throws<MyException>().WithInnerException(_ => { });

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
				.And.WithParamName("expectations");
		}

		[Fact]
		public async Task InnerException_WithExpectations_WhenInnerExceptionIsMissing_ShouldFail()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws<MyException>().WithInnerException(e => e.IsNull());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner exception whose is null,
				             but it was <null>
				             """);
		}

		[Fact]
		public async Task Type_WithConditionallyEmptyExpectations_ShouldThrowArgumentException()
		{
			bool checkMessage = false;
			void Delegate() => throw new MyException(innerException: new ArgumentException());

			async Task Act()
				=> await That(Delegate).Throws<MyException>().WithInner(typeof(ArgumentException), e =>
				{
					if (checkMessage)
					{
						e.HasMessage("inner");
					}
				});

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
				.And.WithParamName("expectations");
		}

		[Fact]
		public async Task Type_WithEmptyExpectations_ShouldThrowArgumentException()
		{
			void Delegate() => throw new MyException(innerException: new ArgumentException());

			async Task Act()
				=> await That(Delegate).Throws<MyException>().WithInner(typeof(ArgumentException), _ => { });

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
				.And.WithParamName("expectations");
		}

		[Fact]
		public async Task Whose_WithEmptyExpectations_ShouldThrowArgumentException()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws<MyException>().Whose(e => e.Message, _ => { });

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
				.And.WithParamName("expectations");
		}

		[Fact]
		public async Task WhichWhose_WithEmptyExpectations_ShouldThrowArgumentException()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws<MyException>().Which.Whose(e => e.Message, _ => { });

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
				.And.WithParamName("expectations");
		}
	}
}
