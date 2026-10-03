using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Recording;
using static aweXpect.Expect;

namespace aweXpect.Aot;

internal static class Checks
{
	private const string TrimmingHint = "trimming or Native AOT";

	public static readonly Check[] All =
	[
		new("the test framework adapter is registered",
			() => ShouldFail(async () => await That(1).IsEqualTo(2), "Expected that 1")),
		new("an equivalent object graph passes",
			() => ShouldPass(async () => await That(CreateOrder()).IsEquivalentTo(CreateOrder()))),
		new("a differing nested member fails and is named",
			() => ShouldFail(async () =>
			{
				Order expected = CreateOrder();
				expected.Customer.Address.City = "Berlin";
				await That(CreateOrder()).IsEquivalentTo(expected);
			}, "Customer.Address.City differed")),
		new("a differing collection element fails and is named",
			() => ShouldFail(async () =>
			{
				Order expected = CreateOrder();
				expected.Items[1].Price = 99;
				await That(CreateOrder()).IsEquivalentTo(expected);
			}, "Items[1].Price differed")),
		// The interfaces that select the set and the dictionary comparison are found through reflection, so a
		// comparison that silently degrades into a positional one is only visible from its result.
		new("a set is compared without its order",
			() => ShouldPass(async () =>
			{
				HashSet<string> actual = ["a", "b", "c",];
				List<string> expected = [..actual,];
				expected.Reverse();
				await That(actual).IsEquivalentTo(expected);
			})),
		new("a dictionary that is only a generic one is compared by key",
			() => ShouldFail(async () =>
			{
				ReadOnlyTags actual = new(new Dictionary<string, int>
				{
					["vip"] = 1,
				});
				ReadOnlyTags expected = new(new Dictionary<string, int>
				{
					["vip"] = 2,
				});
				await That(actual).IsEquivalentTo(expected);
			}, "[vip]")),
		new("a dictionary is compared with its key comparer",
			() => ShouldPass(async () =>
			{
				Dictionary<string, int> actual = new(StringComparer.OrdinalIgnoreCase)
				{
					["A"] = 1,
				};
				await That(actual).IsEquivalentTo(new Dictionary<string, int>
				{
					["a"] = 1,
				});
			})),
		new("a read-only dictionary is compared with the key comparer of the dictionary it wraps",
			() => ShouldPass(async () =>
			{
				ReadOnlyDictionary<string, int> actual = new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
				{
					["A"] = 1,
				});
				Dictionary<string, int> expected = new()
				{
					["a"] = 1,
				};
				await That(actual).IsEquivalentTo(expected);
				await That(actual).IsEqualTo(expected);
				await That(actual).Keys.Contains("a");
			})),
		new("a subject the generator did not see fails loudly",
			() => ShouldFailOrFailLoudly(async () =>
			{
				object subject = new Hidden
				{
					Secret = "a",
				};
				object expected = new Hidden
				{
					Secret = "b",
				};
				await That(subject).IsEquivalentTo(expected);
			}, "differed")),
		new("a recorded event with value-type parameters is counted",
			() => ShouldPass(async () =>
			{
				Publisher publisher = new();
				IEventRecording<Publisher> recording = publisher.Record().Events();
				publisher.RaiseCounted(1);
				publisher.RaiseCounted(2);
				publisher.RaiseTicked(7);
				await That(recording).Triggered(nameof(Publisher.Counted)).Exactly(2.Times())
					.And.Triggered(nameof(Publisher.Ticked)).Once()
					.And.DidNotTrigger(nameof(Publisher.Changed));
			})),
		new("a recorded event that was not raised fails and is named",
			() => ShouldFail(async () =>
			{
				Publisher publisher = new();
				IEventRecording<Publisher> recording = publisher.Record().Events();
				publisher.RaiseChanged();
				await That(recording).Triggered(nameof(Publisher.Ticked));
			}, "Ticked")),
		new("a subject recorded through an interface passes or fails loudly",
			() => ShouldPassOrFailLoudly(async () =>
			{
				IPublisher publisher = CreateHiddenPublisher();
				IEventRecording<IPublisher> recording = publisher.Record().Events();
				publisher.RaiseChanged();
				await That(recording).Triggered(nameof(IPublisher.Changed));
			})),
		new("a combination of flags is defined and an unnamed bit is not",
			() => ShouldPass(async () =>
			{
				await That(RegexOptions.Multiline | RegexOptions.IgnoreCase).IsDefined();
				await That((RegexOptions)(1 << 20)).IsNotDefined();
			})),
		new("a failure message renders the members of an object",
			() => ShouldFail(async () =>
			{
				Order other = CreateOrder();
				other.Id = 2;
				await That(CreateOrder()).IsEqualTo(other);
			}, "Id = 1", "Name = \"Alice\"", "City = \"Vienna\"")),
		// An anonymous type is rendered member-wise instead of through its own `ToString()`, so its members have to
		// reach the formatter the same way a named type's do.
		new("a failure message renders the members of an anonymous object",
			() => ShouldFail(async () => await That(new
			{
				Id = 1,
				Name = "Alice",
			}).IsEqualTo(new
			{
				Id = 2,
				Name = "Alice",
			}), "Id = 1", "Name = \"Alice\"")),
		// The pair type is registered because the walk from the orders above follows `Order.Tags`.
		new("a failure message renders a dictionary and a boxed pair",
			() => ShouldFail(async () =>
			{
				object boxed = new KeyValuePair<string, int>("k", 1);
				await That(boxed).IsEqualTo(CreateOrder().Tags);
			}, "[\"k\"] = 1", "[\"vip\"] = 1")),
		// The interfaces that name an async iterator and mark a dictionary are found through reflection, so a
		// rendering that silently degrades is only visible from the message. This is the only async iterator of the
		// program, so the AOT compiler resolves every use of it statically, even enumerating and casting it, and drops
		// its interfaces, unless they are kept by a runtime type check like the one a consumer would do.
		new("a failure message names an async iterator by its async enumerable type",
			() => ShouldFail(async () =>
			{
				object numbers = CountAsync();
				if (!typeof(IAsyncEnumerable<int>).IsAssignableFrom(numbers.GetType()))
				{
					throw new InvalidOperationException("The iterator is no async enumerable.");
				}

				await That(numbers).IsNull();
			}, "IAsyncEnumerable<int>")),
		new("a failure message renders a dictionary that is only a generic one in braces",
			() => ShouldFail(async () => await That((object)new ReadOnlyTags(CreateOrder().Tags)).IsNull(),
				"but it was {", "[\"vip\"] = 1")),
		new("a failure message renders a tuple positionally",
			() => ShouldFail(async () => await That((object)(1.5, "a")).IsNull(), "(1.5, \"a\")")),
	];

	private static async IAsyncEnumerable<int> CountAsync()
	{
		await Task.Yield();
		yield return 1;
	}

	/// <remarks>
	///     Returns the interface on purpose: the generator registers the static type of a recorded subject, and the
	///     check needs the runtime type to stay unregistered.
	/// </remarks>
	private static IPublisher CreateHiddenPublisher() => new HiddenPublisher();

	private static Order CreateOrder()
		=> new()
		{
			Id = 1,
			Customer = new Customer
			{
				Name = "Alice",
				Address = new Address
				{
					City = "Vienna",
				},
			},
			Items =
			[
				new Item
				{
					Sku = "A",
					Price = 1,
				},
				new Item
				{
					Sku = "B",
					Price = 2,
				},
			],
			Tags = new Dictionary<string, int>
			{
				["vip"] = 1,
			},
		};

	private static async Task<string?> ShouldPass(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName}: {exception.Message}";
		}
	}

	/// <summary>
	///     Expects the framework's assertion exception whose message contains every part.
	/// </summary>
	private static async Task<string?> ShouldFail(Func<Task> act, params string[] parts)
	{
		try
		{
			await act();
		}
		catch (Exception exception) when (exception.GetType().FullName == Framework.FailureExceptionName)
		{
			string? missing = Array.Find(parts, part => !exception.Message.Contains(part, StringComparison.Ordinal));
			return missing is null ? null : $"message lacks \"{missing}\": {exception.Message}";
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} instead of {Framework.FailureExceptionName}: {exception.Message}";
		}

		return "did not throw";
	}

	/// <remarks>
	///     A type the generator did not see is compared through reflection, which fails like a registered one under
	///     the JIT and with the error naming the fix under trimming. Both are acceptable, a silent pass is not.
	/// </remarks>
	private static async Task<string?> ShouldFailOrFailLoudly(Func<Task> act, string part)
	{
		try
		{
			await act();
		}
		catch (Exception exception) when (exception.GetType().FullName == Framework.FailureExceptionName &&
		                                  exception.Message.Contains(part, StringComparison.Ordinal))
		{
			return null;
		}
		catch (Exception exception) when (exception.Message.Contains(TrimmingHint, StringComparison.Ordinal))
		{
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} without naming the fix: {exception.Message}";
		}

		return "did not throw";
	}

	/// <remarks>
	///     A subject whose runtime type the generator did not see is recorded through reflection, which works under
	///     the JIT and has to fail with the actionable error under trimming. Both are acceptable, anything else is not.
	/// </remarks>
	private static async Task<string?> ShouldPassOrFailLoudly(Func<Task> act)
	{
		try
		{
			await act();
			return null;
		}
		catch (Exception exception) when (exception.Message.Contains(TrimmingHint, StringComparison.Ordinal))
		{
			return null;
		}
		catch (Exception exception)
		{
			return $"threw {exception.GetType().FullName} without naming the fix: {exception.Message}";
		}
	}
}
