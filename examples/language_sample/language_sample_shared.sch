
GLOBAL 20 language_sample_main
    INT g_nTimesMainScriptExecuted = 0
    INT g_nNumberOfChildScriptsRunning = 0
    // MY_PROCEDURE_T g_fnProc = DEFAULT_MY_PROCEDURE
    // INT& g_ref
ENDGLOBAL

ENUM eMyValue
    MY_VALUE_A // = 0
    MY_VALUE_B // = 1
    MY_VALUE_C = -2
    MY_VALUE_D // = -1
ENDENUM

PROTO PROC MY_PROCEDURE_T(STRING s, INT n)

PROC DEFAULT_MY_PROCEDURE(STRING s, INT n)
    // empty
ENDPROC

NATIVE PROC BEGIN_TEXT_COMMAND_DISPLAY_TEXT(STRING text)
NATIVE PROC END_TEXT_COMMAND_DISPLAY_TEXT(FLOAT x, FLOAT y, INT p2)
NATIVE PROC ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(STRING text)

PROC DRAW_TEXT(STRING s, FLOAT x, FLOAT y)
    BEGIN_TEXT_COMMAND_DISPLAY_TEXT("STRING")
    ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(s)
    END_TEXT_COMMAND_DISPLAY_TEXT(x, y, 0)
ENDPROC

STRUCT CHILD_ARGS
    INT a
    INT b = 1234
    INT c
ENDSTRUCT

NATIVE FUNC PED_INDEX PLAYER_PED_ID()
NATIVE FUNC INT GET_ENTITY_MODEL(ENTITY_INDEX entity)
NATIVE PROC WAIT(INT ms)
NATIVE PROC TERMINATE_THIS_THREAD()
NATIVE FUNC INT START_NEW_SCRIPT_WITH_ARGS(STRING scriptName, ANY& args, INT argsSize, INT stackSize)
NATIVE PROC REQUEST_SCRIPT(STRING scriptName)
NATIVE PROC SET_SCRIPT_AS_NO_LONGER_NEEDED(STRING scriptName)
NATIVE FUNC BOOL HAS_SCRIPT_LOADED(STRING scriptName)
NATIVE FUNC BOOL IS_CONTROL_PRESSED(INT padIndex, eInput input)
NATIVE FUNC BOOL IS_CONTROL_JUST_PRESSED(INT padIndex, eInput input)

ENUM eInput
    INPUT_RELOAD = 45
    INPUT_CONTEXT = 51
    INPUT_CONTEXT_SECONDARY = 52
ENDENUM
