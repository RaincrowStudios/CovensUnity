
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

public class CovenConstants : MonoBehaviour
{
    public static bool Debug = true;

    public static string hostAddress
    {
        get
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.GetString("game") == "Local")
            {
                return "http://localhost:8080/api/";
            }
            else if (UnityEditor.EditorPrefs.GetString("game") == "Release")
            {
                return "https://game-server-dot-raincrow-pantheon.appspot.com/api/";
            }
            else if (UnityEditor.EditorPrefs.GetString("game") == "Gustavo")
            {
                return "http://192.168.0.108:8080/api/";
            }
            else
            {
                return "http://35.196.97.86:8080/api/";
            }
#elif PRODUCTION
           return "https://game-server-dot-raincrow-pantheon.appspot.com/api/";
        //   return "http://35.237.95.2:8080/api/";
#else
            return "http://35.196.97.86:8080/api/";
#endif
        }
    }

    public static string analyticsAddress
    {
        get
        {
#if PRODUCTION
            return "https://analytics-server-dot-raincrow-pantheon.appspot.com/";
#else
            return "http://35.196.97.86:8082/";
#endif
        }
    }

    public static string wssAddress
    {
        get
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.GetString("ws") == "Local")
            {
                return "ws://localhost:8084?";
            }
            else if (UnityEditor.EditorPrefs.GetString("ws") == "Release")
            {
                return "ws://mqtt.raincrowstudios.xyz:8084?";
            }
            else if (UnityEditor.EditorPrefs.GetString("ws") == "Gustavo")
            {
                return "http://192.168.0.108:8084?";
            }
            else
            {
                return "ws://35.196.97.86:8084?";
            }
#elif PRODUCTION
            return "ws://mqtt.raincrowstudios.xyz:8084?";
#else
            return "ws://35.196.97.86:8084?";
#endif
        }
    }

    public static string pushNotificationKey
    {
        get
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.GetString("Server") == "Release")
            {
                return "dfff330b-58b1-47dc-a287-a83b714ec95b";
            }
            else
            {
                return "8a7fddbb-ab0b-48d5-a2e1-0ac91bf7238f";
            }
#elif PRODUCTION
            return "dfff330b-58b1-47dc-a287-a83b714ec95b";
#else
            return "8a7fddbb-ab0b-48d5-a2e1-0ac91bf7238f";
#endif
        }
    }

    public static string chatAddress
    {
        get
        {
#if UNITY_EDITOR

            if (UnityEditor.EditorPrefs.GetString("chat") == "Local")
            {
                return "http://localhost:8083/socket.io/?";
            }
            else if (UnityEditor.EditorPrefs.GetString("chat") == "Release")
            {
                return "http://35.227.88.204:8083/socket.io/";
            }
            else if (UnityEditor.EditorPrefs.GetString("chat") == "Gustavo")
            {
                return "http://192.168.0.108:8083/socket.io/?";
            }
            else
            {
                return "http://35.196.97.86:8083/socket.io/";
            }
#elif PRODUCTION
            return "http://35.227.88.204:8083/socket.io/";
#else
            return "http://35.196.97.86:8083/socket.io/";
#endif
        }
    }

    public static string chatAddressHTTP
    {
        get
        {
#if UNITY_EDITOR

            if (UnityEditor.EditorPrefs.GetString("chat") == "Local")
            {
                return "http://localhost:8083/api/chat/";
            }
            else if (UnityEditor.EditorPrefs.GetString("chat") == "Release")
            {
                return "http://35.227.88.204:8083/api/chat/";
            }
            else
            {
                return "http://35.196.97.86:8083/api/chat/";
            }
#elif PRODUCTION
            return "http://35.227.88.204:8083/api/chat/";
#else
            return "http://35.196.97.86:8083/api/chat/";
#endif
        }
    }

    public static string wsMapServer
    {
        get
        {

#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.GetString("map") == "Release")
            {
                return "wss://map-server-dot-raincrow-pantheon.appspot.com/";
            }
            else if (UnityEditor.EditorPrefs.GetString("map") == "Local")
            {
                return "ws://localhost:8081";
            }
            else if (UnityEditor.EditorPrefs.GetString("map") == "Gustavo")
            {
                return "ws://192.168.0.108:8081";
            }
            else
            {
                // return "ws://localhost:8081";
                return "ws://35.196.97.86:8081";
            }
#elif PRODUCTION
            return "wss://map-server-dot-raincrow-pantheon.appspot.com/";
#else
            return "ws://35.196.97.86:8081";
#endif
        }
    }




    public static string wssAddressChat = "ws://staging.raincrowstudios.xyz/Chat";
    public static string wssAddressBase = "ws://staging.raincrowstudios.xyz/";

    public static string whiteCard = "A Shadow Witch draws wisdom and energy from darkness";
    public static string shadowCard = "A White Witch draws wisdom and energy from light";
    public static string greyCard = "Not sure where does a Grey Witch draws it stuff from";
    public static int hexCost = 1;   // hitfx 1
    public static int blessCost = 1;  // hitfx 3
    public static int graceCost = 2; // hitfx 3
    public static int ressurectCost = 2;// hitfx 3
    public static int sunEaterCost = 3;
    public static int whiteFlameCost = 3;
    public static int bindCost = 2;
    public static int banishCost = 2;
    public static int silenceCost = 2;   // hitfx 2
    public static int wasteCost = 3;      // hitfx 1
    public static int USER_USERNAME_PASSWORD_NULL_OR_EMPTY = 4100;
    public static int USER_USERNAME_PASSWORD_WRONG = 4101;
    public static int USER_EMAIL_NULL_OR_EMPTY = 4102;
    public static int USER_NAME_IN_USE = 4103;
    public static int USER_NAME_INVALID = 4104;
    public static int USER_GAME_NULL_EMPTY_OR_INVALID = 4105;
    public static int USER_ID_CHARACTER_MISMATCH = 4106;
    public static int TOKEN_NULL_OR_EMPTY = 4200;
    public static int TOKEN_EXPIRED = 4201;
    public static int TOKEN_INVALID = 4202;
    public static int REDIS_KEY_NULL_OR_EMPTY = 4300;
    public static int REDIS_KEY_NOT_FOUND = 4301;
    public static int REDIS_GEOHASH_NULL_OR_EMPTY = 4302;
    public static int REDIS_GEOHASH_ENTITY_NOT_FOUND = 4304;
    public static int DATASTORE_NO_ENTITY_WITH_ID = 4400;
    public static int DATASTORE_NO_ENTITIES_MATCH_QUERY = 4401;
    public static int ARENA_ID_NULL_OR_EMPTY = 4500;
    public static int ARENA_ID_NO_MATCH = 4501;
    public static int ARENA_FULL = 4502;
    public static int ARENA_ALREADY_JOINED = 4503;
    public static int ARENA_POSITION_ALREADY_FILLED = 4504;
    public static int ARENA_NOT_YOUR_TURN = 4505;
    public static int ARENA_NOT_STARTED_TURN = 4506;
    public static int ARENA_TURN_ALREADY_STARTED = 4507;
    public static int ARENA_ALREADY_GONE = 4508;
    public static int SPELL_INVALID_TARGET = 4600;
    public static int SPELL_TARGET_IMMUNE = 4601;
    public static int SPELL_IN_COOLDOWN = 4602;
    public static int SPELL_CASTER_SILENCED = 4603;
    public static int SPELL_DEGREE_RESTRICTED = 4604;
    public static int CHARACTER_STATUS_DEAD = 4700;
    public static int COVEN_NOT_AUTHORIZED = 4800;
    public static int COVEN_IN_COVEN = 4801;
    public static int COVEN_NOT_MEMBER = 4802;
    public static int COVEN_ALREADY_INVITED = 4803;
    public static int COVEN_COVEN_DISBANDED = 4804;
    public static int COVEN_ALREADY_REQUESTED = 4805;
    public static int COVEN_ALREADY_ALLY = 4806;
    public static int COVEN_REQUEST_NOT_FOUND = 4807;
    public static int COVEN_NOT_ALLY = 4808;
    public static int REDIS_ERROR = 5300;
    public static int DATASTORE_ERROR = 5400;
    public class Commands
    {
        //public const string character_coven_invite = "character_coven_invite";
        //public const string character_coven_reject = "character_coven_reject";
        //public const string coven_member_kick = "character_coven_kick";   // ok
        //public const string coven_member_request = "coven_invite_requested";// ok
        //public const string coven_member_promote = "coven_member_promoted"; // ok
        //public const string coven_member_join = "coven_member_join";   // ok
        //public const string coven_member_leave = "coven_member_left";    // ok
        //public const string coven_member_invited = "coven_member_invited"; // new
        //public const string coven_title_change = "coven_member_titled";   // ok
        // 
        //public const string coven_member_ally = "coven_allied";       // ok
        //public const string coven_member_unally = "coven_unallied";     // ok
        //public const string coven_was_allied = "coven_was_allied";     // ok
        //public const string coven_was_unallied = "coven_was_unallied";   // ok
        //
        //public const string coven_request_invite = "coven_request_invite";
        //public const string coven_disbanded = "coven_disbanded";      // ok
        //public const string map_portal_add = "map_portal_add";
    }
    public static string MessageIDToString(int iID)
    {
        return Utilities.GetConstantValues(iID, typeof(CovenConstants));
    }
}
