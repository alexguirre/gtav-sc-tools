﻿namespace ScTools.Tests.ScriptAssembly
{
    using Xunit;

    public class DisassemblerTests
    {
        [Fact]
        public void TestBasic()
        {
            using var initialAsm = Util.Assemble(@"
            .script_name my_script
            .script_hash 0x1234ABCD

            .const MY_FLOAT_VALUE 4.5

            .code
main:       ENTER 0, 2
            CALL getMyFloat
            DROP

loop:
            PUSH_CONST_U8_U8_U8 1, 2, 3
            DROP
            DROP

            SWITCH 1:case1, 2:case2, 3:case3, 4:0
caseDefault:
            PUSH_CONST_U8 0
            J switchEnd
case1:
            PUSH_CONST_U8 1
            J switchEnd
case2:
            PUSH_CONST_U8 2
            J switchEnd
case3:
            PUSH_CONST_U8 3
            J switchEnd
switchEnd:
            DROP
            J loop

            LEAVE 0, 0

getMyFloat: ENTER 0, 2
            PUSH_CONST_F MY_FLOAT_VALUE
            LEAVE 0, 1
            ");

            var disassembly = Util.Disassemble(initialAsm.OutputScript);

            using var finalAsm = Util.Assemble(disassembly);

            var initialDump = Util.Dump(initialAsm.OutputScript);
            var finalDump = Util.Dump(finalAsm.OutputScript);

            Assert.Equal(initialDump, finalDump);
            Util.AssertScriptsAreEqual(initialAsm.OutputScript, finalAsm.OutputScript);
        }
    }
}
