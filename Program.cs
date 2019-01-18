//------------------------------------------------------------------------------
//<copyright company="Microsoft">
//    The MIT License (MIT)
//    
//    Copyright (c) 2017 Microsoft
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//
//    This sample code is not supported under any Microsoft standard support program or service. 
//    The entire risk arising out of the use or performance of the sample scripts and documentation remains with you. 
//    In no event shall Microsoft, its authors, or anyone else involved in the creation, production, or delivery of the scripts
//    be liable for any damages whatsoever (including, without limitation, damages for loss of business profits,
//    business interruption, loss of business information, or other pecuniary loss) arising out of the use of or inability
//    to use the sample scripts or documentation, even if Microsoft has been advised of the possibility of such damages.
//</copyright>
//------------------------------------------------------------------------------
namespace Microsoft.SqlServer.Utils.Misc.SQLCallStackResolver
{
    using System;
    using System.Windows.Forms;
    using CommandLine;

    static class Program
    {

        [Verb("Parse", HelpText = "Parse call stack")]
        public class ParseOptions
        {
            [Option("CallStack", Required = false, HelpText = "Call stack to resolve")]
            public string CallStack { get; set; }
            [Option("SingleLine", Required = false, HelpText = "Frames on single line separated by a space")]
            public bool SingleLine { get; set; }

            [Option("PdbPath", Required = false, HelpText = "Path to PDB files separated by a semicolon")]
            public string PdbPath { get; set; }
            [Option("PdbRecurse", Required = false, HelpText = "Recurse PDB files")]
            public bool PdbRecurse { get; set; }

            [Option("DllPath", Required = false, HelpText = "Path to DLL files separated by a semicolon")]
            public string DllPath { get; set; }
            [Option("DllRecurse", Required = false, HelpText = "Recurse DLL files")]
            public bool DllRecurse { get; set; }

            [Option("IncludeLineNumbers", Required = false, HelpText = "Include line numbers")]
            public bool IncludeLineNumbers { get; set; }
            [Option("RelookupSource", Required = false, HelpText = "Relookup source")]
            public bool LookupSource { get; set; }
        }

        [Verb("Script", HelpText = "Script PDB download")]
        public class ScriptOptions
        {
            [Option("DllPath", Required = false, HelpText = "Path to DLL files")]
            public string DllPath { get; set; }
            [Option("DllRecurse", Required = false, HelpText = "Recurse DLL files")]
            public bool DllRecurse { get; set; }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                StackResolver _resolver = new StackResolver();

                CommandLine.Parser.Default.ParseArguments<ParseOptions, ScriptOptions>(args)
                    .MapResult(
                       (ParseOptions o) =>
                       {
                           string text = _resolver.ResolveCallstacks(
                              o.CallStack,                                              // callStackInput.Text,
                              o.PdbPath,                                                // pdbPaths.Text,
                              o.PdbRecurse,                                             // pdbRecurse.Checked,
                              new System.Collections.Generic.List<string>(o.DllPath.Split(';')),   // binaryPaths.Text,
                              o.DllRecurse,                                             // DLLrecurse.Checked,
                              o.SingleLine,                                             // FramesOnSingleLine.Checked,
                              o.IncludeLineNumbers,                                     // IncludeLineNumbers.Checked,
                              o.LookupSource                                            // RelookupSource.Checked
                              );

                           System.Console.Write(text);
                           return 1;
                       },
                        (ScriptOptions o) =>
                        {
                            string text = _resolver.ObtainPDBDownloadCommandsfromDLL(o.DllPath, o.DllRecurse);

                            System.Console.Write(text);
                            return 1;
                        },
                        errs => 1);
            }
        }
    }
}
