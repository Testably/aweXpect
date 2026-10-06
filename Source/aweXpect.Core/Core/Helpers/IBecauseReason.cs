using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

internal interface IBecauseReason
{
	ValueTask<ConstraintResult>
		ApplyTo(ConstraintResult result);
}
