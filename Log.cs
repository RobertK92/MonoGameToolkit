using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoGameToolkit
{
    enum LogType : byte
    {
        Message,
        Warning,
        Error,
        Exception,
        Success
    }

    /// <summary>
    /// Utility for logging and handling errors, warnings, exceptions and messages.
    /// </summary>
    public static class Log
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        
        /// <summary>
        /// If the memory usage in MB is above this number, the ReportMemory function will be logged using an error.
        /// </summary>
        public static int MemoryErrorThreshold { get; set; }

        /// <summary>
        /// If the memory usage in MB is above this number, the ReportMemory function will be logged using a warning.
        /// </summary>
        public static int MemoryWarningThreshold { get; set; }

        /// <summary>
        /// Whether or not the logs will be displayed in the console.
        /// (default = true).
        /// </summary>
        public static bool UseConsole { get; set; }

        /// <summary>
        /// Whether or not the logs will be displayed in the log file.
        /// (default = true).
        /// </summary>
        public static bool UseDumpFile { get; set; }

        /// <summary>
        /// Whether or not executing should pause when an error occurs.
        /// </summary>
        public static bool PauseOnError { get; set; }

        /// <summary>
        /// Whether or not an error should also cause an assert.
        /// </summary>
        public static bool AssertOnError { get; set; }

        /// <summary>
        /// Whether or not caller info (Class, Method, Line numbers) should be logged for errors.
        /// (default = true).
        /// </summary>
        public static bool LogStackInfoForErrors { get; set; }

        /// <summary>
        /// Whether or not caller info (Class, Method, Line numbers) should be logged for warnings.
        /// (default = true).
        /// </summary>
        public static bool LogStackInfoForWarnings { get; set; }

        /// <summary>
        /// Whether or not caller info (Class, Method, Line numbers) should be logged for messages.
        /// (default = true).
        /// </summary>
        public static bool LogStackInfoForMessages { get; set; }

        /// <summary>
        /// The minimum amount of characters printed in console before logging the stack info.
        /// If the log has fewer characters that this number, spaces will be added.
        /// </summary>
        public static int CharacterCountBeforeStackInfoConsole { get; set; }

        /// <summary>
        /// The minimum amount of characters logged to file before logging the stack info.
        /// If the log has fewer characters that this number, spaces will be added.
        /// </summary>
        public static int CharacterCountBeforeStackInfoFile { get; set; }

        static FileStream _stream;
        static StreamWriter _writer;
        static bool _isInitialized;
        static int _stackFrame;
        
        internal static void Init()
        {
            if (_isInitialized)
                return;

            UseConsole = (GetConsoleWindow() != IntPtr.Zero);
            UseDumpFile = true;

            AssertOnError = false;
            LogStackInfoForErrors = true;
            LogStackInfoForWarnings = true;
            LogStackInfoForMessages = true;
            CharacterCountBeforeStackInfoConsole = 64;
            CharacterCountBeforeStackInfoFile = 100;
            MemoryErrorThreshold = 5000;
            MemoryWarningThreshold = 500;

            _stackFrame = 3;
            _stream = File.Create("LogDump.txt");
            _writer = new StreamWriter(_stream);
            _writer.WriteLine("-- LOG DUMP --" + _writer.NewLine);
            _writer.Flush();
            _isInitialized = true;
            Message("Log.Init complete");
        }

        static void LogAny(string msg, LogType type)
        {
            if (!_isInitialized)
                Init();

            string stackInfo = LogStackInfoForMessages ? GetStackInfo() : string.Empty;
            
            if (UseDumpFile)
            {
                LogToDumpFile(msg.PadRight(CharacterCountBeforeStackInfoFile) + stackInfo, type);
            }
            if (UseConsole)
            {
                LogToConsole(msg.PadRight(CharacterCountBeforeStackInfoConsole) + stackInfo, type);
            }
        }

        static void LogToDumpFile(string msg, LogType type)
        {
            _writer.WriteLine(msg);
            _writer.Flush();
        }

        static void LogToConsole(string msg, LogType type)
        {
            switch (type)
            {
                case LogType.Message:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Exception:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    break;
            }

            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Dumps the current memory usage.
        /// </summary>
        public static void ReportMemoryUsage()
        {
            long mb = -1;
            try
            {
                //mb = Process.GetCurrentProcess().WorkingSet64 / 1000000;
                mb = GC.GetTotalMemory(true) / 1000000;
            }
            catch(OutOfMemoryException e)
            {
                Exception(e);
            }

            string msg = string.Format("Memory usage: {0} MB", mb);
            if (mb > MemoryErrorThreshold)
            {
                Error(msg);
                return;
            }
            if(mb > MemoryWarningThreshold)
            {
                Warning(msg);
                return;
            }
            Success(msg);
        }

        /// <summary>
        /// Dumps an empty line.
        /// </summary>
        public static void NewLine()
        {
            LogAny("--> ", LogType.Message);
        }

        /// <summary>
        /// Dumps an info message.
        /// </summary>
        /// <param name="message"></param>
        public static void Message(object message)
        {
            LogAny(string.Format("--> {0}", message.ToString()), LogType.Message);
        }

        /// <summary>
        /// Dumps a success message.
        /// </summary>
        /// <param name="message"></param>
        public static void Success(object message)
        {
            LogAny(string.Format("--> {0}", message.ToString()), LogType.Success);
        }

        /// <summary>
        /// Dumps a warning message.
        /// </summary>
        /// <param name="warningMessage"></param>
        public static void Warning(object warningMessage)
        {
            LogAny(string.Format("--> [WARNING]: {0}", warningMessage.ToString()), LogType.Warning);
        }

        /// <summary>
        /// Dumps an error message.
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void Error(object errorMessage)
        {
            LogAny(string.Format("--> [ERROR]: {0}", errorMessage.ToString()), LogType.Error);
            Trace.Assert(!AssertOnError, "[ERROR]" + Environment.NewLine, errorMessage.ToString() + Environment.NewLine);
            
            if (PauseOnError)
            {
                // TODO: pause execution.
            }
        }

        /// <summary>
        /// Dumps an exception message.
        /// </summary>
        /// <param name="e"></param>
        public static void Exception(Exception e)
        {
            LogAny(string.Format("--> [EXCEPTION]: {0}", e.Message), LogType.Exception);
        }

        internal static void UpdateScreenLogs()
        {

        }

        static string GetStackInfo()
        {
            StackTrace stack = new StackTrace(true);
            StackFrame lastFrame = stack.GetFrame(_stackFrame);
            MethodBase method = lastFrame.GetMethod();
            string className = method.DeclaringType.ToString();
            string methodName = method.Name;
            string line = lastFrame.GetFileLineNumber().ToString();

            if (line == "0")
                line = "?";

            return string.Format("({0}.{1}, line:{2})", className, methodName, line.ToString());
        }
    }
}