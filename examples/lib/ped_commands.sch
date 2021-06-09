NATIVE FUNC PED_INDEX CREATE_PED(INT pedType, INT modelHash, VECTOR position, FLOAT heading, BOOL isNetwork, BOOL netMissionEntity)
NATIVE PROC DELETE_PED(PED_INDEX& ped)
NATIVE PROC SET_PED_COMBAT_MOVEMENT(PED_INDEX ped, INT combatMovement)
NATIVE PROC SET_PED_TARGET_LOSS_RESPONSE(PED_INDEX ped, INT response)

CONST INT PED_TYPE_PLAYER_0 = 0
CONST INT PED_TYPE_PLAYER_1 = 1
CONST INT PED_TYPE_NETWORK_PLAYER = 2
CONST INT PED_TYPE_PLAYER_2 = 3
CONST INT PED_TYPE_CIVMALE = 4
CONST INT PED_TYPE_CIVFEMALE = 5
CONST INT PED_TYPE_COP = 6
CONST INT PED_TYPE_GANG_ALBANIAN = 7
CONST INT PED_TYPE_GANG_BIKER_1 = 8
CONST INT PED_TYPE_GANG_BIKER_2 = 9
CONST INT PED_TYPE_GANG_ITALIAN = 10
CONST INT PED_TYPE_GANG_RUSSIAN = 11
CONST INT PED_TYPE_GANG_RUSSIAN_2 = 12
CONST INT PED_TYPE_GANG_IRISH = 13
CONST INT PED_TYPE_GANG_JAMAICAN = 14
CONST INT PED_TYPE_GANG_AFRICAN_AMERICAN = 15
CONST INT PED_TYPE_GANG_KOREAN = 16
CONST INT PED_TYPE_GANG_CHINESE_JAPANESE = 17
CONST INT PED_TYPE_GANG_PUERTO_RICAN = 18
CONST INT PED_TYPE_DEALER = 19
CONST INT PED_TYPE_MEDIC = 20
CONST INT PED_TYPE_FIREMAN = 21
CONST INT PED_TYPE_CRIMINAL = 22
CONST INT PED_TYPE_BUM = 23
CONST INT PED_TYPE_PROSTITUTE = 24
CONST INT PED_TYPE_SPECIAL = 25
CONST INT PED_TYPE_MISSION = 26
CONST INT PED_TYPE_SWAT = 27
CONST INT PED_TYPE_ANIMAL = 28
CONST INT PED_TYPE_ARMY = 29

// combat movement
CONST INT PED_CM_STATIONARY = 0
CONST INT PED_CM_DEFENSIVE = 1
CONST INT PED_CM_WILL_ADVANCE = 2
CONST INT PED_CM_WILL_RETREAT = 3

// target loss response
CONST INT PED_TLR_EXIT_TASK = 0
CONST INT PED_TLR_NEVER_LOSE_TARGET = 1
CONST INT PED_TLR_SEARCH_FOR_TARGET = 2