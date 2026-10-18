using System.Collections.Generic;

namespace aweXpect.Snapshots.PublicApi.Fixture;

public static class ExtensionMembers
{
	extension(string text)
	{
		public bool IsBlank => string.IsNullOrWhiteSpace(text);
		public bool IsLongerThan(int length = 10) => text.Length > length;
	}

	extension(string)
	{
		public static string Default
		{
			get => _default;
			set => _default = value.Trim();
		}

		public static string Create(char character, int count) => new(character, count);
	}

	private static string _default = "";

	extension<T>(IEnumerable<T> source) where T : class
	{
		public T? FirstOrNothing() => null;
	}

	extension(int number)
	{
		internal bool IsHidden => number < 0;
	}

	extension([Marker("receiver")] IList<string> list)
	{
		public bool IsEmpty => list.Count == 0;
	}

	public static bool IsOld(this string text) => false;
	public static bool IsNew([Marker("this")] this string text) => true;
}
