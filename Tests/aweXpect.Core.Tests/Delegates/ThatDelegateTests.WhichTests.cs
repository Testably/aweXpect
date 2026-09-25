using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class WhichTests
	{
		[Fact]
		public async Task Throws_Which_WithChainedWhose_ShouldNotRepeatConnector()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.Which.Whose(e => e.HResult, h => h.IsEqualTo(5))
					.And.Whose(e => e.InnerException, i => i.IsNull());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException whose HResult is equal to 5 and whose InnerException is null,
				             but HResult was -2146233088, which differs by -2146233093
				             """);
		}

		[Fact]
		public async Task Throws_Which_WithOtherExpectationBeforeWhose_ShouldKeepWhich()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.Which.HasHResult(5)
					.And.Whose(e => e.InnerException, i => i.IsNull());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException that has HResult equal to 5 and whose InnerException is null,
				             but it had HResult -2146233088
				             """);
		}

		[Fact]
		public async Task Throws_Which_WithWhose_ShouldNotRepeatConnector()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws<MyException>()
					.Which.Whose(e => e.HResult, h => h.IsEqualTo(5));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws a MyException whose HResult is equal to 5,
				             but HResult was -2146233088, which differs by -2146233093
				             """);
		}

		[Fact]
		public async Task ThrowsException_Which_WithWhose_ShouldNotRepeatConnector()
		{
			void Delegate() => throw new MyException();

			async Task Act()
				=> await That(Delegate).Throws()
					.Which.Whose(e => e.HResult, h => h.IsEqualTo(5));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that Delegate
				             throws an exception whose HResult is equal to 5,
				             but HResult was -2146233088, which differs by -2146233093
				             """);
		}
	}
}
