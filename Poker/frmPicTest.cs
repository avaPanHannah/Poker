using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Poker
{
    public partial class frmPicTest : Form
    {
        /// <summary>
        /// 建構子
        /// </summary>
        public frmPicTest()
        {
            InitializeComponent();
        }

        #region 自定義方法
        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        private Image GetImage(int num)
        {
            return GetImage($"pic{num}");
        }


        #endregion


        #region 事件處理程序

        /// <summary>
        /// 當按下換牌按鈕時，隨機產生一個1~52的數字，並將對應的圖片顯示在picTest上，數字也顯示在lblNum上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            // Create a random number generator
            Random rand = new Random();

            // Generate a random number between 1 and 52 (inclusive)
            int r = rand.Next(1, 53);

            //this.picTest.Image = Properties.Resources.ResourceManager.GetObject($"pic{r}") as Image;
            //this.picTest.Image = GetImage("back");
            this.picTest.Image = GetImage(r);

            //this.lblNum.Text = r + "";
            //this.lblNum.Text =  r.ToString();
            this.lblNum.Text = $"{r}";

        }

        /// <summary>
        /// 當按下 picTest 時，將 picTest 的圖片換成 back 的圖片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picTest_Click(object sender, EventArgs e)
        {
            this.picTest.Image = GetImage("back");
        }


        #endregion


    }
}
