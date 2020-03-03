using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Raincrow.BattleArena.Model;

namespace Raincrow.BattleArena.Converters
{
    public class ActionResponseModelContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(System.Type objectType)
        {
            if (typeof(ActionResponseModel).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }

    public class ActionResponseModelConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ActionResponseModelContractResolver() };

        public override bool CanConvert(System.Type objectType)
        {
            return (objectType == typeof(ActionResponseModel));
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string actionResponseType = jo["event"].Value<string>();
            switch (actionResponseType)
            {
                case ActionResponseType.Cast:
                    return JsonConvert.DeserializeObject<CastActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResponseType.Move:
                    return JsonConvert.DeserializeObject<MoveActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResponseType.Summon:
                    return JsonConvert.DeserializeObject<SummonActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResponseType.Flee:
                    return JsonConvert.DeserializeObject<FleeActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResponseType.Banish:
                    return JsonConvert.DeserializeObject<BanishActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
            }
            throw new System.NotImplementedException();
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }
    }

    public class ActionRequestModelContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(System.Type objectType)
        {
            if (typeof(ActionRequestModel).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }

    public class ActionRequestModelConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ActionRequestModelContractResolver() };

        public override bool CanConvert(System.Type objectType)
        {
            return (objectType == typeof(ActionRequestModel));
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string actionRequestType = jo["type"].Value<string>();
            switch (actionRequestType)
            {
                case ActionRequestType.Cast:
                    return JsonConvert.DeserializeObject<CastActionRequestModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionRequestType.Move:
                    return JsonConvert.DeserializeObject<MoveActionRequestModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionRequestType.Summon:
                    return JsonConvert.DeserializeObject<SummonActionRequestModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionRequestType.Flee:
                    return JsonConvert.DeserializeObject<FleeActionRequestModel>(jo.ToString(), SpecifiedSubclassConversion);
            }
            throw new System.NotImplementedException();
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}