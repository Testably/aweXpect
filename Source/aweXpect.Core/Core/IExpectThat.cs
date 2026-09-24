using System.ComponentModel;

namespace aweXpect.Core;

/// <summary>
///     An expectation on a subject of type <typeparamref name="T" /> that gives access to its
///     <see cref="ExpectationBuilder" />.
/// </summary>
// ReSharper disable once UnusedTypeParameter
#pragma warning disable S2326 // 'T' is not used in the interface
public interface IExpectThat<out T> : IThat<T>
{
	/// <summary>
	///     The expectation builder.<br />
	///     <b>This property is only intended for extensions.</b>
	/// </summary>
	/// <remarks>
	///     Consider adding support for <see cref="EditorBrowsableAttribute" /> with state set to
	///     <see cref="EditorBrowsableState.Advanced" /> to hide this method from code suggestions.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	ExpectationBuilder ExpectationBuilder { get; }
}
#pragma warning restore S2326 // 'T' is not used in the interface
