using System.Net;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

#nullable enable
public static partial class ThatIPAddress
{
	/// <summary>
	///     Verifies that the subject is a loopback address.
	/// </summary>
	/// <remarks>
	///     <seealso cref="IPAddress.IsLoopback(IPAddress)" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IPAddress, IThat<IPAddress?>> IsLoopback(this IThat<IPAddress?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsKindConstraint(it, grammars, IPAddress.IsLoopback, "a loopback address", "loopback addresses")),
			subject);

	/// <summary>
	///     Verifies that the subject is not a loopback address.
	/// </summary>
	/// <remarks>
	///     <seealso cref="IPAddress.IsLoopback(IPAddress)" />
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<IPAddress, IThat<IPAddress?>> IsNotLoopback(this IThat<IPAddress?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars) =>
				new IsKindConstraint(it, grammars, IPAddress.IsLoopback, "a loopback address", "loopback addresses")
					.Invert()),
			subject);
}
