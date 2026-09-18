using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class ParameterNameTests
	{
		[Fact]
		public async Task DoesNotThrow_WithValue_ShouldAcceptTypeAsNamedArgument()
		{
			Func<int> @delegate = () => 1;

			async Task Act()
				=> await That(@delegate).DoesNotThrow(type: typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task DoesNotThrow_WithoutValue_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).DoesNotThrow(type: typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task DoesNotThrowExactly_WithValue_ShouldAcceptTypeAsNamedArgument()
		{
			Func<int> @delegate = () => 1;

			async Task Act()
				=> await That(@delegate).DoesNotThrowExactly(type: typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task DoesNotThrowExactly_WithoutValue_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).DoesNotThrowExactly(type: typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task OnlyIf_ShouldAcceptConditionAsNamedArgument()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).Throws<MyException>().OnlyIf(condition: false);

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task Throws_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException();

			async Task Act()
				=> await That(@delegate).Throws(type: typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task ThrowsExactly_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException();

			async Task Act()
				=> await That(@delegate).ThrowsExactly(type: typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WithInner_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException(innerException: new MyException());

			async Task Act()
				=> await That(@delegate).Throws<MyException>().WithInner(type: typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Fact]
		public async Task WithInner_WithExpectations_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException(innerException: new MyException());

			async Task Act()
				=> await That(@delegate).Throws<MyException>()
					.WithInner(type: typeof(MyException), expectations: it => it.Is<MyException>());

			await That(Act).DoesNotThrow();
		}
	}
}
