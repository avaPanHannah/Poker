using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poker
{
    public partial class frmPoker : Form
    {
        #region 欄位
        /// <summary>
        /// 用來存放牌桌上五張牌的 PictureBox 陣列
        /// </summary>
        PictureBox[] pic = new PictureBox[5];

        /// <summary>
        /// 所有的牌的編號，從 0 到 51，對應到 52 張牌
        /// </summary>
        int[] allPoker = new int[52];

        /// <summary>
        /// 記錄玩家手牌的編號，從 0 到 51，對應到 52 張牌
        /// </summary>
        int[] playerPoker = new int[5];

        // --- 新增下注相關欄位 ---
        /// <summary>
        /// 玩家總資金
        /// </summary>
        int totalFunds = 1000000;

        /// <summary>
        /// 目前押注金額
        /// </summary>
        int currentBet = 0;
        #endregion

        public frmPoker()
        {
            InitializeComponent();
            InitializePoker();
            InitializeBetting(); // 初始化下注介面狀態
        }

        #region 自定義方法
        private void InitializePoker()
        {
            for (int i = 0; i < pic.Length; i++)
            {
                pic[i] = new PictureBox();
                pic[i].Image = GetImage("back");
                pic[i].Name = "pic" + i;
                pic[i].SizeMode = PictureBoxSizeMode.AutoSize;
                pic[i].Top = 30;
                pic[i].Left = 10 + ((pic[i].Width + 10) * i);
                // 預設牌桌上的牌不可點擊
                pic[i].Enabled = false;
                // 預設牌桌上的牌的 Tag 為 "back"，表示牌面朝下
                pic[i].Tag = "back";
                pic[i].Visible = true;

                // 將 pic 丟至到 grpPorker 內
                this.grpPoker.Controls.Add(pic[i]);

                pic[i].Click += Pic_Click;
            }
        }

        /// <summary>
        /// 初始化下注系統的初始狀態
        /// </summary>
        private void InitializeBetting()
        {
            // 如果您尚未在介面上建立這些控制項，程式可能會報錯。請確保控制項名稱一致。
            txtTotalFunds.Text = totalFunds.ToString();
            txtTotalFunds.ReadOnly = true;
            txtBet.Text = "500";

            // 遊戲一開始必須先下注才能發牌
            btnDealCard.Enabled = false;
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
        }

        /// <summary>
        /// 顯示五張撲克牌到桌面上
        /// </summary>
        private void ShowCards()
        {
            for (int i = 0; i < playerPoker.Length; i++)
            {
                pic[i].Image = this.GetImage($"pic{playerPoker[i] + 1}");
            }
        }

        /// <summary>
        /// 取得圖片資源
        /// </summary>
        private Image GetImage(string name)
        {
            return Properties.Resources.ResourceManager.GetObject(name) as Image;
        }

        /// <summary>
        /// 取得圖片資源
        /// </summary>
        private Image GetImage(int num)
        {
            return GetImage($"pic{num}");
        }

        /// <summary>
        /// 將 allPoker 陣列中的牌隨機打亂，模擬洗牌的過程
        /// </summary>
        private void Shuffle()
        {
            Random rand = new Random();
            for (int i = 0; i < 1000; i++)
            {
                int r = rand.Next(allPoker.Length);
                int temp = allPoker[r];
                allPoker[r] = allPoker[0];
                allPoker[0] = temp;
            }
        }
        #endregion

        #region 事件處理程序

        /// <summary>
        /// 處理押注按鈕點擊事件
        /// </summary>
        private void btnBet_Click(object sender, EventArgs e)
        {
            // 檢查輸入的押注金額是否為有效的正整數
            if (int.TryParse(txtBet.Text, out int betAmount) && betAmount > 0)
            {
                if (betAmount <= totalFunds)
                {
                    currentBet = betAmount;
                    totalFunds -= currentBet; // 扣除押注金
                    txtTotalFunds.Text = totalFunds.ToString();

                    // 更新按鈕狀態
                    btnBet.Enabled = false;
                    txtBet.Enabled = false;
                    btnDealCard.Enabled = true; // 允許發牌
                    lblResult.Text = $"已押注 {currentBet} 元，請點擊「發牌」開始遊戲。";
                }
                else
                {
                    MessageBox.Show("總資金不足，請重新輸入押注金額！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("請輸入有效的押注金額！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Pic_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;

            int index = int.Parse(pic.Name.Replace("pic", ""));
            int cardNum = playerPoker[index] + 1;

            // 如果牌面朝下，則翻開牌面；如果牌面朝上，則翻回背面
            if (pic.Tag.ToString() == "back")
            {
                pic.Tag = "front";
                pic.Image = GetImage(cardNum);
            }
            else
            {
                pic.Tag = "back";
                pic.Image = GetImage("back");
            }
        }

        private async void btnDealCard_Click(object sender, EventArgs e)
        {
            this.lblResult.Text = "請點擊要更換的牌，然後點擊「換牌」";

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Image = GetImage("back");
            }

            for (int i = 0; i < allPoker.Length; i++)
            {
                allPoker[i] = i;
            }

            this.Shuffle();
            await Task.Delay(500);

            for (int i = 0; i < playerPoker.Length; i++)
            {
                playerPoker[i] = allPoker[i];
            }

            this.ShowCards();

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = true;
                pic[i].Tag = "front";
            }

            btnChangeCard.Enabled = true;
            btnDealCard.Enabled = false;
            btnCheck.Enabled = true; // 也可以選擇不換牌直接判斷
        }

        private void btnChangeCard_Click(object sender, EventArgs e)
        {
            int startIndex = 5;

            for (int i = 0; i < playerPoker.Length; i++)
            {
                if (pic[i].Tag.ToString() == "back")
                {
                    playerPoker[i] = allPoker[startIndex];
                    pic[i].Image = GetImage(playerPoker[i] + 1);
                    pic[i].Tag = "front";
                    startIndex++;
                }
            }

            for (int i = 0; i < pic.Length; i++)
            {
                pic[i].Enabled = false;
            }

            this.btnChangeCard.Enabled = false;
            this.btnCheck.Enabled = true;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            string[] colorList = { "梅花", "方塊", "愛心", "黑桃" };
            string[] pointList = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            int[] pokerColor = new int[5];
            int[] pokerPoint = new int[5];

            for (int i = 0; i < playerPoker.Length; i++)
            {
                pokerColor[i] = playerPoker[i] % 4;
                pokerPoint[i] = playerPoker[i] / 4;
            }

            int[] colorCount = new int[4];
            int[] pointCount = new int[13];

            for (int i = 0; i < pokerColor.Length; i++)
            {
                int color = pokerColor[i];
                int point = pokerPoint[i];

                colorCount[color]++;
                pointCount[point]++;
            }

            Array.Sort(colorCount, colorList);
            Array.Reverse(colorCount);
            Array.Reverse(colorList);

            Array.Sort(pointCount, pointList);
            Array.Reverse(pointCount);
            Array.Reverse(pointList);

            bool isFlush = (colorCount[0] == 5);
            bool isSingle = (pointCount[0] == 1 && pointCount[1] == 1 && pointCount[2] == 1 && pointCount[3] == 1 && pointCount[4] == 1);
            bool isDiffFout = (pokerPoint.Max() - pokerPoint.Min() == 4);
            bool isRoyal = pokerPoint.Contains(0) && pokerPoint.Contains(9) && pokerPoint.Contains(10) && pokerPoint.Contains(11) && pokerPoint.Contains(12);
            bool isRoyalisFlush = isFlush && isRoyal;
            bool isStraightFlush = isFlush && isSingle && isDiffFout;
            bool isStraight = isSingle && (isDiffFout || isRoyal);
            bool isFourOfAKind = (pointCount[0] == 4);
            bool isFullHouse = (pointCount[0] == 3 && pointCount[1] == 2);
            bool isThreeOfAKind = (pointCount[0] == 3 && pointCount[1] == 1);
            bool isTwoPair = (pointCount[0] == 2 && pointCount[1] == 2);
            bool isOnePair = (pointCount[0] == 2 && pointCount[1] == 1);

            string result = "";
            int multiplier = 0; // 紀錄賠率

            // 根據作業規定設定賠率
            if (isRoyalisFlush)
            {
                result = $"{colorList[0]} 皇家同花順";
                multiplier = 250; // 皇家同花順賠率 250
            }
            else if (isStraightFlush)
            {
                result = $"{colorList[0]} 同花順";
                multiplier = 50; // 同花順賠率 50
            }
            else if (isFourOfAKind)
            {
                result = $"{pointList[0]} 四條";
                multiplier = 25; // 四條賠率 25
            }
            else if (isFullHouse)
            {
                result = $"{pointList[0]}三張{pointList[1]}兩張 葫蘆";
                multiplier = 9; // 葫蘆賠率 9
            }
            else if (isFlush)
            {
                result = $"{colorList[0]} 同花";
                multiplier = 6; // 同花賠率 6
            }
            else if (isStraight)
            {
                result = "順子";
                multiplier = 4; // 順子賠率 4
            }
            else if (isThreeOfAKind)
            {
                result = $"{pointList[0]} 三條";
                multiplier = 3; // 三條賠率 3
            }
            else if (isTwoPair)
            {
                result = $"{pointList[0]},{pointList[1]} 兩對";
                multiplier = 2; // 兩對賠率 2
            }
            else if (isOnePair)
            {
                result = $"{pointList[0]} 一對";
                multiplier = 1; // 一對賠率 1
            }
            else
            {
                result = "雜牌";
                multiplier = 0; // 沒中獎
            }

            // 計算獎金並更新資金
            int winAmount = currentBet * multiplier;
            totalFunds += winAmount;
            txtTotalFunds.Text = totalFunds.ToString();

            // 顯示結果
            if (multiplier > 0)
            {
                lblResult.Text = $"{result}！贏得 {winAmount} 元！";
            }
            else
            {
                lblResult.Text = $"{result}。未中獎，損失 {currentBet} 元。";
            }

            // 重置 UI 狀態，為下一局做準備
            btnChangeCard.Enabled = false;
            btnCheck.Enabled = false;
            btnBet.Enabled = true;   // 開放重新下注
            txtBet.Enabled = true;
            btnDealCard.Enabled = false;
            currentBet = 0;
        }

        private void frmPoker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.btnDealCard.Enabled == false && this.btnCheck.Enabled == true) // 確保在遊戲中才作弊
            {
                switch (e.KeyChar)
                {
                    case 'q':
                        playerPoker[0] = 51; playerPoker[1] = 47; playerPoker[2] = 43; playerPoker[3] = 39; playerPoker[4] = 3;
                        break;
                    case 'w':
                        playerPoker[0] = 37; playerPoker[1] = 33; playerPoker[2] = 29; playerPoker[3] = 25; playerPoker[4] = 21;
                        break;
                    case 'e':
                        playerPoker[0] = 50; playerPoker[1] = 38; playerPoker[2] = 34; playerPoker[3] = 22; playerPoker[4] = 18;
                        break;
                    case 'r':
                        playerPoker[0] = 48; playerPoker[1] = 39; playerPoker[2] = 38; playerPoker[3] = 37; playerPoker[4] = 36;
                        break;
                    case 't':
                        playerPoker[0] = 30; playerPoker[1] = 29; playerPoker[2] = 6; playerPoker[3] = 5; playerPoker[4] = 4;
                        break;
                    case 'y':
                        playerPoker[0] = 48; playerPoker[1] = 39; playerPoker[2] = 15; playerPoker[3] = 14; playerPoker[4] = 13;
                        break;
                }
                this.ShowCards();
            }
        }
        #endregion

        private void frmPoker_Load(object sender, EventArgs e)
        {

        }

        private void txtBet_TextChanged(object sender, EventArgs e)
        {

        }
    }
}