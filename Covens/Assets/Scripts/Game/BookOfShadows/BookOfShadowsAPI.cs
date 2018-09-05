using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;

public class BookOfShadowsAPI
{
    public static BookOfShadows_Display data;

	public static void Display(Action<BookOfShadows_Display> pSuccess, Action<string> pError)
    {
        //Profiler.BeginSample("Book of Shadows Json");
        BookOfShadows_Display pDisplayData = new BookOfShadows_Display();
        CovenManagerAPI.GetCoven<BookOfShadows_Display>("character/display", pDisplayData, pSuccess, pError);
        //Profiler.EndSample();
    }
}
