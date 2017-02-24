using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AccessMethodsPerformance
{
	class Program
	{
		static void Main(string[] args)
		{
			var users = GenerateUsers(20000);
			Func<int> testManual = () => CreateListViewItems.CreateListItemsManual(users).Count;
			Func<int> testReflection = () => CreateListViewItems.CreateListItemsReflection(typeof(User), users).Count;
			Func<int> testCompiledExpression = () => CreateListViewItems.CreateListItemsCompiledExpression(typeof(User), users).Count;

			int cold = 100;
			int run = 100;

			Console.WriteLine("Create ListViewItem items:");
			while (users.Count >= 5)
			{
				Console.Write(users.Count + ",");
				var manual = Test(testManual, cold, run);
				Console.Write(manual + ",");
				var reflection = Test(testReflection, cold, run);
				Console.Write(reflection + ",");
				var compiledExpression = Test(testCompiledExpression, cold, run);
				Console.WriteLine(compiledExpression);
				users = users.Take(users.Count/2).ToList();
			}

			Console.WriteLine("Create ListViewItem items:");
			while (users.Count >= 5)
			{
				Console.Write(users.Count + ",");
				var manual = Test(testManual, cold, run);
				Console.Write(manual + ",");
				var reflection = Test(testReflection, cold, run);
				Console.Write(reflection + ",");
				var compiledExpression = Test(testCompiledExpression, cold, run);
				Console.WriteLine(compiledExpression);
				users = users.Take(users.Count/2).ToList();
			}
		}

		private static long Test(Func<int> test, int cold, int run)
		{
			int n = cold;
			bool last = false;
			var sw = new Stopwatch();
			long elapsed;
		GO:
			sw.Restart();
			for (int i = 0; i < n; i++)
			{
				test();
			}
			elapsed = sw.ElapsedMilliseconds;
			if (!last)
			{
				n = run;
				last = true;
				goto GO;
			}
			return elapsed;
		}
		
		private static List<User> GenerateUsers(int count)
		{
			var users = new List<User>();
			var rnd = new Random(12345);
			for (int i = 0; i < count; i++)
			{
				users.Add(new User
				{
					Id = i + 1,
					Name = GenerateName(rnd),
					BirthDate = new DateTime(1950, 1, 1).AddDays(rnd.Next(24000)),
				});
			}
			return users;
		}

		private static string GenerateName(Random rnd)
		{
			var length = rnd.Next(3, 11);
			return new string(Enumerable.Range(0, length).Select(r => (char)((r == 0 ? 'A' : 'a') + rnd.Next(26))).ToArray());
		}
	}
}
