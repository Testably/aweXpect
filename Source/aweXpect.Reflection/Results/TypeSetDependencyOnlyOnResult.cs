using aweXpect.Core;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     The result of a depends-only-on assertion whose allowed targets are filtered collections of types,
///     allowing to widen the allowed collections and to opt out of the implicit allowance of the type's own
///     sub-namespaces.
/// </summary>
public sealed class TypeSetDependencyOnlyOnResult<TType, TValue>
	: AndOrResult<TType, IThat<TValue>, TypeSetDependencyOnlyOnResult<TType, TValue>>
{
	private readonly OptionsHolder<TypeSetDependencyOptions> _options;

	internal TypeSetDependencyOnlyOnResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<TypeSetDependencyOptions> options)
		: base(expectationBuilder, subject)
	{
		_options = options;
	}

	/// <summary>
	///     Widens the expression by the given <paramref name="targets" />.
	/// </summary>
	public TypeSetDependencyOnlyOnResult<TType, TValue> OrOn(params Filtered.Types[] targets)
	{
		_options.Value = _options.Value.OrOn(targets);
		return this;
	}

	/// <summary>
	///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (so a <c>Foo</c> type
	///     referencing <c>Foo.Bar</c> becomes a violation unless <c>Foo.Bar</c> types are part of an allowed
	///     collection).
	/// </summary>
	/// <remarks>
	///     The type's own namespace itself is always allowed.
	/// </remarks>
	public TypeSetDependencyOnlyOnResult<TType, TValue> ExcludingOwnSubNamespaces()
	{
		_options.Value = _options.Value.ExcludingOwnSubNamespaces();
		return this;
	}
}
