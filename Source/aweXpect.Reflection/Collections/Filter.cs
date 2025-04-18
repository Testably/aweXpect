using System;
using System.Collections.Generic;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Filters specify which entities must satisfy the requirements.
/// </summary>
internal static class Filter
{
	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IChangeableFilter<TEntity> Prefix<TEntity>(Func<TEntity, bool> predicate, string prefix)
		=> new GenericPrefixFilter<TEntity>(predicate, prefix);

	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, string suffix)
		=> new GenericSuffixFilter<TEntity>(predicate, suffix);

	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, Func<string> suffix)
		=> new GenericSuffixFuncFilter<TEntity>(predicate, suffix);

	private abstract class GenericFilter<TEntity>(Func<TEntity, bool> filter) : IChangeableFilter<TEntity>
	{
		private List<Func<string, string>>? _descriptions;
		private List<Func<bool, TEntity, bool>>? _predicates;

		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity)" />
		public bool Applies(TEntity value)
		{
			bool result = filter(value);
			if (_predicates != null)
			{
				foreach (Func<bool, TEntity, bool>? predicate in _predicates)
				{
					result = predicate(result, value);
				}
			}

			return result;
		}

		/// <inheritdoc cref="IFilter{TEntity}.Describes(string)" />
		public string Describes(string text)
		{
			string result = DescribeCore(text);
			if (_descriptions != null)
			{
				foreach (Func<string, string>? description in _descriptions)
				{
					result = description(result);
				}
			}

			return result;
		}

		/// <inheritdoc cref="IChangeableFilter{TEntity}.UpdateFilter(Func{bool, TEntity, bool}, Func{string, string})" />
		public void UpdateFilter(Func<bool, TEntity, bool> predicate, Func<string, string> description)
		{
			_predicates ??= [];
			_predicates.Add(predicate);
			_descriptions ??= [];
			_descriptions.Add(description);
		}

		protected abstract string DescribeCore(string text);
	}

	private sealed class GenericPrefixFilter<TEntity>(Func<TEntity, bool> filter, string prefix)
		: GenericFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> prefix + text;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> prefix;
	}

	private sealed class GenericSuffixFilter<TEntity>(Func<TEntity, bool> filter, string suffix)
		: GenericFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> text + suffix;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> suffix;
	}

	private sealed class GenericSuffixFuncFilter<TEntity>(Func<TEntity, bool> filter, Func<string> suffix)
		: GenericFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> text + suffix();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> suffix();
	}
}
