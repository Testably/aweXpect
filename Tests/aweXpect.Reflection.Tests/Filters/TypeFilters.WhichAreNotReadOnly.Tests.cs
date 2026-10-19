using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotReadOnly
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyNonReadOnlyTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotReadOnly>().Types()
					.WhichAreNotReadOnly();

				await That(subject).AreNotReadOnly().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotReadOnly>().Types()
					.WhichAreNotReadOnly();

				await That(subject.GetDescription())
					.IsEqualTo("non-read-only types in assembly").AsPrefix();
			}
		}
	}
}
