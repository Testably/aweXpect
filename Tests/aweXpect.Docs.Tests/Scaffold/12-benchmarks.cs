global using static Snippets.Prelude;

namespace Snippets;

internal static class Prelude
{
	public static string _stringSubject = "foo";
	public static string _stringExpectation = "FOO";
	public static bool _boolSubject = true;
	public static int _intSubject = 2;
	public static int _intMinimum = 1;
	public static Nested _nestedSubject = new();
	public static Nested _nestedExpectation = new();
	public static IEnumerable<int> _enumerableSubject = Enumerable.Range(1, 1000);
	public static int _enumerableCount = 1000;
	public static string[] _stringArraySubject = ["foo", "bar", "baz",];
	public static string[] _stringArrayExpectation = ["foo", "bar", "baz",];
	public static string[] _stringArrayAnyOrderSubject = ["foo", "bar", "baz",];
	public static string[] _stringArrayAnyOrderExpectation = ["baz", "foo", "bar",];

	public class Nested
	{
		public Nested? Inner { get; set; }
	}
}
