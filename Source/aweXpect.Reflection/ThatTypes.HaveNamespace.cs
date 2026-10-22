using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     the <paramref name="expected" /> namespace.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> HaveNamespace(
		this IThat<IEnumerable<Type?>?> subject, string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(string Expected, StringEqualityOptions Options), IEnumerable<Type?>>((expected, options),
					static (s, it, grammars)
					=> new HaveNamespaceConstraint(it, grammars, s.Expected, s.Options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     the <paramref name="expected" /> namespace.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> HaveNamespace(
		this IThat<IAsyncEnumerable<Type?>?> subject, string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(string Expected, StringEqualityOptions Options), IAsyncEnumerable<Type?>>((expected, options),
					static (s, it, grammars)
					=> new HaveNamespaceConstraint(it, grammars, s.Expected, s.Options)),
			subject,
			options);
	}
#endif

	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have
	///     the <paramref name="unexpected" /> namespace.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="AreNotWithinNamespace(IThat{IEnumerable{Type}}, string)" />, this compares the whole namespace
	///     as a string with the string options (e.g. <c>AsPrefix()</c> or <c>IgnoringCase()</c>), instead of excluding a
	///     namespace and its sub-namespaces.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> DoNotHaveNamespace(
		this IThat<IEnumerable<Type?>?> subject, string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(string Unexpected, StringEqualityOptions Options), IEnumerable<Type?>>((unexpected, options),
					static (s, it, grammars)
					=> new DoNotHaveNamespaceConstraint(it, grammars, s.Unexpected, s.Options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Type" /> have
	///     the <paramref name="unexpected" /> namespace.
	/// </summary>
	/// <remarks>
	///     Unlike <see cref="AreNotWithinNamespace(IThat{IAsyncEnumerable{Type}}, string)" />, this compares the whole
	///     namespace as a string with the string options (e.g. <c>AsPrefix()</c> or <c>IgnoringCase()</c>), instead of
	///     excluding a namespace and its sub-namespaces.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> DoNotHaveNamespace(
		this IThat<IAsyncEnumerable<Type?>?> subject, string unexpected)
	{
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringEqualityTypeResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(string Unexpected, StringEqualityOptions Options), IAsyncEnumerable<Type?>>((unexpected, options),
					static (s, it, grammars)
					=> new DoNotHaveNamespaceConstraint(it, grammars, s.Unexpected, s.Options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: CollectionConstraintResult<Type?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				type => options.AreConsideredEqual(type?.Namespace, expected), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context,
				type => options.AreConsideredEqual(type?.Namespace, expected), cancellationToken);

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			base.AppendContexts(contexts);
			options.AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have namespace ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have namespace ")
				.Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching types");
	}

	private sealed class DoNotHaveNamespaceConstraint(
		string it,
		ExpectationGrammars grammars,
		string unexpected,
		StringEqualityOptions options)
		: CollectionConstraintResult<Type?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, DoesNotHaveNamespace, cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context, DoesNotHaveNamespace, cancellationToken);

		private async ValueTask<bool> DoesNotHaveNamespace(Type? type)
			=> type is not null && !await options.AreConsideredEqual(type.Namespace, unexpected);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have namespace ").Append(options.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have namespace ")
				.Append(options.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching types");
	}
}
