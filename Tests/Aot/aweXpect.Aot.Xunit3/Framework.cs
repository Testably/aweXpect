namespace aweXpect.Aot;

internal static class Framework
{
	public const string Name = "xunit.v3.assert";

	/// <remarks>
	///     The adapter for the assertion library alone throws xunit's own exception; the adapter for the core
	///     package would throw a generated one.
	/// </remarks>
	public const string FailureExceptionName = "Xunit.Sdk.XunitException";
}
