namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     Why an evaluation was canceled.
/// </summary>
public enum CancellationReason
{
	/// <summary>
	///     The evaluation was not canceled.
	/// </summary>
	None,

	/// <summary>
	///     The effective timeout of the evaluation elapsed.
	/// </summary>
	Timeout,

	/// <summary>
	///     The caller canceled the evaluation, with <c>WithCancellation(…)</c> or with the token of
	///     <c>Customize.aweXpect.Settings().TestCancellation</c>; this also applies when the timeout elapsed as well.
	/// </summary>
	Caller,
}
