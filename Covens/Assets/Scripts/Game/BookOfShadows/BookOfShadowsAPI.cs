using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BookOfShadowsAPI
{

	public static void Display(Action<BookOfShadows_Display> pSuccess, Action<string> pError)
    {
        BookOfShadows_Display pDisplayData = new BookOfShadows_Display();
        CovenManagerAPI.GetCoven<BookOfShadows_Display>("character/display", pDisplayData, pSuccess, pError);
    }
}
