using System.Collections.Generic;

namespace System.Data.SQLite.Tools.Async
{
	class SQLiteConnectionPool
	{
		class Entry
		{
			WeakReference<SQLiteConnectionWithLock> connection;

			public SQLiteConnectionString ConnectionString { get; }

			public Entry (SQLiteConnectionString connectionString)
			{
				ConnectionString = connectionString;
			}

			public SQLiteConnectionWithLock Connect ()
			{
				SQLiteConnectionWithLock c = null;
				var wc = connection;
				if (wc == null || !wc.TryGetTarget (out c)) {
					c = new SQLiteConnectionWithLock (ConnectionString);

					// If the database is FullMutex, then we don't need to bother locking
					if (ConnectionString.OpenFlags.HasFlag (SQLiteOpenFlags.FullMutex)) {
						c.SkipLock = true;
					}

					connection = new WeakReference<SQLiteConnectionWithLock> (c);
				}
				return c;
			}

			public void Close ()
			{
				var wc = connection;
				if (wc != null && wc.TryGetTarget (out var c)) {
					c.Close ();
				}
				connection = null;
			}
		}

		readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry> ();
		readonly object _entriesLock = new object ();

		static readonly SQLiteConnectionPool _shared = new SQLiteConnectionPool ();

		/// <summary>
		/// Gets the singleton instance of the connection tool.
		/// </summary>
		public static SQLiteConnectionPool Shared {
			get {
				return _shared;
			}
		}

		public SQLiteConnectionWithLock GetConnection (SQLiteConnectionString connectionString)
		{
			var key = connectionString.UniqueKey;
			Entry entry;
			lock (_entriesLock) {
				if (!_entries.TryGetValue (key, out entry)) {
					entry = new Entry (connectionString);
					_entries [key] = entry;
				}
			}
			return entry.Connect ();
		}

		public void CloseConnection (SQLiteConnectionString connectionString)
		{
			var key = connectionString.UniqueKey;
			Entry entry;
			lock (_entriesLock) {
				if (_entries.TryGetValue (key, out entry)) {
					_entries.Remove (key);
				}
			}
			entry?.Close ();
		}

		/// <summary>
		/// Closes all connections managed by this pool.
		/// </summary>
		public void Reset ()
		{
			List<Entry> entries;
			lock (_entriesLock) {
				entries = new List<Entry> (_entries.Values);
				_entries.Clear ();
			}

			foreach (var e in entries) {
				e.Close ();
			}
		}
	}
}
