namespace System.Data.SQLite.Tools
{
	public class SQLiteException : Exception
	{
		public Result Result { get; private set; }

		protected SQLiteException (Result r, string message) : base (message)
		{
			Result = r;
		}

		public static SQLiteException New (Result r, string message)
		{
			return new SQLiteException (r, message);
		}
	}
}
