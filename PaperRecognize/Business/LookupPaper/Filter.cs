using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperRecognize.Business.LookupPaper
{
    public class Filter
    {
        /// <summary>
        /// 利用过滤规则，过滤掉不可能成功匹配的字符串
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="t">目标字符串</param>
        /// <param name="k">距离阈值</param>
        /// <returns></returns>
        public bool filter_str(string s, string t, int k, string[] s_ary, string[] t_ary)
        {
            if (Math.Abs(s.Length - t.Length) > k) return false;
            
            int num1 = 0, num2 = 0, left = 0, right = 0;
            
            for (int i = 0; i < s_ary.Length && i < t_ary.Length; i++)
            {
                if (s_ary[i] == t_ary[i])
                {
                    num1 += s_ary[i].Length;
                }
                else
                {
                    left = 0; right = 0;
                    for (int j = 0; j < s_ary[i].Length && j < t_ary[i].Length; j++)
                    {
                        if (s_ary[i][j] == t_ary[i][j]) left++;
                    }
                    for (int m = s_ary[i].Length - 1, n = t_ary[i].Length - 1; m >= 0 && n >= 0; m--, n--)
                    {
                        if (s_ary[i][m] == t_ary[i][n]) right++;
                    }
                    num1 += left + right;
                }                
            }
            for (int i = s_ary.Length - 1, j = t_ary.Length - 1; i >= 0 && j >= 0; i--, j--)
            {
                if (s_ary[i] == t_ary[j])
                {
                    num2 += s_ary[i].Length;
                }
                else
                {
                    left = 0; right = 0;
                    for (int p = 0; p < s_ary[i].Length && p < t_ary[j].Length; p++)
                    {
                        if (s_ary[i][p] == t_ary[j][p]) left++;
                    }
                    for (int m = s_ary[i].Length - 1, n = t_ary[j].Length - 1; m >= 0 && n >= 0; m--, n--)
                    {
                        if (s_ary[i][m] == t_ary[j][n]) right++;
                    }
                    num2 += left + right;
                }                
            }
           
             if (Math.Abs(s.Length - num1 - num2) <= k)
             {
                 return true;
             }
             else if ((num1 + num2) <= (s.Length * 0.2))
             {
                 return false;
             }
            
            
            var res = s_ary.Intersect(t_ary, new StrSimilarityComparer()).Distinct().ToArray();

            if (res.Count() >= (s_ary.Length * 0.7))
            {
                return true;
            }
            else 
            { 
                return false;
            }
            
        }
    }

    public class StrSimilarityComparer : IEqualityComparer<string>
    {
        LevenshteinAnalyser analyser = new LevenshteinAnalyser();

        public bool Equals(string s, string t)
        {
            float sim = analyser.GetLikenessValue(s, t);
            return (sim >= 0.8);
        }
        public int GetHashCode(string s)
        {
            return s.GetHashCode();
        }  
    }
}
