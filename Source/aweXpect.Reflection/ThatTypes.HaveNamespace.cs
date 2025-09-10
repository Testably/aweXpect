using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     the <paramref name="expected" /> namespace.
	/// </summary>
	public static StringEqualityTypeResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> HaveNamespace(
		this IThat<IEnumerable<Type?>> subject, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new HaveNamespaceConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     the <paramref name="expected" /> namespace.
	/// </summary>
	public static StringEqualityTypeResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> HaveNamespace(
		this IThat<IAsyncEnumerable<Type?>> subject, string expected)
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new HaveNamespaceConstraint(it, grammars, expected, options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: CollectionConstraintResult<Type?>(grammars),
			IAsyncConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => options.AreConsideredEqual(type?.Namespace, expected));
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, CancellationToken cancellationToken)
			=> await SetValue(actual, type => options.AreConsideredEqual(type?.Namespace, expected));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have namespace ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have namespace ")
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
