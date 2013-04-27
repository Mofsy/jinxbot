// Async Console in C#
// Provides a mechanism by which to manipulate the console with independent input and output
// Version 0.1
//
//Copyright (c) 2013, Robert Paveza  <<http://robpaveza.net>>
//All rights reserved.
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met:
//    * Redistributions of source code must retain the above copyright
//      notice, this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in the
//      documentation and/or other materials provided with the distribution.
//    * Neither the name of the <organization> nor the
//      names of its contributors may be used to endorse or promote products
//      derived from this software without specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
//DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// Change History
//
// Version 0.1 - initial release.  Input history support, colors support, no left/right/home/end support
//


using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Paveza
{
    /// <summary>
    /// Provides a mechanism to have independent input and output on the console.
    /// </summary>
    public class AsyncConsole : IDisposable
    {
        private static int _refsAvailable = 1;

        private StringBuilder _inputBuffer;
        private int _outWidth, _outHeight, _outX, _outY, _inX, _inY;
        private object _consoleLock;
        private string _separatorLine, _emptyLine;
        private const char SEPARATOR_CHAR = '═';

        private List<string> _inputQueue;
        private int _lastInputIndex;

        /// <summary>
        /// Creates a new AsyncConsole with the specified buffer width and height.
        /// </summary>
        /// <param name="bufferWidth">The buffer width of the console.</param>
        /// <param name="bufferHeight">The buffer height of the console.</param>
        /// <exception cref="InvalidOperationException">Thrown if there is already an AsyncConsole active in this process.  Each process may only have one active Console, and 
        /// consequently only one active AsyncConsole.</exception>
        public AsyncConsole(int bufferWidth, int bufferHeight)
        {
            if (Interlocked.CompareExchange(ref _refsAvailable, 0, 1) != 1)
                throw new InvalidOperationException("Could not create another AsyncConsole; there is an outstanding reference to one already.");

            _consoleLock = new object();

            _inputBuffer = new StringBuilder();
            _outWidth = bufferWidth;
            _outHeight = bufferHeight;
            _inY = bufferHeight - 1;
            _inputQueue = new List<string>();
            _lastInputIndex = 0;

            Console.BufferWidth = bufferWidth;
            Console.BufferHeight = bufferHeight;

            StringBuilder separator = new StringBuilder(bufferWidth);
            StringBuilder empty = new StringBuilder(bufferWidth);
            for (int i = 0; i < bufferWidth; i++)
            {
                separator.Append(SEPARATOR_CHAR);
                empty.Append(' ');
            }
            empty.Remove(empty.Length - 1, 1);
            _separatorLine = separator.ToString();
            _emptyLine = empty.ToString();

            _outX = 0;
            _outY = Console.BufferHeight - 3;
            Console.CursorVisible = false;

            OutputBackgroundColor = InputBackgroundColor = Console.BackgroundColor;
            OutputForegroundColor = InputForegroundColor = Console.ForegroundColor;
            MaxHistory = 20;

            RenderFullInputBuffer();
        }

        #region IDisposable implementation
        /// <inheritdoc/>
        ~AsyncConsole()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the AsyncConsole, freeing any resources it holds.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases components onto which this AsyncConsole holds.  Derived classes which need to implement IDisposable should override this method.
        /// </summary>
        /// <param name="disposing">True if the object is being disposed, or false if it is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            Interlocked.CompareExchange(ref _refsAvailable, 1, 0);
        }
        #endregion

        #region Console members
        /// <summary>
        /// Gets or sets the background color used for the input area.
        /// </summary>
        public ConsoleColor InputBackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the foreground color used for the input area.
        /// </summary>
        public ConsoleColor InputForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the background color used for the next output content.
        /// </summary>
        public ConsoleColor OutputBackgroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the foreground color used for the next output content.
        /// </summary>
        public ConsoleColor OutputForegroundColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whether caps-lock is enabled.
        /// </summary>
        public bool CapsLock
        {
            get { return Console.CapsLock; }
        }

        /// <summary>
        /// Gets whether number-lock is enabled.
        /// </summary>
        public bool NumberLock
        {
            get { return Console.NumberLock; }
        }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get { return Console.Title; }
            set { Console.Title = value; }
        }

        /// <summary>
        /// Gets or sets whether to treat Ctrl+C as an input combination or an end-command function.
        /// </summary>
        public bool TreatControlCAsInput
        {
            get { return Console.TreatControlCAsInput; }
            set { Console.TreatControlCAsInput = value; }
        }

        /// <summary>
        /// Gets or sets the window width.
        /// </summary>
        public int WindowWidth
        {
            get { return Console.WindowWidth; }
            set { Console.WindowWidth = value; }
        }

        /// <summary>
        /// Gets or sets the window height.
        /// </summary>
        public int WindowHeight
        {
            get { return Console.WindowHeight; }
            set { Console.WindowHeight = value; }
        }

        /// <summary>
        /// Beeps the console.
        /// </summary>
        public void Beep()
        {
            Console.Beep();
        }

        /// <summary>
        /// Beeps the console for a specified frequency and duration.
        /// </summary>
        /// <param name="frequency">The audio frequency to use, in Hz.</param>
        /// <param name="duration">The duration for the tone, in ms.</param>
        public void Beep(int frequency, int duration)
        {
            Console.Beep(frequency, duration);
        }

        /// <summary>
        /// Gets or sets the maximum history of commands to keep.  The default is 20.
        /// </summary>
        public int MaxHistory
        {
            get;
            set;
        }
        #endregion

        #region Specialized methods
        /// <summary>
        /// Clears the console, preserving the input buffer.
        /// </summary>
        public void Clear()
        {
            lock (_consoleLock)
            {
                Console.ForegroundColor = OutputForegroundColor;
                Console.BackgroundColor = OutputBackgroundColor;

                Console.Clear();

                _outY = Console.BufferHeight - 3;
                _outX = 0;

                RenderFullInputBuffer();
            }
        }

        private void RenderFullInputBuffer()
        {
            Console.CursorLeft = 0;
            Console.CursorTop = Console.BufferHeight - 2;

            Console.ForegroundColor = InputForegroundColor;
            Console.BackgroundColor = InputBackgroundColor;

            Console.Write(_separatorLine);
            Console.Write(_emptyLine);
            //Console.Write(" ");

            Console.CursorLeft = 0;
            Console.CursorTop = _inY;

            if (_inputBuffer.Length > (_outWidth - 1))
            {
                string temp = _inputBuffer.ToString().Substring(_inputBuffer.Length - 79);
                Console.Write(temp);
            }
            else
            {
                Console.Write(_inputBuffer.ToString());
            }
        }

        private void ClearInputLinePrivate()
        {
            Console.ForegroundColor = InputForegroundColor;
            Console.BackgroundColor = InputBackgroundColor;

            Console.CursorTop = Console.BufferHeight - 1;
            Console.CursorLeft = 0;
            Console.Write(_emptyLine);
            _inX = 0;
            Console.CursorLeft = 0;
        }

        private void AddToInputBuffer(char toAdd)
        {
            _inputBuffer.Append(toAdd);

            lock (_consoleLock)
            {
                bool moved = false;
                Console.ForegroundColor = InputForegroundColor;
                Console.BackgroundColor = InputBackgroundColor;
                if (_inX >= (_outWidth - 1))
                {
                    int inputLine = Console.BufferHeight - 1;
                    Console.MoveBufferArea(1, inputLine, Console.BufferWidth - 2, 1, 0, inputLine);
                    moved = true;
                }

                Console.CursorLeft = Math.Min(_inX, _outWidth - 2);
                Console.CursorTop = Console.BufferHeight - 1;
                Console.Write(toAdd);

                if (!moved)
                    _inX++;
            }
        }

        private void RemoveLastFromInputBuffer()
        {
            if (_inputBuffer.Length == 0)
                return;

            _inputBuffer.Remove(_inputBuffer.Length - 1, 1);

            lock (_consoleLock)
            {
                if (_inX > 0)
                {
                    _inX--;
                    Console.CursorTop = Console.BufferHeight - 1;
                    Console.CursorLeft = _inX;
                    Console.ForegroundColor = InputForegroundColor;
                    Console.BackgroundColor = InputBackgroundColor;
                    Console.Write(' ');
                    Console.CursorLeft = _inX;
                }
            }
        }

        /// <summary>
        /// Writes text to the console, without a new line.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void Write(string text)
        {
            lock (_consoleLock)
            {
                WritePrivate(text);
            }
        }

        /// <summary>
        /// Writes formatted text to the console, without a new line.
        /// </summary>
        /// <param name="format">The format string to use.</param>
        /// <param name="args">The arguments to the format string.</param>
        public void Write(string format, params object[] args)
        {
            string result = string.Format(format, args);
            Write(result);
        }

        // requires a lock on the console lock object
        private void WritePrivate(string text, bool endWithNewLine = false)
        {
            string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.None);
            // figure out lines
            lines = BreakIntoLines(lines, endWithNewLine);
            int lineCount = lines.Length;

            _outY = _outHeight - 3;
            // render the text
            if (lineCount > 1)
            {
                Console.MoveBufferArea(0, lineCount, Console.BufferWidth, (Console.BufferHeight - 2 - lineCount), 0, 0);
                _outY -= lineCount;
            }
            Console.CursorTop = _outY;
            Console.CursorLeft = _outX;

            Console.BackgroundColor = OutputBackgroundColor;
            Console.ForegroundColor = OutputForegroundColor;

            foreach (string line in lines)
            {
                if (line.Length == _outWidth)
                {
                    Console.Write(line);
                    _outX = 0;
                }
                else
                {
                    Console.Write(line);
                    if (line.Length + _outX == _outWidth)
                    {
                        _outX = 0;
                    }
                    else
                    {
                        _outX += line.Length;
                    }
                }
            }

            if (endWithNewLine)
            {
                Console.MoveBufferArea(0, lineCount, Console.BufferWidth, (Console.BufferHeight - 3), 0, 0);
                _outY = _outHeight - 3;
                _outX = 0;
            }

            _outY = Console.BufferHeight - 2;

            Console.CursorTop = _inY;
            Console.CursorLeft = _inX;
        }

        // requires a lock on the console lock object
        private string[] BreakIntoLines(string[] lines, bool endWithNewLine)
        {
            List<string> result = new List<string>();

            int startOutX = _outX;
            foreach (string line in lines)
            {
                string l = line;
                int remainingLineLength = _outWidth - startOutX;
                while (l.Length > remainingLineLength)
                {
                    string curLine = l.Substring(0, remainingLineLength);
                    l = l.Substring(remainingLineLength);
                    result.Add(curLine);
                    remainingLineLength = _outWidth;
                }
                result.Add(line);
                startOutX = 0;
            }

            //if (endWithNewLine)
            //    result.Add("");

            return result.ToArray();
        }

        /// <summary>
        /// Writes text to the console, with a new line.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void WriteLine(string text)
        {
            lock (_consoleLock)
            {
                WritePrivate(text, true);
            }
        }

        /// <summary>
        /// Writes formatted text to the console, with a new line.
        /// </summary>
        /// <param name="format">The format string to use.</param>
        /// <param name="args">The arguments to the format string.</param>
        public void WriteLine(string format, params object[] args)
        {
            string result = string.Format(format, args);
            WriteLine(result);
        }

        private int _alreadyReading = 0;
        /// <summary>
        /// Reads a line of text from the console asynchronously.  The returned task completes when the user presses Enter.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if multiple callers attempt to call ReadLineAsync() before the first completes.</exception>
        /// <returns>A Task of string which, when fulfilled, has the text entered by the user.</returns>
        public Task<string> ReadLineAsync()
        {
            if (Interlocked.CompareExchange(ref _alreadyReading, 1, 0) == 1)
                throw new InvalidOperationException("The console is already reading asynchronously.");

            return Task.Run(() =>
            {
                _inputBuffer = new StringBuilder();

                Console.CursorLeft = _inX;
                Console.CursorTop = _inY;
                Console.CursorVisible = true;

                bool enterPressed = false;
                while (!enterPressed)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        enterPressed = true;
                    }
                    else if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        enterPressed = true;
                        _inputBuffer = new StringBuilder();
                    }
                    else if (char.IsControl(key.KeyChar))
                    {
                        switch (key.Key)
                        {
                            case ConsoleKey.Backspace:
                                RemoveLastFromInputBuffer();
                                break;
                            case ConsoleKey.Home:
                                // TODO
                                break;
                            case ConsoleKey.End:
                                // TODO
                                break;
                            case ConsoleKey.LeftArrow:
                                // TODO
                                break;
                            case ConsoleKey.RightArrow:
                                // TODO
                                break;
                            case ConsoleKey.UpArrow:
                                string lastInput = GetStringFromInputHistory(-1);
                                _inputBuffer = new StringBuilder(lastInput);
                                lock (_consoleLock)
                                {
                                    RenderFullInputBuffer();
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                string nextInput = GetStringFromInputHistory(1);
                                _inputBuffer = new StringBuilder(nextInput);
                                lock (_consoleLock)
                                {
                                    RenderFullInputBuffer();
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (key.KeyChar != '\0' && !key.Modifiers.HasFlag(ConsoleModifiers.Control) && !char.IsControl(key.KeyChar))
                        {
                            AddToInputBuffer(key.KeyChar);
                        }
                    }
                }

                Console.CursorVisible = false;
                _inX = 0;
                _inY = _outHeight - 1;

                ClearInputLinePrivate();

                Interlocked.CompareExchange(ref _alreadyReading, 0, 1);

                string result = _inputBuffer.ToString();
                _inputBuffer.Clear();
                AddStringToInputHistory(result);
                return result;
            });
        }

        private void AddStringToInputHistory(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            _inputQueue.Add(input);
            if (_inputQueue.Count > MaxHistory)
                _inputQueue.RemoveRange(0, _inputQueue.Count - MaxHistory);

            _lastInputIndex = _inputQueue.Count;
        }

        private string GetStringFromInputHistory(int direction)
        {
            if (direction > 0)
            {
                int test = _lastInputIndex + direction;
                if (test >= MaxHistory || test >= _inputQueue.Count)
                    return string.Empty;

                _lastInputIndex = test;
                return _inputQueue[_lastInputIndex];
            }
            else
            {
                int test = _lastInputIndex + direction;
                if (test < 0)
                    return string.Empty;

                _lastInputIndex = test;
                return _inputQueue[_lastInputIndex];
            }
        }
        #endregion
    }
}