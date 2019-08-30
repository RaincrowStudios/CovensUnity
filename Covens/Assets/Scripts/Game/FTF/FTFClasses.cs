using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Raincrow.FTF
{
    [System.Serializable]
    public struct FTFPointData
    {
        [JsonProperty("show")]
        public bool show;

        [JsonProperty("anchorMin"), SerializeField]
        private List<float> _anchorMin;

        [JsonProperty("anchorMax"), SerializeField]
        private List<float> _anchorMax;

        [JsonProperty("position"), SerializeField]
        private List<float> _position;


        [JsonIgnore]
        public Vector2 anchorMin
        {
            get => new Vector2(
                _anchorMin?.Count > 0 ? _anchorMin[0] : 0,
                _anchorMin?.Count > 1 ? _anchorMin[1] : 0);
            set => _anchorMin = new List<float>() { value.x, value.y };
        }

        [JsonIgnore]
        public Vector2 anchorMax
        {
            get => new Vector2(
                _anchorMax?.Count > 0 ? _anchorMax[0] : 0,
                _anchorMax?.Count > 1 ? _anchorMax[1] : 0);
            set => _anchorMax = new List<float>() { value.x, value.y };
        }

        [JsonIgnore]
        public Vector2 position
        {
            get => new Vector2(
                _position?.Count > 0 ? _position[0] : 0,
                _position?.Count > 1 ? _position[1] : 0);
            set => _position = new List<float>() { value.x, value.y };
        }
    }

    [System.Serializable]
    public struct FTFRectData
    {
        [JsonProperty("show")]
        public bool show;

        [JsonProperty("anchorMin"), SerializeField]
        private List<float> _anchorMin;

        [JsonProperty("anchorMax"), SerializeField]
        private List<float> _anchorMax;

        [JsonProperty("position"), SerializeField]
        private List<float> _position;

        [JsonProperty("size"), SerializeField]
        private List<float> _size;

        [JsonIgnore]
        public Vector2 anchorMin
        {
            get => new Vector2(
                _anchorMin?.Count > 0 ? _anchorMin[0] : 0,
                _anchorMin?.Count > 1 ? _anchorMin[1] : 0);
            set => _anchorMin = new List<float>() { value.x, value.y };
        }

        [JsonIgnore]
        public Vector2 anchorMax
        {
            get => new Vector2(
                _anchorMax?.Count > 0 ? _anchorMax[0] : 0,
                _anchorMax?.Count > 1 ? _anchorMax[1] : 0);
            set => _anchorMax = new List<float>() { value.x, value.y };
        }

        [JsonIgnore]
        public Vector2 position
        {
            get => new Vector2(
                _position?.Count > 0 ? _position[0] : 0,
                _position?.Count > 1 ? _position[1] : 0);
            set => _position = new List<float>() { value.x, value.y };
        }

        [JsonIgnore]
        public Vector2 size
        {
            get => new Vector2(
                _size?.Count > 0 ? _size[0] : 0,
                _size?.Count > 1 ? _size[1] : 0);
            set => _size = new List<float>() { value.x, value.y };
        }
    }

    [System.Serializable]
    public struct FTFActionData
    {
        [SerializeField,DefaultValue("")]
        public string method;

        [SerializeField]
        public List<object> parameters;
    }

    [System.Serializable]
    public struct FTFStepData
    {
        [SerializeField]
        public float timer;

        [SerializeField]
        public FTFRectData button;

        [SerializeField]
        public FTFRectData highlight;
        
        [SerializeField]
        public FTFPointData pointer;
                
        [SerializeField]
        public List<FTFActionData> onEnter;

        [SerializeField]
        public List<FTFActionData> onExit;
    }

    public struct FinishFTFResponse
    {
        public List<CollectableItem> tools;
        public List<CollectableItem> herbs;
        public List<CollectableItem> gems;
        public ulong xp;
    }
}
