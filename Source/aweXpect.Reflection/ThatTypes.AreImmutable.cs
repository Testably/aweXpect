using System;
using System.Collections.Generic;
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

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreImmutable(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreImmutableConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreImmutable(
		this IThat<IAsyncEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreImmutableConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>>> AreNotImmutable(
		this IThat<IEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Type?>>((it, grammars)
				=> new AreNotImmutableConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not immutable.
	/// </summary>
	/// <remarks>
	///     A type is considered immutable when all instance fields (including inherited ones) are
	///     <see langword="readonly" /> and all instance properties (including inherited ones) have no setter
	///     or an init-only setter.
	/// </remarks>
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>>> AreNotImmutable(
		this IThat<IAsyncEnumerable<Type?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Type?>>((it, grammars)
				=> new AreNotImmutableConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreImmutableConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => type.IsImmutable());
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => type.IsImmutable());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all immutable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained mutable types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all immutable");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained immutable types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotImmutableConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Type?>(grammars),
			IValueConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, type => !type.IsImmutable());
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual)
			=> SetValue(actual, type => !type.IsImmutable());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not immutable");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained immutable types ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain an immutable type");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained mutable types ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
