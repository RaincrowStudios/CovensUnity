using System.Collections;
using System.Collections.Generic;
using System;

public interface IAnalyticsWrapper
{
    void Init();
    void OnEvent(Dictionary<string, object> data);
    void OnUserProperty(string id, object value);

    Action OnInit { get; set; }
    Action<string> OnInitError { get; set; }
}
