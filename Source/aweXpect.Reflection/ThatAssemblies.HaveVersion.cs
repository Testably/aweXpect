using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     a <see cref="AssemblyName.Version" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>> HaveVersion(
		this IThat<IEnumerable<Assembly?>> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
				=> new HaveVersionConstraint(it, grammars, predicate, doNotPopulateThisValue.TrimCommonWhiteSpace())),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     a <see cref="AssemblyName.Version" /> that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>>> HaveVersion(
		this IThat<IAsyncEnumerable<Assembly?>> subject,
		Func<Version, bool> predicate,
		[CallerArgumentExpression(nameof(predicate))]
		string doNotPopulateThisValue = "")
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
				=> new HaveVersionConstraint(it, grammars, predicate, doNotPopulateThisValue.TrimCommonWhiteSpace())),
			subject);
#endif

	/// <summary>
	///     Verifies the individual components of the <see cref="AssemblyName.Version" /> of all items in the filtered
	///     collection of <see cref="Assembly" />.
	/// </summary>
	public static HaveVersionResult<IEnumerable<Assembly?>> HaveVersion(this IThat<IEnumerable<Assembly?>> subject)
		=> new(subject, (expectationBuilder, checks)
			=> expectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
				=> new HaveVersionComponentsConstraint(it, grammars, checks)));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies the individual components of the <see cref="AssemblyName.Version" /> of all items in the filtered
	///     collection of <see cref="Assembly" />.
	/// </summary>
	public static HaveVersionResult<IAsyncEnumerable<Assembly?>> HaveVersion(
		this IThat<IAsyncEnumerable<Assembly?>> subject)
		=> new(subject, (expectationBuilder, checks)
			=> expectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
				=> new HaveVersionComponentsConstraint(it, grammars, checks)));
#endif

	/// <summary>
	///     The result of verifying the <see cref="AssemblyName.Version" /> of a collection of <see cref="Assembly" />,
	///     allowing comparisons on the individual version components.
	/// </summary>
	public class HaveVersionResult<TCollection> : AndOrResult<TCollection, IThat<TCollection>>
	{
		private readonly List<VersionComponentCheck> _checks = [];

		internal HaveVersionResult(IThat<TCollection> subject,
			Func<ExpectationBuilder, List<VersionComponentCheck>, ExpectationBuilder> addConstraint)
			: this(subject, subject.Get().ExpectationBuilder, addConstraint)
		{
		}

		private HaveVersionResult(IThat<TCollection> subject, ExpectationBuilder expectationBuilder,
			Func<ExpectationBuilder, List<VersionComponentCheck>, ExpectationBuilder> addConstraint)
			: base(expectationBuilder, subject)
		{
			addConstraint(expectationBuilder, _checks);
		}

		/// <summary>
		///     Compares the <see cref="Version.Major" /> version component.
		/// </summary>
		public VersionComponentResult<TCollection> WithMajor => new(this, "major version", version => version.Major);

		/// <summary>
		///     Compares the <see cref="Version.Minor" /> version component.
		/// </summary>
		public VersionComponentResult<TCollection> WithMinor => new(this, "minor version", version => version.Minor);

		/// <summary>
		///     Compares the <see cref="Version.Build" /> version component.
		/// </summary>
		public VersionComponentResult<TCollection> WithBuild => new(this, "build version", version => version.Build);

		/// <summary>
		///     Compares the <see cref="Version.Revision" /> version component.
		/// </summary>
		public VersionComponentResult<TCollection> WithRevision
			=> new(this, "revision version", version => version.Revision);

		internal HaveVersionResult<TCollection> Add(string component, string comparisonText,
			Func<Version, int> selector, Func<int, bool> comparison)
		{
			_checks.Add(new VersionComponentCheck(component, comparisonText, selector, comparison));
			return this;
		}
	}

	/// <summary>
	///     Comparison of a single component of the <see cref="AssemblyName.Version" /> for all items in a collection of
	///     <see cref="Assembly" />.
	/// </summary>
	public class VersionComponentResult<TCollection>(
		HaveVersionResult<TCollection> owner,
		string component,
		Func<Version, int> selector)
	{
		/// <summary>
		///     The component is greater than the <paramref name="expected" /> value.
		/// </summary>
		public HaveVersionResult<TCollection> GreaterThan(int expected)
			=> owner.Add(component, $"greater than {expected}", selector, value => value > expected);

		/// <summary>
		///     The component is greater than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public HaveVersionResult<TCollection> GreaterThanOrEqualTo(int expected)
			=> owner.Add(component, $"greater than or equal to {expected}", selector, value => value >= expected);

		/// <summary>
		///     The component is less than the <paramref name="expected" /> value.
		/// </summary>
		public HaveVersionResult<TCollection> LessThan(int expected)
			=> owner.Add(component, $"less than {expected}", selector, value => value < expected);

		/// <summary>
		///     The component is less than or equal to the <paramref name="expected" /> value.
		/// </summary>
		public HaveVersionResult<TCollection> LessThanOrEqualTo(int expected)
			=> owner.Add(component, $"less than or equal to {expected}", selector, value => value <= expected);

		/// <summary>
		///     The component is equal to the <paramref name="expected" /> value.
		/// </summary>
		public HaveVersionResult<TCollection> EqualTo(int expected)
			=> owner.Add(component, $"equal to {expected}", selector, value => value == expected);

		/// <summary>
		///     The component is not equal to the <paramref name="expected" /> value.
		/// </summary>
		public HaveVersionResult<TCollection> NotEqualTo(int expected)
			=> owner.Add(component, $"not equal to {expected}", selector, value => value != expected);
	}

	internal sealed class VersionComponentCheck(
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

	private sealed class HaveVersionComponentsConstraint(
		string it,
		ExpectationGrammars grammars,
		List<VersionComponentCheck> checks)
		: CollectionConstraintResult<Assembly?>(grammars),
			IValueConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, Matches);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Assembly?> actual)
			=> SetValue(actual, Matches);

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
		{
			stringBuilder.Append(it).Append(" contained assemblies with a non-matching version ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

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
			stringBuilder.Append(it).Append(" only contained assemblies with a matching version ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}

		private bool Matches(Assembly? assembly)
		{
			Version? version = assembly?.GetName().Version;
			if (version is null)
			{
				return false;
			}

			foreach (VersionComponentCheck check in checks)
			{
				if (!check.Comparison(check.Selector(version)))
				{
					return false;
				}
			}

			return true;
		}

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

	private sealed class HaveVersionConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Version, bool> predicate,
		string predicateExpression)
		: CollectionConstraintResult<Assembly?>(grammars),
			IValueConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, assembly => assembly?.GetName().Version is { } version && predicate(version));
#endif

		public ConstraintResult IsMetBy(IEnumerable<Assembly?> actual)
			=> SetValue(actual, assembly => assembly?.GetName().Version is { } version && predicate(version));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have version matching ").Append(predicateExpression);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained assemblies with a non-matching version ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have version matching ").Append(predicateExpression);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained assemblies with a matching version ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
