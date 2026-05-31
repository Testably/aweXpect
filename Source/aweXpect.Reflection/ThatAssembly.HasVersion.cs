using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has a <see cref="AssemblyName.Version" /> that
	///     satisfies the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<Assembly?, IThat<Assembly?>> HasVersion(
		this IThat<Assembly?> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasVersionConstraint(it, grammars, predicate, doNotPopulateThisValue.TrimCommonWhiteSpace())),
			subject);

	/// <summary>
	///     Verifies the individual components of the <see cref="AssemblyName.Version" /> of the <see cref="Assembly" />.
	/// </summary>
	public static HasVersionResult HasVersion(this IThat<Assembly?> subject)
		=> new(subject);

	/// <summary>
	///     The result of verifying the <see cref="AssemblyName.Version" /> of an <see cref="Assembly" />,
	///     allowing comparisons on its individual components.
	/// </summary>
	public class HasVersionResult : AndOrResult<Assembly?, IThat<Assembly?>>
	{
		private readonly List<VersionComponentCheck> _checks = [];

		internal HasVersionResult(IThat<Assembly?> subject)
			: this(subject, subject.Get().ExpectationBuilder)
		{
		}

		private HasVersionResult(IThat<Assembly?> subject, ExpectationBuilder expectationBuilder)
			: base(expectationBuilder, subject)
		{
			expectationBuilder.AddConstraint((it, grammars) => new HasVersionComponentsConstraint(it, grammars, _checks));
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

		internal HasVersionResult Add(string component, string comparisonText, Func<Version, int> selector,
			Func<int, bool> comparison)
		{
			_checks.Add(new VersionComponentCheck(component, comparisonText, selector, comparison));
			return this;
		}
	}

	/// <summary>
	///     Comparison of a single component of the <see cref="AssemblyName.Version" /> of an <see cref="Assembly" />.
	/// </summary>
	public class VersionComponent(HasVersionResult owner, string component, Func<Version, int> selector)
	{
		/// <summary>
		///     The component is greater than the <paramref name="expected" /> value.
		/// </summary>
		public HasVersionResult GreaterThan(int expected)
			=> owner.Add(component, $"greater than {expected}", selector, value => value > expected);

		/// <summary>
		///     The component is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public HasVersionResult GreaterThanOrEqualTo(int expected)
			=> owner.Add(component, $"greater than or equal to {expected}", selector, value => value >= expected);

		/// <summary>
		///     The component is less than the <paramref name="expected" /> value.
		/// </summary>
		public HasVersionResult LessThan(int expected)
			=> owner.Add(component, $"less than {expected}", selector, value => value < expected);

		/// <summary>
		///     The component is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public HasVersionResult LessThanOrEqualTo(int expected)
			=> owner.Add(component, $"less than or equal to {expected}", selector, value => value <= expected);

		/// <summary>
		///     The component is equal to the <paramref name="expected" /> value.
		/// </summary>
		public HasVersionResult EqualTo(int expected)
			=> owner.Add(component, $"equal to {expected}", selector, value => value == expected);

		/// <summary>
		///     The component is not equal to the <paramref name="expected" /> value.
		/// </summary>
		public HasVersionResult NotEqualTo(int expected)
			=> owner.Add(component, $"not equal to {expected}", selector, value => value != expected);
	}

	private sealed class VersionComponentCheck(
		string component,
		string comparisonText,
		Func<Version, int> selector,
		Func<int, bool> comparison)
	{
		public string Component { get; } = component;
		public string ComparisonText { get; } = comparisonText;
		public Func<Version, int> Selector { get; } = selector;
		public Func<int, bool> Comparison { get; } = comparison;
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

			foreach (VersionComponentCheck check in checks)
			{
				if (!check.Comparison(check.Selector(_version)))
				{
					_failed = check;
					Outcome = Outcome.Failure;
					return this;
				}
			}

			Outcome = Outcome.Success;
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

				stringBuilder.Append(checks[i].Component).Append(' ').Append(checks[i].ComparisonText);
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
			Outcome = actual?.GetName().Version is { } version && predicate(version)
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
