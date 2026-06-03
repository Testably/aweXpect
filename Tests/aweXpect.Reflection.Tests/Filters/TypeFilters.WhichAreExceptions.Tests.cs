using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreExceptions
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOnlyExceptionTypes()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreExceptions>().Types()
					.WhichAreExceptions();

				await That(subject).AreExceptions().And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldUpdateDescription()
			{
				Filtered.Types subject = In.AssemblyContaining<WhichAreExceptions>().Types()
					.WhichAreExceptions();

				await That(subject.GetDescription()).Contains("types which are exceptions in");
			}
		}
	}
}
