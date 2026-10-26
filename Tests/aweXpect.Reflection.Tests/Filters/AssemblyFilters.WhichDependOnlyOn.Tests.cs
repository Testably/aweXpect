using System.Collections.Generic;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichDependOnlyOn
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldDescribeTheFilterForASingleAllowedAssembly()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichDependOnlyOn("aweXpect.Core");

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on assemblies equal to \"aweXpect.Core\"");
			}

			[Test]
			public async Task ShouldDescribeTheFilterForEnumerableOfAllowedAssemblies()
			{
				List<string> expected = ["aweXpect.Core", "aweXpect",];

				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichDependOnlyOn(expected);

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on assemblies equal to \"aweXpect.Core\" or equal to \"aweXpect\"");
			}

			[Test]
			public async Task ShouldDescribeTheFilterForMultipleAllowedAssemblies()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichDependOnlyOn("aweXpect.Core", "aweXpect");

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on assemblies equal to \"aweXpect.Core\" or equal to \"aweXpect\"");
			}

			[Test]
			public async Task ShouldDescribeTheFilterForNoAllowedAssemblies()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichDependOnlyOn();

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on no assemblies");
			}

			[Test]
			public async Task ShouldFilterForAssembliesDependingOnlyOnAllowed()
			{
				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichDependOnlyOn("aweXpect.Core");

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Test]
			public async Task ShouldFilterForAssembliesDependingOnlyOnEnumerable()
			{
				List<string> expected = ["aweXpect.Core",];

				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly, typeof(PublicAbstractClass).Assembly)
					.WhichDependOnlyOn(expected);

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Test]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichDependOnlyOn("aweXpect.*").AsWildcard();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichDependOnlyOn("awexpect.core").IgnoringCase();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Test]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldBeFilteredOut()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichDependOnlyOn("aweXpect.Core");

				await That(assemblies).IsEmpty();
			}

			[Test]
			public async Task WhenEnumerableIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
				{
					_ = In.AssemblyContaining<PublicAbstractClass>().WhichDependOnlyOn((IEnumerable<string>)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}
		}
	}
}
