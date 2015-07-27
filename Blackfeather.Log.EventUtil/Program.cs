﻿/* 
The MIT License (MIT)

Copyright (c) 2014 - 2015 Timothy D Meadows II

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Diagnostics;
using System.Threading;

namespace Blackfeather.Log.EventUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            var logName = "Application Log";
            var source = "Blackfeather.ApplicationLog";

            switch (args.Length)
            {
                case 1:
                    source = args[0];
                    break;
                case 2:
                    source = args[0];
                    logName = args[1];
                    break;
            }

            try
            {
                if (EventLog.SourceExists(source))
                {
                    return;
                }
            }
            catch (System.Security.SecurityException)
            {
                throw new Exception("You must be of the administrators group to create a new event log source!");
            }

            EventLog.CreateEventSource(source, logName);
            Thread.Sleep(1000); // we need to give windows at lest 1 second to create the event source before use.
        }
    }
}
