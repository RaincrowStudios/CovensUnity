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
            IActionResponseModel response;
            switch (actionResponseType)
            {
                case ActionResponseType.Cast:
                    response = JsonConvert.DeserializeObject<CastActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                    return response;
                case ActionResponseType.Summon:
                    response = JsonConvert.DeserializeObject<SummonActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                    return response;
                case ActionResponseType.Flee:
                    response = JsonConvert.DeserializeObject<FleeActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                    return response;
                case ActionResponseType.Banish:
                    response = JsonConvert.DeserializeObject<BanishActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                    response.IsSuccess = true;
                    return response;
                case ActionResponseType.Move:
                    response = JsonConvert.DeserializeObject<MoveActionResponseModel>(jo.ToString(), SpecifiedSubclassConversion);
                    return response;
            }
            throw new System.ArgumentException("Invalid action response type!");
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