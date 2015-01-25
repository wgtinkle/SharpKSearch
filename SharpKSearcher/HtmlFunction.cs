using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections;

//页面生成
using HtmlAgilityPack;

namespace SharpKSearcher
{
    class HtmlFunction
    {
        public static string GetEncoding(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader reader = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 20000;
                request.AllowAutoRedirect = false;

                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK && response.ContentLength < 1024 * 1024)
                {
                    if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                        reader = new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress));
                    else
                        reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII);

                    string html = reader.ReadToEnd();

                    Regex reg_charset = new Regex(@"charset\b\s*=\s*(?<charset>[^""]*)");
                    if (reg_charset.IsMatch(html))
                    {
                        return reg_charset.Match(html).Groups["charset"].Value;
                    }
                    else if (response.CharacterSet != string.Empty)
                    {
                        return response.CharacterSet;
                    }
                    else
                        return Encoding.Default.BodyName;
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (reader != null)
                    reader.Close();

                if (request != null)
                    request = null;
            }

            return Encoding.Default.BodyName;
        } 
        //转码
        public static string Gbk_To_Unicode(string sText)
        {
            byte[] buffer = Encoding.GetEncoding("GBK").GetBytes(sText);
            buffer = Encoding.Convert(Encoding.GetEncoding("GBK"), Encoding.Unicode, buffer);
            return Encoding.Unicode.GetString(buffer);
        }
        public static string Gb2312_To_Unicode(string sText)
        {
            byte[] buffer = Encoding.GetEncoding("GB2312").GetBytes(sText);
            buffer = Encoding.Convert(Encoding.GetEncoding("GB2312"), Encoding.Unicode, buffer);
            return Encoding.Unicode.GetString(buffer);
        }
        public static string Utf8_To_Unicode(string sText)
        {
            byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(sText);
            buffer = Encoding.Convert(Encoding.GetEncoding("UTF-8"), Encoding.Unicode, buffer);
            return Encoding.Unicode.GetString(buffer);
        }
        //Md5加密
        public static string String_To_Md5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.Unicode.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 

                pwd = pwd + s[i].ToString("X");

            }
            return pwd;
        }
        public static ArrayList GetPageHref(string filepath)
        {
            ArrayList HrefSet = new ArrayList();

            HtmlDocument hdom = new HtmlDocument();
            hdom.Load(filepath, Encoding.Unicode);

            HtmlNode rootNode = hdom.DocumentNode; //根节点
            HtmlNodeCollection hrefnodelist = rootNode.SelectNodes("//a");

            try
            {
                string strExpression = ""; //开始
                strExpression = strExpression + "(^#\\w*)";
                strExpression = strExpression + "|(\\w*@\\w*\\.)";
                strExpression = strExpression + "|(javascript\\w*)";

                foreach (HtmlNode aNode in hrefnodelist)
                {
                    if (aNode.Attributes["href"] != null)
                    {
                        string strA = aNode.Attributes["href"].Value;

                        //过滤掉无效的链接
                        Regex reg_WrongHref = new Regex(@strExpression); 
                        if (reg_WrongHref.IsMatch(strA))
                            continue;
                        //将相对地址补全

                        //有效的链接添加到链表
                        HrefSet.Add(strA);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            
            
            return HrefSet;
        }
    }
}
