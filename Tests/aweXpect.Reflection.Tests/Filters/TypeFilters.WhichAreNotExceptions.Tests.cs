using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotExceptions
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOnlyNonExceptionTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotExceptions>().Types()
					.WhichAreNotExceptions();

				await That(subject).AreNotExceptions().And.IsNotEmpty();
			}

			[Test]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreNotExceptions>().Types()
					.WhichAreNotExceptions();

				await That(subject.GetDescription()).Contains("types which are not exceptions in");
			}
		}
	}
}
