using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;
using System.Diagnostics;
using System.Timers;
using Jurassic.Library;
using JinxBot.Controls;
using System.Drawing;
using System.Threading;
using Timer = System.Timers.Timer;

namespace JinxBot.Plugins.Script
{
    internal class JsHost
    {
        private ScriptEngine _engine;
        private IJinxBotClient _client;
        private JsJinxBotClientObjectInstance _jsClient;

        private int _timeoutId;
        private Dictionary<int, Timer> _timers = new Dictionary<int, Timer>();

        public JsHost(ScriptEngine engine, IJinxBotClient client)
        {
            Debug.Assert(engine != null);
            Debug.Assert(client != null);

            _engine = engine;
            _client = client;
            _jsClient = new JsJinxBotClientObjectInstance(client, this, engine);

            _engine.SetGlobalValue("Colors", new Colors(engine));
            _engine.SetGlobalValue("CssClasses", new CssClasses(engine));
            _engine.SetGlobalFunction("send", (Action<string>)send);
            _engine.SetGlobalFunction("addChat", (Action<ArrayInstance>)addChat);
            _engine.SetGlobalFunction("format", (Func<string, ArrayInstance, string>)format);
            _engine.SetGlobalFunction("setTimeout", (Func<double, ObjectInstance, int>)setTimeout);
            _engine.SetGlobalFunction("clearTimeout", (Action<int>)clearTimeout);
            _engine.SetGlobalFunction("setInterval", (Func<double, ObjectInstance, int>)setInterval);
            _engine.SetGlobalFunction("clearInterval", (Action<int>)clearInterval);

            FunctionInstance instance = engine.Evaluate(DUMP_FUNC) as FunctionInstance;
            engine.SetGlobalValue("dump", instance);
            engine.SetGlobalValue("client", _jsClient);

        }

        private const string DUMP_FUNC = @"(function(arr,level) {
/**
 * Function : dump()
 * Arguments: The data - array,hash(associative array),object
 *    The level - OPTIONAL
 * Returns  : The textual representation of the array.
 * This function was inspired by the print_r function of PHP.
 * This will accept some data as the argument and return a
 * text that will be a more readable version of the
 * array/hash/object that is given.
 * Docs: http://www.openjs.com/scripts/others/dump_function_php_print_r.php
 */
	var dumped_text = """";
	if(!level) level = 0;
	
	//The padding given at the beginning of the line.
	var level_padding = """";
	for(var j=0;j<level+1;j++) level_padding += ""    "";
	
	if(typeof(arr) == 'object') { //Array/Hashes/Objects 
		for(var item in arr) {
			var value = arr[item];
			
			if(typeof(value) == 'object') { //If it is an array,
				dumped_text += level_padding + ""'"" + item + ""' ...\n"";
				dumped_text += dump(value,level+1);
			} else {
				dumped_text += level_padding + ""'"" + item + ""' => \"""" + value + ""\""\n"";
			}
		}
	} else { //Stings/Chars/Numbers etc.
		dumped_text = ""===>""+arr+""<===(""+typeof(arr)+"")"";
	}
	return dumped_text;
})";

        public ObjectInstance ClientHost
        {
            get { return _jsClient; }
        }

        #region Standard JavaScript functions - setTimeout/setInterval

        public int setTimeout(double msTimeout, ObjectInstance callback)
        {
            if (callback == null)
                return -1;

            StringInstance strInst = callback as StringInstance;
            FunctionInstance funcInst = callback as FunctionInstance;

            if (funcInst == null && strInst == null)
                return -1;

            int id = Interlocked.Increment(ref _timeoutId);
            Timer timer = new Timer(msTimeout);
            timer.AutoReset = false;
            _timers.Add(id, timer);
            timer.Elapsed += (sender, e) =>
            {
                if (funcInst != null)
                    funcInst.Call(null);
                else
                    _engine.Evaluate(strInst.ToString());

                timer.Dispose();
            };
            timer.Start();
            return id;
        }

        public void clearTimeout(int timeoutId)
        {
            Timer timer;
            if (_timers.TryGetValue(timeoutId, out timer))
            {
                timer.Stop();
                _timers.Remove(timeoutId);
                timer.Dispose();
            }
        }

        public int setInterval(double msTimeout, ObjectInstance callback)
        {
            if (callback == null)
                return -1;

            StringInstance strInst = callback as StringInstance;
            FunctionInstance funcInst = callback as FunctionInstance;

            if (funcInst == null && strInst == null)
                return -1;

            int id = Interlocked.Increment(ref _timeoutId);
            Timer timer = new Timer(msTimeout);
            timer.AutoReset = true;
            _timers.Add(id, timer);
            timer.Elapsed += (sender, e) =>
            {
                if (funcInst != null)
                    funcInst.Call(null);
                else
                    _engine.Evaluate(strInst.ToString());
            };
            timer.Start();
            return id;
        }

        public void clearInterval(int timeoutId)
        {
            Timer timer;
            if (_timers.TryGetValue(timeoutId, out timer))
            {
                timer.Stop();
                _timers.Remove(timeoutId);
                timer.Dispose();
            }
        }

        #endregion

        public string format(string fmtString, ArrayInstance args)
        {
            object[] arguments = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
                arguments[i] = args[i];

            return string.Format(fmtString, arguments);
        }

        internal void send(string message)
        {
            try
            {
                _client.SendMessage(message);
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.Message);
            }
        }

        internal void addChat(ArrayInstance chatNodes)
        {
            List<ChatNode> result = new List<ChatNode>();

            for (int i = 0; i < chatNodes.Length; i++)
            {
                ObjectInstance obj = chatNodes[i] as ObjectInstance;
                if (obj == null)
                    continue;

                string str = obj.GetPropertyValue("text") as string;
                object oTxt = obj.GetPropertyValue("text");
                if (oTxt == null) continue;
                str = oTxt.ToString();

                ColorInstance color = obj.GetPropertyValue("color") as ColorInstance;
                string cssClass = obj.GetPropertyValue("cssClass") as string;

                if (str == null)
                    continue;
                else if (color == null && cssClass == null)
                    continue;

                ChatNode node;
                if (color == null)
                {
                    node = new ChatNode(str, cssClass);
                }
                else
                {
                    node = new ChatNode(str, Color.FromArgb(255, color.R, color.G, color.B));
                }

                result.Add(node);
            }

            try
            {
                _client.MainWindow.AddChat(result);
            }
            catch (Exception ex)
            {
                throw new JavaScriptException(_engine, ex.GetType().Name, ex.Message);
            }
        }
    }
}
