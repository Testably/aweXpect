using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class ClosestMatchTests
	{
		[Test]
		public async Task Contains_WhenKeyIsMissing_ShouldSuggestTheClosestKey()
		{
			IDictionary<string, int> subject = ToDictionary(["Username", "Password",], [1, 2,]);

			async Task Act()
				=> await That(subject).Contains("Usrname", 1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains ["Usrname"] = 1,
				             but it did not contain key "Usrname" (did you mean "Username"?)

				             Dictionary:
				             {["Username"] = 1, ["Password"] = 2}
				             """);
		}

		[Test]
		public async Task ContainsKey_WhenKeyIsMissing_ShouldSuggestTheClosestKey()
		{
			IDictionary<string, int> subject = ToDictionary(["Username", "Password",], [1, 2,]);

			async Task Act()
				=> await That(subject).ContainsKey("Usrname");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains key "Usrname",
				             but it did not contain key "Usrname" (did you mean "Username"?)

				             Dictionary:
				             {["Username"] = 1, ["Password"] = 2}
				             """);
		}

		[Test]
		public async Task ContainsKey_WhenKeyIsMissing_WithACaseInsensitiveComparer_ShouldSuggestTheClosestKey()
		{
			Dictionary<string, int> subject = new(StringComparer.OrdinalIgnoreCase)
			{
				["Username"] = 1,
				["Password"] = 2,
			};

			async Task Act()
				=> await That(subject).ContainsKey("USRNAME");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains key "USRNAME",
				             but it did not contain key "USRNAME" (did you mean "Username"?)

				             Dictionary:
				             {["Username"] = 1, ["Password"] = 2}
				             """);
		}

		[Test]
		public async Task ContainsKey_WhenKeyIsMissing_WithWhoseValue_ShouldSuggestTheClosestKey()
		{
			IDictionary<string, int> subject = ToDictionary(["Username", "Password",], [1, 2,]);

			async Task Act()
				=> await That(subject).ContainsKey("Usrname").WhoseValue.IsEqualTo(1);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains key "Usrname" whose value is equal to 1,
				             but it did not contain key "Usrname" (did you mean "Username"?)

				             Dictionary:
				             {["Username"] = 1, ["Password"] = 2}
				             """);
		}

		[Test]
		public async Task ContainsKey_WhenNoKeyIsClose_ShouldNotSuggestAKey()
		{
			IDictionary<string, int> subject = ToDictionary(["Username", "Password",], [1, 2,]);

			async Task Act()
				=> await That(subject).ContainsKey("Phone");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains key "Phone",
				             but it did not contain key "Phone"

				             Dictionary:
				             {["Username"] = 1, ["Password"] = 2}
				             """);
		}

		[Test]
		public async Task ContainsKey_WhenTheEnumerationThrows_ShouldNotSuggestAKey()
		{
			InvalidOperationException exception = new("enumeration failed");
			IDictionary<string, int> subject =
				new ThrowingDictionary<string, int>(exception, ThrowingMembers.Enumeration)
				{
					["Username"] = 1,
				};

			async Task Act()
				=> await That(subject).ContainsKey("Usrname");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains key "Usrname",
				             but it did not contain key "Usrname"

				             Dictionary:
				             [the enumeration did throw an InvalidOperationException: enumeration failed]
				             """)
				.Because("the failure is reported even when the keys cannot be compared");
		}

		[Test]
		public async Task ContainsKeys_WhenKeysAreMissing_ShouldSuggestTheClosestKeyForEach()
		{
			IDictionary<string, int> subject = ToDictionary(["Username", "Password", "Email",], [1, 2, 3,]);

			async Task Act()
				=> await That(subject).ContainsKeys("Usrname", "Email", "Pasword", "Phone");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains keys ["Usrname", "Email", "Pasword", "Phone"],
				             but it did not contain [
				               "Usrname" (did you mean "Username"?),
				               "Pasword" (did you mean "Password"?),
				               "Phone"
				             ]

				             Dictionary:
				             {["Username"] = 1, ["Password"] = 2, ["Email"] = 3}
				             """);
		}

		[Test]
		public async Task ContainsKeys_WhenManyKeysAreMissing_ShouldListOnlyTheFirstKeys()
		{
			IDictionary<string, int> subject = ToDictionary(["Username",], [1,]);
			string[] keys = ["Usrname", ..Enumerable.Range(1, 11).Select(index => $"Key {index:00}"),];

			async Task Act()
				=> await That(subject).ContainsKeys(keys);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains keys ["Usrname", "Key 01", "Key 02", "Key 03", "Key 04", "Key 05", "Key 06", "Key 07", "Key 08", "Key 09", (… and 2 more)],
				             but it did not contain [
				               "Usrname" (did you mean "Username"?),
				               "Key 01",
				               "Key 02",
				               "Key 03",
				               "Key 04",
				               "Key 05",
				               "Key 06",
				               "Key 07",
				               "Key 08",
				               "Key 09",
				               (… and 2 more)
				             ]

				             Dictionary:
				             {["Username"] = 1}
				             """);
		}

		[Test]
		public async Task ContainsKeys_WhenNoKeyIsClose_ShouldNotSuggestAKey()
		{
			IDictionary<string, int> subject = ToDictionary(["Username", "Password",], [1, 2,]);

			async Task Act()
				=> await That(subject).ContainsKeys("Username", "Phone");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains keys ["Username", "Phone"],
				             but it did not contain [
				               "Phone"
				             ]

				             Dictionary:
				             {["Username"] = 1, ["Password"] = 2}
				             """);
		}

		[Test]
		public async Task ContainsValue_WhenValueIsMissing_ShouldSuggestTheClosestValue()
		{
			IDictionary<int, string> subject = ToDictionary([1, 2,], ["Username", "Password",]);

			async Task Act()
				=> await That(subject).ContainsValue("Usrname");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains value "Usrname",
				             but it did not contain value "Usrname" (did you mean "Username"?)

				             Dictionary:
				             {
				               [1] = "Username",
				               [2] = "Password"
				             }
				             """);
		}

		[Test]
		public async Task ContainsValues_WhenValuesAreMissing_ShouldSuggestTheClosestValueForEach()
		{
			IDictionary<int, string?> subject = ToDictionary<int, string?>([1, 2,], ["Username", "Password",]);

			async Task Act()
				=> await That(subject).ContainsValues("Usrname", null, "Password");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains values ["Usrname", <null>, "Password"],
				             but it did not contain [
				               "Usrname" (did you mean "Username"?),
				               <null>
				             ]

				             Dictionary:
				             {
				               [1] = "Username",
				               [2] = "Password"
				             }
				             """);
		}

		[Test]
		public async Task DoesNotContainKey_WhenKeyExists_ShouldNotSuggestAKey()
		{
			IDictionary<string, int> subject = ToDictionary(["Username", "Usernames",], [1, 2,]);

			async Task Act()
				=> await That(subject).DoesNotContainKey("Username");

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain key "Username",
				             but it did

				             Dictionary:
				             {["Username"] = 1, ["Usernames"] = 2}
				             """)
				.Because("a negated expectation never suggests a key");
		}
	}
}
