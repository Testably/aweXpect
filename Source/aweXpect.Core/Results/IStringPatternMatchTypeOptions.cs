namespace aweXpect.Results;

/// <summary>
///     Marks a result whose expected <see langword="string" /> can be interpreted as a pattern, via
///     <c>AsWildcard()</c> or <c>AsRegex()</c>.
/// </summary>
/// <remarks>
///     A result opts into these options by implementing this interface in addition to
///     <see cref="aweXpect.Core.IOptionsProvider{TOptions}" /> for the <see cref="aweXpect.Options.StringEqualityOptions" />.
///     <br />
///     Implement <see cref="IStringMatchTypeOptions" /> instead, when the expected <see langword="string" /> is compared
///     with a whole <see langword="string" />, so that it can also be interpreted as a prefix or suffix of it.
/// </remarks>
public interface IStringPatternMatchTypeOptions;
