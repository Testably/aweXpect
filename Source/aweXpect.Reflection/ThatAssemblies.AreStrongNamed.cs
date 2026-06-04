using System.Collections.Generic;
using System.Reflection;
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
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> are strong named.
	/// </summary>
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>> AreStrongNamed(
		this IThat<IEnumerable<Assembly?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
				=> new AreStrongNamedConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> are strong named.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>>> AreStrongNamed(
		this IThat<IAsyncEnumerable<Assembly?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
				=> new AreStrongNamedConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> are not strong named.
	/// </summary>
	public static AndOrResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>>> AreNotStrongNamed(
		this IThat<IEnumerable<Assembly?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
				=> new AreNotStrongNamedConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> are not strong named.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>>> AreNotStrongNamed(
		this IThat<IAsyncEnumerable<Assembly?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
				=> new AreNotStrongNamedConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreStrongNamedConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Assembly?>(grammars),
			IValueConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, assembly => assembly.IsStrongNamed());
#endif

		public ConstraintResult IsMetBy(IEnumerable<Assembly?> actual)
			=> SetValue(actual, assembly => assembly.IsStrongNamed());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all strong named");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not strong named assemblies ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all strong named");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained strong named assemblies ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotStrongNamedConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<Assembly?>(grammars),
			IValueConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, assembly => !assembly.IsStrongNamed());
#endif

		public ConstraintResult IsMetBy(IEnumerable<Assembly?> actual)
			=> SetValue(actual, assembly => !assembly.IsStrongNamed());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not strong named");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained strong named assemblies ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a strong named assembly");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained not strong named assemblies ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
