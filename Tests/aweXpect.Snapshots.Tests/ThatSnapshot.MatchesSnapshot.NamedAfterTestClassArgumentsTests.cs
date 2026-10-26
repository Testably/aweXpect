using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		[Arguments("de-DE")]
		public sealed class NamedAfterTestClassArgumentsTests(string culture)
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			[Arguments("Abbey Road")]
			public async Task ShouldNameTheSnapshotAfterTheArgumentsOfTheTestClassFirst(string title)
			{
				await ShouldBeNamed(Id, "de-DE_Abbey Road",
					$"the cases of {culture} and of the other instances of the test class are kept apart as well");
			}
		}

		[ClassDataSource(typeof(Studio), Shared = [SharedType.PerClass,])]
		public sealed class NamedAfterInjectedTestClassArgumentsTests(Studio studio)
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			[Arguments("Abbey Road")]
			public async Task ShouldSkipTheInjectedObject(string title)
			{
				await ShouldBeNamed(Id, title,
					$"an injected object like the {studio.Name} studio is no value, and its members can change from run to run");
			}
		}

		[ClassDataSource(typeof(Playlist), Shared = [SharedType.PerClass,])]
		public sealed class NamedAfterInjectedCollectionTestClassArgumentsTests(Playlist playlist)
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			[Arguments("Abbey Road")]
			public async Task ShouldNotEnumerateTheInjectedCollection(string title)
			{
				await ShouldBeNamed(Id, title,
					$"an injected collection like the {playlist.Name} playlist is no value, and could throw when it is enumerated");
			}
		}

		public sealed class Playlist : IEnumerable<string>
		{
			public string Name { get; } = "Beatles";

			public IEnumerator<string> GetEnumerator()
				=> throw new InvalidOperationException("The playlist is not loaded yet.");

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
		}

		public sealed class Studio
		{
			public string Name { get; } = "Abbey Road";
			public Guid Session { get; } = Guid.NewGuid();
		}

		private static async Task ShouldBeNamed(SnapshotId id, string name, string because)
		{
			InMemorySnapshotStore store = new();
			store.Snapshots[id with { Name = name, }] = "foo";
			using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
			using CustomizationLifetime __ = Customize.aweXpect.Snapshots().NamedAfterTestArguments.Set(true);

			async Task Act()
				=> await That("foo").MatchesSnapshot(id.SourceFile, id.MemberName);

			await That(Act).DoesNotThrow().Because(because);
		}
	}
}
