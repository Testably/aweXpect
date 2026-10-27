using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Filters specify which entities must satisfy the requirements.
/// </summary>
internal static class Filter
{
	/// <summary>
	///     Creates a new prefix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IFilter<TEntity> Prefix<TEntity>(Func<TEntity, ValueTask<bool>> predicate,
		string prefix)
		=> new GenericAsyncPrefixFilter<TEntity>((value, _) => predicate(value), prefix);

	/// <summary>
	///     Creates a new prefix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static GenericFilter<TEntity> Prefix<TEntity>(Func<TEntity, bool> predicate, string prefix)
		=> new GenericPrefixFilter<TEntity>(predicate, prefix);

	/// <summary>
	///     Creates a new suffix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IFilter<TEntity> Suffix<TEntity>(Func<TEntity, ValueTask<bool>> predicate,
		string suffix)
		=> new GenericAsyncSuffixFilter<TEntity>((value, _) => predicate(value), suffix);

	/// <summary>
	///     Creates a new suffix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static GenericFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, string suffix)
		=> new GenericSuffixFilter<TEntity>(predicate, suffix);

	/// <summary>
	///     Creates a new suffix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static GenericFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, Func<string> suffix)
		=> new GenericSuffixFuncFilter<TEntity>(predicate, suffix);

	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IFilter<TEntity> Suffix<TEntity>(Func<TEntity, ValueTask<bool>> predicate,
		Func<string> suffix)
		=> new GenericAsyncSuffixFuncFilter<TEntity>((value, _) => predicate(value), suffix);

	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />, which receives the
	///     token of the enumeration.
	/// </summary>
	public static IFilter<TEntity> Suffix<TEntity>(Func<TEntity, CancellationToken, ValueTask<bool>> predicate,
		Func<string> suffix)
		=> new GenericAsyncSuffixFuncFilter<TEntity>(predicate, suffix);

	private abstract class GenericAsyncFilter<TEntity>(Func<TEntity, CancellationToken, ValueTask<bool>> filter)
		: IFilter<TEntity>
	{
		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity, CancellationToken)" />
		public ValueTask<bool> Applies(TEntity value, CancellationToken cancellationToken)
			=> filter(value, cancellationToken);

		/// <inheritdoc cref="IFilter{TEntity}.Describes(string)" />
		public string Describes(string text)
			=> DescribeCore(text);

		protected abstract string DescribeCore(string text);
	}

	/// <summary>
	///     A synchronous <see cref="IFilter{TEntity}" /> that can be extended without changing it.
	/// </summary>
	internal abstract class GenericFilter<TEntity>(Func<TEntity, bool> filter) : IFilter<TEntity>
	{
		private List<Func<string, string>>? _descriptions;
		private List<Func<bool, TEntity, bool>>? _predicates;

		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity, CancellationToken)" />
		public ValueTask<bool> Applies(TEntity value, CancellationToken cancellationToken)
		{
			bool result = filter(value);
			if (_predicates != null)
			{
				foreach (Func<bool, TEntity, bool> predicate in _predicates)
				{
					result = predicate(result, value);
				}
			}

			return new ValueTask<bool>(result);
		}

		/// <inheritdoc cref="IFilter{TEntity}.Describes(string)" />
		public string Describes(string text)
		{
			string result = DescribeCore(text);
			if (_descriptions != null)
			{
				foreach (Func<string, string> description in _descriptions)
				{
					result = description(result);
				}
			}

			return result;
		}

		/// <summary>
		///     Returns a copy of this filter that additionally applies the <paramref name="predicate" /> on the original
		///     result and updates the <paramref name="description" />, while this filter stays unchanged.
		/// </summary>
		public GenericFilter<TEntity> Extend(Func<bool, TEntity, bool> predicate, Func<string, string> description)
		{
			GenericFilter<TEntity> copy = (GenericFilter<TEntity>)MemberwiseClone();
			copy._predicates = _predicates is null ? [predicate,] : [.._predicates, predicate,];
			copy._descriptions = _descriptions is null ? [description,] : [.._descriptions, description,];
			return copy;
		}

		protected abstract string DescribeCore(string text);
	}

	private sealed class GenericAsyncPrefixFilter<TEntity>(
		Func<TEntity, CancellationToken, ValueTask<bool>> filter,
		string prefix)
		: GenericAsyncFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericAsyncFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> prefix + text;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> prefix;
	}

	private sealed class GenericAsyncSuffixFilter<TEntity>(
		Func<TEntity, CancellationToken, ValueTask<bool>> filter,
		string suffix)
		: GenericAsyncFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericAsyncFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> text + suffix;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> suffix;
	}

	private sealed class GenericAsyncSuffixFuncFilter<TEntity>(
		Func<TEntity, CancellationToken, ValueTask<bool>> filter,
		Func<string> suffix)
		: GenericAsyncFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericAsyncFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> text + suffix();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> suffix();
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
