using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotDelegates
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyNonDelegateTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotDelegates>().Types()
					.WhichAreNotDelegates();

				await That(subject).AreNotDelegates().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotDelegates>().Types()
					.WhichAreNotDelegates();

				await That(subject.GetDescription()).Contains("types which are not delegates in");
			}
		}
	}
}
