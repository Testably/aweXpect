using System.Collections.Generic;
using System.IO;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class AcceptingChangesTests
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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges()
						.And.IsNotEmpty();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAccepting_AndAcceptingThrows_ShouldNameTheErrorAndRecordAMismatchInstead()
			{
				InMemorySnapshotStore store = new()
				{
					AcceptException = new IOException("read-only"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

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
			public async Task WhenAccepting_AndAcceptingAndRecordingThrow_ShouldNameBothErrors()
			{
				InMemorySnapshotStore store = new()
				{
					AcceptException = new IOException("read-only"),
					RecordMismatchException = new IOException("disk full"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

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
			public async Task WhenAccepting_AndNegated_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Fact]
			public async Task WhenAccepting_AndReadingThrows_ShouldNotAccept()
			{
				InMemorySnapshotStore store = new()
				{
					ReadException = new IOException("locked"),
				};
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot could not be read: locked").AsWildcard();
				await That(store.Accepted).IsEmpty()
					.Because("an unreadable snapshot might match, so it must not be replaced");
			}

			[Fact]
			public async Task WhenAccepting_AndSnapshotDiffers_ShouldAcceptTheSubjectAndFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo bar";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo baz";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

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
			public async Task WhenAccepting_AndSnapshotIsMissing_ShouldAcceptTheSubjectAndFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

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
			public async Task WhenAccepting_AndSnapshotIsUsedForAnotherValue_ShouldNotAccept()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName);
				string subject = "bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage("*was already used for another value in this test run*").AsWildcard();
				await That(store.Accepted).IsEmpty()
					.Because("the values would replace each other's snapshot");
			}

			[Fact]
			public async Task WhenAccepting_AndSnapshotMatches_ShouldSucceedWithoutAccepting()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

				await That(Act).DoesNotThrow();
				await That(store.Accepted).IsEmpty();
				await That(store.Matches).IsEqualTo(new[] { Id, });
			}

			[Fact]
			public async Task WhenAccepting_AndSubjectIsNull_ShouldNotAccept()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was <null>").AsWildcard();
				await That(store.Accepted).IsEmpty();
			}

			[Fact]
			public async Task WhenAcceptingChangesIsFalse_ShouldOverrideTheGlobalSetting()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().AcceptChanges.Set(true);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges(false);

				await That(Act).Throws<XunitException>()
					.WithMessage("*the snapshot did not exist*").AsWildcard();
				await That(store.Accepted).IsEmpty();
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}

			[Fact]
			public async Task WhenAcceptingChangesIsTrue_ShouldOverrideTheGlobalSetting()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().AcceptChanges.Set(false);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges(true);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "foo",
				});
			}

			[Fact]
			public async Task WhenGloballyAcceptingChanges_ShouldAccept()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().AcceptChanges.Set(true);
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
			public async Task WhenNotAccepting_ShouldRecordMismatch()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().AcceptChanges.Set(false);
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
		public sealed class AcceptingChangesEnvironmentTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Theory]
			[InlineData("CI")]
			[InlineData("TF_BUILD")]
			[InlineData("JENKINS_URL")]
			[InlineData("TEAMCITY_VERSION")]
			public async Task WhenAcceptingChangesIsTrue_OnBuildServer_ShouldAccept(string buildServerVariable)
			{
				using EnvironmentScope environment = new((buildServerVariable, "true"));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(store.Accepted.Keys).IsEqualTo(new[] { Id, }).InAnyOrder()
					.Because("only the default depends on the build server");
			}

			[Theory]
			[InlineData("CI")]
			[InlineData("TF_BUILD")]
			[InlineData("JENKINS_URL")]
			[InlineData("TEAMCITY_VERSION")]
			public async Task WhenVariableIsSet_OnBuildServer_ShouldNotAccept(string buildServerVariable)
			{
				using EnvironmentScope environment = new(("AWEXPECT_ACCEPT_SNAPSHOTS", "1"),
					(buildServerVariable, "true"));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEmpty()
					.Because("a variable that was forgotten in a pipeline must not change the snapshots");
			}

			[Theory]
			[InlineData("1")]
			[InlineData("true")]
			[InlineData("yes")]
			public async Task WhenVariableIsSet_WithoutBuildServer_ShouldAccept(string value)
			{
				using EnvironmentScope environment = new(("AWEXPECT_ACCEPT_SNAPSHOTS", value));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*Accepted snapshot:*").AsWildcard();
				await That(store.Accepted.Keys).IsEqualTo(new[] { Id, }).InAnyOrder();
			}

			[Theory]
			[InlineData("0")]
			[InlineData("false")]
			[InlineData("FALSE")]
			public async Task WhenBuildServerVariableIsDisabled_ShouldAccept(string value)
			{
				using EnvironmentScope environment = new(("AWEXPECT_ACCEPT_SNAPSHOTS", "1"), ("CI", value));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted.Keys).IsEqualTo(new[] { Id, }).InAnyOrder();
			}

			[Theory]
			[InlineData("")]
			[InlineData("0")]
			[InlineData("false")]
			[InlineData("False")]
			public async Task WhenVariableIsNotSet_ShouldNotAccept(string value)
			{
				using EnvironmentScope environment = new(("AWEXPECT_ACCEPT_SNAPSHOTS", value));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEmpty();
			}

			[Fact]
			public async Task WhenGloballyNotAcceptingChanges_ShouldIgnoreTheVariable()
			{
				using EnvironmentScope environment = new(("AWEXPECT_ACCEPT_SNAPSHOTS", "1"));
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().AcceptChanges.Set(false);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEmpty()
					.Because("the global setting takes precedence over the environment");
			}
		}
	}
}
