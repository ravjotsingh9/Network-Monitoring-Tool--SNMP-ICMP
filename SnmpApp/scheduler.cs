//******************************************************************************
// Product-Name: TaskScheduler 
// -----------------------------------------------------------------------------
// Version:      1.0
// Release Date: 19.07.2009
// -----------------------------------------------------------------------------
// Autor:        Lothar Perr
// EMail:        lothar.perr@call-data.de
// Homepage:     www.call-data.de
// -----------------------------------------------------------------------------
// License:      CPOL (Code Project Open License)
// -----------------------------------------------------------------------------
// THE SOFTWARE AND THE ACCOMPANYING FILES ARE DISTRIBUTED 
// "AS IS" AND WITHOUT ANY WARRANTIES WHETHER EXPRESSED OR IMPLIED. 
// NO REPONSIBILITIES FOR POSSIBLE DAMAGES OR EVEN FUNCTIONALITY CAN BE TAKEN. 
// THE USER MUST ASSUME THE ENTIRE RISK OF USING THIS SOFTWARE.
//******************************************************************************

using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace SnmpApp
{   
    public class scheduler
    {
        public enum DayOccurrence
        {
            First = 0,
            Second = 1,
            Third = 2,
            Fourth = 3,
            Last = 4
        }
        public enum MonthOfTheYeay
        {
            January = 0,
            February = 1,
            March = 2,
            April = 3,
            May = 4,
            June = 5,
            July = 6,
            August = 7,
            September = 8,
            October = 9,
            November = 10,
            December = 11
        }
        public class TriggerSettingsOneTimeOnly
        {
            private DateTime _date;
            private bool _active;

            public DateTime Date
            {
                get
                {
                    return _date;
                }
                set
                {
                    _date = value;
                }
            }
            public bool Active
            {
                get
                {
                    return _active;
                }
                set
                {
                    _active = value;
                }
            }
        }
        public class TriggerSettingsDaily
        {
            private ushort _Interval;

            public ushort Interval
            {
                get
                {
                    return _Interval;
                }
                set
                {
                    _Interval = value;
                    if (_Interval < 0) _Interval = 0;
                }
            }
        }
        public class TriggerSettingsWeekly
        {
            private bool[] _DaysOfWeek;
            public bool[] DaysOfWeek
            {
                get
                {
                    return _DaysOfWeek;
                }
                set
                {
                    _DaysOfWeek = value;
                }
            }

            public TriggerSettingsWeekly()
            {
                _DaysOfWeek = new bool[7];
            }
        }
        public class TriggerSettingsMonthlyWeekDay
        {
            private bool[] _WeekNumber;
            private bool[] _DayOfWeek;
            public bool[] WeekNumber
            {
                get
                {
                    return _WeekNumber;
                }
                set
                {
                    _WeekNumber = value;
                }
            }
            public bool[] DayOfWeek
            {
                get
                {
                    return _DayOfWeek;
                }
                set
                {
                    _DayOfWeek = value;
                }
            }

            public TriggerSettingsMonthlyWeekDay()
            {
                _WeekNumber = new bool[5];
                _DayOfWeek = new bool[7];
            }
        }
        public class TriggerSettingsMonthly
        {
            private bool[] _Month;

            private bool[] _DaysOfMonth;
            private TriggerSettingsMonthlyWeekDay _WeekDay;
            public bool[] Month
            {
                get
                {
                    return _Month;
                }
                set
                {
                    _Month = value;
                }
            }
            public bool[] DaysOfMonth
            {
                get
                {
                    return _DaysOfMonth;
                }
                set
                {
                    _DaysOfMonth = value;
                }
            }
            public TriggerSettingsMonthlyWeekDay WeekDay
            {
                get
                {
                    return _WeekDay;
                }
                set
                {
                    _WeekDay = value;
                }
            }

            public TriggerSettingsMonthly()
            {
                _Month = new bool[12];
                _DaysOfMonth = new bool[32];
                _WeekDay = new TriggerSettingsMonthlyWeekDay();
            }

        }
        public class TriggerSettings
        {
            private TriggerSettingsOneTimeOnly _OneTimeOnly;
            private TriggerSettingsDaily _Daily;
            private TriggerSettingsWeekly _Weekly;
            private TriggerSettingsMonthly _Monthly;

            public TriggerSettingsOneTimeOnly OneTimeOnly
            {
                get
                {
                    return _OneTimeOnly;
                }
                set
                {
                    _OneTimeOnly = value;
                }
            }
            public TriggerSettingsDaily Daily
            {
                get
                {
                    return _Daily;
                }
                set
                {
                    _Daily = value;
                }
            }
            public TriggerSettingsWeekly Weekly
            {
                get
                {
                    return _Weekly;
                }
                set
                {
                    _Weekly = value;
                }
            }
            public TriggerSettingsMonthly Monthly
            {
                get
                {
                    return _Monthly;
                }
                set
                {
                    _Monthly = value;
                }
            }

            public TriggerSettings()
            {
                _OneTimeOnly = new TriggerSettingsOneTimeOnly();
                _Daily = new TriggerSettingsDaily();
                _Weekly = new TriggerSettingsWeekly();
                _Monthly = new TriggerSettingsMonthly();
            }
        }
        public class OnTriggerEventArgs : EventArgs
        {
            public OnTriggerEventArgs(TriggerItem item, DateTime triggerDate)
            {
                _item = item;
                _triggerDate = triggerDate;
            }
            private TriggerItem _item;
            private DateTime _triggerDate;
            public TriggerItem Item
            {
                get { return _item; }
            }
            public DateTime TriggerDate
            {
                get { return _triggerDate; }
            }
        }
        public class TriggerItem
        {
            private DateTime _StartDate = DateTime.MinValue;
            private DateTime _EndDate = DateTime.MaxValue;
            private TriggerSettings _TriggerSettings; 
            private DateTime _TriggerTime; 
            private const byte LastDayOfMonthID = 31; 
            private object _Tag; 
            private bool _Enabled;
            public delegate void OnTriggerEventHandler(object sender, OnTriggerEventArgs e);
            public event OnTriggerEventHandler OnTrigger;
            public TriggerItem()
            {
                _TriggerSettings = new TriggerSettings();
            }
            public String ToXML() 
            {
                XmlSerializer ser = new XmlSerializer(typeof(TriggerItem));
                TextWriter writer = new StringWriter();
                ser.Serialize(writer, this);
                writer.Close();
                return writer.ToString();
            }
            public static TriggerItem FromXML(string Configuration)
            {
                XmlSerializer ser = new XmlSerializer(typeof(TriggerItem));
                TextReader reader = new StringReader(Configuration);
                TriggerItem result = (TriggerItem)ser.Deserialize(reader);
                reader.Close();
                return result;
            }
            public object Tag
            {
                get
                {
                    return _Tag;
                }
                set
                {
                    _Tag = value;
                }
            }
            public bool Enabled
            {
                get
                {
                    return _Enabled;
                }
                set
                {
                    _Enabled = value;
                    if (_Enabled)
                        _TriggerTime = FindNextTriggerDate(_StartDate);
                    else
                        _TriggerTime = DateTime.MaxValue;
                }
            }

            public DateTime StartDate
            {
                get
                {
                    return _StartDate;
                }
                set
                {
                    _StartDate = value;
                    if (_EndDate < _StartDate) _EndDate = _StartDate;
                }
            }

            public DateTime EndDate
            {
                get
                {
                    return _EndDate;
                }
                set
                {
                    _EndDate = value.Date;
                }
            }
            public DateTime TriggerTime
            {
                get
                {
                    return _TriggerTime;
                }
                set
                {
                    _TriggerTime = new DateTime(_TriggerTime.Year, _TriggerTime.Month, _TriggerTime.Day, value.Hour, value.Minute, value.Second);
                }
            }
            public TriggerSettings TriggerSettings
            {
                get
                {
                    return _TriggerSettings;
                }
                set
                {
                    _TriggerSettings = value;
                }
            }
            private DateTime LastDayOfMonth(DateTime date)
            {
                return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
            }
            private int WeekDayOccurInMonth(DateTime date)
            {
                byte count = 0;
                for (int day = 1; day <= date.Day; day++)
                    if (new DateTime(date.Year, date.Month, day).DayOfWeek == date.DayOfWeek)
                        count++;
                return count-1;
            }
            private bool IsLastWeekDayInMonth(DateTime date)
            {
                return (WeekDayOccurInMonth(date.AddDays(7))==0);
            }
            private bool TriggerOneTimeOnly(DateTime date)
            {
                return (_TriggerSettings.OneTimeOnly.Active && (_TriggerSettings.OneTimeOnly.Date == date));
            }
            private bool TriggerDaily(DateTime date)
            {
                if ((date < _StartDate.Date) || (date > _EndDate.Date))
                    return false;
                if (_TriggerSettings.Daily.Interval == 0) 
                    return false;
                DateTime RunTime = _StartDate.Date;
                while (RunTime.Date < date)
                    RunTime = RunTime.AddDays(_TriggerSettings.Daily.Interval);
                return (RunTime == date);
            }
            private bool TriggerWeekly(DateTime date)
            {
                if ((date < _StartDate.Date) || (date > _EndDate.Date)) 
                    return false;
                return (_TriggerSettings.Weekly.DaysOfWeek[(int)date.DayOfWeek]);
            }
            private bool TriggerMonthly(DateTime date)
            {
                if ((date < _StartDate.Date) || (date > _EndDate.Date))
                    return false;

                bool result = false;
                if (_TriggerSettings.Monthly.Month[date.Month - 1])
                {
                    if (_TriggerSettings.Monthly.DaysOfMonth[LastDayOfMonthID])
                        result = (result || (date == LastDayOfMonth(date)));

                    result = (result || (_TriggerSettings.Monthly.DaysOfMonth[date.Day - 1])); 

                    if (_TriggerSettings.Monthly.WeekDay.DayOfWeek[(int)date.DayOfWeek]) 
                    {
                        if (_TriggerSettings.Monthly.WeekDay.WeekNumber[(int)DayOccurrence.Last])
                            result = (result || (IsLastWeekDayInMonth(date)));

                        result = (result || _TriggerSettings.Monthly.WeekDay.WeekNumber[WeekDayOccurInMonth(date)]);
                    }
                }
                return result;
            }
            public bool CheckDate(DateTime date)
            {
                return (TriggerOneTimeOnly(date) || TriggerDaily(date) || TriggerWeekly(date) || TriggerMonthly(date));
            }
            public bool RunCheck(DateTime dateTimeToCheck)
            {
                if (_Enabled)
                {
                    if (dateTimeToCheck >= _TriggerTime) 
                    {
                        OnTriggerEventArgs eventArgs = new OnTriggerEventArgs(this, _TriggerTime);
                        _TriggerTime = FindNextTriggerDate(_TriggerTime.AddDays(1)); // Einen Tag später fortfahren
                        if (OnTrigger != null) 
                            OnTrigger(this, eventArgs);
                        return true;
                    }
                }
                return false;
            }
            private DateTime FindNextTriggerDate(DateTime lastTriggerDate)
            {
                if (!_Enabled)
                    return DateTime.MaxValue;
                
                DateTime date = lastTriggerDate.Date;
                while (date <= _EndDate)
                {
                    if (CheckDate(date.Date))
                        return new DateTime(date.Year, date.Month, date.Day, _TriggerTime.Hour, _TriggerTime.Minute, _TriggerTime.Second);
                    date = date.AddDays(1);
                }
                return DateTime.MaxValue;
            }
            public DateTime GetNextTriggerTime()
            {
                if (_Enabled)
                    return _TriggerTime;
                else
                    return DateTime.MaxValue;
            }
        }
        public class TriggerItemCollection : CollectionBase
        {
            public TriggerItem this[int index]
            {
                get
                {
                    return ((TriggerItem)List[index]);
                }
                set
                {
                    List[index] = value;
                }
            }

            public int Add(TriggerItem value)
            {
                return (List.Add(value));
            }

            public int IndexOf(TriggerItem value)
            {
                return (List.IndexOf(value));
            }

            public void Insert(int index, TriggerItem value)
            {
                List.Insert(index, value);
            }

            public void Remove(TriggerItem value)
            {
                List.Remove(value);
            }

            public bool Contains(TriggerItem value)
            {
                return (List.Contains(value));
            }

            protected override void OnInsert(int index, Object value)
            {
            }

            protected override void OnRemove(int index, Object value)
            {
            }

            protected override void OnSet(int index, Object oldValue, Object newValue)
            {
            }

            protected override void OnValidate(Object value)
            {
                if (value.GetType() != typeof(scheduler.TriggerItem))
                    throw new ArgumentException("Das angegebene Argument ist kein scheduler-Element", "value");
            }

        }
        private TriggerItemCollection _triggerItems;
        private int _Interval = 500;
        private bool _Enabled = false;
        private System.Windows.Forms.Timer _triggerTimer;
        public scheduler()
        {
            _triggerItems = new TriggerItemCollection();
            _triggerTimer = new System.Windows.Forms.Timer();
            _triggerTimer.Tick += new EventHandler(_triggerTimer_Tick);
        }
        public int Interval
        {
            get
            {
                return _Interval;
            }
            set
            {
                _Interval = value;
            }
        }
        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
                if (_Enabled) Start();
                else
                    Stop();
            }
        }
        public TriggerItem AddTrigger(TriggerItem item)
        {
            return _triggerItems[_triggerItems.Add(item)];
        }
        private void Start()
        {
            _triggerTimer.Interval = _Interval;
            _triggerTimer.Start();
        }
        private void Stop()
        {
            _triggerTimer.Stop();
        }
        public TriggerItemCollection TriggerItems
        {
            get
            {
                return _triggerItems;
            }
        }
        void _triggerTimer_Tick(object sender, EventArgs e)
        {
            _triggerTimer.Stop();
            foreach (TriggerItem item in TriggerItems)
                while (item.TriggerTime <= DateTime.Now)
                    item.RunCheck(DateTime.Now);
            _triggerTimer.Start();
        }
    }
}
