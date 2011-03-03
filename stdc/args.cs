using System;
using System.Collections.Generic;
using System.Text;

namespace stdc {
	public static partial class C {

		/// <summary>
		/// Build the standard C argv array from the .NET argument list.
		/// The .NET variant does not contain the program name. so we emulate
		/// it by injecting the application domain friendly name.
		/// </summary>
		/// <param name="args">The .NET argument list</param>
		/// <returns></returns>
		public static string[] ToArgv (string[] args)
		{
			string[] argv = new string[args.Length + 1];
			Array.Copy (args, 0, argv, 1, args.Length);
			argv[0] = System.AppDomain.CurrentDomain.FriendlyName;
			return argv;
		}

		/// <summary>
		/// The number of arguments in the array (1 more than in the original
		/// .NET array)
		/// </summary>
		/// <param name="args">The .NET argument list</param>
		/// <returns></returns>
		public static int ToArgc (string[] args)
		{
			return args.Length + 1;
		}
	}
}
