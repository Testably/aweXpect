using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMember
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are private.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> ArePrivate<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
				=> new ArePrivateConstraint<TMember>(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are private.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>> ArePrivate<TMember>(
		this IThat<IAsyncEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
				=> new ArePrivateConstraint<TMember>(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not private.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreNotPrivate<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
				=> new AreNotPrivateConstraint<TMember>(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not private.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>> AreNotPrivate<TMember>(
		this IThat<IAsyncEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
				=> new AreNotPrivateConstraint<TMember>(it, grammars)),
			subject);
#endif

	private sealed class ArePrivateConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: CollectionConstraintResult<TMember>(grammars),
			IValueConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, member => member.HasAccessModifier(AccessModifiers.Private));
#endif

		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
			=> SetValue(actual, member => member.HasAccessModifier(AccessModifiers.Private));


		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are private");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all are private");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were");
	}

	private sealed class AreNotPrivateConstraint<TMember>(
		string it,
		ExpectationGrammars grammars)
		: CollectionConstraintResult<TMember>(grammars),
			IValueConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, member => !member.HasAccessModifier(AccessModifiers.Private));
#endif

		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
			=> SetValue(actual, member => !member.HasAccessModifier(AccessModifiers.Private));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are not private");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained private items ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("at least one is private");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("none were");
	}
}
