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
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Blackfeather.Log
{
    public class ApplicationLog : IApplicationLog
    {
        private string _logName = "Application Log";
        private string _source = "Blackfeather.ApplicationLog";
        private readonly object _syncroot = new object();

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        public string LogName
        {
            get { return _logName; }
            set { _logName = value; }
        }
        public long MaximumKilobytes { get; set; }

        public object Syncroot
        {
            get { return _syncroot; }
        }


        public ApplicationLog()
        {
            
        }
        public ApplicationLog(string applicationLogName)
        {
            if (!string.IsNullOrEmpty(applicationLogName))
                _logName = applicationLogName;
        }

        private struct WriteBackThreadInfo
        {
            public Assembly Caller;
            public int Id;
            public LogEntryType Type;
            public string Message;
            public List<string> Messages;
            public bool StackTrace;
        }

        public void Write(Exception exception)
        {
            lock (_syncroot)
            {
                WriteBack(Assembly.GetCallingAssembly(), string.Format("{0}\n\n{1}", exception.Message, exception.StackTrace), LogEntryType.Error);
            } 
        }

        public void Write(string message, LogEntryType type = LogEntryType.Information, int id = 0)
        {
            lock (_syncroot)
            {
                WriteBack(Assembly.GetCallingAssembly(), message, type, true, id);
            }
        }

        public void Write(List<string> message, LogEntryType type = LogEntryType.Information, int id = 0)
        {
            lock (_syncroot)
            {
                WriteBack(Assembly.GetCallingAssembly(), message, type, true, id);
            }
        }

        public List<EventLogEntry> Read(LogEntryType type, out int totalLogs, int? pageIndex = null, int? pageSize = null, int? id = null, DateTime? from = null, DateTime? to = null, List<string> searchList = null, int? index = null)
        {

            EventLog aLog = new EventLog(_logName);
            EventLogEntryType logType = TranslateLogType(type); 
            var ev = from EventLogEntry e in aLog.Entries
                     where e.Source == _source && e.EntryType == logType
                     select e;

            if (id.HasValue)
            {
                ev = ev.Where(e => e.InstanceId.Equals(id.Value));
            }
            if (index.HasValue)
            {
                ev = ev.Where(e => e.Index.Equals(index.Value));
            }
            if (searchList != null)
            {
                ev = ev.Where(e=>e.ReplacementStrings.ToList().Exists(searchList.Contains));
            }
            if (from.HasValue && to.HasValue)
            {
                ev = ev.Where(e => e.TimeWritten >= from && e.TimeWritten <= to);
            }
            else if (from.HasValue)
            {
                ev = ev.Where(e => e.TimeWritten >= from);
            }
            else if (to.HasValue)
            {
                ev = ev.Where(e => e.TimeWritten <= to);
            }

            totalLogs = ev.Count();
            ev = ev.OrderByDescending(e => e.TimeWritten);

            if (!pageIndex.HasValue)
            {
                pageIndex = 0;
            }
            if (!pageSize.HasValue)
            {
                ev = ev.Skip(pageIndex.Value);
            }
            else
            {
                ev = ev.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);
            }


            return ev.ToList();
        }


        public void UpdateMaximumKilobytes()
        {
            Validate();
            if (MaximumKilobytes > 0)
            {
                EventLog log = new EventLog(_logName);
                log.MaximumKilobytes = MaximumKilobytes;
            }
        }


        public void Validate()
        {
            try
            {
                if (EventLog.SourceExists(_source))
                {
                    return;
                }
            }
            catch (System.Security.SecurityException)
            {
                throw new Exception("You must be of the administrators group to create a new event log source!");
            }

            EventLog.CreateEventSource(_source, _logName);
            Thread.Sleep(1000); // we need to give windows at lest 1 second to create the event source before use.
        }


        public void ClearSource()
        {
            try
            {
                if (EventLog.SourceExists(_source))
                {
                    EventLog.DeleteEventSource(_source);
                    return;
                }
            }
            catch (System.Security.SecurityException)
            {
                throw new Exception("You must be of the administrators group to create a new event log source!");
            }
        }

        private void WriteBack(Assembly assembly, string message, LogEntryType type = LogEntryType.Information, bool includeStack = false, int id = 0)
        {
            Validate();

            var writeThreadInfo = new WriteBackThreadInfo() { Id = id, Type = type, Message = message, StackTrace = includeStack, Caller = assembly };
            var writeThreadStartInfo = new ParameterizedThreadStart(WriteBackThread);
            var writeThread = new Thread(writeThreadStartInfo);
            writeThread.Start(writeThreadInfo);
        }

        private void WriteBack(Assembly assembly, List<string> message, LogEntryType type = LogEntryType.Information, bool includeStack = false, int id = 0)
        {
            Validate();

            var writeThreadInfo = new WriteBackThreadInfo() { Id = id, Type = type, Messages = message, StackTrace = includeStack, Caller = assembly };
            var writeThreadStartInfo = new ParameterizedThreadStart(WriteEventBackThread);
            var writeThread = new Thread(writeThreadStartInfo);
            writeThread.Start(writeThreadInfo);
        }

        private void WriteBackThread(object startInfo)
        {
            if (startInfo == null)
            {
                return;
            }

            var threadInfo = (WriteBackThreadInfo) startInfo;
            var stackTrace = new StackTrace(true);
            var logEntry = new EventLog { Source = _source };
            var logType = TranslateLogType(threadInfo.Type);
            var logMessage = string.Format("Source: {1}\nLocation: {2}\n\n{0}\n\n", threadInfo.Message, threadInfo.Caller.FullName, threadInfo.Caller.Location);

            if (threadInfo.StackTrace)
            {
                for (var i = 0; i <= stackTrace.FrameCount - 1; i++)
                {
                    logMessage += stackTrace.GetFrame(i).ToString().Replace("file:line:column ", string.Empty);
                }
            }

            logEntry.WriteEntry(logMessage, logType, threadInfo.Id);
        }


        private void WriteEventBackThread(object startInfo)
        {
            if (startInfo == null)
            {
                return;
            }

            var threadInfo = (WriteBackThreadInfo)startInfo;
            var stackTrace = new StackTrace(true);
            var logEntry = new EventLog { Source = _source };
            var logType = TranslateLogType(threadInfo.Type);
            threadInfo.Messages = threadInfo.Messages ?? new List<string>();
            string stackTraceString = "StackTrace=";
            if (threadInfo.StackTrace)
            {
                for (var i = 0; i <= stackTrace.FrameCount - 1; i++)
                {
                    stackTraceString += stackTrace.GetFrame(i).ToString().Replace("file:line:column ", string.Empty);
                }
            }
            threadInfo.Messages.Add(string.Format("Source={0}", threadInfo.Caller.FullName));
            threadInfo.Messages.Add(string.Format("Location={0}", threadInfo.Caller.Location));
            threadInfo.Messages.Add(stackTraceString);


            EventInstance eventInstance = new EventInstance(threadInfo.Id, 0) { EntryType = logType };
            //Array of string.Event data node is generated based on the array size.
            object[] eventLog = threadInfo.Messages.ToArray();
            //Need to specify the source

            EventLog log = new EventLog { Source = _source };
            log.WriteEvent(eventInstance, eventLog);
        }

        private EventLogEntryType TranslateLogType(LogEntryType type)
        {
            switch (type)
            {
                case LogEntryType.Error:
                    return EventLogEntryType.Error;
                case LogEntryType.Warning:
                    return EventLogEntryType.Warning;
                default:
                    return EventLogEntryType.Information;
            }
        }

    }
}
