using System.Collections.Generic;
using System.Reflection;
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

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Assembly" /> have
	///     the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> DoNotHaveName(
		this IThat<IEnumerable<Assembly?>?> subject, string unexpected)
	{
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(string Unexpected, OptionsSlot<StringEqualityOptions> Options), IEnumerable<Assembly?>>((unexpected, options),
					static (s, it, grammars)
					=> new DoNotHaveNameConstraint(it, grammars, s.Unexpected, s.Options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="Assembly" /> have
	///     the <paramref name="unexpected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>
		DoNotHaveName(
			this IThat<IAsyncEnumerable<Assembly?>?> subject, string unexpected)
	{
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<(string Unexpected, OptionsSlot<StringEqualityOptions> Options), IAsyncEnumerable<Assembly?>>((unexpected, options),
					static (s, it, grammars)
					=> new DoNotHaveNameConstraint(it, grammars, s.Unexpected, s.Options)),
			subject,
			options);
	}
#endif

	private sealed class DoNotHaveNameConstraint(
		string it,
		ExpectationGrammars grammars,
		string unexpected,
		OptionsSlot<StringEqualityOptions> options)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
		private StringEqualityOptions? _evaluationOptions;

		private StringEqualityOptions EvaluationOptions => _evaluationOptions ?? options.Value;

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			return await SetAsyncValue(actual, context,
				async assembly => !await EvaluationOptions.AreConsideredEqual(assembly?.GetName().Name, unexpected, nameof(unexpected)), cancellationToken);
		}
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			return await SetValue(actual, context,
				async assembly => !await EvaluationOptions.AreConsideredEqual(assembly?.GetName().Name, unexpected, nameof(unexpected)), cancellationToken);
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			base.AppendContexts(contexts);
			EvaluationOptions.AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.Value.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching assemblies");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name ").Append(options.Value.GetExpectation(unexpected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching assemblies");
	}
}
