using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="WeakReference" /> and <see cref="WeakReference{T}" /> values.
/// </summary>
public static partial class ThatWeakReference
{
	private const int MaxCollectionAttempts = 3;

	/// <remarks>
	///     Each attempt waits for the pending finalizers, which can release further objects.
	/// </remarks>
	private static bool IsCollected(Func<bool> isAlive)
	{
		for (int attempt = 0; attempt < MaxCollectionAttempts && isAlive(); attempt++)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		return !isAlive();
	}

	/// <remarks>
	///     The target obtained to check it is only stored in the frame of this method, which is gone when the collection
	///     is forced.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool IsAlive<T>(WeakReference<T> reference) where T : class
		=> reference.TryGetTarget(out _);

	private sealed class IsCollectedConstraint<TReference>(
		string it,
		ExpectationGrammars grammars,
		Func<TReference, bool> isAlive)
		: ConstraintResult.WithNotNullValue<TReference>(it, grammars),
			IValueConstraint<TReference>
		where TReference : class
	{
		public ConstraintResult IsMetBy(TReference actual)
		{
			Actual = actual;
			if (actual is not null)
			{
				Outcome = IsCollected(() => isAlive(actual)) ? Outcome.Success : Outcome.Failure;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is collected", "are collected"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were")).Append(" still alive");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not collected", "are not collected"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were")).Append(" collected");
	}
}
