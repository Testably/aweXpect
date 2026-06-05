using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that the namespaces of the filtered collection of <see cref="Type" /> are free of dependency cycles,
	///     i.e. no set of namespaces (transitively) depends on each other.
	/// </summary>
	/// <remarks>
	///     A namespace <c>A</c> depends on a namespace <c>B</c> when some type in <c>A</c> references a type in <c>B</c>,
	///     read through the same dependency resolver as the other dependency assertions (the signature-level default,
	///     unless a custom resolver is configured). Only namespaces present in the collection form nodes, so
	///     dependencies on framework or otherwise out-of-set namespaces never create an edge; self-references are
	///     ignored.
	///     <para />
	///     A namespace and its sub-namespaces are treated as one family, so references within a family never create an
	///     edge. Use <see cref="DependencyCyclesResult{TThat}.ExcludingSubNamespaces" /> to treat every namespace as its
	///     own node instead.
	/// </remarks>
	public static DependencyCyclesResult<IEnumerable<Type?>> HaveNoDependencyCycles(
		this IThat<IEnumerable<Type?>> subject)
	{
		DependencyCyclesOptions options = new(null);
		return new DependencyCyclesResult<IEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new HaveNoDependencyCyclesConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the namespaces of the filtered collection of <see cref="Type" /> are free of dependency cycles,
	///     grouping all namespaces below <paramref name="sliceRoot" /> into one slice each (by the namespace segment
	///     immediately following the root), so e.g. <c>MyApp.Orders.*</c> collapses to the single slice
	///     <c>MyApp.Orders</c>.
	/// </summary>
	/// <remarks>
	///     A slice <c>A</c> depends on a slice <c>B</c> when some type in <c>A</c> references a type in <c>B</c>, read
	///     through the same dependency resolver as the other dependency assertions (the signature-level default, unless
	///     a custom resolver is configured). Only slices present in the collection form nodes, so dependencies on
	///     framework or otherwise out-of-set namespaces never create an edge; references within the same slice are
	///     ignored.
	/// </remarks>
	public static DependencyCyclesResult<IEnumerable<Type?>> HaveNoDependencyCycles(
		this IThat<IEnumerable<Type?>> subject, string sliceRoot)
	{
		DependencyCyclesOptions options = new(sliceRoot);
		return new DependencyCyclesResult<IEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IEnumerable<Type?>>((it, grammars)
					=> new HaveNoDependencyCyclesConstraint(it, grammars, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <inheritdoc cref="HaveNoDependencyCycles(IThat{IEnumerable{Type}})" />
	public static DependencyCyclesResult<IAsyncEnumerable<Type?>> HaveNoDependencyCycles(
		this IThat<IAsyncEnumerable<Type?>> subject)
	{
		DependencyCyclesOptions options = new(null);
		return new DependencyCyclesResult<IAsyncEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new HaveNoDependencyCyclesConstraint(it, grammars, options)),
			subject,
			options);
	}

	/// <inheritdoc cref="HaveNoDependencyCycles(IThat{IEnumerable{Type}},string)" />
	public static DependencyCyclesResult<IAsyncEnumerable<Type?>> HaveNoDependencyCycles(
		this IThat<IAsyncEnumerable<Type?>> subject, string sliceRoot)
	{
		DependencyCyclesOptions options = new(sliceRoot);
		return new DependencyCyclesResult<IAsyncEnumerable<Type?>>(subject.Get().ExpectationBuilder
				.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
					=> new HaveNoDependencyCyclesConstraint(it, grammars, options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNoDependencyCyclesConstraint(
		string it,
		ExpectationGrammars grammars,
		DependencyCyclesOptions options)
		: ConstraintResult.WithNotNullValue<IEnumerable<Type?>>(it, grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
		private List<string> _cycles = [];

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
		{
			// Materialize once: the source (e.g. In.Namespace(...)) may be lazy and re-scan assemblies on every
			// enumeration, while both the cycle detection and the later result formatting read Actual.
			List<Type?> materialized = [.. actual];
			Actual = materialized;
			return SetResult(materialized);
		}

#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, CancellationToken cancellationToken)
		{
			List<Type?> materialized = [];
			await foreach (Type? type in actual.WithCancellation(cancellationToken))
			{
				materialized.Add(type);
			}

			Actual = materialized;
			return SetResult(materialized);
		}
#endif

		private HaveNoDependencyCyclesConstraint SetResult(IEnumerable<Type?> types)
		{
			_cycles = NamespaceDependencyGraph.Build(types, options.SliceRoot, options.ExcludeSubNamespaces).Cycles
				.Select(cycle => string.Join(" -> ", cycle))
				.ToList();
			Outcome = _cycles.Count == 0 ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("have no dependency cycles");
			AppendSliceInfo(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had ");
			if (_cycles.Count == 1)
			{
				stringBuilder.Append("a dependency cycle:");
			}
			else
			{
				stringBuilder.Append(_cycles.Count).Append(" dependency cycles:");
			}

			string itemIndentation = (indentation ?? string.Empty) + "  ";
			foreach (string cycle in _cycles)
			{
				stringBuilder.Append(Environment.NewLine).Append(itemIndentation).Append(cycle);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("have dependency cycles");
			AppendSliceInfo(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had none");

		private void AppendSliceInfo(StringBuilder stringBuilder)
		{
			if (options.SliceRoot is not null)
			{
				stringBuilder.Append(" when grouped into slices under ").Append(Formatter.Format(options.SliceRoot));
			}
		}
	}
}
