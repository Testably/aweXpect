using System;

namespace aweXpect.Reflection.Options;

/// <summary>
///     The target of a dependency assertion that is matched against the dependencies of a single type.
/// </summary>
internal interface IDependencyTarget
{
	/// <summary>
	///     Checks whether the <paramref name="type" /> has at least one dependency that matches the target.
	/// </summary>
	bool IsMatchedBy(Type type);

	/// <summary>
	///     Describes the target for an expectation message.
	/// </summary>
	string Describe();
}
