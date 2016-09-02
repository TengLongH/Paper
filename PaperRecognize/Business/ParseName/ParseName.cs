using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace PaperRecognize.ParseName
{
    public class NameStyle
    {
        public int asc;
        public int notAsc;

        public int desc;
        public int notDesc;

        public int firstAbbr;
        public int notFirstAbbr;
        public int notSecondAbbr;
        public int secondAbbr;
        public int sureAsc;
        public int sureDesc;

        public void clear()
        {

            asc = -1;
            notAsc = -1;
            desc = -1;
            notDesc = -1;
            firstAbbr = 0;
            secondAbbr = 0;
            notFirstAbbr = 0;
            notSecondAbbr = 0;
            sureAsc = 0;
            sureDesc = 0;

        }
    }
    class Name
    {
        public String pre;
        public String next;

        public static bool asc;
        public static bool preAbbr;
        public static bool nextAbbr;

        public String FirstName
        {
            get
            {
                if (asc)
                    return pre;
                else
                    return next;
            }
            set
            {
                if (asc)
                    pre = value;
                else
                    next = value;
            }
        }
        public String SecondName
        {

            get
            {
                if (asc)
                    return next;
                else
                    return pre;
            }
            set
            {
                if (asc)
                    next = value;
                else
                    pre = value;
            }
        }

        public bool IsFirstAbbr
        {
            get
            {
                if (asc)
                {
                    return preAbbr;
                }
                else
                {
                    return nextAbbr;
                }
            }
        }
        public bool IsSecondAbbr
        {
            get
            {
                if (asc)
                    return nextAbbr;
                else
                    return preAbbr;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("F:");
            builder.Append(FirstName);
            builder.Append(" ");
            builder.Append("S:");
            builder.Append(SecondName);
            builder.Append(" ");
            builder.Append("FAbbr:");
            builder.Append(IsFirstAbbr);
            builder.Append(" ");
            builder.Append("SAbbr:");
            builder.Append(IsSecondAbbr);
            return builder.ToString();
        }
    }

    class ParseName
    {
        //private static readonly log4net.ILog Log = PaperRecognize.Log.LogHelper.GetLogger();
        private NameStyle style;
        private List<String> singleFirstName;
        private List<String> multyFirstName;
        private List<Name> nameObjList;
        private string yinjieReg;

        private string regFile = System.AppDomain.CurrentDomain.BaseDirectory;
            
        private string singleFile =System.AppDomain.CurrentDomain.BaseDirectory;

        private string multyFile = System.AppDomain.CurrentDomain.BaseDirectory;


        public ParseName()
        {
            regFile += @"\Business\ParseName\yinjie.txt";
            singleFile += @"\Business\ParseName\single.txt";
            multyFile += @"\Business\ParseName\multy.txt";
            singleFirstName = new List<string>();
            multyFirstName = new List<string>();

            style = new NameStyle();

            //加载拼音音节的正则表达式
            using (StreamReader reader = new StreamReader(regFile, Encoding.Default))
            {
                yinjieReg = reader.ReadLine();
            }
            //加载汉语单姓拼音
            using (StreamReader reader = new StreamReader(singleFile, Encoding.UTF8))
            {
                String str = null;
                while (null != (str = reader.ReadLine()))
                {
                    singleFirstName.Add(str);
                }
            }
            //加载汉语复姓拼音
            using (StreamReader reader = new StreamReader(multyFile, Encoding.UTF8))
            {
                String str = null;
                while (null != (str = reader.ReadLine()))
                {
                    multyFirstName.Add(str);
                }

            }
        }

        public List<Name> parseName(String strItem, List<Name> list)
        {

            //作者名用；分割
            String[] nameList = strItem.Split(new char[] { ';' });
            if (null == nameList)
            {
                throw new Exception("the format of strItem is unilegal");
            }


            nameObjList = new List<Name>();
            style.clear();

            foreach (String name in nameList)
            {
                try
                {
                    parseSingleName(name.Trim());
                }
                catch (Exception e)
                {
                    //Log.Error(e.Message);
                }

            }

            setNameStyle();
            if (null != list)
            {
                list.Clear();
                list.AddRange(nameObjList);
            }

            return nameObjList;
        }
        private void setNameStyle()
        {
            if (style.firstAbbr > style.notFirstAbbr)
                Name.preAbbr = true;
            else if (style.firstAbbr < style.notFirstAbbr)
                Name.preAbbr = false;
            else
                //Log.Warn("cant confirm preAbbr");

            if (style.secondAbbr > style.notSecondAbbr)
                Name.nextAbbr = true;
            else if (style.secondAbbr < style.notSecondAbbr)
                Name.nextAbbr = false;
            else
                //Log.Warn("cant confirm nextAbbr");

            if (nameObjList.Count <= 0) return;
            if (style.sureAsc > style.sureDesc)
            {
                Name.asc = true;
            }
            else if (style.sureAsc < style.sureDesc)
            {
                Name.asc = false;
            }
            else
            {
                if (style.asc > style.desc)
                    Name.asc = true;
                else if (style.asc < style.desc)
                    Name.asc = false;
                else
                {
                    if (style.notAsc < style.notDesc)
                    {
                        Name.asc = true;
                    }
                    else if (style.notAsc > style.notDesc)
                    {
                        Name.asc = false;
                    }
                    else
                    {
                        //Log.Warn("Asc Desc cant confirm");
                    }
                }

            }

        }

        private void parseSingleName(String name)
        {
            int i = 0;
            String[] arr = new String[1];
            char[] split = new char[] { ',', ' ' };
            while (i < split.Length && arr.Length != 2)
            {
                arr = name.Split(new char[] { split[i] });
                i++;
            }


            if (arr.Length != 2)
            {
                //Log.Warn("name format is false " + name);
            }

            Name nameObj = new Name();
            nameObjList.Add(nameObj);
            nameObj.pre = removeNotLetterChar(arr[0]);
            if (arr.Length > 1)
            {
                nameObj.next = removeNotLetterChar(arr[1]);
            }
            else
            {
                nameObj.next = "";
            }

            if (!isChineseName(nameObj)) return;

            int preFirstName = -1, nxtFirstName = -1;
            bool firstAddr = isAbbr(nameObj.pre);
            bool secondAbbr = isAbbr(nameObj.next);

            if (firstAddr)
                style.firstAbbr++;
            else
                style.notFirstAbbr++;
            if (secondAbbr)
                style.secondAbbr++;
            else
                style.notSecondAbbr++;

            if (!firstAddr)
            {
                if (singleFirstName.Contains(nameObj.pre.ToLower()) || multyFirstName.Contains(nameObj.pre.ToLower()))
                {
                    style.asc++;
                    preFirstName = 1;
                }
                else
                {
                    style.notAsc++;
                    preFirstName = 0;
                }
            }
            if (!secondAbbr)
            {
                if (singleFirstName.Contains(nameObj.next.ToLower()) || multyFirstName.Contains(nameObj.next.ToLower()))
                {
                    style.desc++;
                    nxtFirstName = 1;
                }
                else
                {
                    style.notDesc++;
                    nxtFirstName = 0;
                }
            }
            if (preFirstName == 1 && nxtFirstName == 0)
            {
                style.sureAsc++;
            }
            if (preFirstName == 0 && nxtFirstName == 1)
            {
                style.sureDesc++;
            }
        }
        private bool isAbbr(String str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char c = str.ElementAt(i);
                if (!(c >= 'A' && c <= 'Z')) return false;
            }
            return true;
        }
        private String removeNotLetterChar(String input)
        {
            if (null == input) return null;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input.ElementAt(i);
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        private bool isChineseName(Name name)
        {
            bool pinyin = true;
            if (!isAbbr(name.pre))
            {
                pinyin = isChineseCharPinyin(name.pre.ToLower());
            }
            if (!isAbbr(name.next))
            {
                pinyin = pinyin && isChineseCharPinyin(name.next.ToLower());
            }
            return (pinyin);
        }
        private bool isChineseCharPinyin(String pinyin)
        {
            String input = pinyin.ToLower();
            try
            {
                Regex reg = new Regex(yinjieReg);
                bool match = reg.IsMatch(input);
                if (!match)
                {
                    //Log.Warn("not chinese name: " + input);
                    Console.Write("not china: ");
                    Console.WriteLine(input);
                }
                return match;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
