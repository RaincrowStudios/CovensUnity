using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Raincrow.BattleArena.Model;

namespace Raincrow.BattleArena.Converters
{
    public class ActionResultModelContractResolver : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(System.Type objectType)
        {
            if (typeof(ActionResultModel).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }

    public class ActionResultModelConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new ActionResultModelContractResolver() };

        public override bool CanConvert(System.Type objectType)
        {
            return (objectType == typeof(ActionResultModel));
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string actionResultType = jo["event"].Value<string>();
            switch (actionResultType)
            {
                case ActionResultType.Cast:
                    return JsonConvert.DeserializeObject<CastActionResultModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResultType.Move:
                    return JsonConvert.DeserializeObject<MoveActionResultModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResultType.Summon:
                    return JsonConvert.DeserializeObject<SummonActionResultModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResultType.Flee:
                    return JsonConvert.DeserializeObject<FleeActionResultModel>(jo.ToString(), SpecifiedSubclassConversion);
                case ActionResultType.Banish:
                    return JsonConvert.DeserializeObject<BanishActionResultModel>(jo.ToString(), SpecifiedSubclassConversion);
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