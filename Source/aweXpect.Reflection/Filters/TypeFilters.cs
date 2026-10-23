using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect.Reflection;

/// <summary>
///     Extensions methods to filter <see cref="Filtered.Types" />.
/// </summary>
public static partial class TypeFilters
{
	private static TypesContainingMembers Containing<TMember, TFiltered>(Filtered.Types @this,
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		ContainedMembersFilter<TMember, TFiltered> containedMembersFilter = new(navigate, filter, new Quantifier());
		return new TypesContainingMembers(@this, containedMembersFilter, containedMembersFilter.WithQuantifier);
	}

	/// <summary>
	///     Creates the shared <see cref="IContainedMembersFilter" /> that counts the members of a single
	///     <see cref="Type" /> matching the <paramref name="filter" />, reusing the same counting and description logic
	///     as the <see cref="Filtered.Types" /> filters.
	/// </summary>
	internal static IContainedMembersFilter ContainedMembers<TMember, TFiltered>(
		Func<Filtered.Types, TFiltered> navigate,
		Func<TFiltered, TFiltered> filter,
		Quantifier quantifier)
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
		=> new ContainedMembersFilter<TMember, TFiltered>(navigate, filter, quantifier);

	/// <summary>
	///     A filterable collection of <see cref="Type" /> that contain matching members, allowing to specify a quantifier
	///     for the number of matching members.
	/// </summary>
	public class TypesContainingMembers : Filtered.Types
	{
		private readonly Filtered.Types _source;
		private readonly Func<Quantifier, IFilter<Type>> _withQuantifier;

		internal TypesContainingMembers(Filtered.Types source, IFilter<Type> filter,
			Func<Quantifier, IFilter<Type>> withQuantifier)
			: base(source.Which(filter))
		{
			_source = source;
			_withQuantifier = withQuantifier;
		}

		/// <summary>
		///     Verifies, that the matching members occur at least…
		/// </summary>
		public CountTimesResult<Filtered.Types> AtLeast()
			=> new(value => Quantify(quantifier => quantifier.AtLeast(value)));

		/// <summary>
		///     Verifies, that the matching members occur at least <paramref name="minimum" /> times.
		/// </summary>
		public Filtered.Types AtLeast(Times minimum)
			=> Quantify(quantifier => quantifier.AtLeast(minimum.Value));

		/// <summary>
		///     Verifies, that the matching members occur at most…
		/// </summary>
		public CountTimesResult<Filtered.Types> AtMost()
			=> new(value => Quantify(quantifier => quantifier.AtMost(value)));

		/// <summary>
		///     Verifies, that the matching members occur at most <paramref name="maximum" /> times.
		/// </summary>
		public Filtered.Types AtMost(Times maximum)
			=> Quantify(quantifier => quantifier.AtMost(maximum.Value));

		/// <summary>
		///     Verifies, that the matching members occur between <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<Filtered.Types> Between(int minimum)
			=> new(maximum => Quantify(quantifier => quantifier.Between(minimum, maximum)));

		/// <summary>
		///     Verifies, that the matching members occur exactly <paramref name="expected" /> times.
		/// </summary>
		public Filtered.Types Exactly(Times expected)
			=> Quantify(quantifier => quantifier.Exactly(expected.Value));

		/// <summary>
		///     Verifies, that the matching members occur less than…
		/// </summary>
		public CountTimesResult<Filtered.Types> LessThan()
			=> new(value => Quantify(quantifier => quantifier.LessThan(value)));

		/// <summary>
		///     Verifies, that the matching members occur less than <paramref name="maximum" /> times.
		/// </summary>
		public Filtered.Types LessThan(Times maximum)
			=> Quantify(quantifier => quantifier.LessThan(maximum.Value));

		/// <summary>
		///     Verifies, that the matching members occur more than…
		/// </summary>
		public CountTimesResult<Filtered.Types> MoreThan()
			=> new(value => Quantify(quantifier => quantifier.MoreThan(value)));

		/// <summary>
		///     Verifies, that the matching members occur more than <paramref name="minimum" /> times.
		/// </summary>
		public Filtered.Types MoreThan(Times minimum)
			=> Quantify(quantifier => quantifier.MoreThan(minimum.Value));

		/// <summary>
		///     Verifies, that no member matches the filter.
		/// </summary>
		public Filtered.Types Never()
			=> Quantify(quantifier => quantifier.Exactly(0));

		/// <summary>
		///     Verifies, that exactly one member matches the filter.
		/// </summary>
		public Filtered.Types Once()
			=> Quantify(quantifier => quantifier.Exactly(1));

		/// <summary>
		///     Verifies, that exactly two members match the filter.
		/// </summary>
		public Filtered.Types Twice()
			=> Quantify(quantifier => quantifier.Exactly(2));

		private Filtered.Types Quantify(Action<Quantifier> specify)
		{
			Quantifier quantifier = new();
			specify(quantifier);
			return _source.Which(_withQuantifier(quantifier));
		}
	}

	private sealed class ContainedMembersFilter<TMember, TFiltered> : IContainedMembersFilter
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		private readonly Func<TFiltered, TFiltered> _filter;
		private readonly Func<Filtered.Types, TFiltered> _navigate;
		private readonly Quantifier _quantifier;

		public ContainedMembersFilter(Func<Filtered.Types, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter,
			Quantifier quantifier)
		{
			ThrowHelper.ThrowIfNull(filter, nameof(filter));
			_navigate = navigate;
			_filter = filter;
			_quantifier = quantifier;

			// Derive the inner description from an empty probe, e.g. "methods with FactAttribute or with TheoryAttribute ".
			string inner = _filter(_navigate(new Filtered.Types([], ""))).GetDescription();
			if (inner.EndsWith("in "))
			{
				inner = inner.Substring(0, inner.Length - "in ".Length);
			}

			MembersDescription = inner;
		}

		private ContainedMembersFilter(ContainedMembersFilter<TMember, TFiltered> original, Quantifier quantifier)
		{
			_navigate = original._navigate;
			_filter = original._filter;
			_quantifier = quantifier;
			MembersDescription = original.MembersDescription;
		}

		/// <inheritdoc cref="IContainedMembersFilter.MembersDescription" />
		public string MembersDescription { get; }

		/// <summary>
		///     Returns a copy of this filter that uses the given <paramref name="quantifier" />.
		/// </summary>
		public IFilter<Type> WithQuantifier(Quantifier quantifier)
			=> new ContainedMembersFilter<TMember, TFiltered>(this, quantifier);

#if NET8_0_OR_GREATER
		/// <inheritdoc cref="IContainedMembersFilter.CountMatchingMembers(Type, CancellationToken)" />
		public async ValueTask<int> CountMatchingMembers(Type value, CancellationToken cancellationToken)
		{
			int count = 0;
			await foreach (var _ in Filter(value).WithCancellation(cancellationToken))
			{
				cancellationToken.ThrowIfCancellationRequested();
				count++;
			}

			return count;
		}
#else
		/// <inheritdoc cref="IContainedMembersFilter.CountMatchingMembers(Type, CancellationToken)" />
		public ValueTask<int> CountMatchingMembers(Type value, CancellationToken cancellationToken)
		{
			int count = 0;
			foreach (var _ in Filter(value))
			{
				cancellationToken.ThrowIfCancellationRequested();
				count++;
			}

			return new ValueTask<int>(count);
		}
#endif

		public async ValueTask<bool> Applies(Type value)
			=> _quantifier.Check(await CountMatchingMembers(value, CancellationToken.None), true) ?? false;

		private TFiltered Filter(Type value)
			=> UserCode.Invoke(_filter, _navigate(new Filtered.Types([value,], "")), "the filter");

		public string Describes(string text)
			=> $"{text.TrimEnd()} which contain {MembersDescription}{_quantifier} ";
	}
}
