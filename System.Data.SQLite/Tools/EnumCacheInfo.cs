using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace System.Data.SQLite.Tools
{
	class EnumCacheInfo
	{
		public EnumCacheInfo (Type type)
		{
			var typeInfo = type.GetTypeInfo ();

			IsEnum = typeInfo.IsEnum;

			if (IsEnum) {
				StoreAsText = typeInfo.CustomAttributes.Any (x => x.AttributeType == typeof (StoreAsTextAttribute));

				if (StoreAsText) {
					EnumValues = new Dictionary<int, string> ();
					foreach (object e in Enum.GetValues (type)) {
						EnumValues [Convert.ToInt32 (e)] = e.ToString ();
					}
				}
			}
		}

		public bool IsEnum { get; private set; }

		public bool StoreAsText { get; private set; }

		public Dictionary<int, string> EnumValues { get; private set; }
	}
}
