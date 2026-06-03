using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreDelegates
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyDelegateTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreDelegates>().Types()
					.WhichAreDelegates();

				await That(subject).AreDelegates().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreDelegates>().Types()
					.WhichAreDelegates();

				await That(subject.GetDescription()).Contains("types which are delegates in");
			}
		}
	}
}
