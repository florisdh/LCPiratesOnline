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
			int port, maxPort;
			string desc, client, proto;

			switch (args[0])
			{
				// bind [PROTO] [MIN_PORT] [MAX_PORT] [CLIENT] [DESC]
				case "bind":
					if (args.Length != 6)
					{
						Console.WriteLine("Error: Argument mismatch. use: bind [PROTO] [MIN_PORT] [MAX_PORT] [CLIENT] [DESC]");
						return;
					}

					if (!Int32.TryParse(args[2], out port) || !Int32.TryParse(args[3], out maxPort))
					{
						Console.WriteLine("Error: Failed to parse port.");
						return;
					}

					proto = args[1];
					client = args[4];
					desc = args[5];

					// Check if port is used
					while (true)
					{
						try
						{
							IStaticPortMapping item = PORT_MAPPING[port, proto];
							if (item.InternalClient == client)
							{
								//Console.WriteLine("Error: Port {0} is already assigned to self.");
								Console.WriteLine("Succeed. At port " + port.ToString());
								return;
							}
							else
							{
								Console.WriteLine("Error: Port {0} is already used by other client.");
								port++;
								if (port > maxPort)
									return;
							}
						}
						catch (Exception e)
						{
							Console.WriteLine("Port is avaiable.");
							break;
						}	
					}

					// Add Port
					try
					{
						PORT_MAPPING.Add(port, proto, port, client, true, desc);
					}
					catch (Exception e)
					{
						Console.WriteLine("Error: " + e.ToString());
						return;
					}

					Console.WriteLine("Succeed. At port " + port.ToString());

					break;
				// unbind [PROTO] [EXTERN_PORT]
				case "unbind":
					if (args.Length != 3)
					{
						Console.WriteLine("Error: Argument mismatch. use: unbind [PROTO] [EXTERN_PORT]");
						return;
					}

					proto = args[1];

					if (!Int32.TryParse(args[2], out port))
					{
						Console.WriteLine("Error: Failed to parse port.");
						return;
					}

					try
					{
						PORT_MAPPING.Remove(port, proto);
					}
					catch (Exception e)
					{
						Console.WriteLine("Error: " + e.ToString());
						return;
					}

					Console.WriteLine("Succeed. At port " + port.ToString());

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
