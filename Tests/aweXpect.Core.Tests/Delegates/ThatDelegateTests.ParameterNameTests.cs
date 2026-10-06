using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
	public sealed class ParameterNameTests
	{
		[Test]
		public async Task DoesNotThrow_WithoutValue_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).DoesNotThrow(typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task DoesNotThrow_WithValue_ShouldAcceptTypeAsNamedArgument()
		{
			Func<int> @delegate = () => 1;

			async Task Act()
				=> await That(@delegate).DoesNotThrow(typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task DoesNotThrowExactly_WithoutValue_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).DoesNotThrowExactly(typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task DoesNotThrowExactly_WithValue_ShouldAcceptTypeAsNamedArgument()
		{
			Func<int> @delegate = () => 1;

			async Task Act()
				=> await That(@delegate).DoesNotThrowExactly(typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task OnlyIf_ShouldAcceptConditionAsNamedArgument()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).Throws<MyException>().OnlyIf(false);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Throws_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException();

			async Task Act()
				=> await That(@delegate).Throws(typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task ThrowsExactly_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException();

			async Task Act()
				=> await That(@delegate).ThrowsExactly(typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WithInner_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException(innerException: new MyException());

			async Task Act()
				=> await That(@delegate).Throws<MyException>().WithInner(typeof(MyException));

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WithInner_WithExpectations_ShouldAcceptTypeAsNamedArgument()
		{
			Action @delegate = () => throw new MyException(innerException: new MyException());

			async Task Act()
				=> await That(@delegate).Throws<MyException>()
					.WithInner(typeof(MyException), it => it.Is<MyException>());

			await That(Act).DoesNotThrow();
		}
	}
#pragma warning restore CA2263
}
