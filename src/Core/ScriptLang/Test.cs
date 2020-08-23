﻿namespace ScTools.ScriptLang
{
    using System;
    using System.IO;
    using System.Linq;

    using ScTools.ScriptLang.Semantics;
    using ScTools.ScriptLang.Semantics.Symbols;

    public static class Test
    {
        const string Code = @"
SCRIPT_NAME test

STRUCT VEC2
    FLOAT x
    FLOAT y
ENDSTRUCT

PROTO PROC DRAW_CALLBACK(INT alpha)
PROTO FUNC INT GET_ALPHA_CALLBACK()

STRUCT CALLBACKS
    DRAW_CALLBACK       draw
    GET_ALPHA_CALLBACK  getAlpha
ENDSTRUCT

CALLBACKS myCallbacks// = <<DRAW_OTHER_STUFF, GET_ALPHA_VALUE>>
INT someStaticValue
FLOAT otherStaticValue

PROC MAIN()
    VEC2 pos = <<0.5, 0.5>>
    pos.y = 0.75
    INT a = 10
    INT b = 5 + -a * 2

    WHILE TRUE
        WAIT(0)

        DRAW_RECT(pos.x, pos.y, 0.1, 0.1, 255, 0, 0, 255, FALSE)

        myCallbacks.draw(myCallbacks.getAlpha())

        IF NOT TRUE

        ENDIF
    ENDWHILE
ENDPROC

FUNC INT GET_ALPHA_VALUE()
    RETURN 200
ENDFUNC

PROC DRAW_OTHER_STUFF(INT alpha)
    DRAW_RECT(0.6, 0.6, 0.2, 0.2, 100, 100, 20, alpha, FALSE)
ENDPROC

// tmp procs until there are native command definitions
PROC DRAW_RECT(FLOAT x, FLOAT y, FLOAT w, FLOAT h, INT r, INT g, INT b, INT a, BOOL unk)
ENDPROC
PROC WAIT(INT ms)
ENDPROC
";

        public static void DoTest()
        {
            using var reader = new StringReader(Code);
            var module = Module.Compile(reader);

            var d = module.Diagnostics;
            var symbols = module.SymbolTable;
            Console.WriteLine($"Errors:   {d.HasErrors} ({d.Errors.Count()})");
            Console.WriteLine($"Warnings: {d.HasWarnings} ({d.Warnings.Count()})");
            foreach (var diagnostic in d.AllDiagnostics)
            {
                diagnostic.Print(Console.Out);
            }

            foreach (var s in symbols.Symbols)
            {
                if (s is TypeSymbol t && t.Type is StructType struc)
                {
                    Console.WriteLine($"  > '{t.Name}' Size = {struc.SizeOf}");
                }
            }
            ;
        }
    }
}
