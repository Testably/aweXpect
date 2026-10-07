using aweXpect.Helpers;

namespace aweXpect.Internal.Tests.Helpers;

public sealed class DefaultEqualityTests
{
	[Test]
	public async Task IsUsedBy_WhenOptionsAreOfAnUnknownType_ShouldReturnFalse()
	{
		object options = new();

		bool result = DefaultEquality.IsUsedBy(options);

		await That(result).IsFalse()
			.Because("only options that are known to compare by their default equality may leave it to the subject");
	}
}
