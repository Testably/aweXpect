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
				.ExpectationBuilder.AddConstraint((it, grammars)
					=> new HaveNamespaceConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

	private sealed class HaveNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: ConstraintResult.WithValue<IEnumerable<Type?>>(grammars),
			IAsyncConstraint<IEnumerable<Type?>>
	{
		private Type?[] _matching = [];
		private Type?[] _unmatching = [];

		public async Task<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			(_matching, _unmatching) = await actual
				.SplitAsync(type => options.AreConsideredEqual(type?.Namespace, expected));
			Outcome = _unmatching.Length == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have namespace ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching types ");
			Formatter.Format(stringBuilder, _unmatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have namespace ")
				.Append(options.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching types ");
			Formatter.Format(stringBuilder, _matching, FormattingOptions.Indented(indentation));
		}
	}
}
