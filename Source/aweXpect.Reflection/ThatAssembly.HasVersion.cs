using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has a <see cref="AssemblyName.Version" /> that
	///     satisfies the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<Assembly, IThat<Assembly?>> HasVersion(
		this IThat<Assembly?> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(predicate, nameof(predicate));
		return new(subject.Get().ExpectationBuilder.AddConstraint((Predicate: predicate, DoNotPopulateThisValue: doNotPopulateThisValue),
				static (s, it, grammars)
				=> new HasVersionConstraint(it, grammars, s.Predicate, s.DoNotPopulateThisValue.TrimExpression())),
			subject);
	}

	/// <summary>
	///     Verifies the individual components of the <see cref="AssemblyName.Version" /> of the <see cref="Assembly" />.
	/// </summary>
	[GuaranteesNotNull]
	public static VersionResult<Assembly> HasVersion(this IThat<Assembly?> subject)
		=> new(subject, (expectationBuilder, checks)
			=> expectationBuilder.AddConstraint(checks,
				static (checks, it, grammars) => new HasVersionComponentsConstraint(it, grammars, checks)));

	private sealed class HasVersionComponentsConstraint(
		string it,
		ExpectationGrammars grammars,
		IReadOnlyOptionsHolder<VersionComponentCheck[]> checks)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IValueConstraint<Assembly?>
	{
		private VersionComponentCheck? _failed;
		private Version? _version;

		public ConstraintResult IsMetBy(Assembly? actual)
		{
			Actual = actual;
			_version = actual?.GetName().Version;
			_failed = null;
			if (_version is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			foreach (VersionComponentCheck check in checks.Value)
			{
				Outcome outcome = check.Comparison.Evaluate(check.Selector(_version));
				if (outcome == Outcome.FailureBothWays)
				{
					_failed = check;
					Outcome = outcome;
					return this;
				}

				if (outcome == Outcome.Failure && _failed is null)
				{
					_failed = check;
					Outcome = outcome;
				}
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (checks.Value.Length == 0)
			{
				stringBuilder.Append("has a version");
				return;
			}

			stringBuilder.Append("has ");
			AppendComponents(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_version is null)
			{
				stringBuilder.Append(It).Append(" had no version");
				return;
			}

			if (_failed is { } failed)
			{
				stringBuilder.Append(It).Append(" had ").Append(failed.Component).Append(' ');
				Formatter.Format(stringBuilder, failed.Selector(_version));
				return;
			}

			stringBuilder.Append(It).Append(" had version ").Append(Formatter.Format(_version));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (checks.Value.Length == 0)
			{
				stringBuilder.Append("does not have a version");
				return;
			}

			stringBuilder.Append("does not have ");
			AppendComponents(stringBuilder);
		}

		private void AppendComponents(StringBuilder stringBuilder)
		{
			for (int i = 0; i < checks.Value.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(" and ");
				}

				stringBuilder.Append(checks.Value[i].Component).Append(' ');
				if (checks.Value[i].Comparison.IsNegated)
				{
					stringBuilder.Append("not ");
				}

				checks.Value[i].Comparison.AppendExpectation(stringBuilder);
			}
		}
	}

	private sealed class HasVersionConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Version, bool> predicate,
		string predicateExpression)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IValueConstraint<Assembly?>
	{
		public ConstraintResult IsMetBy(Assembly? actual)
		{
			Actual = actual;
			Outcome = actual?.GetName().Version is { } version && UserCode.Invoke(predicate, version, "the predicate")
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has version matching ").Append(predicateExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had version ").Append(Formatter.Format(Actual?.GetName().Version));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not have version matching ").Append(predicateExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had version ").Append(Formatter.Format(Actual?.GetName().Version));
	}
}
