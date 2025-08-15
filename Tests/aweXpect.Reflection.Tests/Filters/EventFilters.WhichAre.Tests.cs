using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAre
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAre(AccessModifiers.Internal);

				await That(events).AreInternal();
				await That(events.GetDescription())
					.IsEqualTo("internal events in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAre(AccessModifiers.Private);

				await That(events).ArePrivate();
				await That(events.GetDescription())
					.IsEqualTo("private events in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAre(AccessModifiers.Protected);

				await That(events).AreProtected();
				await That(events.GetDescription())
					.IsEqualTo("protected events in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAre(AccessModifiers.Public);

				await That(events).ArePublic();
				await That(events.GetDescription())
					.IsEqualTo("public events in assembly").AsPrefix();
			}
		}
	}
}
