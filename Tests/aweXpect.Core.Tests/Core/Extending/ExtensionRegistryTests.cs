using System.Collections.Generic;
using aweXpect.Core.Extending;

namespace aweXpect.Core.Tests.Core.Extending;

public class ExtensionRegistryTests
{
	public sealed class GetValues
	{
		[Test]
		public async Task WhenKeyIsNull_ShouldThrowArgumentNullException()
		{
			void Act()
				=> ExtensionRegistry.GetValues(null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("key").And
				.WithMessage("The 'key' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task WhenKeyIsUnknown_ShouldReturnAnEmptyList()
		{
			IReadOnlyList<string> values = ExtensionRegistry.GetValues(UniqueKey());

			await That(values).IsEmpty();
		}

		[Test]
		public async Task WhenNoValueIsAdded_ShouldReturnTheSameList()
		{
			string key = UniqueKey();
			ExtensionRegistry.Register(key, "Abbey Road");

			IReadOnlyList<string> first = ExtensionRegistry.GetValues(key);
			IReadOnlyList<string> second = ExtensionRegistry.GetValues(key);

			await That(second).IsSameAs(first)
				.Because("a caller can keep what it derived from the list until a value is added");
		}

		[Test]
		public async Task WhenValueIsAdded_ShouldNotChangeAReturnedList()
		{
			string key = UniqueKey();
			ExtensionRegistry.Register(key, "Abbey Road");
			IReadOnlyList<string> before = ExtensionRegistry.GetValues(key);

			ExtensionRegistry.Register(key, "Let It Be");

			await That(before).IsEqualTo(["Abbey Road",]);
			await That(ExtensionRegistry.GetValues(key)).IsEqualTo(["Abbey Road", "Let It Be",]);
		}
	}

	public sealed class Register
	{
		[Test]
		public async Task ShouldKeepEachValueInTheOrderOfItsRegistration()
		{
			string key = UniqueKey();

			ExtensionRegistry.Register(key, "Revolver");
			ExtensionRegistry.Register(key, "Abbey Road");
			ExtensionRegistry.Register(key, "Revolver");

			await That(ExtensionRegistry.GetValues(key)).IsEqualTo(["Revolver", "Abbey Road", "Revolver",])
				.Because("each test assembly registers its own values, also when another one registered the same");
		}

		[Test]
		public async Task ShouldOnlyAddTheValueToItsKey()
		{
			string key = UniqueKey();
			string otherKey = UniqueKey();

			ExtensionRegistry.Register(key, "Help!");

			await That(ExtensionRegistry.GetValues(otherKey)).IsEmpty();
		}

		[Test]
		public async Task ShouldTellKeysApartByCase()
		{
			string key = UniqueKey();

			ExtensionRegistry.Register(key, "Help!");

			await That(ExtensionRegistry.GetValues(key.ToUpperInvariant())).IsEmpty();
		}

		[Test]
		public async Task WhenKeyIsNull_ShouldThrowArgumentNullException()
		{
			void Act()
				=> ExtensionRegistry.Register(null!, "Help!");

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("key").And
				.WithMessage("The 'key' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task WhenValueIsNull_ShouldThrowArgumentNullException()
		{
			void Act()
				=> ExtensionRegistry.Register(UniqueKey(), null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("value").And
				.WithMessage("The 'value' cannot be null.").AsPrefix();
		}
	}

	private static string UniqueKey()
		=> $"aweXpect.Core.Tests.{Guid.NewGuid():N}";
}
