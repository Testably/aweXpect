using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;
using WithoutValue = aweXpect.Delegates.ThatDelegate.WithoutValue;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
	public sealed class NegatedThrowsTests
	{
		[Fact]
		public async Task Throws_Generic_OnlyIfFalse_WhenDelegateDoesNotThrow_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws<MyException>().OnlyIf(false));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             throws an exception,
				             but it did not throw any exception
				             """);
		}

		[Fact]
		public async Task Throws_Generic_OnlyIfFalse_WhenDelegateThrows_ShouldSucceed()
		{
			DelegateValue value = new(new MyException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws<MyException>().OnlyIf(false));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Throws_Generic_WhenDelegateDoesNotThrow_ShouldSucceed()
		{
			DelegateValue value = new(null, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws<MyException>());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Throws_Generic_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			MyException exception = new("foo");
			DelegateValue value = new(exception, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws<MyException>());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw a MyException,
				             but it did throw a MyException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task Throws_Generic_WhenDelegateThrowsOtherException_ShouldSucceed()
		{
			DelegateValue value = new(new ArgumentException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws<MyException>());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Throws_Generic_Within_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			DelegateValue value = new(new MyException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it
					=> WithoutValue(it).Throws<MyException>().Within(TimeSpan.FromSeconds(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw a MyException within 0:01,
				             but it did throw a MyException:
				               foo
				             """);
		}

		[Fact]
		public async Task Throws_Generic_WithMessage_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			DelegateValue value = new(new MyException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws<MyException>().WithMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw a MyException with message equal to "foo",
				             but it did throw a MyException:
				               foo

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task Throws_Generic_WithMessage_WhenMessageDiffers_ShouldSucceed()
		{
			DelegateValue value = new(new MyException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws<MyException>().WithMessage("bar"));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Throws_Type_OnlyIfFalse_WhenDelegateDoesNotThrow_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it
					=> WithoutValue(it).Throws(typeof(MyException)).OnlyIf(false));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             throws an exception,
				             but it did not throw any exception
				             """);
		}

		[Fact]
		public async Task Throws_Type_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			MyException exception = new("foo");
			DelegateValue value = new(exception, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws(typeof(MyException)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw a MyException,
				             but it did throw a MyException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task Throws_Type_WhenDelegateThrowsOtherException_ShouldSucceed()
		{
			DelegateValue value = new(new ArgumentException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws(typeof(MyException)));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Throws_Type_Within_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			DelegateValue value = new(new MyException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it
					=> WithoutValue(it).Throws(typeof(MyException)).Within(TimeSpan.FromSeconds(1)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw a MyException within 0:01,
				             but it did throw a MyException:
				               foo
				             """);
		}

		[Fact]
		public async Task Throws_Type_WithMessage_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			DelegateValue value = new(new MyException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it
					=> WithoutValue(it).Throws(typeof(MyException)).WithMessage("foo"));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw a MyException with message equal to "foo",
				             but it did throw a MyException:
				               foo

				             Message:
				             foo
				             """);
		}

		[Fact]
		public async Task Throws_WhenDelegateThrows_ShouldFail()
		{
			MyException exception = new("foo");
			DelegateValue value = new(exception, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).Throws());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw any exception,
				             but it did throw a MyException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ThrowsExactly_Generic_OnlyIfFalse_WhenDelegateDoesNotThrow_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it
					=> WithoutValue(it).ThrowsExactly<MyException>().OnlyIf(false));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             throws an exception,
				             but it did not throw any exception
				             """);
		}

		[Fact]
		public async Task ThrowsExactly_Generic_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			MyException exception = new("foo");
			DelegateValue value = new(exception, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).ThrowsExactly<MyException>());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw exactly a MyException,
				             but it did throw a MyException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		[Fact]
		public async Task ThrowsExactly_Generic_WhenDelegateThrowsSubtype_ShouldSucceed()
		{
			DelegateValue value = new(new MyException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).ThrowsExactly<Exception>());

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task ThrowsExactly_Type_WhenDelegateThrowsMatchingException_ShouldFail()
		{
			MyException exception = new("foo");
			DelegateValue value = new(exception, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).ThrowsExactly(typeof(MyException)));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that value
				             does not throw exactly a MyException,
				             but it did throw a MyException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsSameAs(exception));
		}

		private static WithoutValue WithoutValue(IThat<DelegateValue> subject)
			=> new(((IExpectThat<DelegateValue>)subject).ExpectationBuilder);
	}
#pragma warning restore CA2263
}
