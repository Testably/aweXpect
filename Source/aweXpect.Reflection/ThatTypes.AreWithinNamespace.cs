using System;
using System.Collections.Generic;
using System.Text;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are within
	///     the <paramref name="expected" /> namespace (including sub-namespaces).
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreWithinNamespace(
		this IThat<IEnumerable<Type?>> subject, string expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreWithinNamespaceConstraint(it, grammars, expected)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are within
	///     the <paramref name="expected" /> namespace (including sub-namespaces).
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreWithinNamespace(
		this IThat<IAsyncEnumerable<Type?>> subject, string expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreWithinNamespaceConstraint(it, grammars, expected)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not within
	///     the <paramref name="expected" /> namespace (including sub-namespaces).
	/// </summary>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotWithinNamespace(
		this IThat<IEnumerable<Type?>> subject, string expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreNotWithinNamespaceConstraint(it, grammars, expected)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not within
	///     the <paramref name="expected" /> namespace (including sub-namespaces).
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreNotWithinNamespace(
		this IThat<IAsyncEnumerable<Type?>> subject, string expected)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreNotWithinNamespaceConstraint(it, grammars, expected)),
			subject);
#endif

	private sealed class AreWithinNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type.IsWithinNamespace(expected));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type.IsWithinNamespace(expected));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all within namespace ").Append(Formatter.Format(expected));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all within namespace ").Append(Formatter.Format(expected));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotWithinNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => !type.IsWithinNamespace(expected));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => !type.IsWithinNamespace(expected));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not within namespace ").Append(Formatter.Format(expected));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a type within namespace ").Append(Formatter.Format(expected));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
