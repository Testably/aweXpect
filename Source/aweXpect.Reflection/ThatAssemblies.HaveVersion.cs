using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     a <see cref="AssemblyName.Version" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> HaveVersion(
		this IThat<IEnumerable<Assembly?>?> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Func<Version, bool> Predicate, string DoNotPopulateThisValue), IEnumerable<Assembly?>>((predicate, doNotPopulateThisValue),
				static (s, it, grammars)
				=> new HaveVersionConstraint(it, grammars, s.Predicate, s.DoNotPopulateThisValue.TrimExpression())),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     a <see cref="AssemblyName.Version" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> HaveVersion(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
		return new(subject.Get().ExpectationBuilder.AddConstraint<(Func<Version, bool> Predicate, string DoNotPopulateThisValue), IAsyncEnumerable<Assembly?>>((predicate, doNotPopulateThisValue),
				static (s, it, grammars)
				=> new HaveVersionConstraint(it, grammars, s.Predicate, s.DoNotPopulateThisValue.TrimExpression())),
			subject);
	}
#endif

	/// <summary>
	///     Verifies the individual components of the <see cref="AssemblyName.Version" /> of all items in the filtered
	///     collection of <see cref="Assembly" />.
	/// </summary>
	[GuaranteesNotNull]
	public static VersionResult<IEnumerable<Assembly?>> HaveVersion(this IThat<IEnumerable<Assembly?>?> subject)
		=> new(subject, (expectationBuilder, checks)
			=> expectationBuilder.AddConstraint<List<VersionComponentCheck>, IEnumerable<Assembly?>>(checks,
				static (checks, it, grammars)
				=> new HaveVersionComponentsConstraint(it, grammars, checks)));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies the individual components of the <see cref="AssemblyName.Version" /> of all items in the filtered
	///     collection of <see cref="Assembly" />.
	/// </summary>
	[GuaranteesNotNull]
	public static VersionResult<IAsyncEnumerable<Assembly?>> HaveVersion(
		this IThat<IAsyncEnumerable<Assembly?>?> subject)
		=> new(subject, (expectationBuilder, checks)
			=> expectationBuilder.AddConstraint<List<VersionComponentCheck>, IAsyncEnumerable<Assembly?>>(checks,
				static (checks, it, grammars)
				=> new HaveVersionComponentsConstraint(it, grammars, checks)));
#endif

	private sealed class HaveVersionComponentsConstraint(
		string it,
		ExpectationGrammars grammars,
		List<VersionComponentCheck> checks)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
		private bool _isUnanswered;

		/// <inheritdoc />
		/// <remarks>
		///     A comparison that cannot be answered, e.g. an ordering against a <see langword="null" /> bound, fails the
		///     expectation and its negation.
		/// </remarks>
		public override Outcome Outcome => _isUnanswered ? Outcome.FailureBothWays : base.Outcome;

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_isUnanswered = false;
			return await SetAsyncValue(actual, context, Matches, cancellationToken);
		}
#endif

		public ConstraintResult IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context)
		{
			_isUnanswered = false;
			return SetValue(actual, context, Matches);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (checks.Count == 0)
			{
				stringBuilder.Append("all have a version");
				return;
			}

			stringBuilder.Append("all have ");
			AppendComponents(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained assemblies with a non-matching version");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (checks.Count == 0)
			{
				stringBuilder.Append("not all have a version");
				return;
			}

			stringBuilder.Append("not all have ");
			AppendComponents(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_isUnanswered)
			{
				AppendNormalResult(stringBuilder, indentation);
				return;
			}

			stringBuilder.Append(It).Append(" only contained assemblies with a matching version");
		}

		private bool Matches(Assembly? assembly)
		{
			Version? version = assembly?.GetName().Version;
			if (version is null)
			{
				return false;
			}

			bool isMatching = true;
			foreach (VersionComponentCheck check in checks)
			{
				Outcome outcome = check.Comparison.Evaluate(check.Selector(version));
				if (outcome == Outcome.FailureBothWays)
				{
					_isUnanswered = true;
					return false;
				}

				isMatching &= outcome == Outcome.Success;
			}

			return isMatching;
		}

		private void AppendComponents(StringBuilder stringBuilder)
		{
			for (int i = 0; i < checks.Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(" and ");
				}

				stringBuilder.Append(checks[i].Component).Append(' ');
				if (checks[i].Comparison.IsNegated)
				{
					stringBuilder.Append("not ");
				}

				checks[i].Comparison.AppendExpectation(stringBuilder);
			}
		}
	}

	private sealed class HaveVersionConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Version, bool> predicate,
		string predicateExpression)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, Matches, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context)
			=> SetValue(actual, context, Matches);

		private bool Matches(Assembly? assembly)
			=> assembly?.GetName().Version is { } version && UserCode.Invoke(predicate, version, "the predicate");

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have version matching ").Append(predicateExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained assemblies with a non-matching version");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have version matching ").Append(predicateExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained assemblies with a matching version");
	}
}
