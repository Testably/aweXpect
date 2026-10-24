using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
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
	public static HasVersionResult HasVersion(this IThat<Assembly?> subject)
		=> new(subject);

	/// <summary>
	///     The result of verifying the <see cref="AssemblyName.Version" /> of an <see cref="Assembly" />,
	///     allowing comparisons on its individual components.
	/// </summary>
	public class HasVersionResult : AndOrResult<Assembly, IThat<Assembly?>>
	{
		private readonly List<VersionComponentCheck> _checks = [];

		internal HasVersionResult(IThat<Assembly?> subject)
			: this(subject, subject.Get().ExpectationBuilder)
		{
		}

		private HasVersionResult(IThat<Assembly?> subject, ExpectationBuilder expectationBuilder)
			: base(expectationBuilder, subject)
		{
			expectationBuilder.AddConstraint(_checks,
				static (checks, it, grammars) => new HasVersionComponentsConstraint(it, grammars, checks));
		}

		/// <summary>
		///     Compares the <see cref="Version.Major" /> version component.
		/// </summary>
		public VersionComponent WithMajor => new(this, "major version", version => version.Major);

		/// <summary>
		///     Compares the <see cref="Version.Minor" /> version component.
		/// </summary>
		public VersionComponent WithMinor => new(this, "minor version", version => version.Minor);

		/// <summary>
		///     Compares the <see cref="Version.Build" /> version component.
		/// </summary>
		/// <remarks>The build component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponent WithBuild => new(this, "build version", version => version.Build);

		/// <summary>
		///     Compares the <see cref="Version.Revision" /> version component.
		/// </summary>
		/// <remarks>The revision component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponent WithRevision => new(this, "revision version", version => version.Revision);

		internal HasVersionResult Add(string component, Func<Version, int> selector,
			PropertyComparison<int> comparison)
		{
			_checks.Add(new VersionComponentCheck(component, selector, comparison));
			return this;
		}
	}

	/// <summary>
	///     Comparison of a single component of the <see cref="AssemblyName.Version" /> of an <see cref="Assembly" />.
	/// </summary>
	/// <remarks>
	///     The comparison operators, e.g. <c>EqualTo(…)</c> or <c>Between(…).And(…)</c>, are extension methods in
	///     <see cref="IntPropertyResultExtensions" />.
	/// </remarks>
	public class VersionComponent : IIntPropertyResult<HasVersionResult>
	{
		private readonly string _component;
		private readonly HasVersionResult _owner;
		private readonly Func<Version, int> _selector;

		internal VersionComponent(HasVersionResult owner, string component, Func<Version, int> selector)
		{
			_owner = owner;
			_component = component;
			_selector = selector;
		}

		/// <inheritdoc cref="IIntPropertyResult{TResult}.Validation" />
		Action<int?, string>? IIntPropertyResult<HasVersionResult>.Validation => null;

		/// <inheritdoc cref="IIntPropertyResult{TResult}.Compare(PropertyComparison{int})" />
		HasVersionResult IIntPropertyResult<HasVersionResult>.Compare(PropertyComparison<int> comparison)
			=> _owner.Add(_component, _selector, comparison);
	}

	private sealed class VersionComponentCheck(
		string component,
		Func<Version, int> selector,
		PropertyComparison<int> comparison)
	{
		public string Component { get; } = component;
		public Func<Version, int> Selector { get; } = selector;
		public PropertyComparison<int> Comparison { get; } = comparison;
	}

	private sealed class HasVersionComponentsConstraint(
		string it,
		ExpectationGrammars grammars,
		List<VersionComponentCheck> checks)
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
			foreach (VersionComponentCheck check in checks)
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
			if (checks.Count == 0)
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
			if (checks.Count == 0)
			{
				stringBuilder.Append("does not have a version");
				return;
			}

			stringBuilder.Append("does not have ");
			AppendComponents(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

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
