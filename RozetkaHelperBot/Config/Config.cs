using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RozetkaHelperBot.Config
{
    public class Token
    {
        public string token { get; set; }
        
    }
    public class Config
    {
        public string  GetToken()
        {
            Token token = new Token();
            using (StreamReader r = new StreamReader(@".\Files\1dq223fw.json"))
            {
                string json = r.ReadToEnd();
                token = JsonConvert.DeserializeObject<Token>(json);
            }
            return token.token;
        }
    }
}
