using System;

namespace aweXpect.Core;

/// <summary>
///     Marks options that never change: each option method returns new options and leaves the options it is called on
///     unchanged.
/// </summary>
/// <remarks>
///     The analyzer rule <c>aweXpect0009</c> uses this marker to report a call whose returned options are discarded, as
///     such a call has no effect. The option methods are the instance methods of the marked type and the extension
///     methods on it that return the same type.
///     <para />
///     A derived type is not marked by its base type, as it could add option methods that change it.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ImmutableOptionsAttribute : Attribute;
