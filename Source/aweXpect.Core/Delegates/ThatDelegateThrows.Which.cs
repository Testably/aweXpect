using aweXpect.Core;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Further expectations on the <typeparamref name="TException" />.
	/// </summary>
	public IThat<TException> Which
		=> new ThatSubject<TException>(ExpectationBuilder.And(" which "));
}
