namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ThrowsException
	{
		public class WhichTests
		{
			[Theory]
			[AutoData]
			public async Task ShouldGiveAccessToThrowsException(int hResult)
			{
				Exception exception = new HResultException(hResult);
				void Delegate() => throw exception;

				async Task Act()
					=> await That(Delegate).ThrowsException()
						.Which.IsSameAs(exception);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("foo", "bar")]
			public async Task ShouldIncludeWhichInErrorMessage(string paramName, string expectedParamName)
			{
				Exception exception = new ArgumentNullException(paramName, "Must not be null");
				void Delegate() => throw exception;

				async Task Act()
					=> await That(Delegate).Throws<ArgumentNullException>()
						.Which.Whose(h => h.ParamName, r => r.IsEqualTo(expectedParamName));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that Delegate
					             throws an ArgumentNullException which whose .ParamName is equal to "bar",
					             but .ParamName was "foo" which differs at index 0:
					                ↓ (actual)
					               "foo"
					               "bar"
					                ↑ (expected)
					             
					             Actual:
					             foo
					             
					             Expected:
					             bar
					             """);
			}

		}
	}
}
