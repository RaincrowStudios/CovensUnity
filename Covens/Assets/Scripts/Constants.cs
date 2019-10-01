
using UnityEngine;

public class CovenConstants : MonoBehaviour
{
    public const string TERMS_OF_SERVICE_URL = "https://www.raincrowstudios.com/terms-of-service";
    public const string PRIVACY_POLICY_URL = "https://www.raincrowstudios.com/privacy";

    public const string GOOGLE_TERMS_OF_SERVICE_URL = "https://cloud.google.com/maps-platform/terms/";
    public const string GOOGLE_PRIVACY_POLICY_URL = "https://policies.google.com/privacy";

    public static bool Debug = true;
    public static bool isBackUpServer = false;
    public static string hostAddress
    {

        get
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.GetString("game") == "Local")
            {
                return "http://localhost:9000/api/";
            }
            else if (UnityEditor.EditorPrefs.GetString("game") == "Release")
            {
                return "https://game-server.raincrow.pw/api/";
            }
            else if (UnityEditor.EditorPrefs.GetString("game") == "Gustavo")
            {
                return "http://192.168.0.94:9000/api/";
            }
            else
            {
                return "https://staging-game-server.raincrow.pw/api/";
            }
#elif PRODUCTION
           return "https://game-server.raincrow.pw/api/";
#else
            return "https://staging-game-server.raincrow.pw/api/";
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
            return "http://34.73.145.51:8082/";
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
                return "ws://localhost:8084/socket.io/?";
            }
            else if (UnityEditor.EditorPrefs.GetString("ws") == "Release")
            {
                return "http://34.73.54.25:8084/socket.io/";
            }
            else if (UnityEditor.EditorPrefs.GetString("ws") == "Gustavo")
            {
                return "http://192.168.0.94:8084/socket.io/?";
            }
            else
            {
                return "https://staging-comms-server.raincrow.pw/socket.io/";
            }
#elif PRODUCTION
                return "http://34.73.54.25:8084/socket.io/";
#else
                return "https://staging-comms-server.raincrow.pw/socket.io/";
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
                return "http://localhost:8083/socket.io/";
            }
            else if (UnityEditor.EditorPrefs.GetString("chat") == "Release")
            {
                return "http://34.73.54.25:8083/socket.io/";
            }
            else if (UnityEditor.EditorPrefs.GetString("chat") == "Gustavo")
            {
                return "http://192.168.0.94:8083/socket.io/";
            }
            else
            {
                return "https://staging-chat-server.raincrow.pw/socket.io/";
            }
#elif PRODUCTION
            return "http://34.73.54.25:8083/socket.io/";
#else
            return "https://staging-chat-server.raincrow.pw/socket.io/";
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
                return "https://map-server.raincrow.pw/";
            }
            else if (UnityEditor.EditorPrefs.GetString("map") == "Local")
            {
                return "http://localhost:8081/";
            }
            else if (UnityEditor.EditorPrefs.GetString("map") == "Gustavo")
            {
                return "http://192.168.0.94:8081/";
            }
            else
            {
                return "https://staging-map-server.raincrow.pw/";
            }
#elif PRODUCTION
            return "https://map-server.raincrow.pw/";
#else
            return "https://staging-map-server.raincrow.pw/";
#endif
        }
    }

    //public static int USER_USERNAME_PASSWORD_NULL_OR_EMPTY = 4100;
    //public static int USER_USERNAME_PASSWORD_WRONG = 4101;
    //public static int USER_EMAIL_NULL_OR_EMPTY = 4102;
    //public static int USER_NAME_IN_USE = 4103;
    //public static int USER_NAME_INVALID = 4104;
    //public static int USER_GAME_NULL_EMPTY_OR_INVALID = 4105;
    //public static int USER_ID_CHARACTER_MISMATCH = 4106;
    //public static int TOKEN_NULL_OR_EMPTY = 4200;
    //public static int TOKEN_EXPIRED = 4201;
    //public static int TOKEN_INVALID = 4202;
    //public static int REDIS_KEY_NULL_OR_EMPTY = 4300;
    //public static int REDIS_KEY_NOT_FOUND = 4301;
    //public static int REDIS_GEOHASH_NULL_OR_EMPTY = 4302;
    //public static int REDIS_GEOHASH_ENTITY_NOT_FOUND = 4304;
    //public static int DATASTORE_NO_ENTITY_WITH_ID = 4400;
    //public static int DATASTORE_NO_ENTITIES_MATCH_QUERY = 4401;
    //public static int ARENA_ID_NULL_OR_EMPTY = 4500;
    //public static int ARENA_ID_NO_MATCH = 4501;
    //public static int ARENA_FULL = 4502;
    //public static int ARENA_ALREADY_JOINED = 4503;
    //public static int ARENA_POSITION_ALREADY_FILLED = 4504;
    //public static int ARENA_NOT_YOUR_TURN = 4505;
    //public static int ARENA_NOT_STARTED_TURN = 4506;
    //public static int ARENA_TURN_ALREADY_STARTED = 4507;
    //public static int ARENA_ALREADY_GONE = 4508;
    //public static int SPELL_INVALID_TARGET = 4600;
    //public static int SPELL_TARGET_IMMUNE = 4601;
    //public static int SPELL_IN_COOLDOWN = 4602;
    //public static int SPELL_CASTER_SILENCED = 4603;
    //public static int SPELL_DEGREE_RESTRICTED = 4604;
    //public static int CHARACTER_STATUS_DEAD = 4700;
    //public static int COVEN_NOT_AUTHORIZED = 4800;
    //public static int COVEN_IN_COVEN = 4801;
    //public static int COVEN_NOT_MEMBER = 4802;
    //public static int COVEN_ALREADY_INVITED = 4803;
    //public static int COVEN_COVEN_DISBANDED = 4804;
    //public static int COVEN_ALREADY_REQUESTED = 4805;
    //public static int COVEN_ALREADY_ALLY = 4806;
    //public static int COVEN_REQUEST_NOT_FOUND = 4807;
    //public static int COVEN_NOT_ALLY = 4808;
    //public static int REDIS_ERROR = 5300;
    //public static int DATASTORE_ERROR = 5400;

}
