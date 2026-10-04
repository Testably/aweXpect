using aweXpect.Core;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <inheritdoc />
	public IThat<TException> Which
		=> new ThatSubject<TException>(ExpectationBuilder.And(" that "));
}
