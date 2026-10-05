namespace aweXpect.Results;

/// <summary>
///     Marks a result whose expected <see langword="string" /> can be interpreted as a pattern, e.g. via
///     <c>AsWildcard()</c>, or as a prefix or suffix, via <c>AsPrefix()</c> or <c>AsSuffix()</c>.
/// </summary>
/// <remarks>
///     A result opts into these options by implementing this interface in addition to
///     <see cref="aweXpect.Core.IOptionsProvider{TOptions}" /> for the <see cref="aweXpect.Options.StringEqualityOptions" />.
///     <br />
///     A result that searches the expected <see langword="string" /> within a <see langword="string" />, where a prefix
///     or suffix has no meaning, implements only <see cref="IStringPatternMatchTypeOptions" />.
/// </remarks>
public interface IStringMatchTypeOptions : IStringPatternMatchTypeOptions;
