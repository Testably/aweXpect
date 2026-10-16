using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class IsNull
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNodeIsJsonNull_ShouldSucceed()
			{
				JsonNode? subject = FromString("null");

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).DoesNotThrow()
					.Because("the built-in expectation for references verifies the JSON null of a node");
			}

			[Fact]
			public async Task WhenNodeIsNotNull_ShouldFail()
			{
				JsonValue subject = JsonValue.Create(42);

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is null,
					             but it was 42
					             """);
			}
		}
	}
}
