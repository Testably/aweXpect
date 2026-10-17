using System.Collections.Generic;
using System.IO;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class AcceptingTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task ShouldAllowCombiningWithFurtherExpectations()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All)
						.And.IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAcceptingAll_AndAcceptingThrows_ShouldNameTheErrorAndRecordAMismatchInstead()
			{
				InMemorySnapshotStore store = new()
				{
					AcceptException = new IOException("read-only"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             foo

					             Accepted snapshot:
					             could not be written: read-only
					             """);
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				}).Because("the value can still be reviewed and accepted by hand");
			}

			[Fact]
			public async Task WhenAcceptingAll_AndAcceptingAndRecordingThrow_ShouldNameBothErrors()
			{
				InMemorySnapshotStore store = new()
				{
					AcceptException = new IOException("read-only"),
					RecordMismatchException = new IOException("disk full"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             foo

					             Accepted snapshot:
					             could not be written: read-only

					             Pending snapshot:
					             could not be written: disk full
					             """);
			}

			[Fact]
			public async Task WhenAcceptingAll_AndNegated_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).Accepting(SnapshotAcceptance.All));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Fact]
			public async Task WhenAcceptingAll_AndReadingThrows_ShouldNotAccept()
			{
				InMemorySnapshotStore store = new()
				{
					ReadException = new IOException("locked"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot could not be read: locked").AsWildcard();
				await That(store.Accepted).IsEmpty()
					.Because("an unreadable snapshot might match, so it must not be replaced");
			}

			[Fact]
			public async Task WhenAcceptingAll_AndSnapshotDiffers_ShouldAcceptTheSubjectAndFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo baz";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was "foo baz", which differs at index 6:
					                      ↓ (actual)
					               "foo baz"
					               "foo bar"
					                      ↑ (expected)

					             Accepted snapshot:
					             MyTests.MyTest
					             """)
					.Because("an accepted snapshot must not go unnoticed");
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo baz",
				});
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenAcceptingAll_AndSnapshotIsMissing_ShouldAcceptTheSubjectAndFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             foo

					             Accepted snapshot:
					             MyTests.MyTest
					             """);
				await That(Act).DoesNotThrow()
					.Because("the next run compares with the accepted snapshot");
			}

			[Fact]
			public async Task WhenAcceptingAll_AndSnapshotIsUsedForAnotherValue_ShouldNotAccept()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("*was already used for another value in this test run*").AsWildcard();
				await That(store.Accepted).IsEmpty()
					.Because("the values would replace each other's snapshot");
			}

			[Fact]
			public async Task WhenAcceptingAll_AndSnapshotMatches_ShouldSucceedWithoutAccepting()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).DoesNotThrow();
				await That(store.Accepted).IsEmpty();
				await That(store.Matches).IsEqualTo(new[] { Id, });
			}

			[Fact]
			public async Task WhenAcceptingAll_AndSubjectIsNull_ShouldNotAccept()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was <null>").AsWildcard();
				await That(store.Accepted).IsEmpty();
			}

			[Fact]
			public async Task WhenAcceptingNew_AndNegated_ShouldNotAcceptAMissingSnapshot()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).Accepting(SnapshotAcceptance.New));

				await That(Act).Throws<XunitException>()
					.WithMessage("*a snapshot cannot be negated").AsWildcard();
				await That(store.Accepted).IsEmpty()
					.Because("a negated snapshot writes nothing, whatever is accepted");
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenAcceptingNew_AndOrBranchIsMet_ShouldNotAcceptAMissingSnapshot()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.New)
						.Or.IsEqualTo("foo");

				await That(Act).DoesNotThrow();
				await That(store.Accepted).IsEmpty()
					.Because("a passing test must not add a snapshot unnoticed");
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenAcceptingNew_AndSnapshotDiffers_ShouldRecordAMismatchWithoutAccepting()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo baz";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.New);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was "foo baz", which differs at index 6:
					                      ↓ (actual)
					               "foo baz"
					               "foo bar"
					                      ↑ (expected)
					             """)
					.Because("the in-memory store describes no pending file, as without accepting");
				await That(store.Accepted).IsEmpty()
					.Because("only a missing snapshot is accepted, a reviewed snapshot is never replaced");
				await That(store.Snapshots[Id]).IsEqualTo("foo bar");
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo baz",
				});
			}

			[Fact]
			public async Task WhenAcceptingNew_AndSnapshotIsMissing_ShouldAcceptTheSubjectAndFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.New);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             foo

					             Accepted snapshot:
					             MyTests.MyTest
					             """)
					.Because("an added snapshot must not go unnoticed");
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
				await That(store.Mismatches).IsEmpty();
				await That(Act).DoesNotThrow()
					.Because("the next run compares with the accepted snapshot");
			}

			[Fact]
			public async Task WhenAcceptingNone_AndSnapshotDiffers_ShouldRecordAMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo baz";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEmpty();
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo baz",
				});
			}

			[Fact]
			public async Task WhenAcceptingNone_AndSnapshotIsMissing_ShouldRecordAMismatch()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<XunitException>()
					.WithMessage("*the snapshot did not exist*").AsWildcard();
				await That(store.Accepted).IsEmpty();
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}

			[Fact]
			public async Task WhenAcceptingNone_ShouldOverrideTheGlobalSetting()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<XunitException>()
					.WithMessage("*the snapshot did not exist*").AsWildcard();
				await That(store.Accepted).IsEmpty();
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}

			[Fact]
			public async Task WhenAcceptingAll_ShouldOverrideTheGlobalSetting()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.None);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}

			[Fact]
			public async Task WhenGloballyAcceptingAll_ShouldAccept()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.All);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}

			[Fact]
			public async Task WhenGloballyAcceptingNew_ShouldOnlyAcceptAMissingSnapshot()
			{
				SnapshotId missing = Id;
				SnapshotId different = new(Id.SourceFile, "Different");
				InMemorySnapshotStore store = new();
				store.Snapshots[different] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.New);

				async Task ActOnMissing()
					=> await That("foo").MatchesSnapshot(missing.SourceFile, missing.MemberName);

				async Task ActOnDifferent()
					=> await That("foo").MatchesSnapshot(different.SourceFile, different.MemberName);

				await That(ActOnMissing).Throws<XunitException>();
				await That(ActOnDifferent).Throws<XunitException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[missing] = "foo",
				});
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[different] = "foo",
				});
			}

			[Fact]
			public async Task WhenNotAccepting_ShouldRecordMismatch()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.None);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEmpty();
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}
		}

		[Collection(nameof(EnvironmentCollection))]
		public sealed class AcceptingEnvironmentTests
		{
			private const string AcceptVariable = "AWEXPECT_ACCEPT_SNAPSHOTS";

			private string SourceFile { get; } = $"/src/{Guid.NewGuid():N}/MyTests.cs";

			private SnapshotId Id => new(SourceFile, "MyTest");

			public static TheoryData<SnapshotAcceptance?, SnapshotAcceptance?, string?, SnapshotAcceptance>
				Combinations
			{
				get
				{
					SnapshotAcceptance?[] levels =
						[null, SnapshotAcceptance.None, SnapshotAcceptance.New, SnapshotAcceptance.All,];
					TheoryData<SnapshotAcceptance?, SnapshotAcceptance?, string?, SnapshotAcceptance> data = new();
					foreach (SnapshotAcceptance? expectation in levels)
					{
						foreach (SnapshotAcceptance? setting in levels)
						{
							foreach (SnapshotAcceptance? variable in levels)
							{
								data.Add(expectation, setting, variable?.ToString().ToLowerInvariant(),
									expectation ?? setting ?? variable ?? SnapshotAcceptance.None);
							}
						}
					}

					return data;
				}
			}

			[Theory]
			[MemberData(nameof(Combinations))]
			public async Task ShouldPreferTheExpectationOverTheSettingOverTheVariable(
				SnapshotAcceptance? expectation, SnapshotAcceptance? setting, string? variable,
				SnapshotAcceptance expected)
			{
				using EnvironmentScope environment = variable is null ? new() : new((AcceptVariable, variable));
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(setting);

				SnapshotAcceptance result = await GetAcceptance(expectation);

				await That(result).IsEqualTo(expected);
			}

			[Theory]
			[InlineData("CI")]
			[InlineData("TF_BUILD")]
			[InlineData("JENKINS_URL")]
			[InlineData("TEAMCITY_VERSION")]
			public async Task WhenAcceptingAll_OnBuildServer_ShouldAccept(string buildServerVariable)
			{
				using EnvironmentScope environment = new((buildServerVariable, "true"));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted.Keys).IsEqualTo(new[] { Id, }).InAnyOrder()
					.Because("only the default depends on the build server");
			}

			[Theory]
			[InlineData("CI")]
			[InlineData("TF_BUILD")]
			[InlineData("JENKINS_URL")]
			[InlineData("TEAMCITY_VERSION")]
			public async Task WhenGloballyAcceptingNew_OnBuildServer_ShouldAccept(string buildServerVariable)
			{
				using EnvironmentScope environment = new((buildServerVariable, "true"));
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Acceptance.Set(SnapshotAcceptance.New);

				SnapshotAcceptance result = await GetAcceptance(null);

				await That(result).IsEqualTo(SnapshotAcceptance.New)
					.Because("only the environment variable depends on the build server");
			}

			[Theory]
			[InlineData("CI", "1")]
			[InlineData("TF_BUILD", "all")]
			[InlineData("JENKINS_URL", "new")]
			[InlineData("TEAMCITY_VERSION", "1")]
			[InlineData("CI", "invalid")]
			public async Task WhenVariableIsSet_OnBuildServer_ShouldNotAccept(string buildServerVariable, string value)
			{
				using EnvironmentScope environment = new((AcceptVariable, value), (buildServerVariable, "true"));

				SnapshotAcceptance result = await GetAcceptance(null);

				await That(result).IsEqualTo(SnapshotAcceptance.None)
					.Because("a variable that was forgotten in a pipeline must not change the snapshots");
			}

			[Theory]
			[InlineData("all", SnapshotAcceptance.All)]
			[InlineData("ALL", SnapshotAcceptance.All)]
			[InlineData(" All ", SnapshotAcceptance.All)]
			[InlineData("1", SnapshotAcceptance.All)]
			[InlineData("true", SnapshotAcceptance.All)]
			[InlineData("True", SnapshotAcceptance.All)]
			[InlineData("new", SnapshotAcceptance.New)]
			[InlineData("NEW", SnapshotAcceptance.New)]
			[InlineData("\tnew\n", SnapshotAcceptance.New)]
			[InlineData("none", SnapshotAcceptance.None)]
			[InlineData("None", SnapshotAcceptance.None)]
			[InlineData("0", SnapshotAcceptance.None)]
			[InlineData("false", SnapshotAcceptance.None)]
			[InlineData("FALSE", SnapshotAcceptance.None)]
			[InlineData(" ", SnapshotAcceptance.None)]
			public async Task WhenVariableIsSet_WithoutBuildServer_ShouldUseItsLevel(string value,
				SnapshotAcceptance expected)
			{
				using EnvironmentScope environment = new((AcceptVariable, value));

				SnapshotAcceptance result = await GetAcceptance(null);

				await That(result).IsEqualTo(expected);
			}

			[Fact]
			public async Task WhenVariableIsNotSet_ShouldNotAccept()
			{
				using EnvironmentScope environment = new();

				SnapshotAcceptance result = await GetAcceptance(null);

				await That(result).IsEqualTo(SnapshotAcceptance.None);
			}

			[Theory]
			[InlineData("yes")]
			[InlineData("2")]
			[InlineData("al l")]
			[InlineData("missing")]
			public async Task WhenVariableIsInvalid_ShouldThrowNamingTheVariableAndTheValues(string value)
			{
				using EnvironmentScope environment = new((AcceptVariable, value));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage(
						$"The environment variable AWEXPECT_ACCEPT_SNAPSHOTS has the invalid value '{value}'. Use 'none', 'new' or 'all' (or '0'/'false' for 'none' and '1'/'true' for 'all').")
					.Because("a typo must not silently accept nothing");
				await That(store.Accepted).IsEmpty();
			}

			[Theory]
			[InlineData("0")]
			[InlineData("false")]
			[InlineData("FALSE")]
			[InlineData("")]
			public async Task WhenBuildServerVariableIsDisabled_ShouldAccept(string value)
			{
				using EnvironmentScope environment = new((AcceptVariable, "1"), ("CI", value));

				SnapshotAcceptance result = await GetAcceptance(null);

				await That(result).IsEqualTo(SnapshotAcceptance.All);
			}

			/// <summary>
			///     Compares a missing and a different snapshot and returns the level that was accepted, which tells
			///     <see cref="SnapshotAcceptance.All" />, <see cref="SnapshotAcceptance.New" /> and
			///     <see cref="SnapshotAcceptance.None" /> apart.
			/// </summary>
			private async Task<SnapshotAcceptance> GetAcceptance(SnapshotAcceptance? expectation)
			{
				SnapshotId missing = new(SourceFile, "Missing");
				SnapshotId different = new(SourceFile, "Different");
				InMemorySnapshotStore store = new();
				store.Snapshots[different] = "Let It Be";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				foreach (SnapshotId id in new[] { missing, different, })
				{
					async Task Act()
					{
						SnapshotResult<string?, IThat<string?>> result =
							That("Abbey Road").MatchesSnapshot(id.SourceFile, id.MemberName);
						if (expectation is null)
						{
							await result;
						}
						else
						{
							await result.Accepting(expectation.Value);
						}
					}

					await That(Act).Throws<XunitException>();
				}

				if (store.Accepted.ContainsKey(different))
				{
					return SnapshotAcceptance.All;
				}

				return store.Accepted.ContainsKey(missing) ? SnapshotAcceptance.New : SnapshotAcceptance.None;
			}
		}
	}
}
