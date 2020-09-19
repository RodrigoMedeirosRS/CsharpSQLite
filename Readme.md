# SQLiteSharp-NET

SQLiteSharp-NET is a pure C# SQLite ORM made by a combination of two another powerfull libraries, SQLite-Net and CsharpSQLite, it's is a open source library developed to allow NetFramework, .NetCore and Mono applications to store data in [SQLite 3 databases](http://www.sqlite.org) without any external .dll or .so libraries, providing a better support to [Godot Engine](https://godotengine.org) games.

SQLiteSharp-NET  was designed as a quick and convenient database layer. Its design follows from these *goals*:

* Very easy to integrate with existing projects and runs on all Godot Engine, .NetFramework, .NetCore and Mono.
  
* Thin wrapper over SQLite that is fast and efficient. (This library should not be the performance bottleneck of your queries.)

* Native C# acess to SQLite 3 functions.
  
* Very simple methods for executing CRUD operations and queries safely (using parameters) and for retrieving the results of those query in a strongly typed fashion.
  
* Works with your data model without forcing you to change your classes. (Contains a small reflection-driven ORM layer.)

## Source Installation

SQLite-net is all contained into System.Data.SQLite project, to install it into your project you need only copy all files into System.Data.SQLite.NetStandard folder, include System.Data.SQLite.csproj to your solution file and into your project add reference to ystem.Data.SQLite.csproj file.

## Please Contribute!

This is an open source project that welcomes contributions/suggestions/bug reports from those who use it. If you have any ideas on how to improve the library, please [post an issue here on GitHub](https://github.com/praeclarum/sqlite-net/issues). Please check out the [How to Contribute](https://github.com/praeclarum/sqlite-net/wiki/How-to-Contribute).

# Example Time!

Please consult the Wiki for, ahem, [complete documentation](https://github.com/praeclarum/sqlite-net/wiki).

The library contains simple attributes that you can use to control the construction of tables. In a simple stock program, you might use:

```csharp
public class Stock
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	public string Symbol { get; set; }
}

public class Valuation
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }
	[Indexed]
	public int StockId { get; set; }
	public DateTime Time { get; set; }
	public decimal Price { get; set; }
}
```

Once you've defined the objects in your model you have a choice of APIs. You can use the "synchronous API" where calls
block one at a time, or you can use the "asynchronous API" where calls do not block. You may care to use the asynchronous
API for mobile applications in order to increase responsiveness.

Both APIs are explained in the two sections below.

## Synchronous API

Once you have defined your entity, you can automatically generate tables in your database by calling `CreateTable`:

```csharp
// Get an absolute path to the database file
var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");

var db = new SQLiteConnection(databasePath);
db.CreateTable<Stock>();
db.CreateTable<Valuation>();
```

You can insert rows in the database using `Insert`. If the table contains an auto-incremented primary key, then the value for that key will be available to you after the insert:

```csharp
public static void AddStock(SQLiteConnection db, string symbol) {
	var stock = new Stock() {
		Symbol = symbol
	};
	db.Insert(stock);
	Console.WriteLine("{0} == {1}", stock.Symbol, stock.Id);
}
```

Similar methods exist for `Update` and `Delete`.

The most straightforward way to query for data is using the `Table` method. This can take predicates for constraining via WHERE clauses and/or adding ORDER BY clauses:

```csharp
var query = db.Table<Stock>().Where(v => v.Symbol.StartsWith("A"));

foreach (var stock in query)
	Console.WriteLine("Stock: " + stock.Symbol);
```

You can also query the database at a low-level using the `Query` method:

```csharp
public static IEnumerable<Valuation> QueryValuations (SQLiteConnection db, Stock stock) {
	return db.Query<Valuation> ("select * from Valuation where StockId = ?", stock.Id);
}
```

The generic parameter to the `Query` method specifies the type of object to create for each row. It can be one of your table classes, or any other class whose public properties match the column returned by the query. For instance, we could rewrite the above query as:

```csharp
public class Val
{
	public decimal Money { get; set; }
	public DateTime Date { get; set; }
}

public static IEnumerable<Val> QueryVals (SQLiteConnection db, Stock stock) {
	return db.Query<Val> ("select \"Price\" as \"Money\", \"Time\" as \"Date\" from Valuation where StockId = ?", stock.Id);
}
```

You can perform low-level updates of the database using the `Execute` method.

## Asynchronous API

The asynchronous library uses the Task Parallel Library (TPL). As such, normal use of `Task` objects, and the `async` and `await` keywords 
will work for you.

Once you have defined your entity, you can automatically generate tables by calling `CreateTableAsync`:

```csharp
// Get an absolute path to the database file
var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");

var db = new SQLiteAsyncConnection(databasePath);

await db.CreateTableAsync<Stock>();

Console.WriteLine("Table created!");
```

You can insert rows in the database using `Insert`. If the table contains an auto-incremented primary key, then the value for that key will be available to you after the insert:

```csharp
var stock = new Stock()
{
	Symbol = "AAPL"
};

await db.InsertAsync(stock);

Console.WriteLine("Auto stock id: {0}", stock.Id);
```

Similar methods exist for `UpdateAsync` and `DeleteAsync`.

Querying for data is most straightforwardly done using the `Table` method. This will return an `AsyncTableQuery` instance back, whereupon
you can add predicates for constraining via WHERE clauses and/or adding ORDER BY. The database is not physically touched until one of the special 
retrieval methods - `ToListAsync`, `FirstAsync`, or `FirstOrDefaultAsync` - is called.

```csharp
var query = db.Table<Stock>().Where(s => s.Symbol.StartsWith("A"));

var result = await query.ToListAsync();

foreach (var s in result)
	Console.WriteLine("Stock: " + s.Symbol);
```

There are a number of low-level methods available. You can also query the database directly via the `QueryAsync` method. Over and above the change 
operations provided by `InsertAsync` etc you can issue `ExecuteAsync` methods to change sets of data directly within the database.

Another helpful method is `ExecuteScalarAsync`. This allows you to return a scalar value from the database easily:

```csharp
var count = await db.ExecuteScalarAsync<int>("select count(*) from Stock");

Console.WriteLine(string.Format("Found '{0}' stock items.", count));
```

## Manual SQL

**sqlite-net** is normally used as a light ORM (object-relational-mapper) using the methods `CreateTable` and `Table`.
However, you can also use it as a convenient way to manually execute queries.

Here is an example of creating a table, inserting into it (with a parameterized command), and querying it without using ORM features.

```csharp
db.Execute ("create table Stock(Symbol varchar(100) not null)");
db.Execute ("insert into Stock(Symbol) values (?)", "MSFT");
var stocks = db.Query<Stock> ("select * from Stock");
```
## SQLite 3 Direct Acces
If you want you can acces the SQLite 3 funcions directly from CsharpSQLite abstraction. Here is an example:

```csharp
SQLiteConnection con = new SQLiteConnection();

string dbFilename = @"SqliteTest3.db";
string cs = string.Format("Version=3;uri=file:{0}", dbFilename);

if(File.Exists(dbFilename))
	File.Delete(dbFilename);

con.ConnectionString = cs;
con.Open();

IDbCommand cmd = con.CreateCommand();
cmd.CommandText = "CREATE TABLE TEST_TABLE ( COLA INTEGER, COLB TEXT, COLC DATETIME )";
cmd.ExecuteNonQuery();

cmd.CommandText = "INSERT INTO TEST_TABLE ( COLA, COLB, COLC ) VALUES (123,'ABC','2008-12-31 18:19:20' )";
cmd.ExecuteNonQuery();

cmd.CommandText = "INSERT INTO TEST_TABLE ( COLA, COLB, COLC ) VALUES (124,'DEF', '2009-11-16 13:35:36' )";
cmd.ExecuteNonQuery();

cmd.CommandText = "SELECT COLA, COLB, COLC FROM TEST_TABLE";
IDataReader reader = cmd.ExecuteReader();
int r = 0;
while(reader.Read())
{
	int i = reader.GetInt32(reader.GetOrdinal("COLA"));

	string s = reader.GetString(reader.GetOrdinal("COLB"));

	DateTime dt = reader.GetDateTime(reader.GetOrdinal("COLC"));

	r++;
}
con.Close();
con = null;
```
## Want More???
Please check the SQLiteClientTeste to see much more samples!

## Thank you!

Thank you to the Godot Engine, Mono and .NET community for embracing this project, and thank you to all the contributors who have helped to make this great.

## Help Me!

If you like the code, clik on link bellow and buy me a coffe :)

https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=FPQKAA8A9KYAJ&source=url
