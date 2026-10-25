using aweXpect.Core.Extending;
using aweXpect.Customization;

namespace aweXpect.Core.Tests.Core.Extending;

public class AssemblyPrefixesTests
{
	public sealed class IsExcluded
	{
		[Test]
		public async Task WithoutPrefixes_ShouldUseTheCurrentSetting()
		{
			bool result;
			using (Customize.aweXpect.Settings().ExcludedAssemblyPrefixes.Set(["MyCompany",]))
			{
				result = AssemblyPrefixes.IsExcluded("MyCompany.Domain");
			}

			await That(result).IsTrue();
			await That(AssemblyPrefixes.IsExcluded("MyCompany.Domain")).IsFalse()
				.Because("the customization is no longer in effect");
		}

		[Test]
		[Arguments("System.Text.Json", true)]
		[Arguments("SystemsBiology.Core", false)]
		[Arguments(null, false)]
		public async Task WithoutPrefixes_ShouldUseTheDefaultSetting(string? assemblyName, bool expected)
		{
			bool result = AssemblyPrefixes.IsExcluded(assemblyName);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		[Arguments("System", "System", true)]
		[Arguments("System.Text.Json", "System", true)]
		[Arguments("SystemsBiology.Core", "System", false)]
		[Arguments("SystemsBiology", "System", false)]
		[Arguments("system.Text.Json", "System", false)]
		[Arguments("MyCompany.Domain", "MyCompany.", true)]
		[Arguments("MyCompany.", "MyCompany.", true)]
		[Arguments("MyCompany", "MyCompany.", false)]
		[Arguments("MyCompanyOther", "MyCompany.", false)]
		public async Task WithPrefixes_ShouldMatchAtNameSegmentBoundary(
			string assemblyName, string prefix, bool expected)
		{
			bool result = AssemblyPrefixes.IsExcluded(assemblyName, [prefix,]);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task WithPrefixes_WhenAnyPrefixMatches_ShouldReturnTrue()
		{
			bool result = AssemblyPrefixes.IsExcluded("Microsoft.Extensions", ["System", "Microsoft",]);

			await That(result).IsTrue();
		}

		[Test]
		public async Task WithPrefixes_WhenAssemblyNameIsNull_ShouldReturnFalse()
		{
			bool result = AssemblyPrefixes.IsExcluded(null, ["System",]);

			await That(result).IsFalse();
		}

		[Test]
		public async Task WithPrefixes_WhenPrefixesAreNull_ShouldThrowArgumentNullException()
		{
			void Act() => AssemblyPrefixes.IsExcluded("System", null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("excludedPrefixes").And
				.WithMessage("The * cannot be null.*").AsWildcard();
		}

		[Test]
		public async Task WithPrefixes_WhenPrefixIsEmpty_ShouldBeIgnored()
		{
			bool result = AssemblyPrefixes.IsExcluded("AnyAssembly", ["",]);

			await That(result).IsFalse().Because("an empty prefix cannot identify an assembly");
		}
	}
}
