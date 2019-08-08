using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using Newtonsoft.Json;

public class Utilities : MonoBehaviour
{

    public static readonly Color Orange = new Color(1, 0.515625f, 0, 1);
    public static readonly Color Red = Color.red;
    public static readonly Color Grey = Color.grey;
    public static readonly Color darkGrey = new Color(0.2156f, 0.2156f, 0.2156f, 1);
    public static readonly Color Green = Color.green;
    public static readonly Color Blue = new Color(0, 0.67588235f, 1, 1);
    public static readonly Color Purple = new Color(0.6980f, 0, 1, 1);
    public static readonly float ChannelSpeed = .15f;
    public static readonly int BaseBuff = 10;
    public static readonly DateTime UnixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

    public static readonly float DamageMultiplier = 1;
    public static readonly float XPMultiplier = 1;
    public static readonly float CostMultiplier = 1;
    public static readonly float SuccessRateMultiplier = 1;
    public static readonly int minSuccessRate = 5;
    public static readonly float maxSuccessRate = 95;


    private float t = 0;

    static bool showLogs = true;

    public enum Spells
    {
        hex, bless, suneater, whiteflames, grace, ressurect, banish, bind, silence, waste
    };

    public static string ToRoman(int number)
    {
        if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 an1d 3999");
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        throw new ArgumentOutOfRangeException("something bad happened");
    }

    public static string GetDegree(int lp)
    {
        int i = Mathf.Abs(lp);
        string degreeId = string.Empty;
        if (i == 0)
            return string.Empty;
        switch (i)
        {
            case 1:
                degreeId = "degree_1st_";
                break;
            case 2:
                degreeId = "degree_2nd_";
                break;
            case 3:
                degreeId = "degree_3rd_";
                break;
            default:
                degreeId = string.Format("degree_{0}th_", i);
                break;
        }
        return LocalizeLookUp.GetText(degreeId);
    }

    public static string GetSchool(int lp)
    {
        string s = string.Empty;
        if (lp < 0)
        {
            s = string.Concat(s, " ", LocalizeLookUp.GetText("card_witch_shadow")); // " SHADOW WITCH";
        }
        else if (lp > 0)
        {
            s = string.Concat(s, " ", LocalizeLookUp.GetText("card_witch_white")); //" WHITE WITCH";
        }
        else
        {
            s = LocalizeLookUp.GetText("card_witch_grey"); //"GREY WITCH";
        }
        return s;
    }

    public static Color GetSchoolColor(int lp)
    {
        if (lp < 0)
        {
            return Purple; // " SHADOW WITCH";
        }
        else if (lp > 0)
        {
            return Orange; //" WHITE WITCH";
        }
        else
        {
            return Blue; //"GREY WITCH";
        }
    }

    public static string GetSchoolCoven(int lp)
    {
        string s = "";
        if (lp < 0)
        {
            //s = string.Concat(s, "(", LocalizeLookUp.GetText("generic_shadow"), ")");//"(Shadow)";)
            s = LocalizeLookUp.GetText("coven_shadow");
        }
        else if (lp > 0)
        {
            //s = string.Concat(s, "(", LocalizeLookUp.GetText("generic_white"), ")");
            s = LocalizeLookUp.GetText("coven_white");
        }
        else
        {
            //s = string.Concat("(", LocalizeLookUp.GetText("generic_grey"), ")");//"(Grey)";
            s = LocalizeLookUp.GetText("coven_grey");
        }
        return s;
    }

    public static string WitchTypeControlSmallCaps(int lp)
    {
        string degree = GetDegree(lp);
        string witchType = string.Empty;
        if (lp < 0)
        {
            witchType = string.Concat(degree, " ", LocalizeLookUp.GetText("card_witch_shadow")); //" Shadow Witch";
        }
        else if (lp > 0)
        {
            witchType = string.Concat(witchType, " ", LocalizeLookUp.GetText("card_witch_white")); //" White Witch";
        }
        else
        {
            witchType = LocalizeLookUp.GetText("card_witch_grey"); //"Grey Witch";
        }

        return witchType;
    }

    public static GameObject InstantiateObject(GameObject prefab, Transform parent, float scale = 1)
    {
        GameObject g = Instantiate(prefab, parent);
        g.transform.SetParent(parent);
        g.transform.localPosition = Vector3.zero;
        g.transform.localEulerAngles = Vector3.zero;
        g.transform.transform.localScale = new Vector3(scale, scale, scale);
        return g;
    }

    public static GameObject InstantiateUI(GameObject prefab, Transform parent)
    {
        GameObject g = InstantiateObject(prefab, parent);
        g.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        g.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        g.name = prefab.name;
        return g;
    }

    public static void allowMapControl(bool allow, bool allowCameraControl = false)
    {
        MapsAPI.Instance.allowControl = allow;
    }

    public static string EpochToDateTime(double javaTimeStamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
        var timeSpan = (int)dtDateTime.Subtract(DateTime.UtcNow).TotalHours;
        string stamp = "";
        if (timeSpan > 0)
        {
            stamp = timeSpan.ToString() + " " + Oktagon.Localization.Lokaki.GetText("General_Hour");// " hours";
        }
        else

            stamp = Oktagon.Localization.Lokaki.GetText("General_LessThanHour");// "less than an hour";
        return stamp;
    }

    public static System.DateTime FromJavaTime(double timestamp)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(timestamp).ToUniversalTime();
        return dtDateTime;
    }

    /// <summary>
    /// Converts Unix Time to dd/mm/yyyy or similar, accordingly with the CultureInfo
    /// </summary>
    /// <returns></returns>
    public static string ShowDateTimeWithCultureInfo(long unixTime)
    {
        string currentCulture = DictionaryManager.GetCurrentCultureName();
        CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(currentCulture);
        return FromJavaTime(unixTime).ToString("d", cultureInfo);
    }


    public static System.TimeSpan TimespanFromJavaTime(double timestamp)
    {
        System.DateTime date = FromJavaTime(timestamp);
        return date.Subtract(System.DateTime.UtcNow);
    }

    public static string GetSummonTime(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }

        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = dtDateTime.Subtract(DateTime.UtcNow);

        string stamp = "";
        if (timeSpan.Days > 0)
        {
            if (timeSpan.Days == 1)
                stamp = "1 " + LocalizeLookUp.GetText("lt_time_day");// "1 day";
            else
                stamp = timeSpan.Days + " " + LocalizeLookUp.GetText("lt_time_days");// " days";
        }
        else if (timeSpan.Hours > 0)
        {
            if (timeSpan.Hours == 1)
                stamp = "1 " + LocalizeLookUp.GetText("lt_time_hour");//"1 hour";
            else
                stamp = timeSpan.Hours + " " + LocalizeLookUp.GetText("lt_time_hours");//" hours";
        }
        else if (timeSpan.Minutes > 0)
        {
            if (timeSpan.Minutes == 1)
                stamp = "1 " + LocalizeLookUp.GetText("lt_time_minute");//"1 min";
            else
                stamp = timeSpan.Minutes + " " + LocalizeLookUp.GetText("lt_time_minutes");//" mins";
        }
        else
        {
            stamp = timeSpan.Seconds + " " + LocalizeLookUp.GetText("lt_time_secs");//" secs";
        }

        return stamp;
    }

    public static string GetTimeStamp(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();

        return dtDateTime.ToString("g");
    }

    public static string GetTimeStampBOS(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();

        return dtDateTime.ToString("m") + ", " + dtDateTime.Year.ToString();
    }

    public static string EpochToDateTimeChat(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }
        System.DateTime dtDateTime = UnixStartTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var timeSpan = dtDateTime.Subtract(DateTime.UtcNow);
        string stamp = "";
        if (timeSpan.TotalDays < -1)
        {
            stamp = (Mathf.Abs((int)timeSpan.TotalDays)).ToString() + " " + LocalizeLookUp.GetText("lt_time_days");//days";
        }
        else
        {
            if (timeSpan.TotalHours < -1)
            {
                stamp = (Mathf.Abs((int)timeSpan.TotalHours)).ToString() + " " + LocalizeLookUp.GetText("lt_time_hours");//hours";
            }
            else
            {
                if (timeSpan.TotalMinutes < -1)
                {
                    stamp = (Mathf.Abs((int)timeSpan.TotalMinutes)).ToString() + " " + LocalizeLookUp.GetText("lt_time_minutes");//mins";
                }
                else
                {
                    stamp = LocalizeLookUp.GetText("chat_time_ago_seconds");//"few seconds";
                    return stamp;
                }
            }
        }
        stamp += " " + LocalizeLookUp.GetText("generic_ago");//" ago";
        return stamp;
    }

    public static string GetTimeRemaining(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }

        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        if (DateTime.Compare(dtDateTime, DateTime.UtcNow) > 0)
        {
            TimeSpan timeSpan = dtDateTime.Subtract(DateTime.UtcNow);
            return String.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
        else
        {
            return "";
        }
    }

    public static string GetTimeRemainingPOPUI(double javaTimeStamp)
    {
        if (javaTimeStamp < 159348924)
        {
            string s = "unknown";
            return s;
        }

        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToUniversalTime();
        var span = -DateTime.UtcNow.Subtract(dtDateTime);
        if (span.Days >= 1)
        {
            return String.Format("{0}d:{1}h:{2}m", span.Days, span.Hours, span.Minutes);
        }
        else if (span.Hours >= 1)
        {
            return String.Format("{0}h:{1}m", span.Hours, span.Minutes);
        }
        else if (span.Minutes >= 0)
        {
            return String.Format("{0}m", span.Minutes);
        }
        else
        {
            return "";
        }
    }

    public static void SetActiveList(bool bActive, params GameObject[] vGOs)
    {
        foreach (GameObject pGO in vGOs)
        {
            if (pGO != null)
                pGO.SetActive(bActive);
        }
    }
    public static void SetEnableButtonList(bool bActive, params GameObject[] vGOs)
    {
        foreach (GameObject pGO in vGOs)
        {
            if (pGO != null)
            {
                UnityEngine.UI.Button pButton = pGO.GetComponent<UnityEngine.UI.Button>();
                if (pButton != null)
                    pButton.interactable = bActive;
            }
        }
    }


    /// <summary>
    /// super-heavy wat to get values. caching them is a good way 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="iNumber"></param>
    /// <returns></returns>
    public static string GetConstantValues(int iNumber, Type type)
    {
        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (FieldInfo fi in fieldInfos)
        {
            string s = "1";
            try
            {
                if ((int)fi.GetValue(type) == iNumber)
                {
                    return fi.Name;
                }
            }
            catch (Exception e)
            {
                //Debug.LogError("ParseError [" + s + "] [" + fi.Name + "]:" + e.Message);
            }
        }
        return null;
    }

    public static float GetMinuteDifference(double start, double end)
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(start).ToUniversalTime();
        System.DateTime dtDateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime2 = dtDateTime2.AddMilliseconds(end).ToUniversalTime();
        var timeSpan = dtDateTime2.Subtract(dtDateTime);
        return (float)timeSpan.TotalMinutes;
    }
    public static void SetCatagoryApparel(CosmeticData data)
    {
        if (data.position == "head")
        {
            data.catagory = data.position;
            data.storeCatagory = "clothing";
            return;
        }
        else if (data.position.Contains("hair"))
        {
            data.catagory = data.position;
            data.storeCatagory = "hairstyles";
            return;
        }
        else if (data.position.Contains("neck"))
        {
            data.catagory = data.position;
            data.storeCatagory = "accessories";
            return;
        }
        else if (data.position.Contains("chest"))
        {
            data.catagory = data.position;
            data.storeCatagory = "clothing";
            return;
        }
        else if (data.position.Contains("skin"))
        {
            data.catagory = data.position;
            data.storeCatagory = "skinart";
            return;
        }
        else if (data.position.Contains("legs"))
        {
            data.catagory = data.position;
            data.storeCatagory = "clothing";
            return;
        }
        else if (data.position.Contains("feet"))
        {
            data.catagory = data.position;
            data.storeCatagory = "clothing";
            return;
        }
        else if (data.position.Contains("wrist"))
        {
            data.catagory = "wrist";
            data.storeCatagory = "accessories";
            return;
        }
        else if (data.position.Contains("finger"))
        {
            data.catagory = "hands";
            data.storeCatagory = "accessories";
            return;
        }
        else if (data.position.Contains("carryOn"))
        {
            data.catagory = "carryOn";
            data.storeCatagory = "accessories";
            return;
        }
        else if (data.position.Contains("waist"))
        {
            data.catagory = "chest";
            data.storeCatagory = "clothing";
            return;
        }
        else if (data.position.Contains("petFeet"))
        {
            data.catagory = data.position;
            data.storeCatagory = "accessories";
            return;
        }
    }

    public static int GetAvatar()
    {
        var data = PlayerDataManager.playerData;
        if (data.male)
        {
            if (data.race.Contains("A"))
            {
                return 0;
            }
            else if (data.race.Contains("O"))
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (data.race.Contains("A"))
            {
                return 3;

            }
            else if (data.race.Contains("O"))
            {
                return 4;

            }
            else
            {
                return 5;

            }
        }
    }

    public static void Log(string msg)
    {
#if UNITY_EDITOR
        Debug.Log(msg);
#endif
    }

    public static double GetUnixTimestamp(DateTime dateTime)
    {
        return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string value)
    {
        if (value != null)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public static class ArrayExtensions
{
    public static bool Contains<T>(this T[] a, T b)
    {
        if (a == null)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i].Equals(b))
                return true;
        }
        return false;
    }
}

public static class JsonExtensions
{
    public static bool TryParseJson<T>(this string obj, out object result)
    {
        try
        {
            Debug.Log(obj);
            result = JsonConvert.DeserializeObject<T>(obj);
            return true;
        }
        catch (Exception)
        {
            result = default(T);
            return false;
        }
    }

}

