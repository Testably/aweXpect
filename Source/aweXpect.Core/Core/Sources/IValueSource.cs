using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal interface IValueSource<TValue>
{
	ValueTask<TValue> GetValue(ITimeSystem timeSystem, CancellationToken cancellationToken);
}
