
USING 'natives.sch'


SCRIPT startup

    _SET_PLAYER_IS_IN_DIRECTOR_MODE(FALSE)
    _SET_PLAYER_IS_IN_ANIMAL_FORM(FALSE)

    SET_INSTANCE_PRIORITY_MODE(0)

    REQUEST_MODEL(`csb_prolsec`)
    WHILE NOT HAS_MODEL_LOADED(`csb_prolsec`)
        WAIT(0)
    ENDWHILE

    SET_PLAYER_MODEL(PLAYER_ID(), `csb_prolsec`)
    IF NOT IS_ENTITY_DEAD(PLAYER_PED_ID(), FALSE)
        SET_ENTITY_COORDS(PLAYER_PED_ID(), 0.0, 0.0, 75.0, TRUE, FALSE, FALSE, TRUE)
        SET_ENTITY_HEADING(PLAYER_PED_ID(), 180.0)
    ENDIF

    NEW_LOAD_SCENE_START_SPHERE(0.0, 0.0, 75.0, 10.0, 0)
    INT maxLoadTime = GET_GAME_TIMER() + 10000
    WHILE NOT IS_NEW_LOAD_SCENE_LOADED() AND GET_GAME_TIMER() < maxLoadTime
        WAIT(0)
    ENDWHILE

    SHUTDOWN_LOADING_SCREEN()
    DO_SCREEN_FADE_IN(2500)

    NEW_LOAD_SCENE_STOP()

    WHILE TRUE
        IF NOT IS_SCREEN_FADED_IN()
            DO_SCREEN_FADE_IN(0)
        ENDIF

        SET_TEXT_SCALE(0.3, 0.3)
        SET_TEXT_CENTRE(TRUE)
        DRAW_TEXT("CUSTOM STARTUP", 0.5, 0.1)

        WAIT(0)
    ENDWHILE
ENDSCRIPT

PROC DRAW_TEXT(STRING text, FLOAT x, FLOAT y)
    BEGIN_TEXT_COMMAND_DISPLAY_TEXT("STRING")
    ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(text)
    END_TEXT_COMMAND_DISPLAY_TEXT(x, y, 0)
ENDPROC
