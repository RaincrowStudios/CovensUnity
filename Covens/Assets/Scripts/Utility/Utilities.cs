using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class Utilities : MonoBehaviour
{

    public static Color Orange = new Color(1, 0.515625f, 0, 1);
    public static Color Red = Color.red;
    public static Color Grey = Color.grey;
    public static Color darkGrey = new Color(0.2156f, 0.2156f, 0.2156f, 1);
    public static Color Green = Color.green;
    public static Color Blue = new Color(0, 0.67588235f, 1, 1);
    public static Color Purple = new Color(0.6980f, 0, 1, 1);
    public static float ChannelSpeed = .15f;
    public static int BaseBuff = 10;

    public static float DamageMultiplier = 1;
    public static float XPMultiplier = 1;
    public static float CostMultiplier = 1;
    public static float SuccessRateMultiplier = 1;
    public static int minSuccessRate = 5;
    public static float maxSuccessRate = 95;


    private float t = 0;

    static bool showLogs = true;

    public enum Spells
    {
        hex, bless, suneater, whiteflames, grace, ressurect, banish, bind, silence, waste
    };

    public static string ToRoman(int number)
    {
        if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
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
        string s = "";
        if (i == 1)
            s = LocalizeLookUp.GetText("degree_1st_").ToUpper();// "1ST DEGREE";
        if (i == 2)
            s = LocalizeLookUp.GetText("degree_2nd_").ToUpper();//"2ND DEGREE";
        if (i == 3)
            s = LocalizeLookUp.GetText("degree_3rd_").ToUpper();//"3RD DEGREE";
        if (i == 4)
            s = LocalizeLookUp.GetText("degree_4th_").ToUpper();//"4TH DEGREE";
        if (i == 5)
            s = LocalizeLookUp.GetText("degree_5th_").ToUpper();//"5TH DEGREE";
        if (i == 6)
            s = LocalizeLookUp.GetText("degree_6th_").ToUpper();//"6TH DEGREE";
        if (i == 7)
            s = LocalizeLookUp.GetText("degree_7th_").ToUpper();//"7TH DEGREE";
        if (i == 8)
            s = LocalizeLookUp.GetText("degree_8th_").ToUpper();//"8TH DEGREE";
        if (i == 9)
            s = LocalizeLookUp.GetText("degree_9th_").ToUpper();//"9TH DEGREE";
        if (i == 10)
            s = LocalizeLookUp.GetText("degree_10th_").ToUpper();//"10TH DEGREE";
        if (i == 11)
            s = LocalizeLookUp.GetText("degree_11th_").ToUpper();//"11TH DEGREE";
        if (i == 12)
            s = LocalizeLookUp.GetText("degree_12th_").ToUpper();//"12TH DEGREE";
        if (i == 13)
            s = LocalizeLookUp.GetText("degree_13th_").ToUpper();//"13TH DEGREE";
        if (i == 14)
            s = LocalizeLookUp.GetText("degree_14th_").ToUpper();//"14TH DEGREE";

        return s;
    }

    public static string GetSchool(int lp)
    {
        string s = "";
        if (lp < 0)
        {
            s += " " + LocalizeLookUp.GetText("card_witch_shadow").ToUpper();// " SHADOW WITCH";
        }
        else if (lp > 0)
            s += " " + LocalizeLookUp.GetText("card_witch_white").ToUpper();//" WHITE WITCH";
        else
            s = LocalizeLookUp.GetText("card_witch_grey").ToUpper();//"GREY WITCH";
        return s;
    }

    public static string GetSchoolCoven(int lp)
    {
        string s = "";
        if (lp < 0)
        {
            s += "(" + LocalizeLookUp.GetText("generic_shadow") + ")";//"(Shadow)";
        }
        else if (lp > 0)
            s += "(" + LocalizeLookUp.GetText("generic_white") + ")";//"(White)";
        else
            s = "(" + LocalizeLookUp.GetText("generic_grey") + ")";//"(Grey)";
        return s;
    }

    public static string witchTypeControlSmallCaps(int lp)
    {
        int i = Mathf.Abs(lp);
        string s = "";
        if (i == 1)
            s = LocalizeLookUp.GetText("degree_1st_");//"1st Degree";
        if (i == 2)
            s = LocalizeLookUp.GetText("degree_2nd_");//"2nd Degree";
        if (i == 3)
            s = LocalizeLookUp.GetText("degree_3rd_");//"3rd Degree";
        if (i == 4)
            s = LocalizeLookUp.GetText("degree_4th_");//"4th Degree";
        if (i == 5)
            s = LocalizeLookUp.GetText("degree_5th_");//"5th Degree";
        if (i == 6)
            s = LocalizeLookUp.GetText("degree_6th_");//"6th Degree";
        if (i == 7)
            s = LocalizeLookUp.GetText("degree_7th_");//"7th Degree";
        if (i == 8)
            s = LocalizeLookUp.GetText("degree_8th_");//"8th Degree";
        if (i == 9)
            s = LocalizeLookUp.GetText("degree_9th_");//"9th Degree";
        if (i == 10)
            s = LocalizeLookUp.GetText("degree_10th_");//"10th Degree";
        if (i == 11)
            s = LocalizeLookUp.GetText("degree_11th_");//"11th Degree";
        if (i == 12)
            s = LocalizeLookUp.GetText("degree_12th_");//"12th Degree";
        if (i == 13)
            s = LocalizeLookUp.GetText("degree_13th_");//"13th Degree";
        if (i == 14)
            s = LocalizeLookUp.GetText("degree_14th_");//"14th Degree";
        if (lp < 0)
        {
            s += " " + LocalizeLookUp.GetText("card_witch_shadow");//" Shadow Witch";
        }
        else if (lp > 0)
            s += " " + LocalizeLookUp.GetText("card_witch_white");//" White Witch";
        else
            s = LocalizeLookUp.GetText("card_witch_grey");//"Grey Witch";

        return s;
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

    public static string EpocToDateTime(double javaTimeStamp)
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

    public static string EpocToDateTimeChat(double javaTimeStamp)
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
                    stamp = (Mathf.Abs((int)timeSpan.TotalMinutes)).ToString() + " " + LocalizeLookUp.GetText("lt_time_mins");//mins";
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
            return "null";
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
    public static void SetCatagoryApparel(ApparelData data)
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

    public static Int32 GetUnixTimestamp(DateTime utc)
    {
        return (Int32)(utc.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
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


