namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies, that the exception was thrown only if the <paramref name="condition" /> is <see langword="true" />,
	///     otherwise it verifies, that no exception was thrown.
	/// </summary>
	public ThatDelegateThrows<TException?> OnlyIf(bool condition)
	{
		ThrowOptions.DoCheckThrow = condition;
		return new ThatDelegateThrows<TException?>(ExpectationBuilder, ThrowOptions);
	}
}
