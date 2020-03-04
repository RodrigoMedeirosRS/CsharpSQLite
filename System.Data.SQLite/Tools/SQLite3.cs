using Sqlite3DatabaseHandle = System.Data.SQLite.Sqlite3.sqlite3;
using Sqlite3Statement = System.Data.SQLite.Sqlite3.Vdbe;
using Sqlite3BackupHandle = System.Data.SQLite.Sqlite3.sqlite3_backup;

namespace System.Data.SQLite.Tools
{
	public static class SQLite3
	{
		const string LibraryPath = "sqlite3";

		public static Result Open (string filename, out Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_open (filename, out db);
		}

		public static Result Open (string filename, out Sqlite3DatabaseHandle db, int flags, string vfsName)
		{
			return (Result)Sqlite3.sqlite3_open_v2 (filename, out db, flags, vfsName);
		}

		public static Result Close (Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close (db);
		}

		public static Result Close2 (Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_close (db);
		}

		public static Result BusyTimeout (Sqlite3DatabaseHandle db, int milliseconds)
		{
			return (Result)Sqlite3.sqlite3_busy_timeout (db, milliseconds);
		}

		public static int Changes (Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_changes (db);
		}

		public static Sqlite3Statement Prepare2 (Sqlite3DatabaseHandle db, string query)
		{
			Sqlite3Statement stmt = default (Sqlite3Statement);
			stmt = new Sqlite3Statement ();
			var r = Sqlite3.sqlite3_prepare_v2 (db, query, -1, ref stmt, 0);
			if (r != 0) {
				throw SQLiteException.New ((Result)r, GetErrmsg (db));
			}
			return stmt;
		}

		public static Result Step (Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_step (stmt);
		}

		public static Result Reset (Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_reset (stmt);
		}

		public static Result Finalize (Sqlite3Statement stmt)
		{
			return (Result)Sqlite3.sqlite3_finalize (stmt);
		}

		public static long LastInsertRowid (Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_last_insert_rowid (db);
		}

		public static string GetErrmsg (Sqlite3DatabaseHandle db)
		{
			return Sqlite3.sqlite3_errmsg (db);
		}

		public static int BindParameterIndex (Sqlite3Statement stmt, string name)
		{
			return Sqlite3.sqlite3_bind_parameter_index (stmt, name);
		}

		public static int BindNull (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_bind_null (stmt, index);
		}

		public static int BindInt (Sqlite3Statement stmt, int index, int val)
		{
			return Sqlite3.sqlite3_bind_int (stmt, index, val);
		}

		public static int BindInt64 (Sqlite3Statement stmt, int index, long val)
		{
			return Sqlite3.sqlite3_bind_int64 (stmt, index, val);
		}

		public static int BindDouble (Sqlite3Statement stmt, int index, double val)
		{
			return Sqlite3.sqlite3_bind_double (stmt, index, val);
		}

		public static int BindText (Sqlite3Statement stmt, int index, string val, int n, IntPtr free)
		{
			return Sqlite3.sqlite3_bind_text (stmt, index, val, n, null);
		}

		public static int BindBlob (Sqlite3Statement stmt, int index, byte [] val, int n, IntPtr free)
		{
			return Sqlite3.sqlite3_bind_blob (stmt, index, val, n, null);
		}

		public static int ColumnCount (Sqlite3Statement stmt)
		{
			return Sqlite3.sqlite3_column_count (stmt);
		}

		public static string ColumnName (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name (stmt, index);
		}

		public static string ColumnName16 (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_name (stmt, index);
		}

		public static ColType ColumnType (Sqlite3Statement stmt, int index)
		{
			return (ColType)Sqlite3.sqlite3_column_type (stmt, index);
		}

		public static int ColumnInt (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int (stmt, index);
		}

		public static long ColumnInt64 (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_int64 (stmt, index);
		}

		public static double ColumnDouble (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_double (stmt, index);
		}

		public static string ColumnText (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text (stmt, index);
		}

		public static string ColumnText16 (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text (stmt, index);
		}

		public static byte [] ColumnBlob (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_blob (stmt, index);
		}

		public static int ColumnBytes (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_bytes (stmt, index);
		}

		public static string ColumnString (Sqlite3Statement stmt, int index)
		{
			return Sqlite3.sqlite3_column_text (stmt, index);
		}

		public static byte [] ColumnByteArray (Sqlite3Statement stmt, int index)
		{
			int length = ColumnBytes (stmt, index);
			if (length > 0) {
				return ColumnBlob (stmt, index);
			}
			return new byte [0];
		}

		public static Result EnableLoadExtension (Sqlite3DatabaseHandle db, int onoff)
		{
			return (Result)Sqlite3.sqlite3_enable_load_extension (db, onoff);
		}

		public static int LibVersionNumber ()
		{
			return Sqlite3.sqlite3_libversion_number ();
		}

		public static Result GetResult (Sqlite3DatabaseHandle db)
		{
			return (Result)Sqlite3.sqlite3_errcode (db);
		}

		public static ExtendedResult ExtendedErrCode (Sqlite3DatabaseHandle db)
		{
			return (ExtendedResult)Sqlite3.sqlite3_errcode (db);
		}

		public static Sqlite3BackupHandle BackupInit (Sqlite3DatabaseHandle destDb, string destName, Sqlite3DatabaseHandle sourceDb, string sourceName)
		{
			return Sqlite3.sqlite3_backup_init (destDb, destName, sourceDb, sourceName);
		}

		public static Result BackupStep (Sqlite3BackupHandle backup, int numPages)
		{
			return (Result)Sqlite3.sqlite3_backup_step (backup, numPages);
		}

		public static Result BackupFinish (Sqlite3BackupHandle backup)
		{
			return (Result)Sqlite3.sqlite3_backup_finish (backup);
		}

		public enum ColType : int
		{
			Integer = 1,
			Float = 2,
			Text = 3,
			Blob = 4,
			Null = 5
		}
	}
}
