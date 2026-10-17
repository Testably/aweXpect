using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aweXpect.Core.Metadata;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Formatting;

public partial class ValueFormatters
{
	public sealed class FormatValueTests
	{
		[Test]
		public async Task ShouldNotOfferTheFormattedValueItself()
		{
			List<string> paths = [];
			FormattingOptions options = Recording(paths);

			string result = Formatter.Format(new
			{
				A = 1,
			}, options);
			Formatter.Format(5, options);
			Formatter.Format(new[] { 1, 2, }, options);

			await That(result).IsEqualTo("{ A = 1 }");
			await That(paths).IsEqualTo(["A", "[0]", "[1]",])
				.Because("the formatted value belongs to the caller, and its members and items have no prefix");
		}

		[Test]
		public async Task WhenAllItemsAreOmitted_ShouldWriteAnEmptyCollection()
		{
			object value = new
			{
				Items = new[] { 1, 2, },
			};
			FormattingOptions singleLine = FormattingOptions.SingleLine with
			{
				FormatValue = v => v.Kind == FormattedValueKind.CollectionItem
					? ValueFormatting.Omit
					: ValueFormatting.Write,
			};
			FormattingOptions indented = FormattingOptions.Indented("") with
			{
				FormatValue = singleLine.FormatValue,
			};

			string singleLineResult = Formatter.Format(value, singleLine);
			string indentedResult = Formatter.Format(value, indented);

			await That(singleLineResult).IsEqualTo("{ Items = [] }");
			await That(indentedResult).IsEqualTo("""
			                                     {
			                                       Items = []
			                                     }
			                                     """);
		}

		[Test]
		public async Task WhenAllMembersAreOmitted_ShouldWriteAnEmptyObject()
		{
			object value = new
			{
				Inner = new Probe(() => 1),
			};
			FormattingOptions singleLine = FormattingOptions.SingleLine with
			{
				FormatValue = v => v.Name == nameof(Probe.Value) ? ValueFormatting.Omit : ValueFormatting.Write,
			};
			FormattingOptions indented = FormattingOptions.Indented("") with
			{
				FormatValue = singleLine.FormatValue,
			};

			string singleLineResult = Formatter.Format(value, singleLine);
			string indentedResult = Formatter.Format(value, indented);

			await That(singleLineResult).IsEqualTo("{ Inner = ValueFormatters.FormatValueTests.Probe { } }");
			await That(indentedResult).IsEqualTo("""
			                                     {
			                                       Inner = ValueFormatters.FormatValueTests.Probe { }
			                                     }
			                                     """);
		}

		[Test]
		public async Task WhenAnonymousType_ShouldOfferItsMembers()
		{
			List<FormattedValue> offered = [];
			object value = new
			{
				Name = "a",
			};

			Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					offered.Add(v);
					return ValueFormatting.Write;
				},
			});

			await That(offered).HasSingle().Which.Satisfies(v => v.Kind == FormattedValueKind.Member &&
			                                                      v.Path == "Name" &&
			                                                      v.Type == typeof(string) &&
			                                                      v.DeclaringType == value.GetType());
		}

		[Test]
		public async Task WhenCollectionIsAConcurrentBag_ShouldUseAnUnknownIndex()
		{
			List<string> paths = [];
			object value = new
			{
				Tags = new ConcurrentBag<string>(["a", "b",]),
			};

			Formatter.Format(value, Recording(paths));

			await That(paths).IsEqualTo(["Tags", "Tags[*]", "Tags[*]",])
				.Because("the order of a concurrent bag depends on the threads that added the items");
		}

		[Test]
		public async Task WhenCollectionIsAHashSet_ShouldUseAnUnknownIndex()
		{
			List<string> paths = [];
			object value = new
			{
				Tags = new HashSet<Customer>
				{
					new()
					{
						Id = 1,
						Name = "a",
					},
				},
			};

			Formatter.Format(value, Recording(paths));

			await That(paths).IsEqualTo(["Tags", "Tags[*]", "Tags[*].Id", "Tags[*].Name",])
				.Because("the enumeration index of a hash set changes from run to run");
		}

		[Test]
		public async Task WhenCollectionIsAHashSet_WithSnapshot_ShouldSortByTheWrittenText()
		{
			HashSet<string> value = ["a", "b", "c",];

			string result = Formatter.Format(value, FormattingOptions.Snapshot with
			{
				FormatValue = v => (string?)v.GetValue() switch
				{
					"a" => ValueFormatting.Replace("z"),
					"b" => ValueFormatting.Omit,
					_ => ValueFormatting.Write,
				},
			});

			await That(result).IsEqualTo("[\"c\", \"z\"]")
				.Because("the items are sorted by what is written, after leaving out or replacing them");
		}

		[Test]
		public async Task WhenCollectionItemIsOffered_ShouldUseTheRuntimeTypeOfTheItem()
		{
			List<FormattedValue> offered = [];
			object?[] value = ["a", null,];

			Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					offered.Add(v);
					return ValueFormatting.Write;
				},
			});

			await That(offered.Select(v => v.Type)).IsEqualTo([typeof(string), typeof(object),])
				.Because("an item of null has no runtime type, like in an equivalency comparison");
			await That(offered.Select(v => v.Name)).IsEqualTo(["[0]", "[1]",]);
			await That(offered).All().Satisfy(v => v.Kind == FormattedValueKind.CollectionItem &&
			                                        v.DeclaringType == typeof(object[]));
		}

		[Test]
		public async Task WhenCollectionItemIsOmitted_ShouldKeepTheIndexOfTheLaterItems()
		{
			List<string> paths = [];
			Customer[] value =
			[
				new()
				{
					Id = 1,
				},
				new()
				{
					Id = 2,
				},
				new()
				{
					Id = 3,
				},
			];

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					paths.Add(v.Path);
					return v.Path == "[1]" ? ValueFormatting.Omit : ValueFormatting.Write;
				},
			});

			await That(result).IsEqualTo(
				"[ValueFormatters.FormatValueTests.Customer { Id = 1, Name = \"\" }, ValueFormatters.FormatValueTests.Customer { Id = 3, Name = \"\" }]");
			await That(paths).IsEqualTo(["[0]", "[0].Id", "[0].Name", "[1]", "[2]", "[2].Id", "[2].Name",])
				.Because("the path names the position in the collection, not among the written items");
		}

		[Test]
		[Arguments(0, "[2, 3]", "[\n  2,\n  3\n]")]
		[Arguments(1, "[1, 3]", "[\n  1,\n  3\n]")]
		[Arguments(2, "[1, 2]", "[\n  1,\n  2\n]")]
		public async Task WhenCollectionItemIsOmitted_ShouldWriteTheSeparators(int omittedIndex,
			string expectedSingleLine, string expectedIndented)
		{
			int[] value = [1, 2, 3,];
			Func<FormattedValue, ValueFormatting> formatValue = v => v.Name == $"[{omittedIndex}]"
				? ValueFormatting.Omit
				: ValueFormatting.Write;

			string singleLineResult = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = formatValue,
			});
			string indentedResult = Formatter.Format(value, FormattingOptions.Indented("") with
			{
				FormatValue = formatValue,
			});

			await That(singleLineResult).IsEqualTo(expectedSingleLine);
			await That(indentedResult).IsEqualTo(expectedIndented.Replace("\n", Environment.NewLine));
		}

		[Test]
		public async Task WhenCollectionItemIsReplaced_ShouldWriteTheFormattedReplacement()
		{
			object value = new
			{
				Lines = new[] { "a", "b", },
			};

			string result = Formatter.Format(value, FormattingOptions.Indented("") with
			{
				FormatValue = v => v.Path == "Lines[0]"
					? ValueFormatting.Replace(new
					{
						X = 1,
					})
					: ValueFormatting.Write,
			});

			await That(result).IsEqualTo("""
			                             {
			                               Lines = [
			                                 {
			                                   X = 1
			                                 },
			                                 "b"
			                               ]
			                             }
			                             """);
		}

		[Test]
		public async Task WhenDictionaryEntryIsOffered_ShouldUseTheKeyInTheInvariantCulture()
		{
			using CultureOverride _ = new("de-DE");
			List<FormattedValue> offered = [];
			object value = new
			{
				Prices = new Dictionary<double, Customer?>
				{
					[1.5] = new()
					{
						Id = 1,
					},
					[2.5] = null,
				},
			};

			Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					offered.Add(v);
					return ValueFormatting.Write;
				},
			});

			await That(offered.Select(v => v.Path)).IsEqualTo([
				"Prices", "Prices[1.5]", "Prices[1.5].Id", "Prices[1.5].Name", "Prices[2.5]",
			]).Because("the key is part of the path like in an equivalency comparison, and not offered itself");
			await That(offered[1]).Satisfies(v => v.Kind == FormattedValueKind.CollectionItem &&
			                                      v.Name == "[1.5]" &&
			                                      v.Type == typeof(Customer) &&
			                                      v.DeclaringType == typeof(Dictionary<double, Customer?>));
			await That(offered[4].Type).IsEqualTo(typeof(object));
		}

		[Test]
		public async Task WhenDictionaryEntryIsOmitted_ShouldDropTheEntry()
		{
			Dictionary<string, int> value = new()
			{
				["a"] = 1,
				["b"] = 2,
				["c"] = 3,
			};

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v => v.Path == "[b]" ? ValueFormatting.Omit : ValueFormatting.Write,
			});

			await That(result).IsEqualTo("{[\"a\"] = 1, [\"c\"] = 3}");
		}

		[Test]
		public async Task WhenDictionaryEntryIsReplaced_ShouldKeepTheKey()
		{
			Dictionary<string, int> value = new()
			{
				["a"] = 1,
				["b"] = 2,
			};

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v => v.Path == "[a]" ? ValueFormatting.Replace("{a}") : ValueFormatting.Write,
			});

			await That(result).IsEqualTo("{[\"a\"] = \"{a}\", [\"b\"] = 2}");
		}

		[Test]
		[NotInParallel(nameof(ValueFormatterTests))]
		public async Task WhenFormatterIsRegistered_ShouldNotOfferTheValuesItRenders()
		{
			using IDisposable _ = ValueFormatter.Register(new ProbeFormatter());
			List<string> paths = [];
			object value = new
			{
				Probe = new Probe(() => 1),
			};

			string result = Formatter.Format(value, Recording(paths));

			await That(result).IsEqualTo("{ Probe = probe }");
			await That(paths).IsEqualTo(["Probe",]);
		}

		[Test]
		[NotInParallel(nameof(ValueFormatterTests))]
		public async Task WhenFormatterIsRegistered_ShouldStillOfferTheDictionaryEntries()
		{
			using IDisposable _ = ValueFormatter.Register(new ProbeFormatter());
			List<string> paths = [];
			object value = new Dictionary<string, int>
			{
				["a"] = 1,
			};

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					paths.Add(v.Path);
					return ValueFormatting.Replace(2);
				},
			});

			await That(result).IsEqualTo("{[\"a\"] = 2}");
			await That(paths).IsEqualTo(["[a]",]);
		}

		[Test]
		public async Task WhenGetterThrows_AndHookCatchesIt_ShouldWriteThePlaceholder()
		{
			int reads = 0;
			object value = new Probe(() =>
			{
				reads++;
				throw new InvalidOperationException("getter failed");
			});

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					try
					{
						return v.GetValue() is null ? ValueFormatting.Omit : ValueFormatting.Write;
					}
					catch (InvalidOperationException)
					{
						return ValueFormatting.Write;
					}
				},
			});

			await That(result).IsEqualTo(
					"ValueFormatters.FormatValueTests.Probe { Value = [Value did throw an InvalidOperationException: getter failed] }")
				.Because("a value that cannot be read is written as a placeholder");
			await That(reads).IsEqualTo(1)
				.Because("the value is not read again for writing it");
		}

		[Test]
		public async Task WhenGetterThrows_ShouldWriteThePlaceholder()
		{
			object value = new Probe(() => throw new InvalidOperationException("getter failed"));
			string expectedResult =
				"ValueFormatters.FormatValueTests.Probe { Value = [Value did throw an InvalidOperationException: getter failed] }";

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v => v.GetValue() is null ? ValueFormatting.Omit : ValueFormatting.Write,
			});
			string resultWithoutHook = Formatter.Format(value, FormattingOptions.SingleLine);

			await That(result).IsEqualTo(expectedResult)
				.Because("the exception of the getter is the exception of the value, not of the hook");
			await That(resultWithoutHook).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task WhenHookIsUnset_ShouldWriteTheSameAsAHookThatWritesEverything()
		{
			Node recursive = new();
			recursive.Next = recursive;
			Node deep = new();
			for (int i = 0; i < 25; i++)
			{
				deep = new Node
				{
					Next = deep,
				};
			}

			ConcurrentDictionary<string, int> concurrent = new();
			concurrent["b"] = 1;
			concurrent["a"] = 2;
			object value = new object?[]
			{
				CreateGraph(), recursive, deep, concurrent, (1, 2, 3, 4, 5, 6, 7, 8, 9),
				new List<KeyValuePair<string, int>>
				{
					new("k", 1),
				},
				Task.FromResult(new Customer()), null,
			};
			HashSet<string> paths = [];
			Func<FormattedValue, ValueFormatting> writeAll = v =>
			{
				paths.Add(v.Path);
				return ValueFormatting.Write;
			};

			foreach (FormattingOptions options in new[]
			         {
				         FormattingOptions.SingleLine, FormattingOptions.MultipleLines, FormattingOptions.Snapshot,
				         FormattingOptions.Indented() with
				         {
					         IsSnapshot = true,
				         },
			         })
			{
				string result = Formatter.Format(value, options);
				string resultWithHook = Formatter.Format(value, options with
				{
					FormatValue = writeAll,
				});

				await That(resultWithHook).IsEqualTo(result)
					.Because("a hook that writes every value must not change the output");
			}

			await That(paths).Contains("[4].Item8").And.Contains("[3][a]").And.Contains("[5][0]")
				.And.Contains("[2].Next.Next");
		}

		[Test]
		[NotInParallel(nameof(ValueFormatterTests))]
		public async Task WhenHookThrows_InsideARegisteredFormatter_ShouldThrowTheException()
		{
			using IDisposable _ = ValueFormatter.Register(new WrapperFormatter());
			object value = new
			{
				Wrapped = new Wrapper(new Probe(() => 1)),
			};

			void Act()
				=> Formatter.Format(value, FormattingOptions.SingleLine with
				{
					FormatValue = v => v.Name == nameof(Probe.Value)
						? throw new InvalidOperationException("hook failed")
						: ValueFormatting.Write,
				});

			await That(Act).Throws<InvalidOperationException>().WithMessage("hook failed")
				.Because("an exception of the hook is a bug in the configuration that a placeholder would hide");
		}

		[Test]
		public async Task WhenHookThrows_ShouldThrowTheException()
		{
			object value = new
			{
				Lines = new[]
				{
					new
					{
						Inner = new Probe(() => 1),
					},
				},
			};

			void Act()
				=> Formatter.Format(value, FormattingOptions.SingleLine with
				{
					FormatValue = v => v.Path == "Lines[0].Inner.Value"
						? throw new InvalidOperationException("hook failed")
						: ValueFormatting.Write,
				});

			await That(Act).Throws<InvalidOperationException>().WithMessage("hook failed")
				.Because("an exception of the hook is a bug in the configuration that a placeholder would hide");
		}

		[Test]
		public async Task WhenKeyValuePairIsNoDictionaryEntry_ShouldOfferItsValuesUnderKeyAndValue()
		{
			List<string> paths = [];
			List<KeyValuePair<string, Customer>> value =
			[
				new("a", new Customer
				{
					Id = 1,
				}),
			];

			Formatter.Format(value, Recording(paths));

			await That(paths).IsEqualTo(["[0]", "[0].Value.Id", "[0].Value.Name",])
				.Because("an equivalency comparison compares a pair outside of a dictionary by its members");
		}

		[Test]
		public async Task WhenMemberIsOffered_ShouldUseTheDeclaredType()
		{
			List<FormattedValue> offered = [];
			Holder value = new()
			{
				Count = 1,
				Value = "a",
			};

			Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					offered.Add(v);
					return ValueFormatting.Write;
				},
			});

			await That(offered.Select(v => v.Type)).IsEqualTo([typeof(int), typeof(object),])
				.Because("a nullable value type is unwrapped like in an equivalency comparison");
			await That(offered).All().Satisfy(v => v.Kind == FormattedValueKind.Member &&
			                                        v.DeclaringType == typeof(Holder));
		}

		[Test]
		public async Task WhenMemberIsOmitted_ShouldNotReadIt()
		{
			int reads = 0;
			object value = new Probe(() =>
			{
				reads++;
				return 1;
			});

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = _ => ValueFormatting.Omit,
			});

			await That(result).IsEqualTo("ValueFormatters.FormatValueTests.Probe { }");
			await That(reads).IsEqualTo(0)
				.Because("a member that is left out is only read when the hook asks for its value");
		}

		[Test]
		[Arguments("A", "{ B = 2, C = 3 }", "{\n  B = 2,\n  C = 3\n}")]
		[Arguments("B", "{ A = 1, C = 3 }", "{\n  A = 1,\n  C = 3\n}")]
		[Arguments("C", "{ A = 1, B = 2 }", "{\n  A = 1,\n  B = 2\n}")]
		public async Task WhenMemberIsOmitted_ShouldWriteTheSeparators(string omittedMember,
			string expectedSingleLine, string expectedIndented)
		{
			object value = new
			{
				A = 1,
				B = 2,
				C = 3,
			};
			Func<FormattedValue, ValueFormatting> formatValue = v => v.Name == omittedMember
				? ValueFormatting.Omit
				: ValueFormatting.Write;

			string singleLineResult = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = formatValue,
			});
			string indentedResult = Formatter.Format(value, FormattingOptions.Indented("") with
			{
				FormatValue = formatValue,
			});

			await That(singleLineResult).IsEqualTo(expectedSingleLine);
			await That(indentedResult).IsEqualTo(expectedIndented.Replace("\n", Environment.NewLine));
		}

		[Test]
		public async Task WhenMemberIsRegistered_ShouldOfferItWithoutReflection()
		{
			int reads = 0;
			TypeMetadataRegistry.RegisterProperty<RegisteredCustomer, string>(nameof(RegisteredCustomer.Name), x =>
			{
				reads++;
				return x.Name;
			});
			List<FormattedValue> offered = [];
			RegisteredCustomer value = new()
			{
				Id = 1,
				Name = "a",
			};

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					offered.Add(v);
					return ValueFormatting.Replace($"{v.GetValue()}!");
				},
			});

			await That(result).IsEqualTo("ValueFormatters.FormatValueTests.RegisteredCustomer { Name = \"a!\" }");
			await That(offered).HasSingle().Which.Satisfies(v => v.Path == "Name" && v.Type == typeof(string));
			await That(reads).IsEqualTo(1)
				.Because("the registered getter reads the member, which is not read again for writing it");
		}

		[Test]
		public async Task WhenMemberIsReplaced_ShouldWriteTheFormattedReplacement()
		{
			object value = new
			{
				Created = new DateTime(2026, 10, 17),
				Id = 1,
			};

			string result = Formatter.Format(value, FormattingOptions.Indented("") with
			{
				FormatValue = v => v.Name == "Created"
					? ValueFormatting.Replace("{created}")
					: ValueFormatting.Write,
			});

			await That(result).IsEqualTo("""
			                             {
			                               Created = "{created}",
			                               Id = 1
			                             }
			                             """);
		}

		[Test]
		public async Task WhenMemberIsReplaced_WithACollection_ShouldIndentTheReplacement()
		{
			object value = new
			{
				Inner = new
				{
					Id = 1,
				},
			};

			string result = Formatter.Format(value, FormattingOptions.Indented("") with
			{
				FormatValue = v => v.Path == "Inner"
					? ValueFormatting.Replace(new[] { 1, 2, })
					: ValueFormatting.Write,
			});

			await That(result).IsEqualTo("""
			                             {
			                               Inner = [
			                                 1,
			                                 2
			                               ]
			                             }
			                             """);
		}

		[Test]
		public async Task WhenMultiDimensionalArray_ShouldOfferTheIndexOfEachDimension()
		{
			List<string> paths = [];
			int[,] grid =
			{
				{ 1, 2, },
				{ 3, 4, },
			};

			string result = Formatter.Format(new
			{
				Grid = grid,
			}, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					paths.Add(v.Path);
					return v.Name is "[0,0]" or "[1,0]" or "[1,1]" ? ValueFormatting.Omit : ValueFormatting.Write;
				},
			});

			await That(result).IsEqualTo("{ Grid = [[2], []] }");
			await That(paths).IsEqualTo(["Grid", "Grid[0,0]", "Grid[0,1]", "Grid[1,0]", "Grid[1,1]",]);
		}

		[Test]
		public async Task WhenNested_ShouldBuildThePathLikeAnEquivalencyComparison()
		{
			List<string> paths = [];

			Formatter.Format(CreateGraph(), Recording(paths));

			await That(paths).IsEqualTo([
				"Customer", "Customer.Id", "Customer.Name",
				"Grid", "Grid[0,0]", "Grid[0,1]",
				"Id",
				"Lines", "Lines[0]", "Lines[0].Id", "Lines[0].Text", "Lines[1]", "Lines[1].Id", "Lines[1].Text",
				"Pair", "Pair.Item1", "Pair.Item2", "Pair.Item2.Id", "Pair.Item2.Name",
				"Prices", "Prices[p]", "Prices[q]",
				"Tags", "Tags[*]",
			]);
		}

#if NET8_0_OR_GREATER
		[Test]
		public async Task WhenRecord_ShouldOfferItsMembers()
		{
			List<string> paths = [];
			object value = new CustomerRecord(1, "a");

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					paths.Add(v.Path);
					return v.Name == nameof(CustomerRecord.Id) ? ValueFormatting.Omit : ValueFormatting.Write;
				},
			});

			await That(result).IsEqualTo("ValueFormatters.FormatValueTests.CustomerRecord { Name = \"a\" }");
			await That(paths).IsEqualTo(["Id", "Name",]);
		}
#endif

		[Test]
		public async Task WhenReplacementHasNestedValues_ShouldNotOfferThemAgain()
		{
			List<string> paths = [];
			object value = new
			{
				A = 1,
			};

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					paths.Add(v.Path);
					return ValueFormatting.Replace(new
					{
						B = 2,
					});
				},
			});

			await That(result).IsEqualTo("{ A = { B = 2 } }");
			await That(paths).IsEqualTo(["A",]);
		}

		[Test]
		public async Task WhenTupleHasMoreThanSevenItems_ShouldNumberTheItemsOfItsRestLikeTheOthers()
		{
			List<FormattedValue> offered = [];
			object value = (1, 2, 3, 4, 5, 6, 7, "a", 9L);

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					offered.Add(v);
					return v.Name == "Item8" ? ValueFormatting.Omit : ValueFormatting.Write;
				},
			});

			await That(result).IsEqualTo("(1, 2, 3, 4, 5, 6, 7, 9)");
			await That(offered.Select(v => v.Path)).IsEqualTo([
				"Item1", "Item2", "Item3", "Item4", "Item5", "Item6", "Item7", "Item8", "Item9",
			]).Because("the tuple is written with its items in one pair of parentheses, as C# names them");
			await That(offered.Skip(7).Select(v => v.Type)).IsEqualTo([typeof(string), typeof(long),]);
		}

		[Test]
		public async Task WhenTupleItemIsOffered_ShouldUseTheDeclaredItemType()
		{
			List<FormattedValue> offered = [];
			object value = new
			{
				Pair = (1, (object?)"a", (int?)null),
			};

			Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					offered.Add(v);
					return ValueFormatting.Write;
				},
			});

			await That(offered.Skip(1).Select(v => v.Path)).IsEqualTo(["Pair.Item1", "Pair.Item2", "Pair.Item3",]);
			await That(offered.Skip(1).Select(v => v.Type)).IsEqualTo([typeof(int), typeof(object), typeof(int),]);
			await That(offered.Skip(1)).All().Satisfy(v => v.Kind == FormattedValueKind.TupleItem &&
			                                                v.DeclaringType == typeof(ValueTuple<int, object, int?>));
		}

		[Test]
		public async Task WhenTupleItemIsOmitted_ShouldRemoveItFromTheParentheses()
		{
			object value = (1, 2, 3);

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v => v.Name == "Item2" ? ValueFormatting.Omit : ValueFormatting.Write,
			});

			await That(result).IsEqualTo("(1, 3)");
		}

		[Test]
		public async Task WhenTupleItemIsReplaced_ShouldWriteTheFormattedReplacement()
		{
			object value = (1, 2);

			string result = Formatter.Format(value, FormattingOptions.SingleLine with
			{
				FormatValue = v => v.Name == "Item1" ? ValueFormatting.Replace("x") : ValueFormatting.Write,
			});

			await That(result).IsEqualTo("(\"x\", 2)");
		}

		[Test]
		public async Task WhenUsingMemberToIgnoreRules_ShouldOmitTheMatchingValues()
		{
			MemberToIgnore[] rules =
			[
				new MemberToIgnore.ByName("Id"),
				new MemberToIgnore.ByName("Customer.Name"),
				new MemberToIgnore.ByName("Lines[1]"),
				new MemberToIgnore.ByName("Prices[q]"),
				new MemberToIgnore.ByName("Tags[*]"),
				new MemberToIgnore.ByName("Grid[0,1]"),
				new MemberToIgnore.ByName("Item1"),
			];

			string result = Formatter.Format(CreateGraph(), FormattingOptions.SingleLine with
			{
				FormatValue = v => rules.Any(rule => rule.IgnoreMember(v.Path, v.Type))
					? ValueFormatting.Omit
					: ValueFormatting.Write,
			});

			await That(result).IsEqualTo(
					"{ Customer = ValueFormatters.FormatValueTests.Customer { }, Grid = [[1]], Lines = [{ Text = \"x\" }], Pair = (ValueFormatters.FormatValueTests.Customer { Name = \"b\" }), Prices = {[\"p\"] = 1}, Tags = [] }")
				.Because("the paths match those of an equivalency comparison, so that its rules can be reused");
		}

		private static object CreateGraph()
			=> new
			{
				Customer = new Customer
				{
					Id = 1,
					Name = "a",
				},
				Grid = new[,] { { 1, 2, }, },
				Id = 2,
				Lines = new[]
				{
					new
					{
						Id = 3,
						Text = "x",
					},
					new
					{
						Id = 4,
						Text = "y",
					},
				},
				Pair = (1, new Customer
				{
					Id = 5,
					Name = "b",
				}),
				Prices = new Dictionary<string, int>
				{
					["p"] = 1,
					["q"] = 2,
				},
				Tags = new HashSet<string>
				{
					"t",
				},
			};

		private static FormattingOptions Recording(List<string> paths)
			=> FormattingOptions.SingleLine with
			{
				FormatValue = v =>
				{
					paths.Add(v.Path);
					return ValueFormatting.Write;
				},
			};

		private sealed class Customer
		{
			public int Id { get; set; }
			public string Name { get; set; } = "";
		}

		private sealed class Holder
		{
			public int? Count { get; set; }
			public object? Value { get; set; }
		}

		private sealed class Node
		{
			public Node? Next { get; set; }
		}

		private sealed class Probe(Func<object?> read)
		{
			public object? Value => read();
		}

		private sealed class ProbeFormatter : IValueFormatter
		{
			public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			{
				if (value is not Probe)
				{
					return false;
				}

				stringBuilder.Append("probe");
				return true;
			}
		}

		private sealed class RegisteredCustomer
		{
			public int Id { get; set; }
			public string Name { get; set; } = "";
		}

		private sealed class Wrapper(object inner)
		{
			public object Inner { get; } = inner;
		}

		private sealed class WrapperFormatter : IValueFormatter
		{
			public bool TryFormat(StringBuilder stringBuilder, object value, FormattingOptions? options)
			{
				if (value is not Wrapper wrapper)
				{
					return false;
				}

				stringBuilder.Append("Wrapper(");
				Formatter.Format(stringBuilder, wrapper.Inner, options);
				stringBuilder.Append(')');
				return true;
			}
		}

#if NET8_0_OR_GREATER
		private sealed record CustomerRecord(int Id, string Name);
#endif
	}
}
