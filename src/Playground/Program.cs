﻿namespace ScTools.Playground
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using CodeWalker.GameFiles;

    using ScTools;
    using ScTools.GameFiles;
    using ScTools.ScriptAssembly;
    using ScTools.ScriptLang;
    using ScTools.ScriptLang.CodeGen;
    using ScTools.ScriptLang.Grammar;
    using ScTools.ScriptLang.Semantics;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            //LoadGTA5Keys();
            DoTest();
        }

        private static void LoadGTA5Keys()
        {
            string path = ".\\Keys";
            GTA5Keys.PC_AES_KEY = File.ReadAllBytes(path + "\\gtav_aes_key.dat");
            GTA5Keys.PC_NG_KEYS = CryptoIO.ReadNgKeys(path + "\\gtav_ng_key.dat");
            GTA5Keys.PC_NG_DECRYPT_TABLES = CryptoIO.ReadNgTables(path + "\\gtav_ng_decrypt_tables.dat");
            GTA5Keys.PC_NG_ENCRYPT_TABLES = CryptoIO.ReadNgTables(path + "\\gtav_ng_encrypt_tables.dat");
            GTA5Keys.PC_NG_ENCRYPT_LUTs = CryptoIO.ReadNgLuts(path + "\\gtav_ng_encrypt_luts.dat");
            GTA5Keys.PC_LUT = File.ReadAllBytes(path + "\\gtav_hash_lut.dat");
        }

        public static void DoTest()
        {
            //NativeDB.Fetch(new Uri("https://raw.githubusercontent.com/alloc8or/gta5-nativedb-data/master/natives.json"), "ScriptHookV_1.0.2060.1.zip")
            //    .ContinueWith(t => File.WriteAllText("nativedb.json", t.Result.ToJson()))
            //    .Wait();

            var nativeDB = NativeDB.FromJson(System.IO.File.ReadAllText("nativesdb.json"));

            const string BaseDir = "D:\\sources\\gtav-sc-tools\\examples\\language_sample\\";

            Parse(BaseDir + "language_sample_main.sc", nativeDB);
            Parse(BaseDir + "language_sample_child.sc", nativeDB);
            //Parse(BaseDir + "language_sample_shared.sch");

            ;

        }

        private static void Parse(string filePath, NativeDB nativeDB)
        {
            using var r = new StreamReader(filePath);
            var d = new DiagnosticsReport();
            var sw = Stopwatch.StartNew();
            var p = new Parser(r, filePath) { UsingResolver = new FileUsingResolver() };
            p.Parse(d);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            //d.PrintAll(Console.Out);

            var globalSymbols = GlobalSymbolTableBuilder.Build(p.OutputAst, d);
            IdentificationVisitor.Visit(p.OutputAst, d, globalSymbols, nativeDB);
            TypeChecker.Check(p.OutputAst, d, globalSymbols);
            //d.PrintAll(Console.Out);
            ;

            if (!d.HasErrors)
            {
                Console.WriteLine("CodeGen...");
                using var sink = new StringWriter();
                new CodeGenerator(sink, p.OutputAst, globalSymbols, d, nativeDB).Generate();
                var s = sink.ToString();
                ;
                using var reader = new StringReader(s);
                var assembler = Assembler.Assemble(reader, Path.ChangeExtension(filePath, "scasm"), nativeDB, options: new() { IncludeFunctionNames = true });
                assembler.Diagnostics.PrintAll(Console.Out);
                ;
            }
            d.PrintAll(Console.Out);
            ;
        }

        private sealed class FileUsingResolver : IUsingResolver
        {
            public string NormalizeFilePath(string filePath) => Path.GetFullPath(filePath);

            public (Func<TextReader> Open, string FilePath) Resolve(string originPath, string usingPath)
            {
                var p = NormalizeFilePath(Path.Combine(Path.GetDirectoryName(originPath), usingPath));
                return (() => new StreamReader(p), p);
            }
        }
    }
}
