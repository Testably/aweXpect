namespace aweXpect.Options;

/// <summary>
///     The option that specified a <see cref="Quantifier" />, which names it when another option competes with it.
/// </summary>
internal enum QuantifierOption : byte
{
	None,
	AtLeast,
	AtMost,
	Between,
	LessThan,
	MoreThan,
	Exactly,
	Never,
	Once,
	Twice,
}
