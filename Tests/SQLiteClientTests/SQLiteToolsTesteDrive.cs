using System;
using System.Data;
using System.Data.SQLite.Tools;

namespace SQLiteClientTests
{
	public class SQLiteToolsTesteDrive
	{
		private SQLiteConnection db;

		public partial class Stock
		{
			[PrimaryKey, AutoIncrement]
			public int Id { get; set; }
			public string Symbol { get; set; }
		}

		public void Executar()
		{
			try 
			{
				db = new SQLiteConnection(@"SqliteTest3.db");
				db.CreateTable<Stock> ();

				AddStock ("TesteA");
				AddStock ("TesteB");
				AddStock ("TesteC");
			} 
			catch (Exception ex) 
			{
				Console.WriteLine(ex.Message);
			}

		}

		public void AddStock (string symbol)
		{
			try {
				var stock = new Stock () {
					Symbol = symbol
				};
				db.Insert (stock);
				Console.WriteLine(stock.Symbol + " == " + stock.Id);
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}
	}
}
