using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using aweXpect.Customization;
using aweXpect.Equivalency;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonObject
{
	public sealed class IsJsonSerializedAs
	{
		public sealed class JsonTests
		{
			[Test]
			public async Task WhenExpectedIsInvalidJson_ShouldThrowArgumentException()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("{\"Title\":");

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'expected' JSON is invalid: *").AsWildcard();
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithMessage("The 'expected' JSON cannot be null.*").AsWildcard()
					.Because("a null string is no JSON, while the JSON null is written as \"null\"");
			}

			[Test]
			public async Task WhenSerializationThrows_ShouldFail()
			{
				Node subject = new();
				subject.Next = subject;

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("{}");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {},
					             but it could not be serialized: A possible object cycle was detected.*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSerializedHasAdditionalProperties_ShouldFail()
			{
				Track subject = new("Let It Be", Genre.Rock, "Let It Be");

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2}""");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {"Title": "Let It Be", "Genre": 2},
					             but it differed as $.Album had unexpected "Let It Be"

					             Serialized:
					             {"Title":"Let It Be","Genre":2,"Album":"Let It Be"}
					             """)
					.Because("the serialized JSON is compared exactly by default");
			}

			[Test]
			public async Task WhenSerializedHasAdditionalProperties_WhenIgnoringAdditionalProperties_ShouldSucceed()
			{
				Track subject = new("Let It Be", Genre.Rock, "Let It Be");

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be"}""",
						o => o.IgnoringAdditionalProperties());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSerializedMatches_ShouldSucceed()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""
					                                      {
					                                        "Genre": 2,
					                                        "Title": "Let It Be",
					                                        "Album": null
					                                      }
					                                      """);

				await That(Act).DoesNotThrow()
					.Because("the JSON is compared by its structure, regardless of formatting and property order");
			}

			[Test]
			public async Task WhenSerializedMatches_WhenCombinedWithFailingExpectation_ShouldNotAddSerializedContext()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2, "Album": null}""")
						.And.IsNull();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {"Title": "Let It Be", "Genre": 2, "Album": null} and is null,
					             but it was ThatJsonObject.IsJsonSerializedAs.Track {
					                 Album = <null>,
					                 Genre = Rock,
					                 Title = "Let It Be"
					               }
					             """)
					.Because("the serialized JSON only adds to the failure of the serialization expectation");
			}

			[Test]
			public async Task WhenSubjectIsDeclaredAsObject_ShouldSerializeTheRuntimeType()
			{
				object subject = new Track("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2, "Album": null}""");

				await That(Act).DoesNotThrow()
					.Because("the serializer writes the runtime type of a value declared as object");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldBeSerializedAsJsonNull()
			{
				Track? subject = null;

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("null");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_WhenExpectingAnObject_ShouldFail()
			{
				Track? subject = null;

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("{}");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {},
					             but it differed as $ was <null> instead of {}

					             Serialized:
					             null
					             """);
			}

			[Test]
			public async Task WhenSubjectIsValueType_ShouldSucceed()
			{
				int subject = 42;

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("42");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueDiffers_ShouldFail()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Yesterday", "Genre": 2, "Album": null}""");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {"Title": "Yesterday", "Genre": 2, "Album": null},
					             but it differed as $.Title was "Let It Be" instead of "Yesterday"

					             Serialized:
					             {"Title":"Let It Be","Genre":2,"Album":null}
					             """);
			}

			[Test]
			public async Task WithCustomizedDefaultSerializerOptions_ShouldUseThem()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"title": "Let It Be", "genre": 2, "album": null}""");

				using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
				       {
					       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				       }))
				{
					await That(Act).DoesNotThrow();
				}
			}

			[Test]
			public async Task WithSerializerOptions_ShouldUseThem()
			{
				Track subject = new("Let It Be", Genre.Rock);
				JsonSerializerOptions serializerOptions = new()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
					Converters =
					{
						new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
					},
				};

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"title": "Let It Be", "genre": "rock"}""",
						serializerOptions);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSerializerOptions_WhenValueDiffers_ShouldFail()
			{
				Track track = new("Let It Be", Genre.Rock);
				JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

				async Task Act()
					=> await That(track).IsJsonSerializedAs("""{"title": "Let It Be", "genre": "rock"}""",
						serializerOptions);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that track
					             is serialized as JSON {"title": "Let It Be", "genre": "rock"},
					             but it differed as
					               $.genre was number 2 instead of "rock" and
					               $.album had unexpected <null>

					             Serialized:
					             {"title":"Let It Be","genre":2,"album":null}
					             """)
					.Because("it is the failure message shown in the documentation");
			}
		}

		public sealed class ObjectTests
		{
			[Test]
			public async Task WhenExpectedContainsItIs_ShouldEvaluateIt()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						Title = It.Is<string>().That.StartsWith("Yes"),
						Genre = 2,
						Album = (string?)null,
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON new
					             {
					             	Title = It.Is<string>().That.StartsWith("Yes"),
					             	Genre = 2,
					             	Album = (string?)null,
					             },
					             but it differed as $.Title was "Let It Be", which differs at index 0:*

					             Serialized:
					             {"Title":"Let It Be","Genre":2,"Album":null}
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenExpectedContainsMatchingItIs_ShouldSucceed()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						Title = It.Is<string>().That.StartsWith("Let"),
						Genre = 2,
						Album = (string?)null,
					});

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSerializationThrows_ShouldFail()
			{
				Node subject = new();
				subject.Next = subject;

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON new
					             {
					             },
					             but it could not be serialized: A possible object cycle was detected.*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSerializedHasAdditionalProperties_ShouldFail()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						Title = "Let It Be",
					});

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON new
					             {
					             	Title = "Let It Be",
					             },
					             but it differed as
					               $.Genre had unexpected 2 and
					               $.Album had unexpected <null>

					             Serialized:
					             {"Title":"Let It Be","Genre":2,"Album":null}
					             """)
					.Because("the serialized JSON is compared exactly by default");
			}

			[Test]
			public async Task WhenSerializedMatches_ShouldSucceed()
			{
				Track subject = new("Let It Be", Genre.Rock, "Let It Be");

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						Title = "Let It Be",
						Genre = 2,
						Album = "Let It Be",
					});

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldBeSerializedAsJsonNull()
			{
				Track? subject = null;

				async Task Act()
					=> await That(subject).IsJsonSerializedAs((object?)null);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithCustomizedDefaultSerializerOptions_ShouldNotSerializeTheExpectedValue()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						Title = "Let It Be",
						Genre = 2,
						Album = (string?)null,
					});

				using (Customize.aweXpect.Json().DefaultJsonSerializerOptions.Set(new JsonSerializerOptions
				       {
					       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				       }))
				{
					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is serialized as JSON new
						             {
						             	Title = "Let It Be",
						             	Genre = 2,
						             	Album = (string?)null,
						             },
						             but it differed as
						               $.Title had missing "Let It Be" and
						               $.Genre had missing 2 and
						               $.Album had missing <null> and
						               $.title had unexpected "Let It Be" and
						               $.genre had unexpected 2 and
						               $.album had unexpected <null>

						             Serialized:
						             {"title":"Let It Be","genre":2,"album":null}
						             """)
						.Because("a customized naming policy must not hide that the subject is serialized differently");
				}
			}

			[Test]
			public async Task WithExpectedSerializerOptions_ShouldSerializeTheExpectedValue()
			{
				Track subject = new("Let It Be", Genre.Rock);
				JsonSerializerOptions serializerOptions = new()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				};

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						Title = "Let It Be",
						Genre = 2,
						Album = (string?)null,
					}, serializerOptions, o => o.WithSerializerOptions(serializerOptions));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSerializerOptions_ShouldUseThem()
			{
				Track subject = new("Let It Be", Genre.Rock);
				JsonSerializerOptions serializerOptions = new()
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
					Converters =
					{
						new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
					},
				};

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						title = "Let It Be",
						genre = "rock",
					}, serializerOptions);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WithSerializerOptions_WhenIgnoringAdditionalProperties_ShouldFailForDifferentValue()
			{
				Track subject = new("Let It Be", Genre.Rock);
				JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs(new
					{
						genre = "rock",
					}, serializerOptions, o => o.IgnoringAdditionalProperties());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON new
					             {
					             	genre = "rock",
					             },
					             but it differed as $.genre was number 2 instead of "rock"

					             Serialized:
					             {"title":"Let It Be","genre":2,"album":null}

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		public sealed class OptionsTests
		{
			[Test]
			public async Task IgnoringAdditionalArrayElements_ShouldIgnoreAdditionalElements()
			{
				Album subject = new("Let It Be", ["Two of Us", "Dig a Pony", "Across the Universe",]);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Tracks": ["Two of Us"]}""",
						o => o.IgnoringAdditionalArrayElements());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringArrayOrder_ShouldIgnoreTheOrderOfElements()
			{
				Album subject = new("Let It Be", ["Two of Us", "Dig a Pony",]);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Tracks": ["Dig a Pony", "Two of Us"]}""",
						o => o.IgnoringArrayOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringArrayOrder_WhenElementDiffers_ShouldListTheOption()
			{
				Album subject = new("Let It Be", ["Two of Us", "Dig a Pony",]);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Tracks": ["Dig a Pony", "Get Back"]}""",
						o => o.IgnoringArrayOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {"Title": "Let It Be", "Tracks": ["Dig a Pony", "Get Back"]},
					             but it differed as $.Tracks[0] was "Two of Us" instead of "Get Back"

					             Serialized:
					             {"Title":"Let It Be","Tracks":["Two of Us","Dig a Pony"]}

					             JSON options:
					              - ignore array order
					             """);
			}

			[Test]
			public async Task IgnoringMember_ShouldIgnoreTheMember()
			{
				Track subject = new("Let It Be", Genre.Rock, "Let It Be");

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2}""",
						o => o.IgnoringMember("Album"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringMember_WhenNameMatchesNoMember_ShouldSuggestTheMember()
			{
				Track subject = new("Let It Be", Genre.Rock, "Let It Be");

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2}""",
						o => o.IgnoringMember("Albun"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {"Title": "Let It Be", "Genre": 2},
					             but it differed as $.Album had unexpected "Let It Be"

					             Serialized:
					             {"Title":"Let It Be","Genre":2,"Album":"Let It Be"}

					             JSON options:
					              - ignore members: ["Albun"]
					                "Albun" matched no member (did you mean "Album"?)
					             """);
			}

			[Test]
			public async Task TreatingNullAsAbsent_ShouldTreatNullAsAbsent()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 2}""",
						o => o.TreatingNullAsAbsent());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class TypeInfoTests
		{
			[Test]
			public async Task WhenSerializationThrows_ShouldFail()
			{
				Node subject = new();
				subject.Next = subject;

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("{}", GetTypeInfo<Node>(new JsonSerializerOptions()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {},
					             but it could not be serialized: A possible object cycle was detected.*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenSerializedMatches_ShouldSucceed()
			{
				Track subject = new("Let It Be", Genre.Rock);
				JsonTypeInfo<Track> typeInfo = GetTypeInfo<Track>(new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				});

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"title": "Let It Be", "genre": 2, "album": null}""",
						typeInfo);

				await That(Act).DoesNotThrow()
					.Because("the type info carries the naming policy of its options");
			}

			[Test]
			public async Task WhenValueDiffers_ShouldFail()
			{
				Track subject = new("Let It Be", Genre.Rock);
				JsonTypeInfo<Track> typeInfo = GetTypeInfo<Track>(new JsonSerializerOptions());

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("""{"Title": "Let It Be", "Genre": 1}""",
						typeInfo, o => o.TreatingNullAsAbsent());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is serialized as JSON {"Title": "Let It Be", "Genre": 1},
					             but it differed as $.Genre was 2 instead of 1

					             Serialized:
					             {"Title":"Let It Be","Genre":2,"Album":null}

					             JSON options:
					              - treat null as absent
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldBeSerializedAsJsonNull()
			{
				Track? subject = null;
				JsonTypeInfo<Track> typeInfo = GetTypeInfo<Track>(new JsonSerializerOptions());

				async Task Act()
					=> await That(subject).IsJsonSerializedAs("null", typeInfo);

				await That(Act).DoesNotThrow();
			}

			private static JsonTypeInfo<T> GetTypeInfo<T>(JsonSerializerOptions options)
			{
				options.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
				return (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSerializationThrows_ShouldFail()
			{
				Node subject = new();
				subject.Next = subject;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializedAs("{}"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serialized as JSON {},
					             but it could not be serialized: A possible object cycle was detected.*
					             """).AsWildcard()
					.Because("a subject that cannot be serialized is not serialized as anything else either");
			}

			[Test]
			public async Task WhenSerializedDiffers_ShouldSucceed()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializedAs(new
					{
						Title = "Yesterday",
					}));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSerializedMatches_ShouldFail()
			{
				Track subject = new("Let It Be", Genre.Rock);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsJsonSerializedAs("""{"Title": "Let It Be"}""",
						o => o.IgnoringAdditionalProperties()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not serialized as JSON {"Title": "Let It Be"},
					             but it was

					             Serialized:
					             {"Title":"Let It Be","Genre":2,"Album":null}

					             JSON options:
					              - ignore additional properties
					             """);
			}
		}

		private enum Genre
		{
			Pop = 1,
			Rock = 2,
		}

		private sealed record Track(string Title, Genre Genre, string? Album = null);

		private sealed record Album(string Title, IReadOnlyList<string> Tracks);

		private sealed class Node
		{
			public Node? Next { get; set; }
		}
	}
}
