using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public sealed class HasValueAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonNode? subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasValueAt("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenIntermediateIndexDoesNotExist_ShouldFail()
			{
				JsonNode? subject = FromString("{\"tracks\": []}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[0].title", t => t.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "tracks[0].title" which is a string,
					             but index $.tracks[0] did not exist
					             """);
			}

			[Test]
			public async Task WhenIntermediatePropertyIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonNode? subject = FromString("{\"artist\": null}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.name", n => n.IsString()).TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.name" which is a string,
					             but property $.artist did not exist

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				JsonNode? subject = null;

				async Task Act()
					=> await That(subject).HasValueAt("foo", f => f.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "foo" which is a number,
					             but it was null
					             """);
			}

			[Test]
			public async Task WhenValueExists_ShouldPassTheNodeItself()
			{
				JsonObject track = new() { ["title"] = "Come Together", };
				JsonObject subject = new() { ["album"] = new JsonObject { ["tracks"] = new JsonArray(track), }, };

				async Task Act()
					=> await That(subject).HasValueAt("album.tracks[0]", t => t.IsSameAs(track));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsNull_ShouldPassANullNode()
			{
				JsonNode? subject = FromString("{\"tracks\": [null]}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[0]", t => t.IsNull());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
