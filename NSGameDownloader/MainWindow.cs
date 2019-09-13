﻿using System;
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
    public partial class MainWindow : Form
    {
        private SQLiteConnection db;
        private readonly string ConfigPath = "config.json";
        private readonly string ExcelPath = "db.xlsx";
        private string _curTid;

        // 只显示中文游戏
        private bool _onlyShowCn;

        // 显示已下载
        private bool _showDownloaded;


        private JObject _config;
        private List<GameInfo> _gameLists = new List<GameInfo>();
        private GameListViewItemComparer listSorter;
        private List<String> _localGames = new List<String>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FormLoad(object sender, EventArgs e)
        {
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
                downloadCheckBox.Visible = true;
            }


            listSorter = new GameListViewItemComparer();

            if (!Directory.Exists("image")) Directory.CreateDirectory("image");
            //使用winapi 做占位符
            SendMessage(searchTextBox.Handle, 0x1501, 0, "在这里输入 id,中文名,英文名 关键字...");

            var tl = new Thread(ThreadLoad);
            tl.Start();
            var t2 = new Thread(InitDbThread);
            t2.Start();
        }

        private void FormWindowClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        /// <summary>
        ///     线程初始,不会占用启动时间
        /// </summary>
        private void ThreadLoad()
        {
            LoadLocalGames();
        }

        private void InitDbThread()
        {
            db = new SQLiteConnection("gamedb.db");
            CreateTableResult result = db.CreateTable<GameInfo>();
            Console.WriteLine("Create Table Result " + result);
            if (result == CreateTableResult.Created)
            {
                // first time
                Console.WriteLine("success create db file!");
                UpdateTitleKey();
            }

            _gameLists = db.Table<GameInfo>().OrderBy(x => x.tid).ToList<GameInfo>();
            SearchGameName();
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

        public void UpdateTitleKey()
        {
            Invoke(new Action(() =>
            {
                toolStripProgressBar_download.Visible = true;
                toolStripProgressBar_download.Maximum = 4;
                toolStripProgressBar_download.Value = 1;
                searchButton.Text = "下载中";
                searchButton.Enabled = false;
                chCheckBox.Enabled = false;
                downloadCheckBox.Enabled = false;
                searchTextBox.Enabled = false;
                label_progress.Visible = true;
            }));

            try
            {
                Invoke(new Action(() =>
                {
                    toolStripProgressBar_download.Value = 1;
                    label_progress.Text = "更新日区游戏数据库...";
                }));

                if (!UpdateNutTileDb("JP", "ja"))
                {
                    Invoke(new Action(() =>
                    {
                        toolStripProgressBar_download.Visible = false;
                        label_progress.Visible = false;

                        MessageBox.Show("更新nutdb出错", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                    return;
                }

                Invoke(new Action(() =>
                {
                    toolStripProgressBar_download.Value = 2;
                    label_progress.Text = "更新美区游戏数据库...";
                }));

                if (!UpdateNutTileDb("US", "en"))
                {
                    Console.WriteLine("更新美区失败...");
                }

                Invoke(new Action(() =>
                {
                    toolStripProgressBar_download.Value = 3;
                    label_progress.Text = "更新港区游戏数据库...";
                }));

                if (!UpdateNutTileDb("HK", "zh"))
                {
                    Console.WriteLine("更新港区失败...");
                }

                Invoke(new Action(() =>
                {
                    toolStripProgressBar_download.Value = 4;
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
                searchButton.Text = "搜索";
                searchButton.Enabled = true;
                chCheckBox.Enabled = true;
                downloadCheckBox.Enabled = true;
                searchTextBox.Enabled = true;
                toolStripProgressBar_download.Visible = false;
                label_progress.Visible = false;
                SearchGameName();
            }));
        }

        private void SearchButtonClick(object sender, EventArgs e)
        {
            //var keys = Titlekeys.Root.Where(x => x.Contains(textBox_keyword.Text.Trim()));
            Console.WriteLine("start serach:" + searchTextBox.Text);
            SearchGameName(searchTextBox.Text);
        }

        private void SearchGameName(string keywords = "")
        {
            Invoke(new Action(() =>
            {
                gameList.Items.Clear();
                gameList.BeginUpdate();

                if (_showDownloaded) // 搜索本地已下载
                {
                    foreach (var tid in _localGames)
                    {
                        var game = _gameLists.Find(x => x.tid == tid);
                        //全文件查找
                        string allstr = tid;
                        if (game != null)
                        {
                            allstr = allstr + (game.cnName != null ? "#" + game.cnName : "")
                            + (game.hkName != null ? "#" + game.hkName : "")
                            + (game.usName != null ? "#" + game.usName : "")
                            + (game.jpName != null ? "#" + game.jpName : "");
                        }

                        if (_onlyShowCn && (game != null && !game.haveCn))
                        {
                            continue;
                        }

                        if (allstr.ToLower().Contains(keywords.Trim().ToLower()))
                        {
                            if (game == null)
                            {
                                gameList.Items.Add(new ListViewItem(new[]
                                {
                                    tid,tid,"","","",""
                                }));
                            }
                            else
                            {
                                gameList.Items.Add(new ListViewItem(new[]
                                {
                                    game.tid,
                                    GetGameName(game),
                                    game.haveCn ? "●" : "",
                                    game.publisher != null ? game.publisher : "",
                                    game.releaseDate != null ? game.releaseDate : "",
                                    game.star ? "●" : "",
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
                        var allstr = game.tid + (game.cnName != null ? "#" + game.cnName : "")
                            + (game.hkName != null ? "#" + game.hkName : "")
                            + (game.usName != null ? "#" + game.usName : "")
                            + (game.jpName != null ? "#" + game.jpName : "");

                        if (_onlyShowCn && !game.haveCn)
                        {
                            continue;
                        }

                        if (allstr.ToLower().Contains(keywords.Trim().ToLower()))
                        {
                            gameList.Items.Add(new ListViewItem(new[]
                            {
                                game.tid,
                                GetGameName(game),
                                game.haveCn ? "●" : "",
                                game.publisher != null ? game.publisher : "",
                                game.releaseDate != null ? game.releaseDate : "",
                                game.star ? "●" : "",
                            }));
                        }

                    }

                    // 补全主动搜索
                    // 01006C900CC60000
                    if (gameList.Items.Count == 0 && keywords.Length == 16 && (keywords.EndsWith("000") || keywords.EndsWith("800")))
                    {
                        // change update to base
                        String newKeyword = keywords.Substring(0, 13) + "000";
                        gameList.Items.Add(new ListViewItem(new[]
                        {
                            newKeyword,
                            keywords, //防止没有中文名
                            "", "", "", ""
                        }));
                    }
                }


                gameList.EndUpdate();
                gameCountLabel.Text = "总数：" + gameList.Items.Count;

                if (gameList.Items.Count > 0)
                {
                    gameList.SelectedItems.Clear();
                    gameList.Items[0].Selected = true;//设定选中
                    gameList.Items[0].Focused = true;
                    gameList.Select();
                }
            }));
        }

        private void GameListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (gameList.SelectedItems.Count == 0)
            {
                return;
            }

            _curTid = gameList.SelectedItems[0].Text;
            if (_config["localGameDir"].ToString().Length > 0)
            {
                localDirLabel.Visible = true;
                var dir = _config["localGameDir"].ToString() + "\\" + _curTid.Substring(0, 5) + "\\" + _curTid + "\\";
                if (Directory.Exists(dir))
                {
                    localDirLabel.Text = dir;
                    localGameListbox.Visible = false;

                    InitLocalListbox(dir);
                }
                else
                {
                    localDirLabel.Text = "创建目录";
                    localGameListbox.Visible = false;
                }
            }

            var game = _gameLists.Find(x => x.tid == _curTid);
            if (game == null)
            {
                gameNameLabel.Text = _curTid;
                GamePublisherLabel.Text = "发行商：--";
                gameSizeLabel.Text = $"大小：--";
                gameLansLabel.Text = $"支持语言：--";
                gameTypeLabel.Text = "类型：--";
                gameImageBox.Image = Resources.error;
            }
            else
            {
                gameNameLabel.Text = GetGameName(game);
                GamePublisherLabel.Text = "发行商：" + game.publisher;
                gameSizeLabel.Text = $"大小：{ConvertBytes(game.size)}";
                gameLansLabel.Text = $"支持语言：{game.languages}";
                gameTypeLabel.Text = "类型：" + game.category;

                if (game.iconUrl != null && game.iconUrl.Length > 0)
                {
                    GetGameImage(_curTid, game.iconUrl, game.bannerUrl);
                }
                else
                {
                    gameImageBox.Image = Resources.error;
                }
            }
        }

        private void GameListMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (gameList.FocusedItem.Bounds.Contains(e.Location))
                {
                    var tid = gameList.FocusedItem.Text;
                    var game = _gameLists.Find(x => x.tid == tid);
                    if (game == null) return;

                    MenuItem starMenuItem = new MenuItem(game.star ? "取消收藏" : "收藏");
                    starMenuItem.Click += delegate (object sender2, EventArgs e2) {
                        Console.WriteLine("star " + gameList.FocusedItem.Text);
                        var res = db.Execute("UPDATE GameInfo SET star = ? WHERE tid = ?", game.star ? false : true, tid);
                        if (res > 0)
                        {
                            game.star = !game.star;
                            gameList.FocusedItem.SubItems[5].Text = game.star ? "●" : "";
                            Console.WriteLine("result " + game.star);
                        }
                    };

                    MenuItem editMenuItem = new MenuItem("编辑");
                    editMenuItem.Click += delegate (object sender2, EventArgs e2) {
                        GameListMouseDoubleClick(sender, e);
                    };

                    MenuItem[] mi = new MenuItem[] { starMenuItem, editMenuItem };
                    ContextMenu contextMenu = new ContextMenu(mi);
                    contextMenu.Show(gameList, new Point(e.X, e.Y));
                }
            }
        }

        private void GameListMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listview = (ListView)sender;
            ListViewItem lstrow = listview.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem lstcol = lstrow.GetSubItemAt(e.X, e.Y);

            var tid = gameList.SelectedItems[0].Text;
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
            string promptValue = PromptDialog.ShowDialog("修改中文名", strText).Trim();
            if (promptValue.Length > 0)
            {
                var res = db.Execute("UPDATE GameInfo SET cnName = ? WHERE tid = ?", promptValue, tid);
                if (res > 0)
                {
                    Console.WriteLine("update name to " + promptValue);
                    lstcol.Text = promptValue;
                    if (game != null)
                    {
                        game.cnName = promptValue;
                    }

                }
            }
        }

        private void GameListColumnClick(object sender, ColumnClickEventArgs e)
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

            gameList.ListViewItemSorter = listSorter;
            // 用新的排序方法对ListView排序
            gameList.Sort();
        }

        private void InitLocalListbox(string d)
        {
            DirectoryInfo dir = new DirectoryInfo(d);
            if (!dir.Exists) { return; }
            FileInfo[] files = dir.GetFiles();
            localGameListbox.Items.Clear();

            foreach (FileInfo f in files)
            {
                if (f.Name.EndsWith(".nsp") || f.Name.EndsWith(".xci"))
                {
                    localGameListbox.Items.Add(f);
                }
            }

            if (localGameListbox.Items.Count > 0)
            {
                localGameListbox.Visible = true;
            }
        }

        private void GetGameImage(String tid, String url, String bannerUrl)
        {
            var filename = "image\\" + tid + ".jpg";
            if (File.Exists(filename))
            {
                try
                {
                    gameImageBox.Image = Image.FromFile(filename);
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
                    web.DownloadFileCompleted += IconDownloadFileCompleted;
                    web.DownloadFileAsync(new Uri(url), filename, tid);
                    //web.DownloadFile(url, filename);
                    Invoke(new Action(() =>
                    {
                        gameImageBox.Image = Resources.load;
                    }));
                }
                catch
                {
                    Invoke(new Action(() =>
                    {
                        if (tid == _curTid)
                            gameImageBox.Image = Resources.error;
                    }));
                }
            }
        }

        void IconDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.UserState != null)
            {
                string tid = e.UserState.ToString();
                if (tid == _curTid)
                {
                    var filename = "image\\" + tid + ".jpg";
                    gameImageBox.Image = Image.FromFile(filename);
                }
            }
        }

        // 根据文件id计算出游戏TID
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

        // 对文件大小进行转换
        public static string ConvertBytes(long len)
        {
            if (len > 1073741824)
                return (len / 1073741824.0).ToString("F") + "GB";
            if (len > 1048576)
                return (len / 1048576.0).ToString("F") + "MB";
            return (len / 1024.0).ToString("F") + "KB";
        }

        public static string GetGameName(GameInfo game)
        {
            return game.cnName != null ? game.cnName.Trim()
                     : game.hkName != null ? game.hkName.Trim()
                     : game.usName != null ? game.usName.Trim()
                     : game.jpName != null ? game.jpName.Trim() : game.tid;
        }

        private void GameNameInputChange(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true; //防止向上冒泡
            SearchGameName(searchTextBox.Text);
        }

        private void HaveCnCheckedChanged(object sender, EventArgs e)
        {
            _onlyShowCn = ((CheckBox)sender).Checked;
            searchTextBox.Text = "";
            SearchGameName();
        }

        private void DownloadCheckedChanged(object sender, EventArgs e)
        {
            _showDownloaded = ((CheckBox)sender).Checked;
            searchTextBox.Text = "";
            SearchGameName();
        }

        private void LocalDirLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        private void GameImageClick(object sender, EventArgs e)
        {
            if (_curTid == null) return;
            var g = _gameLists.Find(x => x.tid == _curTid);
            if (g == null) return;

            // US AU
            Process.Start($"https://ec.nintendo.com/apps/{_curTid}/US");
        }

        private void LocalGameListSelectedIndexChanged(object sender, EventArgs e)
        {
            FileInfo f = (FileInfo)localGameListbox.SelectedItem;
            Console.WriteLine(f.FullName);
            ProcessStartInfo info = new ProcessStartInfo("XCI-Explorer\\XCI-Explorer.exe", "\"" + f.FullName + "\"");
            //info.WorkingDirectory = Path.GetDirectoryName("");
            Process.Start(info);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        private void UpdateGameDbClick(object sender, EventArgs e)
        {
            var t = new Thread(UpdateTitleKey);
            t.Start();
        }

        private void UpdateCnExcelClick(object sender, EventArgs e)
        {
            var t = new Thread(UpdateExcel);
            t.Start();
        }

        private bool UpdateNutTileDb(string region, string lang)
        {
            var http = new WebClient { Encoding = Encoding.UTF8 };
            ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            String htmlJson;
            try
            {
                String url = "https://raw.githubusercontent.com/blawar/titledb/master/" + region + "." + lang + ".json";
                Console.WriteLine("start download " + url);
                htmlJson = http.DownloadString(url);
                Console.WriteLine("download nuttiledb success ...");
            }
            catch
            {
                MessageBox.Show("无法访问加载tiledb.请检查网络", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Dictionary<string, JObject> dics;
            try
            {
                // read file into a string and deserialize JSON to a type
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
                var category = JsonConvert.DeserializeObject<List<string>>(kvp.Value["category"].ToString());
                var languages = JsonConvert.DeserializeObject<List<string>>(kvp.Value["languages"].ToString());

                if (isBase)
                {
                    var game = _gameLists.Find(x => x.tid == tidStr);
                    if (game == null)
                    {
                        var res = db.Execute("REPLACE INTO GameInfo " +
                        "(tid, " + region.ToLower() + "name, version, releaseDate, category, publisher, iconUrl, bannerUrl, intro, description, languages, size, haveCn) " +
                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                        tidStr, kvp.Value["name"].ToString(), kvp.Value["version"].ToString(),
                        kvp.Value["releaseDate"].ToString(), String.Join(",", category), kvp.Value["publisher"].ToString(),
                        kvp.Value["iconUrl"].ToString(), kvp.Value["bannerUrl"].ToString(), kvp.Value["intro"].ToString(),
                        kvp.Value["description"].ToString(), String.Join(",", languages), kvp.Value["size"].ToObject<long>(),
                        languages.Contains("zh"));
                    }
                    else
                    {
                        var res = db.Execute("UPDATE GameInfo " +
                        "SET " + region.ToLower() + "name = ?, version = ?, releaseDate = ?, category = ?, publisher = ?, " +
                        "iconUrl = ?, bannerUrl = ?, intro = ?, description = ?, languages = ?, size = ?, haveCn = ? " +
                        "WHERE tid = ?",
                        kvp.Value["name"].ToString(), kvp.Value["version"].ToString(),
                        kvp.Value["releaseDate"].ToString(), String.Join(",", category), kvp.Value["publisher"].ToString(),
                        kvp.Value["iconUrl"].ToString(), kvp.Value["bannerUrl"].ToString(), kvp.Value["intro"].ToString(),
                        kvp.Value["description"].ToString(), String.Join(",", languages), kvp.Value["size"].ToObject<long>(),
                        languages.Contains("zh"), tidStr);
                    }

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
        private void UpdateExcel()
        {
            if (!File.Exists(ExcelPath))
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show("无法访问db.xlsx,请将db.xlsx放在程序根目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));

                return;
            }

            Invoke(new Action(() =>
            {
                toolStripProgressBar_download.Visible = true;
                toolStripProgressBar_download.Maximum = 2;
                toolStripProgressBar_download.Value = 1;
                searchButton.Text = "加载中";
                searchButton.Enabled = false;
                chCheckBox.Enabled = false;
                downloadCheckBox.Enabled = false;
                searchTextBox.Enabled = false;
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
                searchButton.Text = "搜索";
                searchButton.Enabled = true;
                chCheckBox.Enabled = true;
                downloadCheckBox.Enabled = true;
                searchTextBox.Enabled = true;
                toolStripProgressBar_download.Visible = false;
                label_progress.Visible = false;
                SearchGameName(searchTextBox.Text);
            }));
        }

        private void ViewHelpClick(object sender, EventArgs e)
        {
            Process.Start("https://github.com/freedom10086/NSGameDownloader/wiki");
        }

        private void SendFeedBackClick(object sender, EventArgs e)
        {
            Process.Start("https://github.com/freedom10086/NSGameDownloader/issues");
        }

        private void AboutProgramClick(object sender, EventArgs e)
        {
            AboutWindow form = new AboutWindow();
            form.Show();
        }
    }
}