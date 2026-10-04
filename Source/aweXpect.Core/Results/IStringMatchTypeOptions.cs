namespace aweXpect.Results;

/// <summary>
///     Marks a result whose expected <see langword="string" /> can be interpreted as a pattern, e.g. via
///     <c>AsWildcard()</c>.
/// </summary>
/// <remarks>
///     A result opts into these options by implementing this interface in addition to
///     <see cref="aweXpect.Core.IOptionsProvider{TOptions}" /> for the <see cref="aweXpect.Options.StringEqualityOptions" />.
/// </remarks>
public interface IStringMatchTypeOptions;
