using System;
using System.Diagnostics;
using System.IO;

public class UPnPHelper
{
	private static string APP_PATH = Path.GetFullPath(".") + "\\UPnPHelper.exe";

	public static string BIND(string client, int locPort, int remPort)
	{
		string res = EXECUTE_COMMAND(APP_PATH,
			string.Format("bind UDP {0} {1} {2} LCPiratesOnline", locPort, remPort, client)
		);
		return res;
	}

	public static void UNBIND(int remPort)
	{
		string res = EXECUTE_COMMAND(APP_PATH,
			string.Format("unbind UDP " + remPort.ToString())
		);
	}

	private static string EXECUTE_COMMAND(string path, string command)
	{
		Process p = new Process();
		p.StartInfo.FileName = path;
		p.StartInfo.Arguments = command;
		p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		p.StartInfo.RedirectStandardOutput = true;
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.CreateNoWindow = true;
		p.Start();
		p.WaitForExit();
		return p.StandardOutput.ReadToEnd();
	}
}