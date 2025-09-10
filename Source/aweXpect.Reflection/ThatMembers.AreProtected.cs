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
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are protected.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreProtected<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
				=> new AreProtectedConstraint<TMember>(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are protected.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>> AreProtected<TMember>(
		this IThat<IAsyncEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
				=> new AreProtectedConstraint<TMember>(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not protected.
	/// </summary>
	public static AndOrResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> AreNotProtected<TMember>(
		this IThat<IEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
				=> new AreNotProtectedConstraint<TMember>(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> are not protected.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>> AreNotProtected<TMember>(
		this IThat<IAsyncEnumerable<TMember>> subject)
		where TMember : MemberInfo?
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
				=> new AreNotProtectedConstraint<TMember>(it, grammars)),
			subject);
#endif

	private sealed class AreProtectedConstraint<TMember>(
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
			=> await SetAsyncValue(actual, member => member.HasAccessModifier(AccessModifiers.Protected));
#endif

		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
			=> SetValue(actual, member => member.HasAccessModifier(AccessModifiers.Protected));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all are protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all were");
	}

	private sealed class AreNotProtectedConstraint<TMember>(
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
			=> await SetAsyncValue(actual, member => !member.HasAccessModifier(AccessModifiers.Protected));
#endif

		public ConstraintResult IsMetBy(IEnumerable<TMember> actual)
			=> SetValue(actual, member => !member.HasAccessModifier(AccessModifiers.Protected));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all are not protected");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained protected items ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("at least one is protected");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("none were");
	}
}
