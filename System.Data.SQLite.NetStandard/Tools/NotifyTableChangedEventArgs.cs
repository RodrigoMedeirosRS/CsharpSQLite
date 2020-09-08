namespace System.Data.SQLite.Tools
{
	public class NotifyTableChangedEventArgs : EventArgs
	{
		public TableMapping Table { get; private set; }
		public NotifyTableChangedAction Action { get; private set; }

		public NotifyTableChangedEventArgs (TableMapping table, NotifyTableChangedAction action)
		{
			Table = table;
			Action = action;
		}
	}
}
