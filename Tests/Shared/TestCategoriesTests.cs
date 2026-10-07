using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aweXpect.TestHelpers;

public sealed class TestCategoriesTests
{
	/// <remarks>
	///     The pipeline selects the slow tests with a filter on their category. TUnit runs explicit tests only when
	///     every test that the filter matches is explicit, so a single slow test that is not explicit would run in
	///     their place, without any failure.
	/// </remarks>
	[Test]
	public async Task SlowTests_ShouldBeExplicit()
	{
		List<string> notExplicit = typeof(TestCategoriesTests).Assembly.GetTypes()
			.Where(type => !type.IsAbstract)
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
			                                    BindingFlags.Static))
			.Where(method => method.IsDefined(typeof(BaseTestAttribute), true))
			.Where(method => IsSlow(method) && !IsExplicit(method))
			.Select(method => $"{method.ReflectedType!.FullName}.{method.Name}")
			.OrderBy(name => name, StringComparer.Ordinal)
			.ToList();

		await That(notExplicit).IsEmpty()
			.Because($"a test of the category \"{TestCategories.Slow}\" that is not [Explicit] keeps the others from running");
	}

	private static bool IsSlow(MethodInfo method)
	{
		Type type = method.ReflectedType!;
		return method.GetCustomAttributes<CategoryAttribute>(true)
			.Concat(type.GetCustomAttributes<CategoryAttribute>(true))
			.Concat(type.Assembly.GetCustomAttributes<CategoryAttribute>())
			.Any(category => category.Category == TestCategories.Slow);
	}

	private static bool IsExplicit(MethodInfo method)
		=> method.IsDefined(typeof(ExplicitAttribute), true) ||
		   method.ReflectedType!.IsDefined(typeof(ExplicitAttribute), true);
}
