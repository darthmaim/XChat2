using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace XChat2.Common.Helper
{
    public static class DateTimeStringFormater
    {
        /* Formatspecifier
            %a	The abbreviated weekday name according to the current locale.
            %A	The full weekday name according to the current locale.
            %b	The abbreviated month name according to the current locale.
            %B	The full month name according to the current locale.
            %c	The preferred date and time representation for the current locale.
            %C	The century number (year/100) as a 2-digit integer. (SU)
            %d	The day of the month as a decimal number (range 01 to 31).
            %D	Equivalent to %m/%d/%y. (Yecch --- for Americans only. Americans should note that in other countries %d/%m/%y is rather common. This means that in international context this format is ambiguous and should not be used.) (SU)
            %e	Like %d, the day of the month as a decimal number, but a leading zero is replaced by a space. (SU)
            %E	Modifier: use alternative format, see below. (SU)
            %F	Equivalent to %Y-%m-%d (the ISO 8601 date format). (C99)
            %G	The ISO 8601 year with century as a decimal number. The 4-digit year corresponding to the ISO week number (see %V). This has the same format and value as %y, except that if the ISO week number belongs to the previous or next year, that year is used instead. (TZ)
            %g	Like %G, but without century, i.e., with a 2-digit year (00-99). (TZ)
            %h	Equivalent to %b. (SU)
            %H	The hour as a decimal number using a 24-hour clock (range 00 to 23).
            %I	The hour as a decimal number using a 12-hour clock (range 01 to 12).
            %j	The day of the year as a decimal number (range 001 to 366).
            %k	The hour (24-hour clock) as a decimal number (range 0 to 23); single digits are preceded by a blank. (See also %H.) (TZ)
            %l	The hour (12-hour clock) as a decimal number (range 1 to 12); single digits are preceded by a blank. (See also %I.) (TZ)
            %m	The month as a decimal number (range 01 to 12).
            %M	The minute as a decimal number (range 00 to 59).
            %n	A newline character. (SU)
            %O	Modifier: use alternative format, see below. (SU)
            %p	Either 'AM' or 'PM' according to the given time value, or the corresponding strings for the current locale. Noon is treated as 'pm' and midnight as 'am'.
            %P	Like %p but in lowercase: 'am' or 'pm' or a corresponding string for the current locale. (GNU)
            %r	The time in a.m. or p.m. notation. In the POSIX locale this is equivalent to '%I:%M:%S %p'. (SU)
            %R	The time in 24-hour notation (%H:%M). (SU) For a version including the seconds, see %T below.
            %s	The number of seconds since the Epoch, i.e., since 1970-01-01 00:00:00 UTC. (TZ)
            %S	The second as a decimal number (range 00 to 60). (The range is up to 60 to allow for occasional leap seconds.)
            %t	A tab character. (SU)
            %T	The time in 24-hour notation (%H:%M:%S). (SU)
            %u	The day of the week as a decimal, range 1 to 7, Monday being 1. See also %w. (SU)
            %U	The week number of the current year as a decimal number, range 00 to 53, starting with the first Sunday as the first day of week 01. See also %V and %W.
            %V	The ISO 8601:1988 week number of the current year as a decimal number, range 01 to 53, where week 1 is the first week that has at least 4 days in the current year, and with Monday as the first day of the week. See also %U and %W. (SU)
            %w	The day of the week as a decimal, range 0 to 6, Sunday being 0. See also %u.
            %W	The week number of the current year as a decimal number, range 00 to 53, starting with the first Monday as the first day of week 01.
            %x	The preferred date representation for the current locale without the time.
            %X	The preferred time representation for the current locale without the date.
            %y	The year as a decimal number without a century (range 00 to 99).
            %Y	The year as a decimal number including the century.
            %z	 The time-zone as hour offset from GMT. Required to emit RFC 822-conformant dates (using "%a, %d %b %Y %H:%M:%S %z"). (GNU)
            %Z	The time zone or name or abbreviation.
            %%	A literal '%' character.
         */

        private static Dictionary<char, Func<DateTime, string>> _formatSpecifier = new Dictionary<char, Func<DateTime, string>>() {
            { 'a', t => t.ToString("ddd")},		//%a	The abbreviated weekday name according to the current locale.
            { 'A', t => t.ToString("dddd")},	//%A	The full weekday name according to the current locale.
            { 'b', t => t.ToString("MMM")},		//%b	The abbreviated month name according to the current locale.
            { 'B', t => t.ToString("MMMM")},	//%B	The full month name according to the current locale.
            { 'c', t => t.ToString()},			//%c	The preferred date and time representation for the current locale.
            { 'C', t => (t.Year / 100).ToString()}, //%C	The century number (year/100) as a 2-digit integer. (SU)
            { 'd', t => t.ToString("dd")},		//%d	The day of the month as a decimal number (range 01 to 31).
            { 'D', t => Format("%m/%d/%y", t)},	//%D	Equivalent to %m/%d/%y. (Yecch --- for Americans only. Americans should note that in other countries %d/%m/%y is rather common. This means that in international context this format is ambiguous and should not be used.) (SU)
            { 'e', t => t.ToString("%M")},		//%e	Like %d, the day of the month as a decimal number, but a leading zero is replaced by a space. (SU)
            { 'F', t => Format("%Y-%m-%d")},	//%F	Equivalent to %Y-%m-%d (the ISO 8601 date format). (C99)
            { 'h', t => _formatSpecifier['b'](t)}, //%h	Equivalent to %b. (SU)
            { 'H', t => t.ToString("HH")}, //%H	The hour as a decimal number using a 24-hour clock (range 00 to 23).
            { 'I', t => t.ToString("hh")}, //%I	The hour as a decimal number using a 12-hour clock (range 01 to 12).
            { 'j', t => t.DayOfYear.ToString("000")}, //%j	The day of the year as a decimal number (range 001 to 366).
            { 'k', t => t.ToString("%H")}, //%k	The hour (24-hour clock) as a decimal number (range 0 to 23); single digits are preceded by a blank. (See also %H.) (TZ)
            { 'l', t => t.ToString("%h")}, //%l	The hour (12-hour clock) as a decimal number (range 1 to 12); single digits are preceded by a blank. (See also %I.) (TZ)
            { 'm', t => t.ToString("MM")}, //%m	The month as a decimal number (range 01 to 12).
            { 'M', t => t.ToString("mm")}, //%M	The minute as a decimal number (range 00 to 59).
            { 'p', t => t.ToString("tt").ToUpper()}, //%p	Either 'AM' or 'PM' according to the given time value, or the corresponding strings for the current locale. Noon is treated as 'pm' and midnight as 'am'.
            { 'P', t => t.ToString("tt").ToUpper()}, //%P	Like %p but in lowercase: 'am' or 'pm' or a corresponding string for the current locale. (GNU)
            { 'r', t => Format("%I:%M:%S %p", t)}, //%r	The time in a.m. or p.m. notation. In the POSIX locale this is equivalent to '%I:%M:%S %p'. (SU)
            { 'R', t => Format("%H:%M", t)}, //%R	The time in 24-hour notation (%H:%M). (SU) For a version including the seconds, see %T below.
            { 's', t => ((int)((t - new DateTime(1970,1,1)).TotalSeconds)).ToString()}, //%s	The number of seconds since the Epoch, i.e., since 1970-01-01 00:00:00 UTC. (TZ)
            { 'S', t => t.ToString("ss")}, //%S	The second as a decimal number (range 00 to 60). (The range is up to 60 to allow for occasional leap seconds.)
            { 't', t => "\t"}, //%t	A tab character. (SU)
            { 'T', t => Format("%H:%M:%S", t)}, //%T	The time in 24-hour notation (%H:%M:%S). (SU)
            { 'u', t => DayOfTheWeek(t.DayOfWeek).ToString()}, //%u	The day of the week as a decimal, range 1 to 7, Monday being 1. See also %w. (SU)
            { 'U', t => WeekNumber(t, DayOfWeek.Sunday).ToString() }, // %U	The week number of the current year as a decimal number, range 00 to 53, starting with the first Sunday as the first day of week 01. See also %V and %W.
            { 'V', t => WeekNumberISO8601(t).ToString()}, //%V	The ISO 8601:1988 week number of the current year as a decimal number, range 01 to 53, where week 1 is the first week that has at least 4 days in the current year, and with Monday as the first day of the week. See also %U and %W. (SU)
            { 'w', t => ((int)t.DayOfWeek).ToString()}, //%w	The day of the week as a decimal, range 0 to 6, Sunday being 0. See also %u.
            { 'W', t => WeekNumber(t, DayOfWeek.Monday).ToString()}, //%W	The week number of the current year as a decimal number, range 00 to 53, starting with the first Monday as the first day of week 01.
            { 'x', t => t.ToShortDateString()}, //%x	The preferred date representation for the current locale without the time.
            { 'X', t => t.ToShortTimeString()}, //%X	The preferred time representation for the current locale without the date.
            { 'y', t => t.ToString("yy")}, //%y	The year as a decimal number without a century (range 00 to 99).
            { 'Y', t => t.ToString("yyyy")}, //%Y	The year as a decimal number including the century.
            { 'z', t => t.ToString("zz")}, //%z	 The time-zone as hour offset from GMT. Required to emit RFC 822-conformant dates (using "%a, %d %b %Y %H:%M:%S %z"). (GNU)
            { '%', t => "%"}, //%%	A literal '%' character.
        };
        
        /// <summary>
        /// Static Method to return ISO 8601:1988 WeekNumber (1-53) for a given year
        /// </summary>
        private static int WeekNumberISO8601(DateTime dt)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        /// <summary>
        /// The week number of the given year as a decimal number, range 00 to 53, starting with the first Monday as the first day of week 01
        /// </summary>
        private static int WeekNumber(DateTime dt, DayOfWeek firstDay)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, firstDay);
            return weekNum;
        }

        private static int DayOfTheWeek(DayOfWeek day)
        {
            int result = (int)day;
            if (result < 1)
                result += 7;
            return result;
        }

        public static string Format(string formatString)
        {
            return Format(formatString, DateTime.Now);
        }

        public static string Format(string formatString, DateTime time)
        {
            StringBuilder resultString = new StringBuilder();
            for (int i = 0; i < formatString.Length; i++)
            {
                if (formatString[i] == '%')
                {
                    i++;

                    if (i < formatString.Length && _formatSpecifier.ContainsKey(formatString[i]))
                    {
                        resultString.Append(_formatSpecifier[formatString[i]](time));
                    }
                    else
                        return "ERROR at char " + i;
                }
                else
                {
                    resultString.Append(formatString[i]);
                }
            }
            return resultString.ToString();
        }

        public static bool FormatIsValid(string formatString)
        {
            for (int i = 0; i < formatString.Length; i++)
            {
                if (formatString[i] == '%')
                {
                    i++;
                    if (i >= formatString.Length || !_formatSpecifier.ContainsKey(formatString[i]))
                        return false;
                }
            }
            return true;
        }
    }
}
