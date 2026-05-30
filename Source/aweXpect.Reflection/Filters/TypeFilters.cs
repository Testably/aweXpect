using System;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
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
		Quantifier quantifier = new();
		return new TypesContainingMembers(
			@this.Which(new ContainedMembersFilter<TMember, TFiltered>(navigate, filter, quantifier)),
			quantifier);
	}

	/// <summary>
	///     A filterable collection of <see cref="Type" /> that contain matching members, allowing to specify a quantifier
	///     for the number of matching members.
	/// </summary>
	public class TypesContainingMembers : Filtered.Types
	{
		private readonly Quantifier _quantifier;

		internal TypesContainingMembers(Filtered.Types inner, Quantifier quantifier) : base(inner)
		{
			_quantifier = quantifier;
		}

		/// <summary>
		///     Verifies, that the matching members occur at least…
		/// </summary>
		public CountTimesResult<Filtered.Types> AtLeast()
			=> new(value =>
			{
				_quantifier.AtLeast(value);
				return this;
			});

		/// <summary>
		///     Verifies, that the matching members occur at least <paramref name="minimum" /> times.
		/// </summary>
		public Filtered.Types AtLeast(Times minimum)
		{
			_quantifier.AtLeast(minimum.Value);
			return this;
		}

		/// <summary>
		///     Verifies, that the matching members occur at most…
		/// </summary>
		public CountTimesResult<Filtered.Types> AtMost()
			=> new(value =>
			{
				_quantifier.AtMost(value);
				return this;
			});

		/// <summary>
		///     Verifies, that the matching members occur at most <paramref name="maximum" /> times.
		/// </summary>
		public Filtered.Types AtMost(Times maximum)
		{
			_quantifier.AtMost(maximum.Value);
			return this;
		}

		/// <summary>
		///     Verifies, that the matching members occur between <paramref name="minimum" />…
		/// </summary>
		public BetweenResult<Filtered.Types> Between(int minimum)
			=> new(maximum =>
			{
				_quantifier.Between(minimum, maximum);
				return this;
			});

		/// <summary>
		///     Verifies, that the matching members occur exactly <paramref name="expected" /> times.
		/// </summary>
		public Filtered.Types Exactly(Times expected)
		{
			_quantifier.Exactly(expected.Value);
			return this;
		}

		/// <summary>
		///     Verifies, that the matching members occur less than…
		/// </summary>
		public CountTimesResult<Filtered.Types> LessThan()
			=> new(value =>
			{
				_quantifier.LessThan(value);
				return this;
			});

		/// <summary>
		///     Verifies, that the matching members occur less than <paramref name="maximum" /> times.
		/// </summary>
		public Filtered.Types LessThan(Times maximum)
		{
			_quantifier.LessThan(maximum.Value);
			return this;
		}

		/// <summary>
		///     Verifies, that the matching members occur more than…
		/// </summary>
		public CountTimesResult<Filtered.Types> MoreThan()
			=> new(value =>
			{
				_quantifier.MoreThan(value);
				return this;
			});

		/// <summary>
		///     Verifies, that the matching members occur more than <paramref name="minimum" /> times.
		/// </summary>
		public Filtered.Types MoreThan(Times minimum)
		{
			_quantifier.MoreThan(minimum.Value);
			return this;
		}

		/// <summary>
		///     Verifies, that no member matches the filter.
		/// </summary>
		public Filtered.Types Never()
		{
			_quantifier.Exactly(0);
			return this;
		}

		/// <summary>
		///     Verifies, that exactly one member matches the filter.
		/// </summary>
		public Filtered.Types Once()
		{
			_quantifier.Exactly(1);
			return this;
		}

		/// <summary>
		///     Verifies, that exactly two members match the filter.
		/// </summary>
		public Filtered.Types Twice()
		{
			_quantifier.Exactly(2);
			return this;
		}
	}

	private sealed class ContainedMembersFilter<TMember, TFiltered> : IFilter<Type>
		where TFiltered : Filtered<TMember, TFiltered>, IDescribableSubject
	{
		private readonly Func<TFiltered, TFiltered> _filter;
		private readonly string _membersDescription;
		private readonly Func<Filtered.Types, TFiltered> _navigate;
		private readonly Quantifier _quantifier;

		public ContainedMembersFilter(Func<Filtered.Types, TFiltered> navigate,
			Func<TFiltered, TFiltered> filter,
			Quantifier quantifier)
		{
			_navigate = navigate;
			_filter = filter;
			_quantifier = quantifier;

			// Derive the inner description from an empty probe, e.g. "methods with FactAttribute or with TheoryAttribute ".
			string inner = _filter(_navigate(new Filtered.Types([], ""))).GetDescription();
			if (inner.EndsWith("in "))
			{
				inner = inner.Substring(0, inner.Length - "in ".Length);
			}

			_membersDescription = inner;
		}

#if NET8_0_OR_GREATER
		public async ValueTask<bool> Applies(Type value)
		{
			int count = 0;
			await foreach (var _ in _filter(_navigate(new Filtered.Types([value,], ""))))
			{
				count++;
			}

			return _quantifier.Check(count, true) ?? false;
		}
#else
		public Task<bool> Applies(Type value)
		{
			int count = _filter(_navigate(new Filtered.Types([value,], ""))).Count();
			return Task.FromResult(_quantifier.Check(count, true) ?? false);
		}
#endif

		public string Describes(string text)
			=> $"{text.TrimEnd()} which contain {_membersDescription}{_quantifier} ";
	}
}
