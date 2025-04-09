using System;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Filters specify which entities must satisfy the requirements.
/// </summary>
internal static class Filter
{
	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IFilter<TEntity> Prefix<TEntity>(Func<TEntity, bool> predicate, string prefix)
		=> new GenericPrefixFilter<TEntity>(predicate, prefix);

	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, string suffix)
		=> new GenericSuffixFilter<TEntity>(predicate, suffix);

	/// <summary>
	///     Creates a new <see cref="IFilter{TEntity}" /> from the given <paramref name="predicate" />.
	/// </summary>
	public static IFilter<TEntity> Suffix<TEntity>(Func<TEntity, bool> predicate, Func<string> suffix)
		=> new GenericSuffixFuncFilter<TEntity>(predicate, suffix);

	private sealed class GenericPrefixFilter<TEntity> : IFilter<TEntity>
	{
		private readonly Func<TEntity, bool> _filter;
		private readonly string _prefix;

		public GenericPrefixFilter(Func<TEntity, bool> filter, string prefix)
		{
			_filter = filter;
			_prefix = prefix;
		}

		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity)" />
		public bool Applies(TEntity type)
			=> _filter(type);

		/// <inheritdoc cref="IFilter{TEntity}.Describes" />
		public string Describes(string text)
			=> _prefix + text;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> _prefix;
	}

	private sealed class GenericSuffixFilter<TEntity> : IFilter<TEntity>
	{
		private readonly Func<TEntity, bool> _filter;
		private readonly string _suffix;

		public GenericSuffixFilter(Func<TEntity, bool> filter, string suffix)
		{
			_filter = filter;
			_suffix = suffix;
		}

		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity)" />
		public bool Applies(TEntity type)
			=> _filter(type);

		/// <inheritdoc cref="IFilter{TEntity}.Describes" />
		public string Describes(string text)
			=> text + _suffix;

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> _suffix;
	}

	private sealed class GenericSuffixFuncFilter<TEntity> : IFilter<TEntity>
	{
		private readonly Func<TEntity, bool> _filter;
		private readonly Func<string> _suffix;

		public GenericSuffixFuncFilter(Func<TEntity, bool> filter, Func<string> suffix)
		{
			_filter = filter;
			_suffix = suffix;
		}

		/// <inheritdoc cref="IFilter{TEntity}.Applies(TEntity)" />
		public bool Applies(TEntity type)
			=> _filter(type);

		/// <inheritdoc cref="IFilter{TEntity}.Describes" />
		public string Describes(string text)
			=> text + _suffix();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
			=> _suffix();
	}
}
