using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;


namespace SharpKSearcher
{
    public partial class MainWnddlg : Form
    {
        public ArrayList HrefList;
        public MainWnddlg()
        {
            InitializeComponent();

            SpilderCrawler SpC = new SpilderCrawler();
            SpC.DownLoadWebPage("http://zx.jiaju.sina.com.cn/anli/43151.html?utm_source=snls&utm_medium=nrw&utm_campaign=n_9998", "C:\\Users\\wjing\\Desktop\\test.txt");            

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HrefList = HtmlFunction.GetPageHref("C:\\Users\\wjing\\Desktop\\test.txt");

            this.listView1.Columns.Add("Index", 50);   
            this.listView1.Columns.Add("Urllist",800);   
            
            for (int i = 0; i < HrefList.Count; i++)   //添加10行数据
            { 
                ListViewItem Row =new ListViewItem(i.ToString());//这个是第一行第一列
                this.listView1.Items.Add(Row);
                listView1.Items[i].SubItems.Add(HrefList[i].ToString());
            }
            this.listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
        }
    }
}
