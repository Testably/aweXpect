using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNot(AccessModifiers.Internal);

				await That(events).AreNotInternal();
				await That(events.GetDescription())
					.IsEqualTo("non-internal events in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNot(AccessModifiers.Private);

				await That(events).AreNotPrivate();
				await That(events.GetDescription())
					.IsEqualTo("non-private events in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNot(AccessModifiers.Protected);

				await That(events).AreNotProtected();
				await That(events.GetDescription())
					.IsEqualTo("non-protected events in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicEvents()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichAreNot(AccessModifiers.Public);

				await That(events).AreNotPublic();
				await That(events.GetDescription())
					.IsEqualTo("non-public events in assembly").AsPrefix();
			}
		}
	}
}
