using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Twikey
{
    public abstract class Gateway
    {
        protected readonly TwikeyClient _twikeyClient;
        
        protected Gateway(TwikeyClient twikeyClient){
            _twikeyClient = twikeyClient;
        }

        protected Dictionary<string, string> CreateParameters(Dictionary<string, string> parameters){
            if(parameters == null)
                return new Dictionary<string, string>();
            return new Dictionary<string, string>(parameters);
        }

        protected void AddIfExists(Dictionary<string, string> parameters, string key, string value){
            if(parameters == null || String.IsNullOrEmpty(key))
                return;
            if(!String.IsNullOrEmpty(value))
                parameters.Add(key, value);
        }

        protected void AddIfExists(JObject parameters, string key, string value){
            if(parameters == null || String.IsNullOrEmpty(key))
                return;
            if(!String.IsNullOrEmpty(value))
                parameters.Add(key, value);
        }
    }
}