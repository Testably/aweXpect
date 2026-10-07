using aweXpect.Core.Constraints;
using aweXpect.Core.Nodes;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Nodes;

public sealed class NullSubjectResultTests
{
	[Test]
	public async Task TryGetStoredValue_WhenTheTypeDoesNotMatch_ShouldReturnFalse()
	{
		ConstraintResult sut = NullSubjectResult.Create<string?>(new DummyConstraintResult(Outcome.Success), null);

		bool result = sut.TryGetStoredValue(out int value);

		await That(result).IsFalse();
		await That(value).IsEqualTo(0);
	}

	[Test]
	public async Task TryGetStoredValue_WhenTheValueHasTheType_ShouldReturnIt()
	{
		ConstraintResult sut = NullSubjectResult.CreateForNullTask(new DummyConstraintResult(Outcome.Success),
			"it", "foo");

		bool result = sut.TryGetStoredValue(out string? value);

		await That(result).IsTrue();
		await That(value).IsEqualTo("foo");
	}

	[Test]
	public async Task TryGetStoredValue_WithNullValueOfTheType_ShouldReturnTrue()
	{
		ConstraintResult sut = NullSubjectResult.Create<string?>(new DummyConstraintResult(Outcome.Success), null);

		bool result = sut.TryGetStoredValue(out string? value);

		await That(result).IsTrue()
			.Because("the result stores a value of the requested type, even though it is null");
		await That(value).IsNull();
	}
}
