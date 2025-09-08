using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a collection of generic arguments with a parameter with an expected name.
/// </summary>
public class GenericArgumentCollectionWithNamedArgumentResult<TThat>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	GenericArgumentsFilterOptions genericArgumentsFilterOptions,
	CollectionIndexOptions collectionIndexOptions,
	StringEqualityOptions options)
	: GenericArgumentCollectionWithArgumentResult<TThat>(
			expectationBuilder, subject, genericArgumentsFilterOptions, collectionIndexOptions),
		IOptionsProvider<StringEqualityOptions>
{
	/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

	/// <summary>
	///     Ignores casing when comparing the generic argument name,
	///     according to the <paramref name="ignoreCase" /> parameter.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TThat> IgnoringCase(bool ignoreCase = true)
	{
		options.IgnoringCase(ignoreCase);
		return this;
	}

	/// <summary>
	///     Uses the provided <paramref name="comparer" /> for comparing generic argument names.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TThat> Using(IEqualityComparer<string> comparer)
	{
		options.UsingComparer(comparer);
		return this;
	}

	/// <summary>
	///     Interprets the expected generic argument name as a prefix, so that the actual value starts with it.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TThat> AsPrefix()
	{
		options.AsPrefix();
		return this;
	}

	/// <summary>
	///     Interprets the expected generic argument name as a <see cref="Regex" /> pattern.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TThat> AsRegex()
	{
		options.AsRegex();
		return this;
	}

	/// <summary>
	///     Interprets the expected generic argument name as a suffix, so that the actual value ends with it.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TThat> AsSuffix()
	{
		options.AsSuffix();
		return this;
	}

	/// <summary>
	///     Interprets the expected generic argument name as wildcard pattern.<br />
	///     Supports * to match zero or more characters and ? to match exactly one character.
	/// </summary>
	public GenericArgumentCollectionWithArgumentResult<TThat> AsWildcard()
	{
		options.AsWildcard();
		return this;
	}
}
