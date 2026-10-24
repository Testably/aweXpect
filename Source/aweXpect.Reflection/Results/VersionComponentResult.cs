using System;
using System.Reflection;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Comparison of a single component of the <see cref="AssemblyName.Version" /> of an <see cref="Assembly" /> or of
///     all items in a collection of <see cref="Assembly" />.
/// </summary>
/// <remarks>
///     The comparison operators, e.g. <c>EqualTo(…)</c> or <c>Between(…).And(…)</c>, are extension methods in
///     <see cref="IntPropertyResultExtensions" />.
/// </remarks>
public sealed class VersionComponentResult<TType> : IIntPropertyResult<VersionResult<TType>>
{
	private readonly string _component;
	private readonly VersionResult<TType> _owner;
	private readonly Func<Version, int> _selector;

	internal VersionComponentResult(
		VersionResult<TType> owner,
		string component,
		Func<Version, int> selector)
	{
		_owner = owner;
		_component = component;
		_selector = selector;
	}

	/// <inheritdoc cref="IIntPropertyResult{TResult}.Validation" />
	Action<int?, string>? IIntPropertyResult<VersionResult<TType>>.Validation => null;

	/// <inheritdoc cref="IIntPropertyResult{TResult}.Compare(PropertyComparison{int})" />
	VersionResult<TType> IIntPropertyResult<VersionResult<TType>>.Compare(PropertyComparison<int> comparison)
		=> _owner.Add(_component, _selector, comparison);
}
