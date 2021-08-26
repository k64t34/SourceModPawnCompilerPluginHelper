
// TODO:Добавить rcon консоль
// TODO:дублировние  консоли ссервера
// TODO:не копировать на сервер папку scripting
// TODO: не копировать на сервер типы файлов err, bak
// TODO: NET USE <\\server> /USER:<username>

//TODO:EXT1 - Изменить проверку файла, если указан не полный путь к файлу а тольк имя
//TODO:EXT3 - Рекурсивный поиск INI файла вверх и в стороны.
//TODO:EXT4 - Проверять расширения
//
//TODO:->Доступ к файлам сервера по FTP
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
		static string INIFile = "smcmphlp.ini"; //INI file for source plugin
		static string PluginFolder; //root plugin folder with .git 
		static string INIFolder; //path INI file		
								 //enum Plugin_Folder_Structure {Singl=0,Strucure_1=1}; //List possible plugins folder structure
								 //Plugin_Folder_Strucure plstr=Plugin_Folder_Structure.Strucure_1;  //using plugins folder structure
								 //Ini file fields
		static string Compilator = "spcomp.exe";// Compiler for Sourcemod plugin
		static string Compilator_Folder;
		static string Compilator_Params = "";//"vasym=\"1\" -O2";
		static string Compilator_Include_Folders = "smk64t\\scripting\\include";
		static string Plugin_Author;
		static string rcon_Address = "127.0.0.1";
		static int rcon_Port = 27015;
		static string rcon_password;
		static string SRCDS_Folder;
		static string SMXFolder = "game\\addons\\sourcemod\\plugins\\";
		static bool MapReload = false;
		const ConsoleColor BGcolor = ConsoleColor.Black;
		const ConsoleColor FGcolor = ConsoleColor.White;
		const ConsoleColor FGcolorH1 = ConsoleColor.Yellow;
		const ConsoleColor FGcolorFieldName = ConsoleColor.DarkGray;
		const ConsoleColor FGcolorFieldValue = ConsoleColor.White;
		public static void Console_ResetColor() {Console.BackgroundColor = BGcolor;Console.ForegroundColor = FGcolor;}
		public static void Console_SetH1Color() { Console.BackgroundColor = BGcolor; Console.ForegroundColor = FGcolorH1; }
		/*public struct Plugin_folder 
		{
    		public string SMXFolder;
		}
		//string[] weekDays = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
		//string[,] plugfolder = new string [2,2]{ {"Sun", "Mon"},{"Tue", "Wed"}};
		static Plugin_folder[] plugfolder = new Plugin_folder[2];*/
		public static void Main(string[] args)
		{
			//plugfolder[0].SMXFolder="";
			string title = "Sourcemod Compiler Helper ver " + (FileVersionInfo.GetVersionInfo((Assembly.GetExecutingAssembly()).Location)).ProductVersion + ": ";
			Console.Title = title;
			Console_ResetColor();
			Console.Clear();
			//Console.BackgroundColor = ConsoleColor.Blue;			
			//Console.ForegroundColor = ConsoleColor.DarkBlue;	
			Console.ForegroundColor =FGcolorH1;
			Console.Write(title);
			Console.ForegroundColor =ConsoleColor.White ;			
			//
			//Parsing argumets
			//
			if (args.Length < 1)
			{
				Console.WriteLine("Usage: MySMcompiler <path\\file.sp>");
				ScriptFinish(true);
				System.Environment.Exit(0);
			}
			Console.Write(" "); Console.WriteLine(args[0]);
			Console_ResetColor();
			mySMcomp_Folder = AppDomain.CurrentDomain.BaseDirectory;

			// or			
			//Application.ExecutablePath;
			//Assembly.GetExecutingAssembly().Location;
			//Application.StartupPath;
			Debug.Print("mySMcomp_Folder=" + mySMcomp_Folder);
			Debug.Print("Args count=" + args.Length);
			Console.Title = title + " " + args[0] + " " + DateTime.Now.ToString();
			SourceFile = args[0];			
			//EXT1
			if (!File.Exists(SourceFile))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ERR: File \"" + SourceFile + "\" not found");
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(1);
			}
			SourceFolder = System.IO.Directory.GetParent(SourceFile).ToString() + "\\";
			CheckFolderString(ref SourceFolder);
			SourceFile = Path.GetFileNameWithoutExtension(SourceFile);
			Console.Title = title + SourceFile + ".sp " + DateTime.Now.ToString();
			//EXT4
			Console.ForegroundColor = FGcolorFieldName;
			Console.Write("Source file \t" );
			Console.BackgroundColor = ConsoleColor.Gray;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.Write(SourceFolder);
			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine(SourceFile + ".sp");
			Console_ResetColor();
			//EXT3
			//Поиск INI вверх по 3-ум способам:
			// 1 просто прыгнуть вверх на пару папок
			// 2 искать папку содежащую .git или addon			
			INIFolder  =  System.IO.Directory.GetParent(SourceFolder).ToString();
			
			if (String.Compare(Path.GetFileName(INIFolder), "SCRIPTING", true) == 0)
			{
				INIFolder = System.IO.Directory.GetParent(INIFolder).ToString();
				INIFolder = System.IO.Directory.GetParent(INIFolder).ToString();
				INIFolder = System.IO.Directory.GetParent(INIFolder).ToString();
				INIFolder = System.IO.Directory.GetParent(INIFolder).ToString();
			}
			PluginFolder = INIFolder;
			CheckFolderString(ref PluginFolder);
			ConsoleWriteField("Plugin Folder", PluginFolder);			
			CheckFolderString(ref INIFolder);
			Debug.Print("INIFolder=" + INIFolder);
			INIFile = INIFolder + INIFile;
			if (!File.Exists(INIFile))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ERR:INI File \t" + INIFile + " not found");
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(2);
			}
			//Console.ForegroundColor = ConsoleColor.White;
			Console_SetH1Color();
			Console.Write("Read config file\t");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.WriteLine(INIFile);
			Console_ResetColor();

			GetConfigFile(INIFile);
			//Parsing include from INI file
			string[] Compilator_Include_Folder = Compilator_Include_Folders.Split(';');
			Array.Resize(ref Compilator_Include_Folder, Compilator_Include_Folder.Length + 1);
			Compilator_Include_Folder[Compilator_Include_Folder.Length - 1] = FolderDifference(SourceFolder, PluginFolder);			
			Compilator_Include_Folders = "";
			ConsoleWriteField("Include","",false);
			bool first = true;
			foreach (string s in Compilator_Include_Folder)
			{
				if (!String.IsNullOrEmpty(s))
				{
					if (!first) Console.Write("\t\t");
					Console.WriteLine(s);
					Compilator_Include_Folders += " -i" + s.Trim();
					first = false;
				}
			}
			//
			//Create include file datetime.inc
			//	
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
			f_inc.WriteLine("\t#define PLUGIN_AUTHOR \"" + Plugin_Author + "\"");
			f_inc.WriteLine("#endif");
			f_inc.Close();

			//Delete old err smx files
			if (!File.Exists(SourceFile + ".err")) File.Delete(SourceFile + ".err");

			//Test compiler file exist
			if (!File.Exists(Compilator_Folder + Compilator))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				if (Directory.Exists(Compilator_Folder))
					Console.WriteLine("ERR: File compiler {0} not found in folder {1}", Compilator, Compilator_Folder);
				else
					Console.WriteLine("ERR: Folder {0} with compiler {1} not found.", Compilator_Folder, Compilator);
				Console_ResetColor();
				
				ScriptFinish(true);
				System.Environment.Exit(4);
			}
			//Test compiled folder exist, delete old smx files
			if (Directory.Exists(PluginFolder + SMXFolder))
			{ if (File.Exists(PluginFolder + SMXFolder + SourceFile + ".smx")) File.Delete(PluginFolder + SMXFolder + SourceFile + ".smx"); }
			else
			{ System.IO.Directory.CreateDirectory(PluginFolder + SMXFolder); }
			//Compiling
			Console.ForegroundColor = FGcolorH1;
			Console.Write("Run compiling "); Console.ForegroundColor = FGcolorFieldValue;
			Process compiler = new Process();
			compiler.StartInfo.RedirectStandardOutput = true;
			compiler.StartInfo.RedirectStandardError = true;
			compiler.StartInfo.CreateNoWindow = true;
			compiler.StartInfo.WorkingDirectory = PluginFolder;
			//compiler.StartInfo.WorkingDirectory = SourceFolder;
			
			compiler.StartInfo.FileName = Compilator_Folder + Compilator;
			Console.WriteLine(compiler.StartInfo.FileName);
			compiler.StartInfo.UseShellExecute = false; //https://msdn.microsoft.com/ru-ru/library/system.diagnostics.processstartinfo.workingdirectory(v=vs.110).aspx			
			string DiffSourceFolder = FolderDifference(SourceFolder, INIFolder);
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

			if (Compilator_Params.Length != 0)
			{
				Console.WriteLine(" {0}", Compilator_Params);
				compiler.StartInfo.Arguments += " " + Compilator_Params;
			}

			Console.WriteLine(Compilator_Include_Folders);
			compiler.StartInfo.Arguments += Compilator_Include_Folders;
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
			//
			// Copy to server
			//
			Console.ForegroundColor = FGcolorH1;
			Console.Write("Copy output files ");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.Write(SourceFile + ".smx");
			Console.ForegroundColor = FGcolorH1;
			Console.Write(" from folder ");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.Write(SRCDS_Folder);
			Console.ForegroundColor = FGcolorH1;
			Console.Write(" to ");
			Console.ForegroundColor = FGcolorFieldValue;
			Console.WriteLine(PluginFolder + "game");
			
			if (!Directory.Exists(SRCDS_Folder))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("ERR:Folder for copy smx file " + SRCDS_Folder + "not found");
				Console_ResetColor();
				ScriptFinish(true);
				System.Environment.Exit(0);
			}
			CopyDirectory(PluginFolder + "game", SRCDS_Folder);
			//
			// Reload plugin
			//   
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("\nReload plugin {0} on server {1}:{2}\n", SourceFile, rcon_Address, rcon_Port);
			Console_ResetColor();

			//make you class https://developer.valvesoftware.com/wiki/Source_RCON_Protocol
			SourceRcon.SourceRcon RCon = new SourceRcon.SourceRcon();
			RCon.Errors += new SourceRcon.StringOutput(ErrorOutput);
			RCon.ServerOutput += new SourceRcon.StringOutput(ConsoleOutput);

			try
			{
				RCon.Connect(new IPEndPoint(IPAddress.Parse(rcon_Address), rcon_Port), rcon_password);
				for (int i = 0; i != 10; i++)
				{
					Thread.Sleep(1000);
					if (RCon.Connected) break;
				}

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
				if (MapReload)
				{
					Console.WriteLine("Restart server");
					RCon.ServerCommand("_restart");
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
					Thread.Sleep(100);
				}
#if DEBUG
				Console.ReadKey(true);
#endif
			}
		}
		//****************************************************			
		public static void GetConfigFile(string ConfigFile)
		{
			//****************************************************				
			IniParser inifile = new IniParser(ConfigFile);
			Compilator_Folder = inifile.ReadString("Compiler", "Compilator_Folder", mySMcomp_Folder/*"smk64t\\sourcemod-1.7.3-git5301"*/);
			CheckFolderString(ref Compilator_Folder, ParentFolder(PluginFolder));

			//Если Compilator_Folder не содержит в начале строки с:\ или \ или \\, то дополнить путь PluginFolder	Compilator_Folder=INIFolder+Compilator_Folder;
			ConsoleWriteField("Compilator_Folder", Compilator_Folder);

			Plugin_Author = inifile.ReadString("Compiler", "Plugin_Author", "");
			rcon_password = inifile.ReadString("Server", "rcon_password", "");
			SRCDS_Folder = inifile.ReadString("Server", "SRCDS_Folder", "");
			CheckFolderString(ref SRCDS_Folder);
			MapReload = inifile.ReadBool("Server", "MapReload", false);
			rcon_Address = inifile.ReadString("Server", "rcon_Address", "");
			Compilator_Include_Folders = inifile.ReadString("Compiler", "Include", Compilator_Include_Folders);
			rcon_Port = inifile.ReadInteger("Server", "rcon_port", rcon_Port);
			Compilator_Params = inifile.ReadString("Compiler", "Parameters", "");




			/*if (!String.IsNullOrEmpty(ConfigFile)) {			
				//http://msdn.microsoft.com/en-us/library/system.string.isnullorempty.aspx
				Console.WriteLine("INF: Use ini file " + ConfigFile);

				Inifile inifile = new Inifile(ConfigFile);
				Compilator = inifile.LoadString("Compiler", "Compilator", Compilator);
				Compilator_Folder = inifile.LoadString("Compiler", "Compilator_Folder", mySMcomp_Folder);
				Compilator_Params = inifile.LoadString("Compiler", "Parameters", Compilator_Params);
				Compilator_Include_Folders = inifile.LoadString("Compiler", "Include", Compilator_Include_Folders);
				SRCDS_Folder = inifile.LoadString("Server", "SRCDS_Folder");
				rcon_Address = inifile.LoadString("Server", "rcon_address", rcon_Address);
				rcon_Port = inifile.LoadInteger("Server", "rcon_port", rcon_Port);
				rcon_password = inifile.LoadString("Server", "rcon_password", rcon_password);
				Plugin_Author = inifile.LoadString("Compiler", "Plugin_Author", "");

			}*/

#if DEBUG
			Debug.Print("MapReload\t\t=" + MapReload);
			Debug.Print("Compilator\t\t=" + Compilator);
			Debug.Print("Compilator_Folder\t=" + Compilator_Folder);
			Debug.Print("Compilator_Params\t=" + Compilator_Params);
			Debug.Print("Compilator_Include_Folders=" + Compilator_Include_Folders);
			Debug.Print("SRCDS_Folder=" + SRCDS_Folder);
			ConsoleWriteField("SRCDS_Folder", SRCDS_Folder);
			Debug.Print("rcon_address=" + rcon_Address);
			Debug.Print("rcon_port=" + rcon_Port);
			Debug.Print("rcon_password=" + rcon_password);
#endif
		}
		//*******************************************
		public static void CheckFolderString(ref string s)
		{
			//*******************************************
			s = s.Trim();
			if (!s.EndsWith("\\")) s += "\\";
		}
		public static void CheckFolderString(ref string s, string basepath)
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