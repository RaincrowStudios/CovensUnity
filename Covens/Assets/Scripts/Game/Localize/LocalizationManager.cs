using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LocalizationManager : MonoBehaviour
{
    public static Dictionary<string, string> LocalizeDictionary = new Dictionary<string, string>();
    public static HashSet<string> localizeIDs = new HashSet<string>();
    public delegate void ChangeLanguage();
    public static event ChangeLanguage OnChangeLanguage;

    class LocalizationData
    {
        public string id { get; set; }
        public string value { get; set; }
    }

    public static void CallChangeLanguage()
    {
        if (OnChangeLanguage != null)
            OnChangeLanguage();
    }

    public static void RefreshIDS()
    {
        localizeIDs.Add("download");
        localizeIDs.Add("loading");
        localizeIDs.Add("raincrow_studios");
        localizeIDs.Add("original_sound");
        localizeIDs.Add("summer_tribunal");
        localizeIDs.Add("winter_tribunal");
        localizeIDs.Add("spring_tribunal");
        localizeIDs.Add("autumn_tribunal");
        localizeIDs.Add("days_remain");
        localizeIDs.Add("dominion_location");
        localizeIDs.Add("strongest_witch_dominion");
        localizeIDs.Add("strongest_coven_dominion");
        localizeIDs.Add("login_past_player_account");
        localizeIDs.Add("login_welcome_witch");
        localizeIDs.Add("sign_in_existing");
        localizeIDs.Add("start_new");
        localizeIDs.Add("choose_upper");
        localizeIDs.Add("savannah_upper");
        localizeIDs.Add("continue");
        localizeIDs.Add("continue_upper");
        localizeIDs.Add("account_4_char");
        localizeIDs.Add("password_4_char");
        localizeIDs.Add("character_4_char");
        localizeIDs.Add("account_special_char");
        localizeIDs.Add("invalid_char");
        localizeIDs.Add("account_name");
        localizeIDs.Add("email");
        localizeIDs.Add("password");
        localizeIDs.Add("create_upper");
        localizeIDs.Add("character_name");
        localizeIDs.Add("male");
        localizeIDs.Add("female");
        localizeIDs.Add("character_empty");
        localizeIDs.Add("choose_gender");
        localizeIDs.Add("username_taken");
        localizeIDs.Add("character_taken");
        localizeIDs.Add("name");
        localizeIDs.Add("forgot_password");
        localizeIDs.Add("sign_in");
        localizeIDs.Add("incorrect_username");
        localizeIDs.Add("reset_password");
        localizeIDs.Add("witch_name");
        localizeIDs.Add("reset");
        localizeIDs.Add("reset_upper");
        localizeIDs.Add("no_email");
        localizeIDs.Add("contact_support_upper");
        localizeIDs.Add("empty_email");
        localizeIDs.Add("enter_digit_code_email");
        localizeIDs.Add("enter_code_upper");
        localizeIDs.Add("enter_password_upper");
        localizeIDs.Add("re_enter_password");
        localizeIDs.Add("privacy_policy");
        localizeIDs.Add("change_wardrobe");
        localizeIDs.Add("continue_without_tutorial");
        localizeIDs.Add("chat_shout");
        localizeIDs.Add("chat_news");
        localizeIDs.Add("chat_world");
        localizeIDs.Add("chat_coven");
        localizeIDs.Add("chat_dominion");
        localizeIDs.Add("chat_shoutMessage");
        localizeIDs.Add("chat_degree");
        localizeIDs.Add("chat_shadow");
        localizeIDs.Add("chat_grey");
        localizeIDs.Add("chat_white");
        localizeIDs.Add("chat_translate");
        localizeIDs.Add("collect");
        localizeIDs.Add("add_to_inventory");
        localizeIDs.Add("card_witch_grey");
        localizeIDs.Add("card_witch_shadow");
        localizeIDs.Add("card_witch_white");
        localizeIDs.Add("card_witch_level");
        localizeIDs.Add("card_witch_energy");
        localizeIDs.Add("card_witch_noCoven");
        localizeIDs.Add("card_witch_dominion");
        localizeIDs.Add("card_witch_worldRank");
        localizeIDs.Add("card_witch_domRank");
        localizeIDs.Add("card_witch_cast");
        localizeIDs.Add("school_watch");
        localizeIDs.Add("school_skip");
        localizeIDs.Add("cast_white");
        localizeIDs.Add("cast_shadow");
        localizeIDs.Add("cast_grey");
        localizeIDs.Add("cast_you");
        localizeIDs.Add("cast_energy");
        localizeIDs.Add("cast_level");
        localizeIDs.Add("spell_cast");
        localizeIDs.Add("spell_increase");
        localizeIDs.Add("spell_cost");
        localizeIDs.Add("card_spirit_desc_upper");
        localizeIDs.Add("card_spirit_attack_upper");
        localizeIDs.Add("card_spirit_lesser");
        localizeIDs.Add("card_spirit_greater");
        localizeIDs.Add("card_spirit_superior");
        localizeIDs.Add("card_spirit_legendary");
        localizeIDs.Add("card_spirit_summoner_upper");
        localizeIDs.Add("card_spirit_coven_coven");
        localizeIDs.Add("card_spirit_behavior_upper");
        localizeIDs.Add("cast_spirit_type");
        localizeIDs.Add("cast_spirit_lesser");
        localizeIDs.Add("cast_spirit_greater");
        localizeIDs.Add("cast_spirit_superior");
        localizeIDs.Add("cast_spirit_legendary");
        localizeIDs.Add("coven_screen_invite");
        localizeIDs.Add("coven_screen_level");
        localizeIDs.Add("coven_screen_name");
        localizeIDs.Add("coven_screen_nothing");
        localizeIDs.Add("coven_screen_create");
        localizeIDs.Add("coven_screen_allied");
        localizeIDs.Add("coven_screen_coven_allied");
        localizeIDs.Add("coven_screen_request");
        localizeIDs.Add("cast_screen_request");
        localizeIDs.Add("cast_screen_no_coven");
        localizeIDs.Add("coven_invite_choose");
        localizeIDs.Add("coven_invite_enter_text");
        localizeIDs.Add("coven_invite_continue");
        localizeIDs.Add("ftf_his");
        localizeIDs.Add("ftf_him");
        localizeIDs.Add("ftf_her");
        localizeIDs.Add("server_down_top");
        localizeIDs.Add("server_down_bottom");
		localizeIDs.Add("login_back");
		localizeIDs.Add("reset_account_name");
		localizeIDs.Add("reset_button");
		localizeIDs.Add("reset_user_invalid");
		localizeIDs.Add("reset_enter_code");
		localizeIDs.Add("reset_bad_code");
		localizeIDs.Add("login_create_appropriate");
		localizeIDs.Add("chat_settings");
		localizeIDs.Add("help_logged_in");
		localizeIDs.Add("contact_support");
		localizeIDs.Add("school_title");
		localizeIDs.Add("generic_yes");
		localizeIDs.Add("generic_no");
		localizeIDs.Add("store_elixir_title");
		localizeIDs.Add("store_silver_upper");
		localizeIDs.Add("store_elixir_upper");
		localizeIDs.Add("store_gear_upper");
		localizeIDs.Add("store_silver_drachs_upper");
		localizeIDs.Add("store_purchase_successful");
		localizeIDs.Add("store_purchase_unsuccessful");
		localizeIDs.Add("store_accept");
		localizeIDs.Add("store_bought_drachs_upper");
		localizeIDs.Add("store_elixir");
		localizeIDs.Add("store_elixir_more");
		localizeIDs.Add("store_gear");
		localizeIDs.Add("store_gear_clothing");
		localizeIDs.Add("store_gear_accessories");
		localizeIDs.Add("store_gear_skin_art");
		localizeIDs.Add("store_gear_hairstyle");
		localizeIDs.Add("store_gear_unlocked_upper");
		localizeIDs.Add("store_gear_unlock_upper");
		localizeIDs.Add("store_gear_owned_upper");
		localizeIDs.Add("store_gear_or_upper");
		localizeIDs.Add("store_gear_or");
		localizeIDs.Add("store_buy_upper");
		localizeIDs.Add("store_buy_gold");
		localizeIDs.Add("store_buy_silver");
		localizeIDs.Add("store_preview");
		localizeIDs.Add("store_silver_no_buy");
		localizeIDs.Add("store_silver");
		localizeIDs.Add("store_buy");
		localizeIDs.Add("store_more_silver");
		localizeIDs.Add("store_not_enough_silver");
		localizeIDs.Add("store_not_enough_gold");
		localizeIDs.Add("store_current");
		localizeIDs.Add("store_cost");
		localizeIDs.Add("store_balance");
		localizeIDs.Add("store_confirm_upper");
		localizeIDs.Add("store_cancel_upper");

    }
}


