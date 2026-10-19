namespace aweXpect.Snapshots;

/// <summary>
///     A rule for all snapshots, e.g. <c>SnapshotRule.For.Guids().Mask()</c>, set with
///     <c>Customize.aweXpect.Snapshots().Rules</c>.
/// </summary>
/// <remarks>
///     A rule is written like the rules of an expectation after <c>.For</c>: select what it applies to, filter it, then
///     one action, e.g. <c>SnapshotRule.For.Members().WithName("PlayCount").Ignore()</c>. The rules for all snapshots
///     are applied after the rules of the expectation. Member and value rules are ignored for a snapshot of a
///     <see cref="string" />, which is compared as raw text, and line rules for a snapshot of an object.
/// </remarks>
public sealed class SnapshotRule
{
	private SnapshotRule()
	{
	}

	internal SnapshotRule(ValueRule rule)
	{
		ValueRule = rule;
	}

	internal SnapshotRule(LineRule rule)
	{
		LineRule = rule;
	}

	internal SnapshotRule(TextMasking masking)
	{
		TextMasking = masking;
	}

	/// <summary>
	///     Selects what a rule for all snapshots applies to, e.g. <c>SnapshotRule.For.Members()</c> or
	///     <c>SnapshotRule.For.Guids()</c>.
	/// </summary>
	public static SnapshotFor<SnapshotRule> For { get; } = new(static rule => rule);

	/// <summary>
	///     The rule that keeps the paths of the environment, which are masked by default.
	/// </summary>
	internal static SnapshotRule KeepPaths { get; } = new()
	{
		KeepsPaths = true,
	};

	internal bool KeepsPaths { get; private init; }

	internal LineRule? LineRule { get; }

	internal TextMasking? TextMasking { get; }

	internal ValueRule? ValueRule { get; }
}
