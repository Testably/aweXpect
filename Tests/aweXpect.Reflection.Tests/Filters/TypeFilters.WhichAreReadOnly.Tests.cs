using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyReadOnlyStructTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreReadOnly>().Types()
					.WhichAreReadOnly();

				await That(subject).AreReadOnly().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreReadOnly>().Types()
					.WhichAreReadOnly();

				await That(subject.GetDescription())
					.IsEqualTo("read-only types in assembly").AsPrefix();
			}
		}
	}
}
