using System;
using aweXpect.Results;

namespace aweXpect.Reflection.Helpers;

internal sealed class VersionComponentCheck(
	string component,
	Func<Version, int> selector,
	PropertyComparison<int> comparison)
{
	public string Component { get; } = component;
	public Func<Version, int> Selector { get; } = selector;
	public PropertyComparison<int> Comparison { get; } = comparison;
}
