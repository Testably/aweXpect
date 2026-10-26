using aweXpect.Core;

namespace aweXpect.Reflection.Options;

/// <summary>
///     A result that adds the parameter options of its expectation to the <see cref="ParameterFilterOptions" />.
/// </summary>
internal interface IParameterFilterOptionsProvider
{
	/// <summary>
	///     The parameter options of the expectation, which the constraint reads when it evaluates.
	/// </summary>
	OptionsHolder<ParameterFilterOptions> ParameterFilterOptions { get; }
}
