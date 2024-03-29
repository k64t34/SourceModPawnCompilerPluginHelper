﻿
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Collections;

namespace SourceModPawnCompilerPluginHelper
{
	class Program
	{
		#region Windows size & position //https://www.cyberforum.ru/csharp-beginners/thread300550.html
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);//https://studassistent.ru/charp/centralnoe-polozhenie-okna-konsoli-c
		static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		static readonly IntPtr HWND_TOP = new IntPtr(0);
		const UInt32 SWP_NOSIZE = 0x0001;
		const UInt32 SWP_NOMOVE = 0x0002;
		const UInt32 SWP_NOZORDER = 0x0004;
		const UInt32 SWP_NOREDRAW = 0x0008;
		const UInt32 SWP_NOACTIVATE = 0x0010;
		const UInt32 SWP_FRAMECHANGED = 0x0020;
		const UInt32 SWP_SHOWWINDOW = 0x0040;
		const UInt32 SWP_HIDEWINDOW = 0x0080;
		const UInt32 SWP_NOCOPYBITS = 0x0100;
		const UInt32 SWP_NOOWNERZORDER = 0x0200;
		const UInt32 SWP_NOSENDCHANGING = 0x0400;

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		[DllImport("user32.dll")]// https://studassistent.ru/charp/centralnoe-polozhenie-okna-konsoli-c
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);		
		#endregion
		//Global 
		static string mySMcomp_Folder; //Gets the directory where the SMcompiler.exe  is stored.	
		static string SourceFile; //Source plugin file .sp
		static string SourceFolder;//Path to source plugin file .sp
		const  string INIFile = "smcmphlp.ini"; //INI file name for source plugins
		
		//static string INIFileFullPath = ""; //Full path and filename to current INI file
		static string PluginFolder; //root plugin folder with .git 		
		const  string Compilator = "spcomp.exe";// Pawn compiler for Sourcemod plugin
		static string Compilator_Include_FoldersList="";
		//static string[] Compilator_Include_FoldersArray;
		static ArrayList Compilator_Include_FoldersArray = new ArrayList();
		static string SRCDS_Folder; // Full path to srcds folder
		//INI file params
		static string ini_Compilator_Folder="";
		static string ini_Compilator_Params="";//= " -h ";//"vasym=\"1\" -O2";
		static string ini_Compilator_Include_FoldersList = "";// "smk64t\\scripting\\include";
		static string ini_Plugin_Author="";
		static string ini_rcon_Address="";
		static int ini_rcon_Port = 27015;
		static string ini_rcon_password="";
		static string ini_SRCDS_Folder="";
		static string ini_Hostname="";
		static string ini_Share="";
		static string ini_Share_User="";
		static string ini_Share_Password="";		
		static bool ini_RestartMap = false;
		static bool ini_RestartServer = false;

		static string SMXFolder = @"game\addons\sourcemod\plugins\";
		const ConsoleColor BGcolor = ConsoleColor.Black;
		const ConsoleColor FGcolor = ConsoleColor.White;
		const ConsoleColor FGcolorH1 = ConsoleColor.Yellow;
		const ConsoleColor FGcolorFieldName = ConsoleColor.DarkGray;
		const ConsoleColor FGcolorFieldValue = ConsoleColor.White;
		public static void Console_ResetColor() {Console.BackgroundColor = BGcolor;Console.ForegroundColor = FGcolor;}
		public static void Console_SetH1Color() { Console.BackgroundColor = BGcolor; Console.ForegroundColor = FGcolorH1; }		
		public static void Main(string[] args)
		{			
			string title = "Sourcemod Compiler Helper ver " + (FileVersionInfo.GetVersionInfo((Assembly.GetExecutingAssembly()).Location)).ProductVersion ;
			Console.Title = title;
			Console_ResetColor();
			Console.Clear();			//Console.BackgroundColor = ConsoleColor.Blue;						//Console.ForegroundColor = ConsoleColor.DarkBlue;	
			Console.ForegroundColor =FGcolorH1;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("----------------------------------------------------------------");
			Console.WriteLine(title);
			Console.WriteLine("----------------------------------------------------------------");
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(@"Free contact me over GitHub https://github.com/k64t34/SourceModPawnCompilerPluginHelper/issues"+"\n");

			Console.ForegroundColor =ConsoleColor.White;
            #region Parsing argumets
            if (args.Length < 1)
			{
				Console.WriteLine("Usage: SMcompiler.exe <path\\file.sp>");
				ScriptFinish(true);
				System.Environment.Exit(0);
			}
			//Console.Write(" "); Console.WriteLine(args[0]);
			Console_ResetColor();            
            mySMcomp_Folder = AppDomain.CurrentDomain.BaseDirectory;ini_Compilator_Folder = mySMcomp_Folder;
			// or			
			//Application.ExecutablePath;
			//Assembly.GetExecutingAssembly().Location;
			//Application.StartupPath;
			ConsoleWriteField("SMcompiler folder", mySMcomp_Folder);			
			Debug.Print("Args count=" + args.Length);
			Console.Title = title + @"   " + args[0] + @"   " + DateTime.Now.ToString();
			SourceFile = args[0];			
			if (!File.Exists(SourceFile))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ERR: File \"" + SourceFile + "\" not found");
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(1);
			}
			SourceFolder = System.IO.Directory.GetParent(SourceFile).ToString() + "\\";
			EndFolderBackslash(ref SourceFolder);
			SourceFile = Path.GetFileNameWithoutExtension(SourceFile);
			Console.Title = title + "   "+SourceFile + ".sp    " + DateTime.Now.ToString();			
			Console.ForegroundColor = FGcolorFieldName;
			Console.Write("Source file \t" );
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(SourceFolder);
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine(SourceFile + ".sp");
			Console_ResetColor();
			#endregion
			PluginFolder =  System.IO.Directory.GetParent(SourceFolder).ToString();			
			if (String.Compare(Path.GetFileName(PluginFolder), "SCRIPTING", true) == 0)
			{
				PluginFolder = System.IO.Directory.GetParent(PluginFolder).ToString();
				PluginFolder = System.IO.Directory.GetParent(PluginFolder).ToString();
				PluginFolder = System.IO.Directory.GetParent(PluginFolder).ToString();
				PluginFolder = System.IO.Directory.GetParent(PluginFolder).ToString();
			}
			EndFolderBackslash(ref PluginFolder);
			ConsoleWriteField("Plugin Folder", PluginFolder);						
			GetConfigFile(mySMcomp_Folder + INIFile);
			GetConfigFile(PluginFolder + INIFile);
			ConsoleWriteField("Compilator_Folder", ini_Compilator_Folder);
			SRCDS_Folder = ini_SRCDS_Folder;
			EndFolderBackslash(ref SRCDS_Folder);
			SRCDS_Folder = @"\\" + ini_Hostname + @"\" + ini_Share + @"\" + SRCDS_Folder;
			if (String.IsNullOrEmpty(ini_rcon_Address)) ini_rcon_Address = ini_Hostname;			
			#region Create include file datetime.inc			
			string curDate = DateTime.Now.ToString("dd.MM.yy HH:mm:ss");
			System.IO.StreamWriter f_inc = new System.IO.StreamWriter(SourceFolder + "datetimecomp.inc", false);
			f_inc.WriteLine("#if defined DEBUG");
			f_inc.WriteLine("\t#define PLUGIN_DATETIME \"" + curDate + "\"");
			f_inc.WriteLine("\t#if defined PLUGIN_VERSION");
			f_inc.WriteLine("\t\t#undef PLUGIN_VERSION");
			f_inc.WriteLine("\t#endif");
			f_inc.WriteLine("\t#define PLUGIN_VERSION \"" + curDate + "\"");
			f_inc.WriteLine("#endif");
			f_inc.WriteLine("#if !defined PLUGIN_NAME");
			f_inc.WriteLine("\t#define PLUGIN_NAME \"" + SourceFile + "\"");
			f_inc.WriteLine("#endif");
			f_inc.WriteLine("#if !defined PLUGIN_AUTHOR");
			f_inc.WriteLine("\t#define PLUGIN_AUTHOR \"" + ini_Plugin_Author + "\"");
			f_inc.WriteLine("#endif");
			f_inc.Close();
            #endregion
            #region Delete old err smx files
            if (!File.Exists(SourceFile + ".err")) File.Delete(SourceFile + ".err");
			#endregion

			#region Test Pawn compiler file spcomp.exe exist
			if (!File.Exists(ini_Compilator_Folder + Compilator))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				if (Directory.Exists(ini_Compilator_Folder))
					Console.WriteLine("ERR: File spcomp.exe Pawn compiler {0} not found in folder {1}", Compilator, ini_Compilator_Folder);
				else
					Console.WriteLine("ERR: Folder {0} with file spcomp.exe Pawn compiler {1} not found.", ini_Compilator_Folder, Compilator);
				Console_ResetColor();
				
				ScriptFinish(true);
				System.Environment.Exit(4);
			}
            #endregion
			#region Prepare folder PLUGINS (game\addons\sourcemod\plugins) for compiled files , delete old smx files
            if (Directory.Exists(PluginFolder + SMXFolder))
			{ if (File.Exists(PluginFolder + SMXFolder + SourceFile + ".smx")) File.Delete(PluginFolder + SMXFolder + SourceFile + ".smx"); }
			else
			{ System.IO.Directory.CreateDirectory(PluginFolder + SMXFolder); }
            #endregion
			#region Compiling
            Console.ForegroundColor = FGcolorH1;
			Console.Write("Run compiling "); Console.ForegroundColor = FGcolorFieldValue;
			Process compiler = new Process();
			compiler.StartInfo.RedirectStandardOutput = true;
			compiler.StartInfo.RedirectStandardError = true;
			compiler.StartInfo.CreateNoWindow = true;
			compiler.StartInfo.WorkingDirectory = PluginFolder;
			//compiler.StartInfo.WorkingDirectory = SourceFolder;
			
			compiler.StartInfo.FileName = ini_Compilator_Folder + Compilator;
			Console.WriteLine(compiler.StartInfo.FileName);
			compiler.StartInfo.UseShellExecute = false; //https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo.workingdirectory(v=vs.110).aspx			
			string DiffSourceFolder = FolderDifference(SourceFolder, PluginFolder);
			string buffArg;
			buffArg = " " + DiffSourceFolder + SourceFile + ".sp";
			Console.WriteLine(buffArg);
			compiler.StartInfo.Arguments += buffArg;

			buffArg = " -D\"" + TrimEndBackslash(PluginFolder) + "\"";
			Console.WriteLine(buffArg);
			compiler.StartInfo.Arguments += buffArg;

			buffArg = " -e" + DiffSourceFolder + SourceFile + ".err";
			Console.WriteLine(buffArg);
			compiler.StartInfo.Arguments += buffArg;

			buffArg = " -o" + SMXFolder + SourceFile + ".smx";
			Console.WriteLine(buffArg);
			compiler.StartInfo.Arguments += buffArg;

			if (ini_Compilator_Params.Length != 0)
			{
				Console.WriteLine(" {0}", ini_Compilator_Params);
				compiler.StartInfo.Arguments += " " + ini_Compilator_Params;
			}

			#region Parsing include from INI file
			//Compilator_Include_FoldersArray[Compilator_Include_FoldersArray.Length - 1] = FolderDifference(SourceFolder, PluginFolder);			
			Compilator_Include_FoldersList = " -i" + SourceFolder;
			foreach (string s in Compilator_Include_FoldersArray)
			{
				String p="\""+FolderDifference(s, PluginFolder)+ "\"";
				Console.WriteLine(" -i "+ p);					
				Compilator_Include_FoldersList += " -i" + p;
			}
			#endregion
			compiler.StartInfo.Arguments += Compilator_Include_FoldersList;
			//Console.WriteLine(compiler.StartInfo.Arguments);
			compiler.StartInfo.UseShellExecute = false;
			compiler.StartInfo.RedirectStandardOutput = true;			
			ConsoleWriteField("WorkingDirectory", compiler.StartInfo.WorkingDirectory);
            #region Set console windows size
            IntPtr ConsoleHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
			const UInt32 WINDOW_FLAGS = SWP_SHOWWINDOW;
			var wndRect = new RECT();
			GetWindowRect(ConsoleHandle, out wndRect); // Получили  размеры текущего она консоли
			var cWidth = wndRect.Right - wndRect.Left;
			var cHeight = wndRect.Bottom - wndRect.Top;
			//Rectangle waRectangle;			

			//int ScreenHeight = SystemInformation.PrimaryMonitorSize.Height;
			int ScreenHeight = SystemInformation.WorkingArea.Height;

			SetWindowPos(ConsoleHandle, HWND_NOTOPMOST, 0, 0, cWidth, cHeight, WINDOW_FLAGS);
			SetWindowPos(ConsoleHandle, HWND_NOTOPMOST, 0, 0, cWidth, ScreenHeight, WINDOW_FLAGS);
            #endregion

            #region Run Compiler
            try
            {
				compiler.Start();
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e.Message);
				Console_ResetColor();
			}
			string output = compiler.StandardOutput.ReadToEnd();
			string err = compiler.StandardError.ReadToEnd();
			if (output.Length != 0) 			Console.WriteLine(output);
			if (err.Length != 0) Console.WriteLine(err);
			compiler.WaitForExit();
			//Delete datetime.inc
			if (File.Exists(SourceFolder + "datetimecomp.inc")) File.Delete(SourceFolder + "datetimecomp.inc");
			//ERRORLEVEL
			ConsoleColor ERRORLEVEL_color;
			if (compiler.ExitCode > 0) ERRORLEVEL_color = ConsoleColor.Red;
			else
			{
				if (File.Exists(SourceFolder + SourceFile + ".err")) ERRORLEVEL_color = ConsoleColor.Yellow;
				else ERRORLEVEL_color = ConsoleColor.Green;
			}
			Console.ForegroundColor = ERRORLEVEL_color;
			Console.WriteLine("ERRORLEVEL="+compiler.ExitCode);
			Console_ResetColor();
			if (File.Exists(SourceFolder + SourceFile + ".err"))
			{
				Console.ForegroundColor = ERRORLEVEL_color;
				if (compiler.ExitCode > 0)
					Console.WriteLine("ERR: " + SourceFolder + SourceFile + ".err\n--------------------------------------------------------");
				else
					Console.WriteLine("WARN: " + SourceFolder + SourceFile + ".err\n--------------------------------------------------------");

				Console_ResetColor();
				try
				{
					using (StreamReader sr = new StreamReader(SourceFolder + SourceFile + ".err"))
					{
						String line = sr.ReadToEnd();
						Console.WriteLine(line);
					}
				}
				catch (Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("The file could not be read " + SourceFolder + SourceFile + ".err");
					Console.WriteLine(e.Message);
					Console_ResetColor();
				}
			}
			if (compiler.ExitCode > 0)
			{
				ScriptFinish(true);
				System.Environment.Exit(0);
			}
			#endregion
			#endregion
			#region Copy to server

			#region Check Hostname & SRCDS_Folder 			

			#endregion
			Console.ForegroundColor = FGcolorH1;
			Console.Write("Copy output files ");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.Write(SourceFile + ".smx");
			Console.ForegroundColor = FGcolorH1;
			Console.Write(" from folder ");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.Write(PluginFolder + "game");
			Console.ForegroundColor = FGcolorH1;
			Console.Write(" to ");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.WriteLine(SRCDS_Folder);
			#region NET USE
			if (isSMBPath(SRCDS_Folder))
			{
				if (!Directory.Exists(SRCDS_Folder))
				{
					try
					{
						compiler.StartInfo.FileName = Path.Combine(Environment.GetEnvironmentVariable("windir") + @"\system32", "NET.exe");
						compiler.StartInfo.Arguments = @"USE \\" + ini_Hostname + @"\" + ini_Share + " /USER:" + ini_Share_User + " " + ini_Share_Password;
						Console.WriteLine(compiler.StartInfo.FileName + " " + compiler.StartInfo.Arguments);
						compiler.Start();
						output = compiler.StandardOutput.ReadToEnd();
						err = compiler.StandardError.ReadToEnd();
						if (output.Length != 0) Console.WriteLine(output);
						if (err.Length != 0) Console.WriteLine(err);
						compiler.WaitForExit();
						//}
					}
					catch (Exception e)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(e.Message);
						Console_ResetColor();
					}
					//NetworkCredential theNetworkCredential = new NetworkCredential("dod","12345678");
					//CredentialCache theNetcache = new CredentialCache();
					//theNetcache.Add(@"\\192.168.56.102\tmp", 445, "Basic", theNetworkCredential);
					//then do whatever, such as getting a list of folders:
					//string[] theFolders = System.IO.Directory.GetDirectories(@"\\192.168.56.102\tmp");
				}
			}
			#endregion
			if (!Directory.Exists(SRCDS_Folder))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ERR:Folder for copy smx file " + SRCDS_Folder + "not found");
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(0);
			}
			CopyDirectory(PluginFolder + "game", SRCDS_Folder);
			#endregion

			#region Reload plugin
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("\nReload plugin {0} on server {1}:{2}\n", SourceFile, ini_rcon_Address, ini_rcon_Port);
			Console_ResetColor();

			//make you class https://developer.valvesoftware.com/wiki/Source_RCON_Protocol
			SourceRcon.SourceRcon RCon = new SourceRcon.SourceRcon();
			RCon.Errors += new SourceRcon.StringOutput(ErrorOutput);
			RCon.ServerOutput += new SourceRcon.StringOutput(ConsoleOutput);

			try
			{				
				IPAddress address;
				if (!IPAddress.TryParse(ini_rcon_Address, out address))
				{
					IPHostEntry host = Dns.GetHostEntry(ini_rcon_Address);
					ini_rcon_Address = host.AddressList[0].ToString();
					Console.WriteLine("Host {0} have IP address {1}", ini_Hostname, ini_rcon_Address); Console_ResetColor();
				}

				Console.Write("Connect to {0}:{1}", ini_rcon_Address, ini_rcon_Port);
				RCon.Connect(new IPEndPoint(IPAddress.Parse(ini_rcon_Address), ini_rcon_Port), ini_rcon_password);
				for (int i = 0; i != 10; i++)
				{
					Thread.Sleep(1000);
					if (RCon.Connected) break;
					Console.Write(".");
				}
				Console.WriteLine(" OK");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message.ToString());
			}
			//if (RCon.Connect(new IPEndPoint(IPAddress.Parse(rcon_Address), rcon_Port), rcon_password))
			if (RCon.Connected)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Connected"); Console_ResetColor();

				RCon.ServerCommand("status");
				Thread.Sleep(100);
				if (ini_RestartServer)
				{
					Console.WriteLine("Restart server");
					RCon.ServerCommand("_restart");
					Thread.Sleep(5000);
				}
				else if (ini_RestartMap)
				{
					Console.WriteLine("Restart map");
					RCon.ServerCommand("sm_map ");
					Thread.Sleep(5000);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Reload plugin"); Console_ResetColor();
					RCon.ServerCommand("sm plugins unload " + SourceFile);
					Thread.Sleep(1000);
					RCon.ServerCommand("sm plugins load " + SourceFile);
					Thread.Sleep(1000);
				}
				Thread.Sleep(1000);
				RCon.ServerCommand("sm plugins info " + SourceFile);
				Thread.Sleep(1000);
				/*Console.Write("Press Esc key to exit . . . ");			
				ConsoleKeyInfo k=Console.ReadKey();
				while (k.Key!= ConsoleKey.Escape)
				{
					k=Console.ReadKey();
					RCon.ServerCommand(Console.ReadLine());
					Console.ReadLine
				}*/
				/*while(true)
				{
				RCon.ServerCommand(Console.ReadLine());
				}				*/
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ERR: No connection.");
				Console_ResetColor();
			}
			RCon = null;
			//Thread.Sleep(10000);
			#endregion 
			ScriptFinish(true);
			System.Environment.Exit(0);

		}

		static void ErrorOutput(string input) { Console.WriteLine("Error: {0}", input); }

		static void ConsoleOutput(string input) { Console.WriteLine("Console: {0}", input); }
		//****************************************************	
		public static void ScriptFinish(bool pause)
		{
			//****************************************************			
			if (pause)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				Console.Write("Press any key to exit . . . "); Console_ResetColor();
				DateTime timeoutvalue = DateTime.Now.AddSeconds(10);
				while (DateTime.Now < timeoutvalue)
				{
					if (Console.KeyAvailable) break;
					Thread.Sleep(1000);
					Console.Write(".");
				}
#if DEBUG
				Console.ReadKey(true);
#endif
			}
		}		
		public static void GetConfigFile(string ConfigFile)
		{
            #region
            if (!File.Exists(ConfigFile))
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("WARN:INI File \t" + ConfigFile + " not found");
				Console_ResetColor();//				ScriptFinish(true);				System.Environment.Exit(2);
				return;
			}
			string INIFolder= Path.GetFullPath(ConfigFile);
			Console_SetH1Color();
			Console.Write("Read config file\t");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.WriteLine(ConfigFile);
			Console_ResetColor();

            IniParser inifile = new IniParser(ConfigFile);
			string buffStr = inifile.ReadString("Compiler", "Compilator_Folder", "");
			if (!String.IsNullOrEmpty(buffStr))
			{
				ini_Compilator_Folder = buffStr;
				EndFolderBackslash(ref ini_Compilator_Folder, ParentFolder(PluginFolder));
			}
			ini_Plugin_Author=inifile.ReadString("Compiler", "Plugin_Author",ini_Plugin_Author);
			ini_rcon_Address = inifile.ReadString("Server", "rcon_Address", ini_rcon_Address);
			ini_rcon_password= inifile.ReadString("Server", "rcon_password", ini_rcon_password);
			ini_Compilator_Params = inifile.ReadString("Compiler", "Parameters", ini_Compilator_Params);
			ini_Hostname = inifile.ReadString("Server", "Hostname", ini_Hostname);
			ini_Share = inifile.ReadString("Server", "Share", ini_Share);
			ini_Share_User = inifile.ReadString("Server", "Share_User", ini_Share_User);
			ini_Share_Password = inifile.ReadString("Server", "Share_Password", ini_Share_User);
			ini_RestartMap = inifile.ReadBool("Server", "RestartMap",		ini_RestartMap);
			ini_RestartServer = inifile.ReadBool("Server", "RestartServer", ini_RestartServer);
			ini_rcon_Port = inifile.ReadInteger("Server", "rcon_port",		ini_rcon_Port);
			ini_SRCDS_Folder = inifile.ReadString("Server", "SRCDS_Folder", ini_SRCDS_Folder);			
			ini_Compilator_Include_FoldersList = inifile.ReadString("Compiler", "Include", "");
			if (!String.IsNullOrEmpty(ini_Hostname))
			{
				SRCDS_Folder = @"\" + ini_Share + @"\" + ini_SRCDS_Folder;
				SRCDS_Folder=SRCDS_Folder.Replace(@"\\", @"\");
				SRCDS_Folder = @"\\" + ini_Hostname + SRCDS_Folder;
			}
			else SRCDS_Folder = ini_SRCDS_Folder;

#if DEBUG
			Debug.Print("----------------------------------");
			Debug.Print(" INI parameters from "+ ConfigFile);
			Debug.Print("----------------------------------");
			Debug.Print("Plugin_Author\t\t=" + ini_Plugin_Author);
			Debug.Print("RestartMap\t\t=" + ini_RestartMap);
			Debug.Print("RestartServer\t\t=" + ini_RestartServer);
			Debug.Print("Compilator\t\t=" + Compilator);
			Debug.Print("Compilator_Folder\t=" + ini_Compilator_Folder);
			Debug.Print("Compilator_Params\t=" + ini_Compilator_Params);
			Debug.Print("Compilator_Include_FoldersList=" + ini_Compilator_Include_FoldersList);
			Debug.Print("Hostname\t=" + ini_Hostname);
			Debug.Print("Share\t=" + ini_Share);
			Debug.Print("Share_User\t=" + ini_Share_User);
			Debug.Print("Share_Password\t=" + ini_Share_Password);
			Debug.Print("SRCDS_Folder=" + ini_SRCDS_Folder);
			//ConsoleWriteField("SRCDS_Folder", SRCDS_Folder);
			Debug.Print("rcon_address=" + ini_rcon_Address);
			Debug.Print("rcon_port=" + ini_rcon_Port);
			Debug.Print("rcon_password=" + ini_rcon_password);
			Debug.Print("SRCDS_Folder=" + SRCDS_Folder);
			
#endif
			#endregion
			#region Parsing SRCDS folder

			
            #endregion
            #region Parsing include path from INI file
            string[] ini_Compilator_Include_FoldersArray = ini_Compilator_Include_FoldersList.Split(';');
			//Array.Resize(ref Compilator_Include_FoldersArray, Compilator_Include_FoldersArray.Length + 1);
			//Compilator_Include_FoldersArray[Compilator_Include_FoldersArray.Length - 1] = FolderDifference(SourceFolder, PluginFolder);
			//ConsoleWriteField("Include", "", false);
			//bool firstBlock = true;
			//Console.WriteLine("Compilator_Include_FoldersArray");
			int i = 1;			
			string FolderiniFile= ParentFolder(ConfigFile);
			//Console.WriteLine("FolderiniFile {0}", FolderiniFile);
			string PathString;
			foreach (string s in ini_Compilator_Include_FoldersArray)
			{
				PathString = s.Trim();
				if (!String.IsNullOrEmpty(PathString))
				{
					PathString = makeFullPath(FolderiniFile, PathString);
					if (!String.IsNullOrEmpty(PathString))
						if (UniqPath(PathString))
							Compilator_Include_FoldersArray.Add(PathString);
				}
			}
			#if DEBUG
			Debug.Print("Compilator_Include_FoldersArray:");
			foreach (string s in Compilator_Include_FoldersArray) Debug.Print(s);			
			#endif

			//	Console.WriteLine("{0} \"{1}\"",i, PathString);
			//	Console.WriteLine("makeFullPath= {0}", makeFullPath(FolderiniFile, PathString));
			//	if (!String.IsNullOrEmpty(PathString))				{

			//		if (!firstBlock) Console.Write("\t\t");

			//		Compilator_Include_FoldersList += " -i" + PathString;
			//		firstBlock = false;
			//	}
			//	Console.WriteLine();
			//	i++;
			//}
#endregion
		}
		public static void SetOnlyExistValue(IniParser inifile, string SectionName, String KeyName, ref String oldValue)
		{
#if DEBUG
			Console.Write("{0} {1} \"{2}\"", SectionName, KeyName, oldValue);
#endif
			String buff = inifile.ReadString(SectionName, KeyName, "");
			if (!String.IsNullOrEmpty(buff))
				oldValue = buff;
#if DEBUG
			Console.WriteLine("  \"{0}\" \"{1}\"",buff, oldValue);
#endif
		}
		public static bool isSMBPath(string Path) 
		{
			bool isSMBPath = false;
			string SMBpatern = @"^\\{2}(?(?=\S)[^\\]|^$)+(\\(?(?=\S)[^\\]|^$)+)+(\\|\S)$";
			Match m = Regex.Match(Path, SMBpatern);
			isSMBPath = (Path.Length == m.Groups[0].Length);			
			return isSMBPath;
		}
		public static bool isLocalPath(string Path)
		{
			bool isLocalPath = false;
			string Localpatern = @"^[A-Za-z]:(\\(?(?=\S)[^\\]|^$)+)+(\\|\S)$";
			Match m = Regex.Match(Path, Localpatern);
			isLocalPath = (Path.Length == m.Groups[0].Length);
			return isLocalPath;
		}
		public static string makeFullPath(string basePath, string relativePath)
		{
			string FullPath = "";
			if (isLocalPath(relativePath)) FullPath = relativePath;
			else
			{
				string fp = basePath + relativePath;
				DirectoryInfo f = new DirectoryInfo(fp);
				if (f.Exists)
				{
					FullPath = f.FullName;
				}
			}
			
			return FullPath;
            #region Base path + relative path description            
			//SMcomp.ini
			//----------
			//basefolder=mySMcomp_Folder
			//include_path 1 (full or relative) - > fullpath 
			//include_path 2 (full or relative) - > fullpath 
			//:
			//include_path n (full or relative) - > fullpath 

			//plugin.ini
			//----------
			//basefolder=PluginFolder
			//include_path 1 (full or relative) - > fullpath 
			//include_path 2 (full or relative) - > fullpath 
			//:
			//include_path n (full or relative) - > fullpath 

			//transform all fullpath to relativepath base on PluginFolder	 
		  
            #endregion

        }
		public static bool UniqPath(string Path)
        {
			bool UniqPath = true;
			foreach (String p in Compilator_Include_FoldersArray)if (String.Equals(p,Path, StringComparison.OrdinalIgnoreCase)){ UniqPath = false;break; }			
			return UniqPath;
		}
		public static void EndFolderBackslash(ref string s)
		{
			//*******************************************
			s = s.Trim();
			if (!s.EndsWith("\\")) s += "\\";
		}
		public static void EndFolderBackslash(ref string s, string basepath)
		{
			s = s.Trim();
			if (!s.EndsWith("\\")) s += "\\";
			if (s.StartsWith(".."))
			{
				s = s.Remove(0, 3);
				s = basepath + s;
			}
			else
			{
				if (!s.StartsWith("\\") & !s.StartsWith("\\\\") & s.Substring(1, 2) != ":\\")
					s = basepath + s;
			}
		}
		public static string FolderDifference(string Minuend, string Subtrahend)
		{
			if (Minuend.StartsWith(Subtrahend))
				return Minuend.Substring(Subtrahend.Length);
			else
				return Minuend;
		}
		private static void CopyDirectory(string sourcePath, string destinationPath)
		{
			//-----------------------------------------------------------------------
			System.IO.DirectoryInfo sourceDirectoryInfo = new System.IO.DirectoryInfo(sourcePath);

			// If the destination folder don't exist then create it
			if (!System.IO.Directory.Exists(destinationPath))
			{
				System.IO.Directory.CreateDirectory(destinationPath);
			}

			System.IO.FileSystemInfo fileSystemInfo = null;
			foreach (FileSystemInfo fileSystemInfo_loopVariable in sourceDirectoryInfo.GetFileSystemInfos())
			{
				fileSystemInfo = fileSystemInfo_loopVariable;
				string destinationFileName = System.IO.Path.Combine(destinationPath, fileSystemInfo.Name);

				// Now check whether its a file or a folder and take action accordingly
				if (fileSystemInfo is System.IO.FileInfo)
				{
					System.IO.File.Copy(fileSystemInfo.FullName, destinationFileName, true); Console.WriteLine(
						 FolderDifference(sourcePath, PluginFolder) + "\\" + fileSystemInfo);
				}
				else
				{
					// Recursively call the mothod to copy all the neste folders
					CopyDirectory(fileSystemInfo.FullName, destinationFileName);
				}
			}
		}
		public static string TrimEndBackslash(string Folder) { return Folder.TrimEnd(new char[] { '\\' }); }

		public static string ParentFolder(string Folder)
		{
			//Folder=Folder.TrimEnd(new char[]{'\\'});
			//if (Folder.EndsWith("\\")) Folder.Remove(Folder.Length-2,1);
			//Console.WriteLine("Folder=\t\t\"{0}\"",Folder);		
			//Console.WriteLine("ParentFolder=\t{0}",System.IO.Directory.GetParent(Folder).ToString());		
			//Console.WriteLine("ParentFolder=\t{0}",Path.GetDirectoryName(Folder));		
			return System.IO.Directory.GetParent(Folder.TrimEnd(new char[] { '\\' })).ToString() + "\\";
		}
		public static void ConsoleWriteField(string Name,string Value, bool CR=true) 
		{
			Console.ForegroundColor = FGcolorFieldName;
			Console.Write(Name+"\t");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.Write(Value + "\t");
			if (CR) Console.WriteLine();
		}
		
	}
}
//Usage: spcomp<filename>[filename...][options]

//Options:
//-a       output assembler code
//         -Dpath   active directory path
//         -e<name> set name of error file (quiet compile)
//         -H < hwnd > window handle to send a notification message on finish
//         -h       show included file paths
//         -i<name> path for include files
//         -l       create list file (preprocess only)
//         -o < name > set base name of(P-code) output file
//         -O<num>  optimization level (default=-O2)
//             0    no optimization
//             2    full optimizations
//         -p<name> set name of "prefix" file
//         -t<num>  TAB indent size (in character positions, default=8)
//         -v < num > verbosity level; 0 = quiet, 1 = normal, 2 = verbose(default = 1)
//			   - w < num > disable a specific warning by its number
//         -z<num>  compression level, default=9 (0=none, 1=worst, 9=best)
//         -E       treat warnings as errors
//         -\       use '\' for escape characters
//         -^       use '^' for escape characters
//         -;< +/->require a semicolon to end each statement (default=-)
//         sym = val  define constant "sym" with value "val"
//         sym=     define constant "sym" with value 0

//Options may start with a dash or a slash; the options "-d0" and "/d0" are
//equivalent.

//Options with a value may optionally separate the value from the option letter
//with a colon (":"), an equal sign ("="), or a space (" "). That is, the options "-d0", "-d=0",
//"-d:0", and "-d 0" are all equivalent. "-;" is an exception to this and cannot use a space.