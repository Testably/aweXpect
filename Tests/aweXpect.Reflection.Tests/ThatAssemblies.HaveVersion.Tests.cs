using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatAssemblies
{
	public sealed class HaveVersion
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllVersionsMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion(version => version.Major >= 0);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenVersionDoesNotMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion(version => version.Major < 0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have version matching version => version.Major < 0,
					             but it contained assemblies with a non-matching version *
					             """).AsWildcard();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllVersionsMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion(version => version.Major >= 0));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have version matching version => version.Major >= 0,
					             but it only contained assemblies with a matching version *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenVersionDoesNotMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion(version => version.Major < 0));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class ComponentTests
		{
			[Fact]
			public async Task ShouldSupportAllComponents()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				Version version = typeof(PublicAbstractClass).Assembly.GetName().Version!;

				async Task Act()
				{
					await That(subject).HaveVersion()
						.WithMajor.EqualTo(version.Major)
						.WithMinor.EqualTo(version.Minor)
						.WithBuild.EqualTo(version.Build)
						.WithRevision.EqualTo(version.Revision);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllComparisonsAreSatisfied_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				int major = typeof(PublicAbstractClass).Assembly.GetName().Version!.Major;

				async Task Act()
				{
					await That(subject).HaveVersion()
						.WithMajor.GreaterThan(major - 1)
						.WithMajor.GreaterThanOrEqualTo(major)
						.WithMajor.LessThan(major + 1)
						.WithMajor.LessThanOrEqualTo(major)
						.WithMajor.EqualTo(major)
						.WithMajor.NotEqualTo(major + 1);
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAllComponentsMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.GreaterThanOrEqualTo(0);
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("GreaterThan", 0)]
			[InlineData("GreaterThanOrEqualTo", 1)]
			[InlineData("LessThan", 0)]
			[InlineData("LessThanOrEqualTo", -1)]
			[InlineData("EqualTo", 1)]
			[InlineData("NotEqualTo", 0)]
			public async Task WhenComparisonIsNotSatisfied_ShouldFail(string comparison, int offset)
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					int expected = typeof(PublicAbstractClass).Assembly.GetName().Version!.Major + offset;
					await (comparison switch
					{
						"GreaterThan" => That(subject).HaveVersion().WithMajor.GreaterThan(expected),
						"GreaterThanOrEqualTo" => That(subject).HaveVersion().WithMajor.GreaterThanOrEqualTo(expected),
						"LessThan" => That(subject).HaveVersion().WithMajor.LessThan(expected),
						"LessThanOrEqualTo" => That(subject).HaveVersion().WithMajor.LessThanOrEqualTo(expected),
						"EqualTo" => That(subject).HaveVersion().WithMajor.EqualTo(expected),
						_ => That(subject).HaveVersion().WithMajor.NotEqualTo(expected),
					});
				}

				await That(Act).Throws<XunitException>();
			}

			[Fact]
			public async Task WhenComponentDoesNotMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion().WithMajor.LessThan(0);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             all have major version less than 0,
					             but it contained assemblies with a non-matching version *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenMultipleComponentsAreCombined_ShouldCombineExpectations()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();
				Version version = typeof(PublicAbstractClass).Assembly.GetName().Version!;

				async Task Act()
				{
					await That(subject).HaveVersion()
						.WithMajor.EqualTo(version.Major)
						.WithMinor.GreaterThan(version.Minor);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage(
						$"*all have major version equal to {version.Major} and minor version greater than {version.Minor}*non-matching version*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhenNegatedAndAllComponentsMatch_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion().WithMajor.GreaterThanOrEqualTo(0));
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type PublicAbstractClass
					             not all have major version greater than or equal to 0,
					             but it only contained assemblies with a matching version *
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNegatedAndAllVersionsExist_ShouldFail()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("*not all have a version*").AsWildcard();
			}

			[Fact]
			public async Task WhenNegatedAndComponentDoesNotMatch_ShouldSucceed()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(they => they.HaveVersion().WithMajor.LessThan(0));
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithoutComponent_ShouldVerifyThatAllVersionsExist()
			{
				Filtered.Assemblies subject = In.AssemblyContaining<PublicAbstractClass>();

				async Task Act()
				{
					await That(subject).HaveVersion();
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
