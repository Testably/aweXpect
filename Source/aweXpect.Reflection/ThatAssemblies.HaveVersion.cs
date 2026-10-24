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
	public static HaveVersionResult<IEnumerable<Assembly?>> HaveVersion(this IThat<IEnumerable<Assembly?>?> subject)
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
	public static HaveVersionResult<IAsyncEnumerable<Assembly?>> HaveVersion(
		this IThat<IAsyncEnumerable<Assembly?>?> subject)
		=> new(subject, (expectationBuilder, checks)
			=> expectationBuilder.AddConstraint<List<VersionComponentCheck>, IAsyncEnumerable<Assembly?>>(checks,
				static (checks, it, grammars)
				=> new HaveVersionComponentsConstraint(it, grammars, checks)));
#endif

	/// <summary>
	///     The result of verifying the <see cref="AssemblyName.Version" /> of a collection of <see cref="Assembly" />,
	///     allowing comparisons on the individual version components.
	/// </summary>
	public class HaveVersionResult<TCollection> : AndOrResult<TCollection, IThat<TCollection?>>
	{
		private readonly List<VersionComponentCheck> _checks = [];

		internal HaveVersionResult(IThat<TCollection?> subject,
			Func<ExpectationBuilder, List<VersionComponentCheck>, ExpectationBuilder> addConstraint)
			: this(subject, subject.Get().ExpectationBuilder, addConstraint)
		{
		}

		private HaveVersionResult(IThat<TCollection?> subject, ExpectationBuilder expectationBuilder,
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
		/// <remarks>The build component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponentResult<TCollection> WithBuild => new(this, "build version", version => version.Build);

		/// <summary>
		///     Compares the <see cref="Version.Revision" /> version component.
		/// </summary>
		/// <remarks>The revision component is <c>-1</c> when it is absent from the version.</remarks>
		public VersionComponentResult<TCollection> WithRevision
			=> new(this, "revision version", version => version.Revision);

		internal HaveVersionResult<TCollection> Add(string component, Func<Version, int> selector,
			PropertyComparison<int> comparison)
		{
			_checks.Add(new VersionComponentCheck(component, selector, comparison));
			return this;
		}
	}

	/// <summary>
	///     Comparison of a single component of the <see cref="AssemblyName.Version" /> for all items in a collection of
	///     <see cref="Assembly" />.
	/// </summary>
	/// <remarks>
	///     The comparison operators, e.g. <c>EqualTo(…)</c> or <c>Between(…).And(…)</c>, are extension methods in
	///     <see cref="IntPropertyResultExtensions" />.
	/// </remarks>
	public class VersionComponentResult<TCollection> : IIntPropertyResult<HaveVersionResult<TCollection>>
	{
		private readonly string _component;
		private readonly HaveVersionResult<TCollection> _owner;
		private readonly Func<Version, int> _selector;

		internal VersionComponentResult(
			HaveVersionResult<TCollection> owner,
			string component,
			Func<Version, int> selector)
		{
			_owner = owner;
			_component = component;
			_selector = selector;
		}

		/// <inheritdoc cref="IIntPropertyResult{TResult}.Validation" />
		Action<int?, string>? IIntPropertyResult<HaveVersionResult<TCollection>>.Validation => null;

		/// <inheritdoc cref="IIntPropertyResult{TResult}.Compare(PropertyComparison{int})" />
		HaveVersionResult<TCollection> IIntPropertyResult<HaveVersionResult<TCollection>>.Compare(
			PropertyComparison<int> comparison)
			=> _owner.Add(_component, _selector, comparison);
	}

	internal sealed class VersionComponentCheck(
		string component,
		Func<Version, int> selector,
		PropertyComparison<int> comparison)
	{
		public string Component { get; } = component;
		public Func<Version, int> Selector { get; } = selector;
		public PropertyComparison<int> Comparison { get; } = comparison;
	}

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
