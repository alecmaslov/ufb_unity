using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDefine
{
    //public static string API_URL = "api.thig.io";
    public static string API_URL = "localhost";
    public static int API_PORT = 8080;
    public static bool isHttps = false;
    public static string WebSocketHeader = "ws://";

    public static class SERVER_MESSAGE
    {
        public static string INIT_TURN = "InitTurn";
        public static string TURN_CHANGED = "turnChanged";
        public static string SPAWN_INIT = "spawnInit";
        public static string RECEIVE_POWERMOVE_LIST = "ReceivePowerMoveList";
        public static string RECEIVE_MOVEITEM = "ReceiveMoveItem";
        public static string SET_MOVEITEM = "SetMoveItem";
        public static string ADD_EXTRA_SCORE = "addExtraScore";
        public static string GET_BOMB_DAMAGE = "getBombDamage";
        public static string GET_MERCHANT_DATA = "getMerchantData";
        public static string RESPAWN_MERCHANT = "respawnMerchant";
        public static string UNEQUIP_POWER_RECEIVED = "unEquipPowerReceived";
        public static string CHARACTER_MOVED = "characterMoved";
        public static string SET_HIGHLIGHT_RECT = "setHighLightRect";
        public static string SET_DICE_ROLL = "SET_DICE_ROLL";
        public static string ENEMY_DICE_ROLL = "ENEMY_DICE_ROLL";
        public static string GET_TURN_START_EQUIP = "GET_TURN_START_EQUIP";
    }

    public static class CLIENT_MESSAGE
    {
        public static string GET_HIGHLIGHT_RECT = "GET_HIGHLIGHT_RECT";
        public static string SET_POWER_MOVE_ITEM = "SET_POWER_MOVE_ITEM";
        public static string SET_DICE_ROLL = "SET_DICE_ROLL";
        public static string SET_MOVE_ITEM = "SET_MOVE_ITEM";
        public static string END_POWER_MOVE_ITEM = "END_POWER_MOVE_ITEM";
        public static string END_TURN = "END_TURN";
        public static string TURN_START_EQUIP = "TURN_START_EQUIP";
        public static string UN_EQUIP_POWER = "UN_EQUIP_POWER";
        public static string EQUIP_POWER = "EQUIP_POWER";
    }
}
