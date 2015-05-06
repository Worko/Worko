using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WorkoProject.Utils
{
    public class HebCalendar
    {
        public static Dictionary<string, string> days;
        public static Dictionary<string, string> holidays;
        public static string[][] csvArray = null;

        public static void SetCalendar(string calendarPath)
        {

            days = new Dictionary<string, string>();
            holidays = new Dictionary<string, string>();

            var contents = File.ReadAllText(calendarPath).Split('\n');
            var csv = from line in contents
                      select line.Split(',').ToArray();
            int headerRows = 5;
            csvArray = csv.Skip(headerRows).TakeWhile(r => r.Length > 1 && r.Last().Trim().Length > 0).ToArray();

            days.Add("Sunday", "ראשון");
            days.Add("Monday", "שני");
            days.Add("Tuesday", "שלישי");
            days.Add("Wednesday", "רביעי");
            days.Add("Thursday", "חמישי");
            days.Add("Friday", "שישי");
            days.Add("Saturday", "שבת");

            holidays.Add("Asara B'Tevet", "עשרה בטבת");
            holidays.Add("Rosh Chodesh Sh'vat", "ראש חודש שבט");
            holidays.Add("Tu BiShvat", "ט\"ו בשבט");
            holidays.Add("Shabbat Shekalim", "שבת שקלים");
            holidays.Add("Rosh Chodesh Adar", "ראש חודש אדר");
            holidays.Add("Rosh Chodesh Adar I", "ראש חודש אדר א׳");
            holidays.Add("Rosh Chodesh Adar II", "ראש חודש אדר ב׳");
            holidays.Add("Shabbat Zachor", "שבת זכור");
            holidays.Add("Ta'anit Esther", "תענית אסתר");
            holidays.Add("Erev Purim", "ערב פורים");
            holidays.Add("Purim", "פורים");
            holidays.Add("Purim Katan", "פורים קטן");
            holidays.Add("Shushan Purim", "שושן פורים");
            holidays.Add("Shabbat Parah", "שבת פרה");
            holidays.Add("Rosh Chodesh Nisan", "ראש חודש ניסן");
            holidays.Add("Shabbat HaChodesh", "שבת הקודש");
            holidays.Add("Shabbat HaGadol", "שבת הגדול");
            holidays.Add("Ta'anit Bechorot", "תענית בכורות");
            holidays.Add("Erev Pesach", "ערב פסח");
            holidays.Add("Pesach I", "פסח יום א׳");
            holidays.Add("Pesach II", "פסח יום ב׳");
            holidays.Add("Pesach III (CH''M)", "פסח יום ג׳ (חל המועד)");
            holidays.Add("Pesach IV (CH''M)", "פסח יום ד׳ (חל המועד)");
            holidays.Add("Pesach V (CH''M)", "פסח יום ה׳ (חל המועד)");
            holidays.Add("Pesach VI (CH''M)", "פסח יום ו׳ (חל המועד)");
            holidays.Add("Pesach VII", "פסח יום ז׳");
            holidays.Add("Pesach VIII", "פסח יום ח׳");
            holidays.Add("Yom HaShoah", "יום השואה");
            holidays.Add("Rosh Chodesh Iyyar", "ראש חודש אייר");
            holidays.Add("Yom HaZikaron", "יום הזכרון");
            holidays.Add("Yom HaAtzma'ut", "יום העצמאות");
            holidays.Add("Pesach Sheni", "פסח שני");
            holidays.Add("Lag B'Omer", "ל\"ג בעומר");
            holidays.Add("Yom Yerushalayim", "יום ירושלים");
            holidays.Add("Rosh Chodesh Sivan", "ראש חודש סיון");
            holidays.Add("Erev Shavuot", "ערב שבועות");
            holidays.Add("Shavuot I", "שבועות יום א׳");
            holidays.Add("Shavuot II", "שבועות יום ב׳");
            holidays.Add("Rosh Chodesh Tamuz", "ראש חודש תמוז");
            holidays.Add("Tzom Tammuz", "צום תמוז");
            holidays.Add("Rosh Chodesh Av", "ראש חודש אב");
            holidays.Add("Shabbat Chazon", "שבת חזון");
            holidays.Add("Erev Tish'a B'Av", "ערב תשעה באב");
            holidays.Add("Tish'a B'Av", "תשעה באב");
            holidays.Add("Shabbat Nachamu", "שבת נחמו");
            holidays.Add("Rosh Chodesh Elul", "ראש חודש אלול");
            holidays.Add("Leil Selichot", "ליל סליחות");
            holidays.Add("Erev Rosh Hashana", "ערב ראש השנה");
            holidays.Add("Rosh Hashana 5776", "ראש השנה");
            holidays.Add("Rosh Hashana II", "ראש השנה");
            holidays.Add("Tzom Gedaliah", "צום גדליה");
            holidays.Add("Shabbat Shuva", "שבת שובה");
            holidays.Add("Erev Yom Kippur", "ערב יום כיפור");
            holidays.Add("Yom Kippur", "יום כיפור");
            holidays.Add("Erev Sukkot", "ערב סוכות");
            holidays.Add("Sukkot I", "סוכות יום א׳");
            holidays.Add("Sukkot II", "סוכות יום ב׳");
            holidays.Add("Sukkot III (CH''M)", "סוכות יום ג׳ (חל המועד)");
            holidays.Add("Sukkot IV (CH''M)", "סוכות יום ד׳ (חל המועד)");
            holidays.Add("Sukkot V (CH''M)", "סוכות יום ה׳ (חל המועד)");
            holidays.Add("Sukkot VI (CH''M)", "סוכות יום ו׳ (חל המועד)");
            holidays.Add("Sukkot VII (Hoshana Raba)", "סוכות יום ז׳ (הושנא רבה)");
            holidays.Add("Shmini Atzeret", "שמיני עצרת");
            holidays.Add("Simchat Torah", "שמחת תורה");
            holidays.Add("Rosh Chodesh Cheshvan", "ראש חודש חשון");
            holidays.Add("Rosh Chodesh Kislev", "ראש חודש כסלו");
            holidays.Add("Chanukah: 1 Candle", "חנוכה נר ראשון");
            holidays.Add("Chanukah: 2 Candles", "חנוכה נר שני");
            holidays.Add("Chanukah: 3 Candles", "חנוכה נר שלישי");
            holidays.Add("Chanukah: 4 Candles", "חנוכה נר רביעי");
            holidays.Add("Chanukah: 5 Candles", "חנוכה נר חמישי");
            holidays.Add("Chanukah: 6 Candles", "חנוכה נר שישי");
            holidays.Add("Chanukah: 7 Candles", "חנוכה נר שביעי");
            holidays.Add("Chanukah: 8 Candles", "חנוכה נר שמיני");
            holidays.Add("Chanukah: 8th Day", "חנוכה יום שמיני");
            holidays.Add("Rosh Chodesh Tevet", "ראש חודש טבת");
        }


    }
}