using System;
using System.Diagnostics;
using System.IO;

public class UPnPHelper
{
	private static string APP_PATH = Path.GetFullPath(".") + "\\UPnPHelper.exe";

	public static event UPnPEvent RESULT;
	public delegate void UPnPEvent(UPnPHelperResult res);

	public static void BIND(string client, int remPort, int maxRemPort)
	{
		EXECUTE_COMMAND(APP_PATH,
			string.Format("bind UDP {0} {1} {2} LCPiratesOnline", remPort, maxRemPort, client)
		);
	}

	public static void UNBIND(int remPort)
	{
		EXECUTE_COMMAND(APP_PATH,
			string.Format("unbind UDP " + remPort.ToString())
		);
	}

	private static void EXECUTE_COMMAND(string path, string command)
	{
		Process p = new Process();
		p.StartInfo.FileName = path;
		p.StartInfo.Arguments = command;
		p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.CreateNoWindow = true;
		p.EnableRaisingEvents = true;
		p.Exited += (s, e) => { EXECUTE_DONE(p.StandardOutput.ReadToEnd()); };
		p.Start();
	}

	private static void EXECUTE_DONE(string res)
	{
		UPnPHelperResultType resType = UPnPHelperResultType.UnknownError;
		int remPort = -1;

		if (res.Contains("Succeed"))
		{
			resType = UPnPHelperResultType.Succeed;

			// Read Port
			int baseIndex = res.IndexOf("Succeed. At port ");
			if (baseIndex >= 0)
				remPort = int.Parse(res.Substring(baseIndex + 17, 5));
		}
		else if (res.Contains("Argument mismatch") || res.Contains("Unknown command"))
		{
			resType = UPnPHelperResultType.VersionMismatch;
		}
		else if (res.Contains("Failed to load UPnP"))
			resType = UPnPHelperResultType.FailedToLoadUPnP;

		if (RESULT != null)
			RESULT(new UPnPHelperResult(resType, remPort));
	}
}

public struct UPnPHelperResult
{
	public UPnPHelperResultType Type;
	public int ExternPort;
	public UPnPHelperResult(UPnPHelperResultType type, int externalPort)
	{
		Type = type;
		ExternPort = externalPort;
	}
}

public enum UPnPHelperResultType
{
	Succeed = 0,
	UnknownError = 1,
	VersionMismatch = 2,
	FailedToLoadUPnP = 3
}