using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WithVersion
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForAssembliesWithMatchingVersion()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithVersion(version => version.Major >= 0);

				await That(assemblies).IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with version matching version => version.Major >= 0");
			}

			[Test]
			public async Task ShouldNotMatchWhenPredicateIsNeverTrue()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithVersion(version => version.Major < 0);

				await That(assemblies).IsEmpty();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.WithVersion(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type AssemblyFilters with version matching _ => throw new InvalidOperationException("boom")
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFailWhenNegated()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.WithVersion(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).DoesNotComplyWith(they => they.IsEmpty());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type AssemblyFilters with version matching _ => throw new InvalidOperationException("boom")
					             is not empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a predicate that throws answers neither the expectation nor its negation");
			}
		}

		public sealed class ComponentTests
		{
			[Test]
			public async Task EqualTo_ShouldMatchOnlyTheExactValue()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				Filtered.Assemblies included = In.Assemblies(assembly).WithVersion().WithMajor.EqualTo(major);
				Filtered.Assemblies excluded = In.Assemblies(assembly).WithVersion().WithMajor.EqualTo(major + 1);

				await That(included).HasSingle().Which.IsEqualTo(assembly);
				await That(excluded).IsEmpty();
			}

			[Test]
			public async Task GreaterThan_ShouldNotMatchTheBoundaryValue()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				Filtered.Assemblies excluded = In.Assemblies(assembly).WithVersion().WithMajor.GreaterThan(major);
				Filtered.Assemblies included = In.Assemblies(assembly).WithVersion().WithMajor.GreaterThan(major - 1);

				await That(excluded).IsEmpty();
				await That(included).HasSingle().Which.IsEqualTo(assembly);
			}

			[Test]
			public async Task GreaterThanOrEqualTo_ShouldMatchTheBoundaryValue()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				Filtered.Assemblies included =
					In.Assemblies(assembly).WithVersion().WithMajor.GreaterThanOrEqualTo(major);
				Filtered.Assemblies excluded =
					In.Assemblies(assembly).WithVersion().WithMajor.GreaterThanOrEqualTo(major + 1);

				await That(included).HasSingle().Which.IsEqualTo(assembly);
				await That(excluded).IsEmpty();
			}

			[Test]
			public async Task LessThanOrEqualTo_ShouldMatchTheBoundaryValue()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				Filtered.Assemblies included =
					In.Assemblies(assembly).WithVersion().WithMajor.LessThanOrEqualTo(major);
				Filtered.Assemblies excluded =
					In.Assemblies(assembly).WithVersion().WithMajor.LessThanOrEqualTo(major - 1);

				await That(included).HasSingle().Which.IsEqualTo(assembly);
				await That(excluded).IsEmpty();
			}

			[Test]
			public async Task MultipleComponents_ShouldCombineWithAnd()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithVersion().WithMajor.GreaterThanOrEqualTo(0).WithMinor.LessThan(100000);

				await That(assemblies).IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with major version greater than or equal to 0 with minor version less than 100000");
			}

			[Test]
			public async Task NotEqualTo_ShouldMatchEverythingExceptTheExactValue()
			{
				Assembly assembly = typeof(PublicAbstractClass).Assembly;
				int major = assembly.GetName().Version!.Major;

				Filtered.Assemblies excluded = In.Assemblies(assembly).WithVersion().WithMajor.NotEqualTo(major);
				Filtered.Assemblies included = In.Assemblies(assembly).WithVersion().WithMajor.NotEqualTo(major + 1);

				await That(excluded).IsEmpty();
				await That(included).HasSingle().Which.IsEqualTo(assembly);
			}

			[Test]
			[Arguments("GreaterThan", 5, "greater than 5")]
			[Arguments("GreaterThanOrEqualTo", 5, "greater than or equal to 5")]
			[Arguments("LessThan", 5, "less than 5")]
			[Arguments("LessThanOrEqualTo", 5, "less than or equal to 5")]
			[Arguments("EqualTo", 5, "equal to 5")]
			[Arguments("NotEqualTo", 5, "not equal to 5")]
			public async Task ShouldDescribeComparison(string comparison, int expected, string expectedText)
			{
				Reflection.AssemblyFilters.VersionComponent major = In.AllLoadedAssemblies().WithVersion().WithMajor;
				Filtered.Assemblies assemblies = comparison switch
				{
					"GreaterThan" => major.GreaterThan(expected),
					"GreaterThanOrEqualTo" => major.GreaterThanOrEqualTo(expected),
					"LessThan" => major.LessThan(expected),
					"LessThanOrEqualTo" => major.LessThanOrEqualTo(expected),
					"EqualTo" => major.EqualTo(expected),
					_ => major.NotEqualTo(expected),
				};

				await That(assemblies.GetDescription())
					.IsEqualTo($"in all loaded assemblies with major version {expectedText}");
			}

			[Test]
			[Arguments("major")]
			[Arguments("minor")]
			[Arguments("build")]
			[Arguments("revision")]
			public async Task ShouldDescribeComponent(string name)
			{
				Reflection.AssemblyFilters.AssembliesWithVersion version = In.AllLoadedAssemblies().WithVersion();
				Reflection.AssemblyFilters.VersionComponent component = name switch
				{
					"major" => version.WithMajor,
					"minor" => version.WithMinor,
					"build" => version.WithBuild,
					_ => version.WithRevision,
				};
				Filtered.Assemblies assemblies = component.GreaterThanOrEqualTo(0);

				await That(assemblies.GetDescription())
					.IsEqualTo($"in all loaded assemblies with {name} version greater than or equal to 0");
			}

			[Test]
			public async Task WhenComparisonIsNeverTrue_ShouldNotMatch()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithVersion().WithMajor.LessThan(0);

				await That(assemblies).IsEmpty();
			}

			[Test]
			public async Task WithMajor_GreaterThanLargeValue_ShouldNotMatch()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithVersion().WithMajor.GreaterThan(100000);

				await That(assemblies).IsEmpty();
			}

			[Test]
			public async Task WithMajor_ShouldFilterAndDescribe()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithVersion().WithMajor.GreaterThanOrEqualTo(0);

				await That(assemblies).IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with major version greater than or equal to 0");
			}
		}
	}
}
