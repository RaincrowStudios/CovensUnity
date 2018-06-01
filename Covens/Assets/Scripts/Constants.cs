using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

public class Constants  {

#if SERVER_RELEASE
    public static string hostAddress = "https://raincrow-pantheon.appspot.com/api/";
    public static string hostAddressRaincrowLocal = "https://raincrow-pantheon.appspot.com/api/raincrow/";
    public static string hostAddressLocal = "https://raincrow-pantheon.appspot.com/api/covens/";
    public static string hostAddressRaincrow = "https://raincrow-pantheon.appspot.com/api/raincrow/";
    public static string wsAddress = "https://raincrowstudios.xyz:8080";
    public static string wssAddress = "wss://raincrowstudios.xyz:8080?";
#else

    public static string hostAddress = "http://localhost:8080/api/";
    public static string hostAddressRaincrowLocal = "http://localhost:8080/api/raincrow/";
    public static string hostAddressLocal = "http://localhost:8080/api/covens/";
    public static string hostAddressRaincrow = "https://raincrow-pantheon.appspot.com/api/raincrow/";
    public static string wsAddress = "https://raincrowstudios.xyz:8080";
    public static string wssAddress = "wss://raincrowstudios.xyz:8080?";
#endif

	public static string hexDiscription = "Eentesque posuere nec dolor ut convallis. Morbi a massa tincidunt, tempus nisi vitae, bibendum augue. ";
	public static string blessDiscription = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque posuere";
	public static string graceiscription = "Morbi a massa tincidunt, tempus nisi vitae, bibendum augue. Duis tempus posuere";
	public static string ressurectiscription = "Aadipiscing elit. Morbi a massa tincidunt, tempus nisi vitae, ";
	public static string sunEaterDiscription = "Nullam magna lectus, tristique non ultricies eget, consectetur adipiscing elit.";
	public static string whiteFlameDiscription = "Aenean sagittis consequat dictum. Nullam magna lectus, tristique non ultricies eget,";
	public static string bindDiscription = "Pellentesque posuere nec dolor ut convallis. Morbi a massa tincidunt, tempus nisi vitae, ";
	public static string banishDiscription = "Vivamus bibendum faucibus leo non viverra. Curabitur faucibus a nibh vel hendrerit.";
	public static string silenceDiscription = "In vel venenatis lacus. Nulla pretium libero id sapien cursus cursus. Praesent non luctus urna, ac posuere. ";
	public static string wasteDiscription = "Nulla pretium libero id sapien cursus cursus. Morbi a massa tincidunt, tempus nisi vitae, bibendum augue..";

	public static string whiteCard = "A Shadow Witch draws wisdom and energy from darkness";
	public static string shadowCard = "A White Witch draws wisdom and energy from light";
	public static string greyCard = "Not sure where does a Grey Witch draws it stuff from";

	public static int hexCost = 1;		// hitfx 1
	public static int blessCost = 1;    // hitfx 3
	public static int graceCost  = 2; // hitfx 3
	public static int ressurectCost =2 ;// hitfx 3
	public static int sunEaterCost = 3 ;
	public static int whiteFlameCost = 3;
	public static int bindCost = 2;
	public static int banishCost = 2;
	public static int silenceCost = 2;     // hitfx 2
	public static int wasteCost = 3;            // hitfx 1





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
        public const string coven_member_ally = "coven_member_ally";
        public const string coven_member_kick = "coven_member_kick";
        public const string coven_member_unally = "coven_member_unally";
        public const string coven_member_request = "coven_member_request";
        public const string coven_member_promote = "coven_member_promote";
        public const string coven_member_join = "coven_member_join";
        public const string coven_title_change = "coven_title_change";
        // 
        public const string coven_was_allied = "coven_was_allied";
        public const string coven_was_unallied = "coven_was_unallied";


        public const string map_portal_add = "map_portal_add";
    }


    public static string MessageIDToString(int iID)
    {
        return Utilities.GetConstantValues(iID, typeof(Constants));
    }
}
    