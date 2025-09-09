using System;
using System.Collections.Generic;
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
#if NET8_0_OR_GREATER
	public static IAsyncChangeableFilter<TEntity> Prefix<TEntity>(Func<TEntity, ValueTask<bool>> predicate,
		string prefix)
#else
	public static IAsyncChangeableFilter<TEntity> Prefix<TEntity>(Func<TEntity, Task<bool>> predicate, string prefix)
#endif
		=> new GenericAsyncPrefixFilter<TEntity>(predicate, prefix);

	/// <summary>
	///     Creates a new prefix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IChangeableFilter<TEntity> Prefix<TEntity>(Func<TEntity, bool> predicate, string prefix)
		=> new GenericPrefixFilter<TEntity>(predicate, prefix);

	/// <summary>
	///     Creates a new suffix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
#if NET8_0_OR_GREATER
	public static IAsyncChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, ValueTask<bool>> predicate,
		string suffix)
#else
	public static IAsyncChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, Task<bool>> predicate, string suffix)
#endif
		=> new GenericAsyncSuffixFilter<TEntity>(predicate, suffix);

	/// <summary>
	///     Creates a new suffix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, string suffix)
		=> new GenericSuffixFilter<TEntity>(predicate, suffix);

	/// <summary>
	///     Creates a new suffix <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, Func<string> suffix)
		=> new GenericSuffixFuncFilter<TEntity>(predicate, suffix);

	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
#if NET8_0_OR_GREATER
	public static IAsyncChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, ValueTask<bool>> predicate,
		Func<string> suffix)
#else
	public static IAsyncChangeableFilter<TEntity> Suffix<TEntity>(Func<TEntity, Task<bool>> predicate, Func<string> suffix)
#endif
		=> new GenericAsyncSuffixFuncFilter<TEntity>(predicate, suffix);

#if NET8_0_OR_GREATER
	private abstract class GenericAsyncFilter<TEntity>(Func<TEntity, ValueTask<bool>> filter)
		: IAsyncChangeableFilter<TEntity>
#else
	private abstract class GenericAsyncFilter<TEntity>(Func<TEntity, Task<bool>> filter) : IAsyncChangeableFilter<TEntity>
#endif
	{
		private List<Func<string, string>>? _descriptions;
#if NET8_0_OR_GREATER
		private List<Func<bool, TEntity, ValueTask<bool>>>? _predicates;
#else
		private List<Func<bool, TEntity, Task<bool>>>? _predicates;
#endif

		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity)" />
#if NET8_0_OR_GREATER
		public async ValueTask<bool> Applies(TEntity value)
#else
		public async Task<bool> Applies(TEntity value)
#endif
		{
			bool result = await filter(value);
			if (_predicates != null)
			{
#if NET8_0_OR_GREATER
				foreach (Func<bool, TEntity, ValueTask<bool>> predicate in _predicates)
#else
				foreach (Func<bool, TEntity, Task<bool>> predicate in _predicates)
#endif
				{
					result = await predicate(result, value);
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
				foreach (Func<string, string> description in _descriptions)
				{
					result = description(result);
				}
			}

			return result;
		}

#if NET8_0_OR_GREATER
		/// <inheritdoc
		///     cref="IAsyncChangeableFilter{TEntity}.UpdateFilter(Func{bool, TEntity, ValueTask{bool}}, Func{string, string})" />
		public void UpdateFilter(Func<bool, TEntity, ValueTask<bool>> predicate, Func<string, string> description)
#else
		/// <inheritdoc cref="IAsyncChangeableFilter{TEntity}.UpdateFilter(Func{bool, TEntity, Task{bool}}, Func{string, string})" />
		public void UpdateFilter(Func<bool, TEntity, Task<bool>> predicate, Func<string, string> description)
#endif
		{
			_predicates ??= [];
			_predicates.Add(predicate);
			_descriptions ??= [];
			_descriptions.Add(description);
		}

		protected abstract string DescribeCore(string text);
	}

	private abstract class GenericFilter<TEntity>(Func<TEntity, bool> filter) : IChangeableFilter<TEntity>
	{
		private List<Func<string, string>>? _descriptions;
		private List<Func<bool, TEntity, bool>>? _predicates;

		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity)" />
#if NET8_0_OR_GREATER
		public ValueTask<bool> Applies(TEntity value)
#else
		public Task<bool> Applies(TEntity value)
#endif
		{
			bool result = filter(value);
			if (_predicates != null)
			{
				foreach (Func<bool, TEntity, bool> predicate in _predicates)
				{
					result = predicate(result, value);
				}
			}

#if NET8_0_OR_GREATER
			return ValueTask.FromResult(result);
#else
			return Task.FromResult(result);
#endif
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

#if NET8_0_OR_GREATER
	private sealed class GenericAsyncPrefixFilter<TEntity>(Func<TEntity, ValueTask<bool>> filter, string prefix)
#else
	private sealed class GenericAsyncPrefixFilter<TEntity>(Func<TEntity, Task<bool>> filter, string prefix)
#endif
		: GenericAsyncFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericAsyncFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> prefix + text;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> prefix;
	}

#if NET8_0_OR_GREATER
	private sealed class GenericAsyncSuffixFilter<TEntity>(Func<TEntity, ValueTask<bool>> filter, string suffix)
#else
	private sealed class GenericAsyncSuffixFilter<TEntity>(Func<TEntity, Task<bool>> filter, string suffix)
#endif
		: GenericAsyncFilter<TEntity>(filter)
	{
		/// <inheritdoc cref="GenericAsyncFilter{TEntity}.DescribeCore(string)" />
		protected override string DescribeCore(string text)
			=> text + suffix;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> suffix;
	}

#if NET8_0_OR_GREATER
	private sealed class GenericAsyncSuffixFuncFilter<TEntity>(
		Func<TEntity, ValueTask<bool>> filter,
		Func<string> suffix)
#else
	private sealed class GenericAsyncSuffixFuncFilter<TEntity>(Func<TEntity, Task<bool>> filter, Func<string> suffix)
#endif
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
