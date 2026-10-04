namespace aweXpect.Results;

/// <summary>
///     Marks a result that verifies that a collection contains or is contained in another collection, so that the
///     two collections can be required to differ via <c>Properly()</c>.
/// </summary>
/// <remarks>
///     A result opts into this option by implementing this interface in addition to
///     <see cref="aweXpect.Core.IOptionsProvider{TOptions}" /> for the <see cref="aweXpect.Options.CollectionMatchOptions" />.
/// </remarks>
public interface IProperContainmentOptions;
