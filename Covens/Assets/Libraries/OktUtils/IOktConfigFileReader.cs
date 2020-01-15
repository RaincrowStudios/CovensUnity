namespace Oktagon.Utils
{
    public interface IOktConfigFileReader
    {
        void SetConfig(string filename);
        string GetStringValue(string key);
        int GetIntValue(string key);
        float GetFloatValue(string key);
        bool GetBoolValue(string key);
    }
}