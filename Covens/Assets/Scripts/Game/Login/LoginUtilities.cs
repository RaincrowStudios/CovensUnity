using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class LoginUtilities
{
    public static string ValidateUsername(string username)
    {
        if (username.IsNullOrWhiteSpace() || username.Length < 4)
            return LocalizeLookUp.GetText("raincrow_id_letters");

        if (ContainsInvalidCharacters(username, false))
            return LocalizeLookUp.GetText("raincrow_id_special");

        return null;
    }

    public static string ValidatePassword(string password)
    {
        if (password.IsNullOrWhiteSpace() || password.Length < 4)
            return LocalizeLookUp.GetText("password_4_char");

        return null;
    }

    public static string ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        Regex regex = new Regex(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
        Match match = regex.Match(email);

        if (match.Success)
            return null;
        else
            return LocalizeLookUp.GetText("account_creation_email_invalid");
    }

    public static string ValidateCharacterName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return LocalizeLookUp.GetText("character_empty");

        if (name.Length < 4)
            return LocalizeLookUp.GetText("character_4_char");

        if (ContainsInvalidCharacters(name, true))
            return LocalizeLookUp.GetText("character_special_char");

        string reservedName = ValidateReservedName(name);
        if (string.IsNullOrEmpty(reservedName) == false)
            return $"You can't use the name \"{reservedName}\"";
                
        return null;
    }

    public static bool ContainsInvalidCharacters(string text, bool allowSpaces)
    {
        HashSet<char> ValidCharacters = new HashSet<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'd', 'b', 'c', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        if (allowSpaces)
            ValidCharacters.Add(' ');

        foreach (var character in text)
        {
            if (!ValidCharacters.Contains(character))
                return true;
        }
        return false;
    }

    public static string ValidateReservedName(string name)
    {
        name = name.ToLower();

        List<string> reservedNames = new List<string>
        {
            "savana", "savanna", "grey", "gray",
            "brigid", "brlgid", "brlgld", "sawyer", "savvyer",
            "madam", "fortuna", "fortunuh", "fortoona", "fortoonuh",
            "snowdrop"
        };

        foreach(string _aux in reservedNames)
        {
            if (name.Contains(_aux))
                return _aux;
        }

        return null;
    }
}
