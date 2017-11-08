// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.HttpDateParse
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Globalization;

namespace System.ServiceModel.Web
{
  internal static class HttpDateParse
  {
    private const int BASE_DEC = 10;
    private const int DATE_INDEX_DAY_OF_WEEK = 0;
    private const int DATE_1123_INDEX_DAY = 1;
    private const int DATE_1123_INDEX_MONTH = 2;
    private const int DATE_1123_INDEX_YEAR = 3;
    private const int DATE_1123_INDEX_HRS = 4;
    private const int DATE_1123_INDEX_MINS = 5;
    private const int DATE_1123_INDEX_SECS = 6;
    private const int DATE_ANSI_INDEX_MONTH = 1;
    private const int DATE_ANSI_INDEX_DAY = 2;
    private const int DATE_ANSI_INDEX_HRS = 3;
    private const int DATE_ANSI_INDEX_MINS = 4;
    private const int DATE_ANSI_INDEX_SECS = 5;
    private const int DATE_ANSI_INDEX_YEAR = 6;
    private const int DATE_INDEX_TZ = 7;
    private const int DATE_INDEX_LAST = 7;
    private const int MAX_FIELD_DATE_ENTRIES = 8;
    private const int DATE_TOKEN_JANUARY = 1;
    private const int DATE_TOKEN_FEBRUARY = 2;
    private const int DATE_TOKEN_MARCH = 3;
    private const int DATE_TOKEN_APRIL = 4;
    private const int DATE_TOKEN_MAY = 5;
    private const int DATE_TOKEN_JUNE = 6;
    private const int DATE_TOKEN_JULY = 7;
    private const int DATE_TOKEN_AUGUST = 8;
    private const int DATE_TOKEN_SEPTEMBER = 9;
    private const int DATE_TOKEN_OCTOBER = 10;
    private const int DATE_TOKEN_NOVEMBER = 11;
    private const int DATE_TOKEN_DECEMBER = 12;
    private const int DATE_TOKEN_LAST_MONTH = 13;
    private const int DATE_TOKEN_SUNDAY = 0;
    private const int DATE_TOKEN_MONDAY = 1;
    private const int DATE_TOKEN_TUESDAY = 2;
    private const int DATE_TOKEN_WEDNESDAY = 3;
    private const int DATE_TOKEN_THURSDAY = 4;
    private const int DATE_TOKEN_FRIDAY = 5;
    private const int DATE_TOKEN_SATURDAY = 6;
    private const int DATE_TOKEN_LAST_DAY = 7;
    private const int DATE_TOKEN_GMT = -1000;
    private const int DATE_TOKEN_LAST = -1000;
    private const int DATE_TOKEN_ERROR = -999;

    private static char MakeUpper(char c)
    {
      return char.ToUpper(c, CultureInfo.InvariantCulture);
    }

    private static int MapDayMonthToDword(char[] lpszDay, int index)
    {
      switch (HttpDateParse.MakeUpper(lpszDay[index]))
      {
        case 'A':
          switch (HttpDateParse.MakeUpper(lpszDay[index + 1]))
          {
            case 'P':
              return 4;
            case 'U':
              return 8;
            default:
              return -999;
          }
        case 'D':
          return 12;
        case 'F':
          switch (HttpDateParse.MakeUpper(lpszDay[index + 1]))
          {
            case 'E':
              return 2;
            case 'R':
              return 5;
            default:
              return -999;
          }
        case 'G':
          return -1000;
        case 'J':
          switch (HttpDateParse.MakeUpper(lpszDay[index + 1]))
          {
            case 'A':
              return 1;
            case 'U':
              switch (HttpDateParse.MakeUpper(lpszDay[index + 2]))
              {
                case 'L':
                  return 7;
                case 'N':
                  return 6;
              }
#if FEATURE_CORECLR
            throw new NotImplementedException("Cannot fall through switch case statement");
#endif
          }
          return -999;
        case 'M':
          switch (HttpDateParse.MakeUpper(lpszDay[index + 1]))
          {
            case 'A':
              switch (HttpDateParse.MakeUpper(lpszDay[index + 2]))
              {
                case 'R':
                  return 3;
                case 'Y':
                  return 5;
              }
#if FEATURE_CORECLR
              throw new NotImplementedException("Cannot fall through switch case statement");
#endif
            case 'O':
              return 1;
          }
          return -999;
        case 'N':
          return 11;
        case 'O':
          return 10;
        case 'S':
          switch (HttpDateParse.MakeUpper(lpszDay[index + 1]))
          {
            case 'A':
              return 6;
            case 'E':
              return 9;
            case 'U':
              return 0;
            default:
              return -999;
          }
        case 'T':
          switch (HttpDateParse.MakeUpper(lpszDay[index + 1]))
          {
            case 'H':
              return 4;
            case 'U':
              return 2;
            default:
              return -999;
          }
        case 'U':
          return -1000;
        case 'W':
          return 3;
        default:
          return -999;
      }
    }

    internal static bool ParseHttpDate(string DateString, out DateTime dtOut)
    {
      int index1 = 0;
      int index2 = 0;
      int num1 = -1;
      bool flag1 = false;
      int[] numArray = new int[8];
      bool flag2 = true;
      char[] charArray = DateString.ToCharArray();
      dtOut = new DateTime();
      while (index1 < DateString.Length && index2 < 8)
      {
        if ((int) charArray[index1] >= 48 && (int) charArray[index1] <= 57)
        {
          numArray[index2] = 0;
          do
          {
            numArray[index2] *= 10;
            numArray[index2] += (int) charArray[index1] - 48;
            ++index1;
          }
          while (index1 < DateString.Length && (int) charArray[index1] >= 48 && (int) charArray[index1] <= 57);
          ++index2;
        }
        else if ((int) charArray[index1] >= 65 && (int) charArray[index1] <= 90 || (int) charArray[index1] >= 97 && (int) charArray[index1] <= 122)
        {
          numArray[index2] = HttpDateParse.MapDayMonthToDword(charArray, index1);
          num1 = index2;
          if (numArray[index2] == -999 && (!flag1 || index2 != 6))
          {
            flag2 = false;
            goto label_26;
          }
          else
          {
            if (index2 == 1)
              flag1 = true;
            do
            {
              ++index1;
            }
            while (index1 < DateString.Length && ((int) charArray[index1] >= 65 && (int) charArray[index1] <= 90 || (int) charArray[index1] >= 97 && (int) charArray[index1] <= 122));
            ++index2;
          }
        }
        else
          ++index1;
      }
      int millisecond = 0;
      int day;
      int month;
      int hour;
      int minute;
      int second;
      int year;
      if (flag1)
      {
        day = numArray[2];
        month = numArray[1];
        hour = numArray[3];
        minute = numArray[4];
        second = numArray[5];
        year = num1 == 6 ? numArray[7] : numArray[6];
      }
      else
      {
        day = numArray[1];
        month = numArray[2];
        year = numArray[3];
        hour = numArray[4];
        minute = numArray[5];
        second = numArray[6];
      }
      if (year < 100)
        year += year < 80 ? 2000 : 1900;
      if (index2 < 4 || day > 31 || (hour > 23 || minute > 59) || second > 59)
      {
        flag2 = false;
      }
      else
      {
        dtOut = new DateTime(year, month, day, hour, minute, second, millisecond);
        if (num1 == 6)
          dtOut = dtOut.ToUniversalTime();
        if (index2 > 7 && numArray[7] != -1000)
        {
          double num2 = (double) numArray[7];
          dtOut.AddHours(num2);
        }
        dtOut = dtOut.ToLocalTime();
      }
label_26:
      return flag2;
    }
  }
}
