using System;
using NATUPNPLib;

namespace UPnPHelper
{
	class Program
	{
		#region Vars

		private static UPnPNATClass UPNP_CONN;
		private static IStaticPortMappingCollection PORT_MAPPING;

		#endregion

		#region Entry

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Error: No arguments given.");
				return;
			}

			Console.WriteLine("Welcome to UPnP Helper by Floris de Haan.");
			Console.WriteLine("Loading...");

			// Connect to UPnP
			UPNP_CONN = new UPnPNATClass();
			PORT_MAPPING = UPNP_CONN.StaticPortMappingCollection;

			if (PORT_MAPPING == null)
			{
				Console.WriteLine("Error: Failed to load UPnP.");
				return;
			}

			Console.WriteLine("UPnP has been loaded succesfully.");

			// Get app task
			int externalPort, internalPort;
			string desc, client, proto;

			switch (args[0])
			{
				// bind [PROTO] [INTERN_PORT] [EXTERN_PORT] [CLIENT] [DESC]
				case "bind":
					if (args.Length != 6)
					{
						Console.WriteLine("Error: Argument mismatch. use: bind [PROTO] [INTERN_PORT] [EXTERN_PORT] [CLIENT] [DESC]");
						return;
					}
					
					if (!Int32.TryParse(args[2], out externalPort) || !Int32.TryParse(args[3], out internalPort))
					{
						Console.WriteLine("Error: Failed to parse port.");
						return;
					}

					proto = args[1];
					client = args[4];
					desc = args[5];

					try
					{
						PORT_MAPPING.Add(externalPort, proto, internalPort, client, true, desc);
					}
					catch (Exception e)
					{
						Console.WriteLine("Error: " + e.ToString());
						return;
					}

					Console.WriteLine("Succeed.");

					break;
				// unbind [PROTO] [EXTERN_PORT]
				case "unbind":
					if (args.Length != 3)
					{
						Console.WriteLine("Error: Argument mismatch. use: unbind [PROTO] [EXTERN_PORT]");
						return;
					}

					proto = args[1];

					if (!Int32.TryParse(args[2], out externalPort))
					{
						Console.WriteLine("Error: Failed to parse port.");
						return;
					}

					try
					{
						PORT_MAPPING.Remove(externalPort, proto);
					}
					catch (Exception e)
					{
						Console.WriteLine("Error: " + e.ToString());
						return;
					}

					Console.WriteLine("Succeed.");

					break;
				default:
					Console.WriteLine("Unknown command.");
					break;
			}
		}

		#endregion

		#region Methods

		#endregion
	}
}
