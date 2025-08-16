using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a named parameter of a specific type.
/// </summary>
public class NamedParameterCollectionResult<TThat, TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	CollectionIndexOptions collectionIndexOptions,
	ParameterFilterOptions parameterFilterOptions,
	StringEqualityOptions options)
	: ParameterCollectionResult<TThat, TParameter>(expectationBuilder, subject, collectionIndexOptions,
			parameterFilterOptions),
		IOptionsProvider<StringEqualityOptions>
{
	/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

	/// <summary>
	///     Ignores casing when comparing the parameter name,
	///     according to the <paramref name="ignoreCase" /> parameter.
	/// </summary>
	public NamedParameterCollectionResult<TThat, TParameter> IgnoringCase(bool ignoreCase = true)
	{
		options.IgnoringCase(ignoreCase);
		return this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing parameter names.
	/// </summary>
	public NamedParameterCollectionResult<TThat, TParameter> Using(IEqualityComparer<string> comparer)
	{
		options.UsingComparer(comparer);
		return this;
	}

	/// <summary>
	///     Interprets the expected parameter name as a prefix, so that the actual value starts with it.
	/// </summary>
	public NamedParameterCollectionResult<TThat, TParameter> AsPrefix()
	{
		options.AsPrefix();
		return this;
	}

	/// <summary>
	///     Interprets the expected parameter name as a <see cref="Regex" /> pattern.
	/// </summary>
	public NamedParameterCollectionResult<TThat, TParameter> AsRegex()
	{
		options.AsRegex();
		return this;
	}

	/// <summary>
	///     Interprets the expected parameter name as a suffix, so that the actual value ends with it.
	/// </summary>
	public NamedParameterCollectionResult<TThat, TParameter> AsSuffix()
	{
		options.AsSuffix();
		return this;
	}

	/// <summary>
	///     Interprets the expected parameter name as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	public NamedParameterCollectionResult<TThat, TParameter> AsWildcard()
	{
		options.AsWildcard();
		return this;
	}
}
