using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;

namespace SharpKSearcher
{
    class SpilderCrawler
    {
        public delegate void CallBackFinish(string str);
        public class CrawlerPara //线程参数
        {            
            public string sUrl;
            public string sfilepath;
            public CallBackFinish callMothd; //回调方法
        }
        //下载程序
        public void DownLoadWebPage(string sWebUrl, string sWebfilepath)
        {
            Thread downthread = new Thread(DownloadFile);
            downthread.Start(new CrawlerPara { sUrl = sWebUrl, sfilepath = sWebfilepath, callMothd = this.DownLoadOk });
        }
        //回调函数
        public void DownLoadOk(string str)
        {
            string strtemp = str;
        }
        public static void DownloadFile(object o)
        {            
            CrawlerPara cp = (CrawlerPara)o;
            Encoding encoding = null;
            try
            {
                //获取并设置编码方式                
                string Chara = HtmlFunction.GetEncoding(cp.sUrl);
                if (Chara == "GBK" || Chara == "gbk")                    
                    encoding = Encoding.GetEncoding("GBK");
                else if (Chara == "GB2312" || Chara == "gb2312")
                    encoding = Encoding.GetEncoding("gb2312");
                else if (Chara == "UTF-8" || Chara == "utf8" || Chara == "utf-8")
                    encoding = Encoding.GetEncoding("UTF-8");

                //发送请求
                HttpWebRequest mClient = (HttpWebRequest)WebRequest.Create(cp.sUrl);
                HttpWebResponse mResp = (HttpWebResponse)mClient.GetResponse();

                //创建读写器
                StreamReader mReader;
                if (mResp.ContentEncoding != null && mResp.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                    mReader = new StreamReader(new GZipStream(mResp.GetResponseStream(), CompressionMode.Decompress), encoding);
                else
                    mReader = new StreamReader(mResp.GetResponseStream(), encoding);

                //读取数据并写入到文件
                string sline;
                FileStream htmlfile = new FileStream(cp.sfilepath, FileMode.Create);
                StreamWriter sw = new StreamWriter(htmlfile, Encoding.Unicode);
                sline = mReader.ReadLine();
                while (mReader.EndOfStream == false)
                {
                    string strtemp = "";
                    if (Chara == "GBK" || Chara == "gbk")
                        strtemp = HtmlFunction.Gbk_To_Unicode(sline);
                    else if (Chara == "GB2312" || Chara == "gb2312")
                        strtemp = HtmlFunction.Gb2312_To_Unicode(sline);
                    else if (Chara == "UTF-8" || Chara == "utf8" || Chara == "utf-8")
                        strtemp = HtmlFunction.Utf8_To_Unicode(sline);
                    sw.WriteLine(strtemp);
                    sline = mReader.ReadLine();
                }
                mReader.Close();
                sw.Close();
            }
            catch(Exception e){}
            
            //委托调用返回处理结果
            cp.callMothd(cp.sUrl);
        }
        

        
    }


}
