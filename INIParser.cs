using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
//using System.Diagnostics;
 
public class IniParser
{
    [DllImport("kernel32.dll")]
    private static extern int WritePrivateProfileString(string ApplicationName, string KeyName, string StrValue, string FileName);
    [DllImport("kernel32.dll")]
    private static extern int GetPrivateProfileString(string ApplicationName, string KeyName, string DefaultValue, StringBuilder ReturnString, int nSize, string FileName);
    
    private static String INIFile;
	
    public IniParser(String iniPath)
    {
    	INIFile=iniPath;
    }
 
    public static void WriteValue(string SectionName , string KeyName, string KeyValue)
    {
        WritePrivateProfileString(SectionName , KeyName, KeyValue, INIFile);
    }
 
    public static string ReadValue(string SectionName , string KeyName)
    {
        StringBuilder szStr = new StringBuilder(255);
        GetPrivateProfileString(SectionName, KeyName, "" , szStr, 255, INIFile);
        return szStr.ToString().Trim();
    }
    public String ReadString(String SectionName , String KeyName, String DefaultValue)
    {
        return ReadValue(SectionName , KeyName);
    }
    public bool ReadBool(String SectionName , String KeyName, bool DefaultValue)
    {
    bool ReadBoolValue=DefaultValue;	
    string keyvalue=ReadValue(SectionName , KeyName);  
    //Debug.Print("ReadBoolValue=" + keyvalue);
    if (keyvalue.Length!=0)
    {    
	    if (String.Compare(keyvalue,"true",true)==0) ReadBoolValue=true;
	    else if (String.Compare(keyvalue,"Yes",true)==0) ReadBoolValue=true;
	    else if (String.Compare(keyvalue,"1",true)==0) ReadBoolValue=true;
	    else if (String.Compare(keyvalue,"false",true)==0) ReadBoolValue=false;
	    else if (String.Compare(keyvalue,"no",true)==0) ReadBoolValue=false;
	    else if (String.Compare(keyvalue,"0",true)==0) ReadBoolValue=false;
    }
    return ReadBoolValue;
    }
    public int ReadInteger(String SectionName , String KeyName, int DefaultValue)
    {
    	int Result;
    	string tmpStr=ReadValue(SectionName ,KeyName);
    	if (!Int32.TryParse(tmpStr,out Result))
    		Result=DefaultValue;
     return Result;
    }

}