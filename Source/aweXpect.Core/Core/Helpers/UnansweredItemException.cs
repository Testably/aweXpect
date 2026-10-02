using System;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Carries the result of an item expectation that fails both ways (e.g. because code of the caller threw or the
///     item is <see langword="null" />), so that the evaluation of the collection reports it instead of a mismatch.
/// </summary>
#pragma warning disable S3871 // Only the evaluation catches it, like the UserCodeException
internal sealed class UnansweredItemException(ConstraintResult itemResult, object? item)
	: Exception("An item expectation was not answered.", itemResult.FailureCause)
{
	/// <summary>
	///     The result of the item expectation.
	/// </summary>
	public ConstraintResult ItemResult { get; } = itemResult;

	/// <summary>
	///     The item that the item expectation did not answer.
	/// </summary>
	public object? Item { get; } = item;
}
#pragma warning restore S3871
