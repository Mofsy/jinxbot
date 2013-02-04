using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.Library;
using Jurassic;
using BNSharp.BattleNet;

namespace JinxBot.Plugins.Script
{
    internal class JsJinxBotClientObjectInstance : ObjectInstance
    {
        private IJinxBotClient _client;
        private JsHost _js;
        private ScriptEngine _engine;

        public JsJinxBotClientObjectInstance(IJinxBotClient client, JsHost js, ScriptEngine engine)
            : base(engine.Object.InstancePrototype)
        {
            this.PopulateFunctions();
            this._client = client;
            _js = js;
            _engine = engine;
        }

        [JSFunction(Name = "send")]
        public void Send(string message)
        {
            try
            {
                _client.SendMessage(message);
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.ToString());
            }
        }

        [JSFunction(Name = "addChat")]
        public void AddChat(ArrayInstance items)
        {
            _js.addChat(items);
        }

        [JSFunction(Name = "connect")]
        public void Connect()
        {
            try
            {
                _client.Client.Connect();
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.ToString());
            }
        }

        [JSFunction(Name = "disconnect")]
        public void Disconnect()
        {
            try
            {
                _client.Client.Close();
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.ToString());
            }
        }

        [JSFunction(Name = "joinChannel")]
        public void JoinChannel(string channelName)
        {
            try
            {
                _client.Client.JoinChannel(channelName, BNSharp.BattleNet.JoinMethod.Default);
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.ToString());
            }
        }

        [JSFunction(Name = "getChannelName")]
        public string GetChannelName()
        {
            try
            {
                return _client.Client.ChannelName;
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.ToString());
            }
        }

        [JSFunction(Name = "getChannelList")]
        public ArrayInstance GetChannelList()
        {
            try
            {
                ArrayInstance result = _engine.Array.Construct(_client.Client.Channel.Select(cu => new JinxBotScriptObjectInstance(_engine, cu)).ToArray());
                return result;
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.ToString());
            }
        }

        [JSFunction(Name = "getUniqueUsername")]
        public string GetUniqueUsername()
        {
            try
            {
                return _client.Client.UniqueUsername;
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.ToString());
            }
        }
    }
}
