using System;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatWeakReference
{
	/// <summary>
	///     Verifies that the target of the <see cref="WeakReference" /> is garbage collected.
	/// </summary>
	/// <remarks>
	///     Full blocking garbage collections are forced before the target is checked.
	///     <para />
	///     A strong reference that the test itself still holds keeps the target alive, e.g. a local variable in a Debug
	///     build, which lives until the end of the method. For an object the test creates itself, prefer
	///     <c>Expect.That(() =&gt; new MyClass()).IsCollectedAfter(x =&gt; x.Dispose())</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<WeakReference, IThat<WeakReference?>> IsCollected(
		this IThat<WeakReference?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsCollectedConstraint<WeakReference>(it, grammars, static reference => reference.IsAlive)),
			subject);

	/// <summary>
	///     Verifies that the target of the <see cref="WeakReference{T}" /> is garbage collected.
	/// </summary>
	/// <remarks>
	///     Full blocking garbage collections are forced before the target is checked.
	///     <para />
	///     A strong reference that the test itself still holds keeps the target alive, e.g. a local variable in a Debug
	///     build, which lives until the end of the method. For an object the test creates itself, prefer
	///     <c>Expect.That(() =&gt; new MyClass()).IsCollectedAfter(x =&gt; x.Dispose())</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<WeakReference<T>, IThat<WeakReference<T>?>> IsCollected<T>(
		this IThat<WeakReference<T>?> subject)
		where T : class
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsCollectedConstraint<WeakReference<T>>(it, grammars, IsAlive)),
			subject);

	/// <summary>
	///     Verifies that the target of the <see cref="WeakReference" /> is not garbage collected.
	/// </summary>
	/// <remarks>
	///     Full blocking garbage collections are forced before the target is checked.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<WeakReference, IThat<WeakReference?>> IsNotCollected(
		this IThat<WeakReference?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsCollectedConstraint<WeakReference>(it, grammars, static reference => reference.IsAlive)
					.Invert()),
			subject);

	/// <summary>
	///     Verifies that the target of the <see cref="WeakReference{T}" /> is not garbage collected.
	/// </summary>
	/// <remarks>
	///     Full blocking garbage collections are forced before the target is checked.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<WeakReference<T>, IThat<WeakReference<T>?>> IsNotCollected<T>(
		this IThat<WeakReference<T>?> subject)
		where T : class
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsCollectedConstraint<WeakReference<T>>(it, grammars, IsAlive).Invert()),
			subject);
}
