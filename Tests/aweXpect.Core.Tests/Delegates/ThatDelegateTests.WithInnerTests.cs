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
				=> await That(Delegate).Throws<MyException>().WithInner(_ => { });

			await That(Act).Throws<ArgumentException>()
				.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
				.And.WithParamName("expectations");
		}

		[Fact]
		public async Task InnerException_WithExpectations_WhenInnerExceptionIsMissing_ShouldFail()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws<MyException>().WithInner(e => e.IsNull());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner exception which is null,
				             but it had no inner exception
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

		[Fact]
		public async Task WithInner_Generic_AfterWhose_ShouldFail()
		{
			void Delegate() => throw new MyException("outer", new ArgumentException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.Whose(e => e.Message, m => m.IsEqualTo("outer"))
					.And.WithInner<MyException>(e => e.HasMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException whose Message is equal to "outer" and with an inner MyException whose Message is equal to "foo",
				             but it had an inner ArgumentException:
				               inner
				             """)
				.Because("a preceding Whose must not change how the inner exception is rendered");
		}

		[Fact]
		public async Task WithInner_Generic_WhenInnerExceptionHasWrongType_ShouldFail()
		{
			void Delegate() => throw new MyException("outer", new ArgumentException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner<MyException>(e => e.HasMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException whose Message is equal to "foo",
				             but it had an inner ArgumentException:
				               inner
				             """)
				.Because("the inner exception type is checked before the expectations on the inner exception");
		}

		[Fact]
		public async Task WithInner_Generic_WithMemberExpectation_ShouldUseWhose()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner<MyException>(e => e.HasMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException whose Message is equal to "foo",
				             but Message was "inner" which differs at index 0:
				                ↓ (actual)
				               "inner"
				               "foo"
				                ↑ (expected)

				             Message:
				             inner
				             """);
		}

		[Fact]
		public async Task WithInner_Generic_WithNegatedExpectation_ShouldOnlyNegateTheExpectation()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner<MyException>(e => e.DoesNotComplyWith(i => i.HasMessage("inner")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException whose Message is not equal to "inner",
				             but Message was "inner"

				             Message:
				             inner
				             """)
				.Because("negating the expectations on the inner exception keeps the inner exception type in its positive form");
		}

		[Fact]
		public async Task WithInner_Generic_WithOtherAndMemberExpectation_ShouldUseBothConnectors()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner<MyException>(e => e.Satisfies(i => i?.Message == "foo").And.HasMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException which satisfies i => i?.Message == "foo" and whose Message is equal to "foo",
				             but it was MyException: inner and Message was "inner" which differs at index 0:
				                ↓ (actual)
				               "inner"
				               "foo"
				                ↑ (expected)

				             Message:
				             inner
				             """);
		}

		[Fact]
		public async Task WithInner_Generic_WithOtherExpectation_ShouldUseWhich()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner<MyException>(e => e.Satisfies(i => i?.Message == "foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException which satisfies i => i?.Message == "foo",
				             but it was MyException: inner
				             """);
		}

		[Fact]
		public async Task WithInner_Generic_WithWhose_ShouldNotRepeatConnector()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner<MyException>(e => e.Whose(i => i?.Message, m => m.IsEqualTo("foo")));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException whose Message is equal to "foo",
				             but Message was "inner" which differs at index 0:
				                ↓ (actual)
				               "inner"
				               "foo"
				                ↑ (expected)
				             """);
		}

		[Fact]
		public async Task WithInner_Type_WithMemberExpectation_ShouldUseWhose()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner(typeof(MyException), e => e.HasMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException whose Message is equal to "foo",
				             but Message was "inner" which differs at index 0:
				                ↓ (actual)
				               "inner"
				               "foo"
				                ↑ (expected)

				             Message:
				             inner
				             """);
		}

		[Fact]
		public async Task WithInner_Type_WithOtherExpectation_ShouldUseWhich()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner(typeof(MyException), e => e.IsNull());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner MyException which is null,
				             but it was MyException: inner
				             """);
		}

		[Fact]
		public async Task WithInner_WithMemberExpectation_ShouldUseWhose()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner(e => e.HasMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner exception whose Message is equal to "foo",
				             but Message was "inner" which differs at index 0:
				                ↓ (actual)
				               "inner"
				               "foo"
				                ↑ (expected)

				             Message:
				             inner
				             """);
		}

		[Fact]
		public async Task WithInner_WithOtherExpectation_ShouldUseWhich()
		{
			void Delegate() => throw new MyException("outer", new MyException("inner"));

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.WithInner(e => e.Satisfies(i => i?.Message == "foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException with an inner exception which satisfies i => i?.Message == "foo",
				             but it was MyException: inner
				             """);
		}
	}
}
