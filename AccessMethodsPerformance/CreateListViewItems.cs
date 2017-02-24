using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace AccessMethodsPerformance
{
	class CreateListViewItems
	{
		public static List<ListViewItem> CreateListItemsManual(List<User> users)
		{
			var items = new List<ListViewItem>();
			foreach (var user in users)
			{
				var subitems = new[]
				{
					user.Id.ToString(),
					user.Name,
					user.BirthDate.ToString("dd.MM.yyyy (ddd)"),
				};
				var lvi = new ListViewItem(subitems);
				items.Add(lvi);
			}
			return items;
		}

		public static List<ListViewItem> CreateListItemsReflection(Type type, IEnumerable<object> users)
		{
			var items = new List<ListViewItem>();
			var fields = type.GetFields();
			foreach (var user in users)
			{
				var subitems = new string[fields.Length];
				for (int i = 0; i < fields.Length; i++)
				{
					string value;
					var field = fields[i];
					if (field.FieldType == typeof(string))
					{
						value = (string)field.GetValue(user);
					}
					else if (field.FieldType == typeof(int))
					{
						value = ((int)field.GetValue(user)).ToString();
					}
					else if (field.FieldType == typeof(DateTime))
					{
						value = ((DateTime)field.GetValue(user)).ToString("dd.MM.yyyy (ddd)");
					}
					else
					{
						value = field.GetValue(user).ToString();
					}
					subitems[i] = value;
				}
				var lvi = new ListViewItem(subitems);
				items.Add(lvi);
			}
			return items;
		}

		public static List<ListViewItem> CreateListItemsCompiledExpression(Type type, IEnumerable<object> users)
		{
			var items = new List<ListViewItem>();
			var fields = type.GetFields();
			Func<object, string>[] fieldGetters = new Func<object, string>[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				Func<object, string> fieldGetter;
				Expression<Func<object, string>> lambda;
				var field = fields[i];
				// user => 
				var userObject = Expression.Parameter(typeof(object), "user");
				// user => (User)user
				var user = Expression.Convert(userObject, type);
				// user => ((User)user)."Field"
				var fld = Expression.Field(user, field);
				if (field.FieldType == typeof(string))
				{
					// user => ((User)user)."Field"
					lambda = Expression.Lambda<Func<object, string>>(fld, userObject);
				}
				else if (field.FieldType == typeof(int))
				{
					// user => ((User)user)."Field".ToString() // int.ToString()
					var toString = Expression.Call(fld, typeof(int).GetMethod("ToString", new Type[0]));
					lambda = Expression.Lambda<Func<object, string>>(toString, userObject);
				}
				else if (field.FieldType == typeof(DateTime))
				{
					// user => ((User)user)."Field".ToString("dd.MM.yyyy (ddd)")
					var toString = Expression.Call(
						fld,
						typeof(DateTime).GetMethod("ToString", new Type[] { typeof(string) }),
						Expression.Constant("dd.MM.yyyy (ddd)"));
					lambda = Expression.Lambda<Func<object, string>>(toString, userObject);
				}
				else
				{
					// user => ((User)user)."Field".ToString() // object.ToString()
					var toString = Expression.Call(fld, typeof(object).GetMethod("ToString", new Type[0]));
					lambda = Expression.Lambda<Func<object, string>>(toString, userObject);
				}
				fieldGetter = lambda.Compile();
				fieldGetters[i] = fieldGetter;
			}
			foreach (var user in users)
			{
				var subitems = new string[fields.Length];
				for (int i = 0; i < fields.Length; i++)
				{
					subitems[i] = fieldGetters[i](user);
				}
				var lvi = new ListViewItem(subitems);
				items.Add(lvi);
			}
			return items;
		}
	}
}
