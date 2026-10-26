using System.Text;
using aweXpect.Core;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Options;

/// <summary>
///     The attribute options and the inheritance scope of a single attribute expectation.
/// </summary>
/// <remarks>
///     The expectation shares the holders with its constraint and its result: the result replaces their values, and
///     the constraint reads them when it evaluates.
/// </remarks>
internal sealed class AttributeFilter<TMember>(AttributeFilterOptions<TMember> options)
{
	/// <summary>
	///     The holder of the alternative attributes.
	/// </summary>
	public OptionsHolder<AttributeFilterOptions<TMember>> Options { get; } = new(options);

	/// <summary>
	///     The holder of the inheritance scope of the attributes.
	/// </summary>
	public OptionsHolder<MemberScopes> Scope { get; } = new(MemberScopes.None);

	/// <inheritdoc cref="AttributeFilterOptions{TMember}.Matches(TMember, MemberScopes)" />
	public bool Matches(TMember member)
		=> Options.Value.Matches(member, Scope.Value);

	/// <inheritdoc cref="AttributeFilterOptions{TMember}.AppendExpectation(StringBuilder, ExpectationGrammars, MemberScopes)" />
	public void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammar)
		=> Options.Value.AppendExpectation(stringBuilder, grammar, Scope.Value);
}
