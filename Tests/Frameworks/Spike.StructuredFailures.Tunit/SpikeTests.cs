using System.Threading.Tasks;
using TUnit.Core;

namespace aweXpect.Spike.Tunit;

public sealed class SpikeTests
{
	private const string Actual = "The quick brown fox\njumps over the lazy dog";
	private const string Expected = "The quick brown cat\njumps over the lazy dog";

	[Test]
	public async Task AweXpect_StringIsEqualTo()
		=> await Expect.That(Actual).IsEqualTo(Expected);

	[Test]
	public async Task TUnit_IsEqualTo()
		=> await Assert.That(Actual).IsEqualTo(Expected);
}
