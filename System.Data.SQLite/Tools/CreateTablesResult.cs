using System.Collections.Generic;

namespace System.Data.SQLite.Tools
{
	public class CreateTablesResult
	{
		public Dictionary<Type, CreateTableResult> Results { get; private set; }

		public CreateTablesResult ()
		{
			Results = new Dictionary<Type, CreateTableResult> ();
		}
	}
}
