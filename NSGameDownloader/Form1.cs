using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ExcelDataReader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSGameDownloader.Properties;
using SQLite;

namespace NSGameDownloader
{
    //todo 增加在外部浏览器打开的功能
    //todo 尝试进行从百度云直接得到真实下载地址 
    public partial class Form1 : Form
    {
        private SQLiteConnection db;
        private readonly string ConfigPath = "config.json";
        private readonly string CookiePath = "cookie.json";
        private readonly string ExcelPath = "db.xlsx";
        private string _curTid;

        // 只显示中文游戏
        private bool _onlyShowCn;

        // 显示已下载
        private bool _showDownloaded;

        /**
         * panUrl: 百度盘地址
         * nutDbUrl: nutDbUrl
         */
        private JObject _config;
        private List<GameInfo> _gameLists = new List<GameInfo>();
        private GameListViewItemComparer listSorter;
        private List<String> _localGames = new List<String>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            btnDownload.Enabled = false;

            if (File.Exists(ConfigPath))
            {
                _config = JObject.Parse(File.ReadAllText(ConfigPath));
            }
            else
            {
                MessageBox.Show("无法访问config.json配置文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_config["localGameDir"].ToString().Length > 0 && Directory.Exists(_config["localGameDir"].ToString()))
            {
                check_box_download.Visible = true;
            }


            listSorter = new GameListViewItemComparer();

            if (!Directory.Exists("image")) Directory.CreateDirectory("image");
            //使用winapi 做占位符
            SendMessage(textBox_keyword.Handle, 0x1501, 0, "在这里输入 id,中文名,英文名 关键字...");

            var tl = new Thread(ThreadLoad);
            tl.Start();
            var t2 = new Thread(InitDbThread);
            t2.Start();
        }

        /// <summary>
        ///     线程初始,不会占用启动时间
        /// </summary>
        private void ThreadLoad()
        {

            if (File.Exists(CookiePath))
                _cookies = JObject.Parse(File.ReadAllText(CookiePath));
            else
                _cookies = new JObject();

            LoadLocalGames();
        }

        private void InitDbThread() {
            db = new SQLiteConnection("gamedb.db");
            CreateTableResult result = db.CreateTable<GameInfo>();
            Console.WriteLine("Create Table Result " + result);
            if (result == CreateTableResult.Created)
            {
                // 第一次 更新数据
                UpdateTitleKey();
            }
            else
            {
                _gameLists = db.Table<GameInfo>().OrderBy(x => x.tid).ToList<GameInfo>();
                SearchGameName();
            }
        }

        private void LoadLocalGames()
        {
            try
            {
                Console.WriteLine("load local games ......");
                DirectoryInfo dir = new DirectoryInfo(_config["localGameDir"].ToString());
                DirectoryInfo[] directories = dir.GetDirectories();
                foreach (DirectoryInfo d in directories)
                {
                    if (d.Name.Length == 5 && d.Name.StartsWith("01"))
                    {
                        DirectoryInfo[] subDirectories = d.GetDirectories();
                        foreach (DirectoryInfo subD in subDirectories)
                        {
                            //01009A700A538000
                            if (subD.Name.Length == 16)
                            {
                                _localGames.Add(subD.Name);
                            }
                        }
                    }
                }

                Console.WriteLine("load local games count:" + _localGames.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine("load local games error:" + e);
                return;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
            WriteCookieFile();
        }

        private JObject _cookies = new JObject();

        private void WriteCookieFile()
        {
            Console.WriteLine("save cookie to file");
            File.WriteAllText(CookiePath, JsonConvert.SerializeObject(_cookies));
        }

        private string GetBaseTid(string id)
        {
            if (id.EndsWith("000"))
            {
                return id;
            }
            else if (id.EndsWith("800"))
            {
                return id.Substring(0, id.Length - 3) + "000";
            }
            else
            {
                long val = long.Parse(id.Substring(0, id.Length - 3), System.Globalization.NumberStyles.HexNumber);
                val = val - 1;

                //Console.WriteLine("dlc:" + id + " base:" + val.ToString("X") + "000");

                return val.ToString("X") + "000";
            }
        }

        public void UpdateTitleKey()
        {
            Invoke(new Action(() =>
            {
                toolStripProgressBar_download.Visible = true;
                toolStripProgressBar_download.Maximum = 5;
                toolStripProgressBar_download.Value = 1;
                button_search.Text = "下载中";
                button_search.Enabled = false;
                checkbox_cn.Enabled = false;
                check_box_download.Enabled = false;
                textBox_keyword.Enabled = false;
                btnDownload.Enabled = false;
                label_progress.Visible = true;
            }));

            try
            {
                Invoke(new Action(() =>
                {
                    toolStripProgressBar_download.Value = 2;
                    label_progress.Text = "更新nutdb...";
                }));

                //ReadNutDb();
                if (!UpdateNutTileDb("US.en"))
                {
                    Invoke(new Action(() =>
                    {
                        toolStripProgressBar_download.Visible = false;
                        label_progress.Visible = false;

                        MessageBox.Show("更新nutdb出错", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                    return;
                }

                if (!UpdateNutTileDb("JP.ja"))
                {
                    Console.WriteLine("更新日区失败...");
                }

                if (!UpdateNutTileDb("HK.zh"))
                {
                    Console.WriteLine("更新港区失败...");
                }

                Invoke(new Action(() =>
                {
                    toolStripProgressBar_download.Value = 5;
                    label_progress.Text = "更新完成";
                }));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "更新titleid出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _gameLists = db.Table<GameInfo>().OrderBy(x => x.tid).ToList<GameInfo>();

            Invoke(new Action(() =>
            {
                button_search.Text = "搜索";
                button_search.Enabled = true;
                checkbox_cn.Enabled = true;
                check_box_download.Enabled = true;
                textBox_keyword.Enabled = true;
                toolStripProgressBar_download.Visible = false;
                label_progress.Visible = false;
                SearchGameName();
            }));
        }

        // https://github.com/blawar/nut/tree/master/titledb
        private bool UpdateNutTileDb(string region)
        {
            var http = new WebClient { Encoding = Encoding.UTF8 };
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            String htmlJson;
            try
            {
                String url = "https://raw.githubusercontent.com/blawar/nut/master/titledb/" + region + ".json";
                Console.WriteLine("start download " + url);
                //http.DownloadFile(url, region + ".json");
                htmlJson = http.DownloadString("https://raw.githubusercontent.com/blawar/nut/master/titledb/US.en.json");
                Console.WriteLine("download nuttiledb success ...");
            }
            catch
            {
                MessageBox.Show("无法访问加载tiledb.请检查网络", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Invoke(new Action(() =>
            {
                toolStripProgressBar_download.Value = 3;
                label_progress.Text = "解析tiledb...";
            }));


            Dictionary<string, JObject> dics;
            try
            {
                // read file into a string and deserialize JSON to a type
                // htmlJson = File.ReadAllText(region + ".json");
                Console.WriteLine("start parse " + region + " nuttiledb ...");
                dics = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(htmlJson);
                Console.WriteLine("parse nuttiledb success!");
            }
            catch
            {
                MessageBox.Show("tiledb解析错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            foreach (KeyValuePair<string, JObject> kvp in dics)
            {
                var tid = kvp.Value["id"];
                if (tid == null) continue;
                var tidStr = tid.ToString();
                if (tidStr.Length != 16) continue;
                var isDemo = kvp.Value["isDemo"] == null ? true : kvp.Value["isDemo"].ToObject<bool>();
                if (isDemo) continue;

                var isUpd = tidStr.EndsWith("800");
                var isBase = tidStr.EndsWith("000");

                var g = new GameInfo();

                if (isBase)
                {

                    var res = db.Execute("REPLACE INTO GameInfo " +
                        "(tid, name, version, releaseDate, category, publisher, iconUrl, bannerUrl, intro, description, languages, size) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                        tidStr, kvp.Value["name"].ToString(), kvp.Value["version"].ToString(),
                        kvp.Value["releaseDate"].ToString(), kvp.Value["category"].ToString(), kvp.Value["publisher"].ToString(),
                        kvp.Value["iconUrl"].ToString(), kvp.Value["bannerUrl"].ToString(), kvp.Value["intro"].ToString(),
                        kvp.Value["description"].ToString(), kvp.Value["languages"].ToString(), kvp.Value["size"].ToObject<long>());
                }
                else
                {

                    // UPD OR DLC
                    var baseTid = GetBaseTid(tidStr);
                    //Console.WriteLine("dlc " + tidStr);
                }
            }

            return true;
        }

        // 从excel导入中文名
        private void UpdateExcel() {
            if (!File.Exists(ExcelPath))
            {
                Invoke(new Action(() => {
                    MessageBox.Show("无法访问db.xlsx,请将db.xlsx放在程序根目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));

                return;
            }

            Invoke(new Action(() =>
            {
                toolStripProgressBar_download.Visible = true;
                toolStripProgressBar_download.Maximum = 2;
                toolStripProgressBar_download.Value = 1;
                button_search.Text = "加载中";
                button_search.Enabled = false;
                checkbox_cn.Enabled = false;
                check_box_download.Enabled = false;
                textBox_keyword.Enabled = false;
                btnDownload.Enabled = false;
                label_progress.Visible = true;
            }));


            Console.WriteLine("start read excel ...");
            var fs = File.Open(ExcelPath, FileMode.Open, FileAccess.Read);
            var er = ExcelReaderFactory.CreateReader(fs);
            var titleDataSet = er.AsDataSet(new ExcelDataSetConfiguration
            {
                UseColumnDataType = true,
                ConfigureDataTable = r => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            //读出第一个表 
            //0         1       2       3       4       5       6
            //tid       iszh    cname   oname   ext     ver     havedlc
            var dt = titleDataSet.Tables[0];

            Invoke(new Action(() =>
            {
                toolStripProgressBar_download.Value = 1;
                label_progress.Text = "解析excel...";
            }));

            foreach (DataRow row in dt.Rows)
            {
                var tid = row[0].ToString().Trim();
                var iszh = row[1].ToString() != "×";
                var cname = row[2].ToString();
                var allnames = row[3].ToString();

                var res = db.Execute("UPDATE GameInfo set cnName = ? , version = ?, haveCn = ? WHERE tid = ?",
                        cname, row[5].ToString(), iszh, tid);
            }

            Console.WriteLine("read excel success!");
            fs.Close();

            _gameLists = db.Table<GameInfo>().OrderBy(x => x.tid).ToList<GameInfo>();

            Invoke(new Action(() =>
            {
                button_search.Text = "搜索";
                button_search.Enabled = true;
                checkbox_cn.Enabled = true;
                check_box_download.Enabled = true;
                textBox_keyword.Enabled = true;
                toolStripProgressBar_download.Visible = false;
                label_progress.Visible = false;
                SearchGameName(textBox_keyword.Text);
            }));
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            //var keys = Titlekeys.Root.Where(x => x.Contains(textBox_keyword.Text.Trim()));
            Console.WriteLine("start serach:" + textBox_keyword.Text);
            SearchGameName(textBox_keyword.Text);
        }

        private void SearchGameName(string keywords = "")
        {
            Invoke(new Action(() =>
            {
                listView1.Items.Clear();
                listView1.BeginUpdate();

                if (_showDownloaded) // 搜索本地已下载
                {
                    foreach (var tid in _localGames)
                    {
                        var game = _gameLists.Find(x => x.tid == tid);
                        //全文件查找
                        string allstr = tid;
                        if (game != null)
                        {
                            allstr = allstr + (game.cnName != null ? "#" + game.cnName : "") + (game.name != null ? "#" + game.name : "");
                        }

                        if (_onlyShowCn && (game != null && !game.haveCn))
                        {
                            continue;
                        }

                        if (allstr.ToLower().Contains(keywords.Trim().ToLower()))
                        {
                            if (game == null)
                            {
                                listView1.Items.Add(new ListViewItem(new[]
                                {
                                    tid,tid,"","",""
                                }));
                            }
                            else
                            {
                                listView1.Items.Add(new ListViewItem(new[]
                                {
                                    game.tid,
                                    game.cnName != null ? game.cnName : game.name,
                                    game.haveCn ? "●" : "",
                                    game.publisher != null ? game.publisher : "",
                                    game.releaseDate != null ? game.releaseDate : ""
                                }));
                            }
                        }
                    }
                }
                else // 搜索全部游戏
                {
                    foreach (var game in _gameLists)
                    {
                        //全文件查找
                        var allstr = game.tid + (game.cnName != null ? "#" + game.cnName : "") + (game.name != null ? "#" + game.name : "");
                        
                        if (_onlyShowCn && !game.haveCn)
                        {
                            continue;
                        }

                        if (allstr.ToLower().Contains(keywords.Trim().ToLower()))
                        {
                            listView1.Items.Add(new ListViewItem(new[]
                            {
                                game.tid,
                                game.cnName != null ? game.cnName : game.name,
                                game.haveCn ? "●" : "",
                                game.publisher != null ? game.publisher : "",
                                game.releaseDate != null ? game.releaseDate : ""
                            }));
                        }

                    }

                    // 补全主动搜索
                    // 01006C900CC60000
                    if (listView1.Items.Count == 0 && keywords.Length == 16 && (keywords.EndsWith("000") || keywords.EndsWith("800")))
                    {
                        // change update to base
                        String newKeyword = keywords.Substring(0, 13) + "000";
                        listView1.Items.Add(new ListViewItem(new[]
                        {
                            newKeyword,
                            keywords, //防止没有中文名
                            "",
                            "",
                            ""
                        }));
                    }
                }


                listView1.EndUpdate();
                label_count.Text = "总数：" + listView1.Items.Count;

                if (listView1.Items.Count > 0)
                {
                    listView1.SelectedItems.Clear();
                    listView1.Items[0].Selected = true;//设定选中
                    listView1.Items[0].Focused = true;
                    listView1.Select();
                }
            }));
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                btnDownload.Enabled = false;
                return;
            }
            else
            {
                btnDownload.Enabled = true;
            }

            _curTid = listView1.SelectedItems[0].Text;
            if (_config["localGameDir"].ToString().Length > 0)
            {
                localDirLabel.Visible = true;
                var dir = _config["localGameDir"].ToString() + "\\" + _curTid.Substring(0, 5) + "\\" + _curTid + "\\";
                if (Directory.Exists(dir))
                {
                    localDirLabel.Text = dir;
                    localFileListbox.Visible = false;

                    InitLocalListbox(dir);
                }
                else
                {
                    localDirLabel.Text = "创建目录";
                    localFileListbox.Visible = false;
                }
            }

            var game = _gameLists.Find(x => x.tid == _curTid);
            if (game == null)
            {
                info_label_name.Text = _curTid;
                info_label_publisher.Text = "发行商：--";
                label_info_size.Text = $"大小：--";
                label_info_support_lan.Text = $"支持语言：--";
                label_info_type.Text = "类型：--";
                label_info.Text = "";
                pictureBox_gameicon.Image = Resources.error;
            }
            else
            {
                info_label_name.Text = game.cnName != null ? game.cnName.Trim() : game.name;
                info_label_publisher.Text = "发行商：" + game.publisher;
                label_info_size.Text = $"大小：{ConvertBytes(game.size)}";
                label_info_support_lan.Text = $"支持语言：{game.languages}";
                label_info_type.Text = "类型：" + game.category;
                label_info.Text = $"{game.description}";

                if (game.iconUrl != null && game.iconUrl.Length > 0)
                {
                    GetGameImage(_curTid, game.iconUrl, game.bannerUrl);
                }
                else
                {
                    pictureBox_gameicon.Image = Resources.error;
                }
            }
        }

        private void InitLocalListbox(string d)
        {
            DirectoryInfo dir = new DirectoryInfo(d);
            if (!dir.Exists) { return; }
            FileInfo[] files = dir.GetFiles();
            localFileListbox.Items.Clear();

            foreach (FileInfo f in files)
            {
                if (f.Name.EndsWith(".nsp") || f.Name.EndsWith(".xci")) {
                    localFileListbox.Items.Add(f);
                }
            }

            if (localFileListbox.Items.Count > 0)
            {
                localFileListbox.Visible = true;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);

            var tid = listView1.SelectedItems[0].Text;
            Console.WriteLine("tid: " + tid);

            string strText = lstcol.Text;
            try
            {
                Clipboard.SetDataObject(strText);
                string info = string.Format("内容【{0}】已经复制到剪贴板", strText);
                Console.WriteLine(info);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var game = _gameLists.Find(x => x.tid == tid);
            if (game == null) return;

            if (strText == game.name || strText == game.cnName)
            {
                string promptValue = PromptDialog.ShowDialog("修改中文名", strText).Trim();
                if (promptValue.Length > 0 && promptValue != _gameLists[lstrow.Index].cnName)
                {
                    var res = db.Execute("UPDATE GameInfo set cnName = ? WHERE tid = ?", promptValue, tid);
                    if (res > 0)
                    {
                        Console.WriteLine("update name to " + promptValue);
                        lstcol.Text = promptValue;
                        game.cnName = promptValue;
                    }
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Console.WriteLine("ColumnClick:" + e.Column);
            // 检查点击的列是不是现在的排序列.
            if (e.Column == listSorter.sortedColumn)
            {
                // 重新设置此列的排序方法.
                if (listSorter.sortOrder == SortOrder.Ascending)
                {
                    listSorter.sortOrder = SortOrder.Descending;
                }
                else
                {
                    listSorter.sortOrder = SortOrder.Ascending;
                }
            }
            else
            {
                // 设置排序列，默认为正向排序
                listSorter.sortedColumn = e.Column;
                listSorter.sortOrder = SortOrder.Ascending;
            }

            listView1.ListViewItemSorter = listSorter;
            // 用新的排序方法对ListView排序
            listView1.Sort();
        }

        private void GetGameImage(String tid, String url, String bannerUrl)
        {
            var filename = "image\\" + tid + ".jpg";
            if (File.Exists(filename))
            {
                try
                {
                    pictureBox_gameicon.Image = Image.FromFile(filename);
                }
                catch
                {
                    File.Delete(filename);
                }
            }

            if (!File.Exists(filename))
            {
                var web = new WebClient { Encoding = Encoding.UTF8 };
                // 解决WebClient不能通过https下载内容问题
                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                try
                {
                    web.DownloadFileCompleted += Icon_DownloadFileCompleted;
                    web.DownloadFileAsync(new Uri(url), filename, tid);
                    //web.DownloadFile(url, filename);
                    Invoke(new Action(() =>
                    {
                        pictureBox_gameicon.Image = Resources.load;
                    }));
                }
                catch
                {
                    Invoke(new Action(() =>
                    {
                        if (tid == _curTid)
                            pictureBox_gameicon.Image = Resources.error;
                    }));
                }
            }
        }

        void Icon_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.UserState != null)
            {
                string tid = e.UserState.ToString();
                if (tid == _curTid)
                {
                    var filename = "image\\" + tid + ".jpg";
                    pictureBox_gameicon.Image = Image.FromFile(filename);
                }
            }
        }


        /// <summary>
        ///     对文件大小进行转换
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string ConvertBytes(long len)
        {
            if (len > 1073741824)
                return (len / 1073741824.0).ToString("F") + "GB";
            if (len > 1048576)
                return (len / 1048576.0).ToString("F") + "MB";
            return (len / 1024.0).ToString("F") + "KB";
        }

        private void textBox_keyword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true; //防止向上冒泡
            SearchGameName(textBox_keyword.Text);
        }


        private void checkbox_cn_CheckedChanged(object sender, EventArgs e)
        {
            _onlyShowCn = ((CheckBox)sender).Checked;
            textBox_keyword.Text = "";
            SearchGameName();
        }

        private void check_box_download_CheckedChanged(object sender, EventArgs e)
        {
            _showDownloaded = ((CheckBox)sender).Checked;
            textBox_keyword.Text = "";
            SearchGameName();
        }

        private void menu_update_game_Click(object sender, EventArgs e)
        {
            var t = new Thread(UpdateTitleKey);
            t.Start();
        }

        private void UpdateCnExcel_Click(object sender, EventArgs e)
        {
            var t = new Thread(UpdateExcel);
            t.Start();
        }

        private void 查看帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/freedom10086/NSGameDownloader/wiki");
        }

        private void 发送反馈ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/freedom10086/NSGameDownloader/issues");
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 form = new AboutBox1();
            form.Show();
        }

        private void localDirLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_curTid == null) return;
            var dir = _config["localGameDir"].ToString() + "\\" + _curTid.Substring(0, 5) + "\\" + _curTid + "\\";
            if (Directory.Exists(dir))
            {
                Process.Start("Explorer.exe", dir);
            }
            else
            {
                Directory.CreateDirectory(dir);
                Process.Start("Explorer.exe", dir);
                localDirLabel.Text = dir;
            }
        }

        private void ToolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.91wii.com/space-uid-2358313.html");
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ningxiaoxiao/NSGameDownloader");
        }

        private void pictureBox_gameicon_Click(object sender, EventArgs e)
        {
            if (_curTid == null) return;
            var g = _gameLists.Find(x => x.tid == _curTid);
            if (g == null) return;

            // US AU
            Process.Start($"https://ec.nintendo.com/apps/{_curTid}/US");
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(_curTid, info_label_name.Text, _cookies, _config);
            form2.Text = info_label_name.Text;
            form2.Show();
        }

        private void LocalFileListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileInfo f = (FileInfo)localFileListbox.SelectedItem;
            Console.WriteLine(f.FullName);
            ProcessStartInfo info = new ProcessStartInfo("XCI-Explorer\\XCI-Explorer.exe", "\"" + f.FullName + "\"");
            //info.WorkingDirectory = Path.GetDirectoryName("");
            Process.Start(info);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

    }
}