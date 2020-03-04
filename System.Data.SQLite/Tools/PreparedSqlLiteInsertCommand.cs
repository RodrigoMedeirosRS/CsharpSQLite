using Sqlite3Statement = System.Data.SQLite.Sqlite3.Vdbe;

namespace System.Data.SQLite.Tools
{
	/// <summary>
	/// Since the insert never changed, we only need to prepare once.
	/// </summary>
	class PreparedSqlLiteInsertCommand : IDisposable
	{
		bool Initialized;

		SQLiteConnection Connection;

		string CommandText;

		Sqlite3Statement Statement;
		static readonly Sqlite3Statement NullStatement = default (Sqlite3Statement);

		public PreparedSqlLiteInsertCommand (SQLiteConnection conn, string commandText)
		{
			Connection = conn;
			CommandText = commandText;
		}

		public int ExecuteNonQuery (object [] source)
		{
			if (Initialized && Statement == NullStatement) {
				throw new ObjectDisposedException (nameof (PreparedSqlLiteInsertCommand));
			}

			if (Connection.Trace) {
				Connection.Tracer?.Invoke ("Executing: " + CommandText);
			}

			var r = Result.OK;

			if (!Initialized) {
				Statement = SQLite3.Prepare2 (Connection.Handle, CommandText);
				Initialized = true;
			}

			//bind the values.
			if (source != null) {
				for (int i = 0; i < source.Length; i++) {
					SQLiteCommand.BindParameter (Statement, i + 1, source [i], Connection.StoreDateTimeAsTicks, Connection.DateTimeStringFormat, Connection.StoreTimeSpanAsTicks);
				}
			}
			r = SQLite3.Step (Statement);

			if (r == Result.Done) {
				int rowsAffected = SQLite3.Changes (Connection.Handle);
				SQLite3.Reset (Statement);
				return rowsAffected;
			} else if (r == Result.Error) {
				string msg = SQLite3.GetErrmsg (Connection.Handle);
				SQLite3.Reset (Statement);
				throw SQLiteException.New (r, msg);
			} else if (r == Result.Constraint && SQLite3.ExtendedErrCode (Connection.Handle) == ExtendedResult.ConstraintNotNull) {
				SQLite3.Reset (Statement);
				throw NotNullConstraintViolationException.New (r, SQLite3.GetErrmsg (Connection.Handle));
			} else {
				SQLite3.Reset (Statement);
				throw SQLiteException.New (r, r.ToString ());
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		void Dispose (bool disposing)
		{
			var s = Statement;
			Statement = NullStatement;
			Connection = null;
			if (s != NullStatement) {
				SQLite3.Finalize (s);
			}
		}

		~PreparedSqlLiteInsertCommand ()
		{
			Dispose (false);
		}
	}
}
