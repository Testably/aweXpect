namespace aweXpect.Core.Nodes;

/// <summary>
///     The outcome of <see cref="Node.IsMetRightAway{TValue}" />.
/// </summary>
internal enum RightAwayOutcome
{
	/// <summary>
	///     The node cannot be decided right away, so it must be evaluated regularly.
	/// </summary>
	Unsupported,

	/// <summary>
	///     All expectations of the node are met.
	/// </summary>
	Met,

	/// <summary>
	///     The expectations of the node are not met, so it must be evaluated regularly for the failure.
	/// </summary>
	NotMet,
}
