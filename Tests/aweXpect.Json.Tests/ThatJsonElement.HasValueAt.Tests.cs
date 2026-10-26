using System.Text.Json;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonElement
{
	public sealed class HasValueAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldFailWithThePathOfTheValue()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"title\": \"Something\"}]}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[0].title",
						t => t.IsString(s => s.IsEqualTo("Come Together")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "tracks[0].title" which is a string which is equal to "Come Together",
					             but $.tracks[0].title was "Something", which differs at index 0:
					                ↓ (actual)
					               "Something"
					               "Come Together"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasValueAt("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenFinalIndexDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"tracks\": [1, 2]}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[2]", t => t.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "tracks[2]" which is a number,
					             but index $.tracks[2] did not exist

					             Actual:
					             {
					               "tracks": [
					                 1,
					                 2
					               ]
					             }
					             """);
			}

			[Test]
			public async Task WhenFinalPropertyDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": {\"origin\": {}}}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.origin.city", c => c.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.origin.city" which is a string,
					             but property $.artist.origin.city did not exist

					             Actual:
					             {
					               "artist": {
					                 "origin": {}
					               }
					             }
					             """);
			}

			[Test]
			public async Task WhenIntermediateIndexDoesNotExist_ShouldOnlyReportTheFirstSegment()
			{
				JsonElement subject = FromString("{\"tracks\": []}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[0].title", t => t.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "tracks[0].title" which is a string,
					             but index $.tracks[0] did not exist

					             Actual:
					             {
					               "tracks": []
					             }
					             """);
			}

			[Test]
			public async Task WhenIntermediatePropertyDoesNotExist_ShouldOnlyReportTheFirstSegment()
			{
				JsonElement subject = FromString("{\"artist\": {}}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.origin.city", c => c.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.origin.city" which is a string,
					             but property $.artist.origin did not exist

					             Actual:
					             {
					               "artist": {}
					             }
					             """);
			}

			[Test]
			public async Task WhenIntermediatePropertyIsNull_ShouldReportItsKind()
			{
				JsonElement subject = FromString("{\"artist\": null}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.name", n => n.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.name" which is a string,
					             but $.artist was null instead of an object

					             Actual:
					             {
					               "artist": null
					             }
					             """);
			}

			[Test]
			public async Task WhenIntermediatePropertyIsNull_WhenTreatingNullAsAbsent_ShouldReportItAsMissing()
			{
				JsonElement subject = FromString("{\"artist\": null}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.name", n => n.IsString()).TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.name" which is a string,
					             but property $.artist did not exist

					             Actual:
					             {
					               "artist": null
					             }

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			[Arguments("1", "a number")]
			[Arguments("{}", "an object")]
			public async Task WhenIntermediateValueIsNoArray_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"tracks\": {value}}}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[0]", t => t.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               has value at "tracks[0]" which is a number,
					               but $.tracks was {{kindString}} instead of an array

					               Actual:
					               {
					                 "tracks": {{value}}
					               }
					               """);
			}

			[Test]
			[Arguments("1", "a number")]
			[Arguments("\"foo\"", "a string")]
			[Arguments("[]", "an array")]
			public async Task WhenIntermediateValueIsNoObject_ShouldFail(string value, string kindString)
			{
				JsonElement subject = FromString($"{{\"artist\": {value}}}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.name", n => n.IsString());

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               has value at "artist.name" which is a string,
					               but $.artist was {{kindString}} instead of an object

					               Actual:
					               {
					                 "artist": {{value}}
					               }
					               """);
			}

			[Test]
			public async Task WhenJsonIsNoObject_ShouldFail()
			{
				JsonElement subject = FromString("[1]");

				async Task Act()
					=> await That(subject).HasValueAt("foo", f => f.IsNumber());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "foo" which is a number,
					             but it was an array

					             Actual:
					             [
					               1
					             ]
					             """);
			}

			[Test]
			public async Task WhenPathIsEmpty_ShouldThrowArgumentException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasValueAt("", f => f.IsNumber());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("path").And
					.WithMessage("The 'path' cannot be empty.").AsPrefix();
			}

			[Test]
			[Arguments("$", "it is relative to the current object and must not start with '$'")]
			[Arguments("$.foo", "it is relative to the current object and must not start with '$'")]
			[Arguments("$[0]", "it is relative to the current object and must not start with '$'")]
			[Arguments(".foo", "expected a property name at position 0")]
			[Arguments("[0]", "expected a property name at position 0")]
			[Arguments("foo.", "expected a property name at position 4")]
			[Arguments("foo.[0]", "expected a property name at position 4")]
			[Arguments("foo..bar", "recursive descent ('..') is not supported")]
			[Arguments("*", "wildcards ('*') are not supported")]
			[Arguments("foo[*]", "wildcards ('*') are not supported")]
			[Arguments("foo[?(@.bar)]", "filters are not supported")]
			[Arguments("foo[bar]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[-1]", "expected a non-negative integer index at position 4")]
			[Arguments("foo[0", "the '[' at position 3 is not closed")]
			[Arguments("foo]", "expected '.' or '[' at position 3")]
			public async Task WhenPathIsInvalid_ShouldThrowArgumentException(string path, string reason)
			{
				JsonElement subject = FromString("{\"foo\": [{\"bar\": 1}]}");

				async Task Act()
					=> await That(subject).HasValueAt(path, f => f.IsNumber());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("path").And
					.WithMessage($"The path '{path}' is invalid: {reason}.").AsPrefix()
					.Because("an invalid path is rejected when the expectation is created");
			}

			[Test]
			public async Task WhenPathIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasValueAt(null!, f => f.IsNumber());

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'path' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenValueExistsAndSatisfiesTheExpectations_ShouldSucceed()
			{
				JsonElement subject = FromString(
					"{\"artist\": {\"origin\": {\"city\": \"Liverpool\"}}, \"tracks\": [{\"title\": \"Come Together\"}]}");

				async Task Act()
					=> await That(subject)
						.HasValueAt("artist.origin.city", c => c.IsString(s => s.IsEqualTo("Liverpool"))).And
						.HasValueAt("tracks[0].title", t => t.IsString());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsNull_ShouldVerifyTheJsonNull()
			{
				JsonElement subject = FromString("{\"artist\": {\"label\": null}}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.label", l => l.IsNull());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": {\"label\": null}}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.label", l => l.IsNull()).TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.label" which is null,
					             but property $.artist.label did not exist

					             Actual:
					             {
					               "artist": {
					                 "label": null
					               }
					             }

					             JSON options:
					              - treat null as absent
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenIntermediateValueHasAnotherKind_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": 1}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValueAt("artist.name", n => n.IsString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "artist.name" or $.artist.name is not a string,
					             but $.artist was a number instead of an object

					             Actual:
					             {
					               "artist": 1
					             }
					             """)
					.Because("the value cannot be looked up in a value of another kind");
			}

			[Test]
			public async Task WhenValueDoesNotExist_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"artist\": {}}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValueAt("artist.name", n => n.IsString()));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueExistsAndSatisfiesTheExpectations_ShouldFail()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"title\": \"Something\"}]}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasValueAt("tracks[0].title", t => t.IsString()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "tracks[0].title" or $.tracks[0].title is not a string,
					             but $.tracks[0].title was a string
					             """);
			}
		}

		public sealed class WithoutExpectationsTests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement subject = FromString("{\"artist\": {\"name\": \"The Beatles\"}}");

				JsonElement result = await That(subject).HasValueAt("artist.name");

				await That(result.GetRawText()).IsEqualTo(subject.GetRawText());
			}

			[Test]
			public async Task WhenIntermediateValueIsNoObject_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": [1]}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.name");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.name",
					             but $.artist was an array instead of an object

					             Actual:
					             {
					               "artist": [
					                 1
					               ]
					             }
					             """);
			}

			[Test]
			public async Task WhenPathIsNull_ShouldThrowArgumentNullException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).HasValueAt(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'path' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenValueDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"title\": \"Something\"}]}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[1].title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "tracks[1].title",
					             but index $.tracks[1] did not exist

					             Actual:
					             {
					               "tracks": [
					                 {
					                   "title": "Something"
					                 }
					               ]
					             }
					             """);
			}

			[Test]
			public async Task WhenValueExists_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"title\": \"Something\"}]}");

				async Task Act()
					=> await That(subject).HasValueAt("tracks[0].title");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsNull_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"artist\": {\"label\": null}}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.label");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsNull_WhenTreatingNullAsAbsent_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": {\"label\": null}}");

				async Task Act()
					=> await That(subject).HasValueAt("artist.label").TreatingNullAsAbsent();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.label",
					             but property $.artist.label did not exist

					             Actual:
					             {
					               "artist": {
					                 "label": null
					               }
					             }

					             JSON options:
					              - treat null as absent
					             """);
			}
		}
	}

	public sealed class DoesNotHaveValueAt
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				JsonElement subject = FromString("{\"artist\": {}}");

				JsonElement result = await That(subject).DoesNotHaveValueAt("artist.name");

				await That(result.GetRawText()).IsEqualTo(subject.GetRawText());
			}

			[Test]
			public async Task WhenIntermediatePropertyDoesNotExist_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"artist\": {}}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("artist.origin.city");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenIntermediateValueIsNoObject_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": \"The Beatles\"}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("artist.name");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "artist.name",
					             but $.artist was a string instead of an object

					             Actual:
					             {
					               "artist": "The Beatles"
					             }
					             """)
					.Because("the value cannot be looked up in a value of another kind");
			}

			[Test]
			public async Task WhenPathIsInvalid_ShouldThrowArgumentException()
			{
				JsonElement subject = FromString("{}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("foo[*]");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("path").And
					.WithMessage("The path 'foo[*]' is invalid: wildcards ('*') are not supported.").AsPrefix();
			}

			[Test]
			public async Task WhenValueExists_ShouldFail()
			{
				JsonElement subject = FromString("{\"tracks\": [{\"title\": \"Something\"}]}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("tracks[0].title");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "tracks[0].title",
					             but it had value at "tracks[0].title" with "Something"

					             Actual:
					             {
					               "tracks": [
					                 {
					                   "title": "Something"
					                 }
					               ]
					             }
					             """);
			}

			[Test]
			public async Task WhenValueIsNull_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": {\"label\": null}}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("artist.label");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "artist.label",
					             but it had value at "artist.label" with <null>

					             Actual:
					             {
					               "artist": {
					                 "label": null
					               }
					             }
					             """);
			}

			[Test]
			public async Task WhenValueIsNull_WhenTreatingNullAsAbsent_ShouldAllowFurtherExpectations()
			{
				JsonElement subject = FromString("{\"artist\": {\"label\": null}}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("artist.label").TreatingNullAsAbsent()
						.And.HasValueAt("artist.name");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have value at "artist.label" and has value at "artist.name",
					             but property $.artist.name did not exist

					             Actual:
					             {
					               "artist": {
					                 "label": null
					               }
					             }
					             """)
					.Because("the option only applies to the expectation it follows");
			}

			[Test]
			public async Task WhenValueIsNull_WhenTreatingNullAsAbsent_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"artist\": {\"label\": null}}");

				async Task Act()
					=> await That(subject).DoesNotHaveValueAt("artist.label").TreatingNullAsAbsent();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenValueDoesNotExist_ShouldFail()
			{
				JsonElement subject = FromString("{\"artist\": {}}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveValueAt("artist.name"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has value at "artist.name",
					             but property $.artist.name did not exist

					             Actual:
					             {
					               "artist": {}
					             }
					             """);
			}

			[Test]
			public async Task WhenValueExists_ShouldSucceed()
			{
				JsonElement subject = FromString("{\"artist\": {\"name\": \"The Beatles\"}}");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveValueAt("artist.name"));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
