﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace IO_projekt
{
    public static class MenusConfig
    {
        public static Font DefaultFont = new Font("Stencil", 24, FontStyle.Bold);
    }

    public class PauseMenuPanel : TableLayoutPanel
    {
        Form1 FormHandle;
        Label ExitLabel;
        Label ContinueLabel;

        string[] Selections = { "continue", "exit" };
        int MaxSelection = 1;
        int CurrentSelection = 0;
        
        public PauseMenuPanel(Form1 fh)
        {
            FormHandle = fh;

            MaxSelection = Selections.Length - 1;

            this.ColumnCount = 3;
            this.RowCount = 3;

            this.ColumnStyles.Clear();
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

            ContinueLabel = new Label();
            this.Controls.Add(ContinueLabel, 1, 0);
            this.Controls.Add(new PictureBox() { Width = 25, Height = 25, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.None }, 0, 0);
            this.Controls.Add(new PictureBox() { Width = 25, Height = 25, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.None }, 2, 0);
            ContinueLabel.Font = MenusConfig.DefaultFont;
            ContinueLabel.Text = "CONTINUE";
            ContinueLabel.ForeColor = Color.White;
            ContinueLabel.AutoSize = true;
            ContinueLabel.Anchor = AnchorStyles.None;

            ExitLabel = new Label();
            this.Controls.Add(ExitLabel, 1, 1);
            this.Controls.Add(new PictureBox() { Image = Properties.Resources.LeftSelectionMarker, Width = 25, Height = 25, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.None }, 0, 1);
            this.Controls.Add(new PictureBox() { Image = Properties.Resources.RightSelectionMarker, Width = 25, Height = 25, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.None }, 2, 1);
            ExitLabel.Font = MenusConfig.DefaultFont;
            ExitLabel.Text = "EXIT";
            ExitLabel.ForeColor = Color.White;
            ExitLabel.AutoSize = true;
            ExitLabel.Anchor = AnchorStyles.None;

            //this.Grid.Controls.Add(ExitButton, 1, 0);
            //this.Grid.Controls.Add(new PictureBox() {BackColor = Color.Green }, 0, 0);

            this.FormHandle.Controls.Add(this);
            

            this.BackColor = Color.Transparent;
            this.Visible = false;

            this.SetSelection();
            this.KeyDown += PauseMenu_KeyDown;
        }

        public void BringUp()
        {        
            this.Visible = true;
            this.Focus();
            this.BringToFront();
        }
        public void HideMenu()
        {
            this.Visible = false;
            this.FormHandle.Focus();
            this.FormHandle.BringToFront();
        }

        private void SetSelection()
        {
            for (int i=0; i<this.RowCount-1; i++)
            {
                ((PictureBox)this.GetControlFromPosition(0, i)).Image = null;
                ((PictureBox)this.GetControlFromPosition(2, i)).Image = null;
            }

            ((PictureBox)this.GetControlFromPosition(0, CurrentSelection)).Image = Properties.Resources.LeftSelectionMarker;
            ((PictureBox)this.GetControlFromPosition(2, CurrentSelection)).Image = Properties.Resources.RightSelectionMarker;
        }

        public void Reposition()
        {
            this.Width = this.FormHandle.Width / 2 - 200;
            this.Height = this.FormHandle.Height / 2;

            this.Top = this.FormHandle.Height / 2 - this.Height / 2;
            this.Left = this.FormHandle.Width / 2 - this.Width / 2;
        }

        private void ConfirmSelection()
        {
            switch (Selections[CurrentSelection])
            {
                case "continue":
                    this.HideMenu();
                    this.FormHandle.MainTimer.Start();
                    this.FormHandle.Focus();
                    break;

                case "exit":
                    this.FormHandle.Close();
                    break;
            }
        }

        private void PauseMenu_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Getting KeyDown in PauseMenuPanel.");
            if (e.KeyCode == Keys.Escape)
            {
                this.HideMenu();
            }

            if (e.KeyCode == Keys.Down)
            {
                CurrentSelection++;
                if (CurrentSelection > MaxSelection)
                    CurrentSelection = 0;
                SetSelection();
            }
            if (e.KeyCode == Keys.Up)
            {
                CurrentSelection--;
                if (CurrentSelection < 0)
                    CurrentSelection = MaxSelection;
                SetSelection();
            }

            if (e.KeyCode == Keys.Enter)
            {
                ConfirmSelection();
            }
        }
    }

    public class MainMenuPanel : TableLayoutPanel
    {
        string[] Selections = { "start", "options", "view score", "exit" };
        List<Label> SelectionList;
        Form1 FormHandle;
        int CurrentSelection;
        int MaxSelection = 1;
        public WindowsMediaPlayer menuMedia;

        public MainMenuPanel(Form1 fh)
        {
            FormHandle = fh;
            SelectionList = new List<Label>();
            CurrentSelection = 0;
            MaxSelection = Selections.Length - 1;
            FormHandle.pauseLabel.Visible = false;

            menuMedia = new WindowsMediaPlayer();
            FormHandle.gameMedia.controls.stop();

            BackgroundImage = Properties.Resources.menubg2;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            File.WriteAllBytes(@"sound\menu_music.wav", Form1.StreamToByteArr(Properties.Resources.menu_music));
            menuMedia.URL = @"sound\menu_music.wav";
            menuMedia.settings.setMode("loop", true);
            menuMedia.settings.volume = 7;

            this.ColumnCount = 3;
            this.RowCount = 1;

            //this.BackgroundImage = Properties.Resources.menubg ;
            //this.BackColor = Color.DarkRed;

            this.ColumnStyles.Clear();
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

            foreach (string s in Selections)
            {
                this.AddSelection(s);
            }

            this.SetSelection();

            FormHandle.Controls.Add(this);

            this.Visible = true;
            this.BringToFront();
            this.Focus();

            this.Reposition();

            this.KeyDown += MainMenu_KeyDown;
        }

        private void SetSelection()
        {
            for (int i = 0; i < this.RowCount - 1; i++)
            {
                ((PictureBox)this.GetControlFromPosition(0, i)).Image = null;
                ((PictureBox)this.GetControlFromPosition(2, i)).Image = null;
            }

            ((PictureBox)this.GetControlFromPosition(0, CurrentSelection)).Image = Properties.Resources.LeftSelectionMarker;
            ((PictureBox)this.GetControlFromPosition(2, CurrentSelection)).Image = Properties.Resources.RightSelectionMarker;
        }

        public void Reposition()
        {
            this.Width = this.FormHandle.Width;
            this.Height = this.FormHandle.Height;

            this.Top = 0;
            this.Left = 0;
        }

        private void AddSelection(string text)
        {
            this.Controls.Add(new Label() { Text=Selections[this.RowCount-1], Font=MenusConfig.DefaultFont, ForeColor=Color.White, AutoSize=true, Anchor=AnchorStyles.None }, 1, this.RowCount-1);
            this.Controls.Add(new PictureBox() { Width = 25, Height = 25, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.Right }, 0, this.RowCount - 1);
            this.Controls.Add(new PictureBox() { Width = 25, Height = 25, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.Left }, 2, this.RowCount - 1);

            this.RowCount++;
        }

        async private void ConfirmSelection()
        {
            switch (Selections[CurrentSelection])
            {
                case "start":
                    menuMedia.controls.stop();
                    menuMedia.close();
                    FormHandle.MainTimer.Start();
                    this.Dispose();
                    FormHandle.Focus();
                    FormHandle.StartNewGame();

                    FormHandle.p.Sprite.Left = this.FormHandle.Width / 2 - FormHandle.p.Sprite.Width / 2;
                    FormHandle.p.Sprite.Top = this.FormHandle.Height + FormHandle.p.Sprite.Height;
                    while (FormHandle.p.Sprite.Top > this.FormHandle.Height - FormHandle.p.Sprite.Height - 50)
                    {
                        FormHandle.p.Sprite.Top -= 10;
                        await System.Threading.Tasks.Task.Delay(30);
                    }

                    FormHandle.gameMedia.controls.play();
                    //FormHandle.StartNewGame();
                    break;

                case "options":
                    
                    break;

                case "view score":
                    ScoreTable st = new ScoreTable(FormHandle, this);
                    break;

                case "exit":
                    FormHandle.Close();
                    break;
            }
        }

        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                CurrentSelection++;
                if (CurrentSelection > MaxSelection)
                    CurrentSelection = 0;
                SetSelection();
            }
            if (e.KeyCode == Keys.Up)
            {
                CurrentSelection--;
                if (CurrentSelection < 0)
                    CurrentSelection = MaxSelection;
                SetSelection();
            }

            if (e.KeyCode == Keys.Enter)
            {
                ConfirmSelection();
            }
        }
    }

    public class ScoreTable : TableLayoutPanel
    {
        Form1 FormHandle;
        int clrlvl = 0;
        Panel parent;
        string[] FileContent;
        private class ScorePosition
        {
            public int Score;
            public string Name;

            public ScorePosition(int s, string n)
            {
                Score = s;
                Name = n;
            }
        }
        List<ScorePosition> Scores;

        Label EscLabel;
        Timer EscLabelTimer;

        public ScoreTable(Form1 fh, Panel p)
        {
            StreamWriter sw;
            string path = @"Scores.txt";
            FormHandle = fh;
            parent = p;
            this.BackgroundImage = Properties.Resources.scorebg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Scores = new List<ScorePosition>();

            this.ColumnCount = 2;
            this.RowCount = 2;
            //this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            this.RowStyles.Clear();
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            this.BackColor = Color.Black;

            this.Controls.Add(new Label() {Text = "High", Font = new Font("Stencil", 60, FontStyle.Bold), BackColor = Color.Transparent, ForeColor =Color.Red, AutoSize=true, Anchor=AnchorStyles.Right }, 0, 0);
            this.Controls.Add(new Label() {Text = "scores", Font = new Font("Stencil", 60, FontStyle.Bold), BackColor = Color.Transparent, ForeColor = Color.Red, AutoSize = true, Anchor = AnchorStyles.Left }, 1, 0);

            if (!File.Exists(path))
            {
                sw = File.CreateText(path);
                Console.WriteLine("plik został utworzony");
                sw.Close();
            }

            FileContent = File.ReadAllLines(@"scores.txt");
            foreach (string s in FileContent)
            {
                Console.WriteLine("LINE: " + s);

                string[] line = s.Split('-');
                Scores.Add(new ScorePosition(Convert.ToInt32(line[0]), line[1]));
            }

            List<ScorePosition> SortedScores = Scores.OrderByDescending(o => o.Score).ToList();
            foreach (ScorePosition se in SortedScores)
            {
                this.AddEntry(se.Score, se.Name);
            }

            FormHandle.Controls.Add(this);
            this.Visible = true;
            this.BringToFront();
            this.Focus();
            this.Reposition();

            this.KeyPress += ScoreTable_KeyPress;

            EscLabel = new Label() { Text="Press esc to quit", Font = MenusConfig.DefaultFont, ForeColor=Color.White, AutoSize=true, Anchor=AnchorStyles.Left, BackColor = Color.Transparent };
            FormHandle.Controls.Add(EscLabel);
            //this.Controls.Add(EscLabel);
            //EscLabel.Parent = this;
            EscLabel.BringToFront();

            EscLabelTimer = new Timer() { Interval=600 };
            EscLabelTimer.Tick += new EventHandler((object sender, EventArgs e) =>
                {
                    if (EscLabel.Visible == true)
                        EscLabel.Visible = false;
                    else
                        EscLabel.Visible = true;
                }
            );
            EscLabelTimer.Start();
        }

        private void AddEntry(int score, string name)
        {
            if(clrlvl == 1)
            {
                this.Controls.Add(new Label() { Text = Convert.ToString(score), Font = new Font("Stencil", 28, FontStyle.Bold), BackColor = Color.Transparent, ForeColor = Color.WhiteSmoke, AutoSize = true, Anchor = AnchorStyles.Right }, 0, this.RowCount - 1);
                this.Controls.Add(new Label() { Text = name, Font = new Font("Stencil", 28, FontStyle.Bold), BackColor = Color.Transparent, ForeColor = Color.WhiteSmoke, AutoSize = true, Anchor = AnchorStyles.Left }, 1, this.RowCount - 1);
                clrlvl = 0;
            }
            else
            {
                this.Controls.Add(new Label() { Text = Convert.ToString(score), Font = new Font("Stencil", 28, FontStyle.Bold), BackColor = Color.Transparent, ForeColor = Color.PowderBlue, AutoSize = true, Anchor = AnchorStyles.Right }, 0, this.RowCount - 1);
                this.Controls.Add(new Label() { Text = name, Font = new Font("Stencil", 28, FontStyle.Bold), BackColor = Color.Transparent, ForeColor = Color.PowderBlue, AutoSize = true, Anchor = AnchorStyles.Left }, 1, this.RowCount - 1);
                clrlvl = 1;
            }
            
            this.RowCount++;
        }

        public void Reposition()
        {
            this.Width = this.FormHandle.Width;
            this.Height = this.FormHandle.Height;
            this.Top = 0;
            this.Left = 0;
        }

        private void ScoreTable_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                parent.Focus();
                this.EscLabelTimer.Stop();
                this.EscLabelTimer.Dispose();
                this.EscLabel.Dispose();
                this.Dispose();
            }
        }
    }

    public class ScoreEntry : FlowLayoutPanel
    {
        Form1 FormHandle;
        string PlayerName = "";
        Label NameLabel;

        public ScoreEntry(Form1 fh)
        {
            FormHandle = fh;

            FormHandle.Controls.Add(this);

            this.BackColor = Color.DarkRed;
            this.AutoSize = true;
            this.BorderStyle = BorderStyle.Fixed3D;

            this.FlowDirection = FlowDirection.TopDown;
            this.Controls.Add(new Label() { Text = "Your score:", Font = MenusConfig.DefaultFont, ForeColor = Color.White, AutoSize=true, Anchor = AnchorStyles.None });
            this.Controls.Add(new Label() { Text = Convert.ToString(Form1.score), Font = MenusConfig.DefaultFont, ForeColor = Color.White, AutoSize = true, Anchor= AnchorStyles.None});
            this.Controls.Add(new Label() { Text = "Enter your name:", Font = MenusConfig.DefaultFont, ForeColor = Color.White, AutoSize = true, Anchor = AnchorStyles.None });

            NameLabel = new Label();
            NameLabel.Font = MenusConfig.DefaultFont;
            NameLabel.ForeColor = Color.White;
            NameLabel.AutoSize = true;
            NameLabel.Anchor = AnchorStyles.None;
            this.Controls.Add(NameLabel);
            

            this.KeyPress += ScoreEntry_KeyDown;
            this.Focus();
            this.Reposition();
            this.Visible = true;
            this.BringToFront();
        }

        public void Reposition()
        {
            this.Top = this.FormHandle.Height / 2 - this.Height / 2;
            this.Left = this.FormHandle.Width / 2 - this.Width / 2;
        }

        private void ScoreEntry_KeyDown(object sender, KeyPressEventArgs e)
        {         
            if (e.KeyChar == (char)Keys.Enter && PlayerName != "")
            {
                File.AppendAllText(@"scores.txt", Convert.ToString(Form1.score) + '-' + PlayerName + '\n');

                this.Dispose();
                FormHandle.MainMenu = new MainMenuPanel(FormHandle);
                Conf.ClearAndDisposeAll();
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Dispose();
                FormHandle.MainMenu = new MainMenuPanel(FormHandle);
                Conf.ClearAndDisposeAll();
            }
            if (e.KeyChar == (char)Keys.Back && PlayerName != "")
            {
                PlayerName = PlayerName.Remove(PlayerName.Length - 1);
            }

             if(PlayerName.Length <= 10 && e.KeyChar != (char)Keys.Back) PlayerName += e.KeyChar;
             NameLabel.Text = PlayerName;
        }
    }

    public class ShopPanel : FlowLayoutPanel
    {
        Form1 FormHandle;
        TableLayoutPanel ShopTablePanel;
        TableLayoutPanel ShopTabTable;
        TableLayoutPanel BottomPanel;
        Label ExitLabel;
        Label ItemDescriptionLabel;
        PictureBox ShopSelectionMarker;

        string[] ShopTabs = { "Bonuses", "Upgrades" };
        ShopEntry[] BonusesSelections = { new ShopEntry("Shield", Properties.Resources.bonusShield),
                                   new ShopEntry("Scatter Gun", Properties.Resources.TempPic),
                                   new ShopEntry("Rocket", Properties.Resources.TempPic),
                                   new ShopEntry("Rate of Fire+", Properties.Resources.TempPic),
                                   new ShopEntry("Bullet Speed", Properties.Resources.TempPic) };
        int CurrentRowSelection = 0;
        int CurrentColumnSelection = 0;
        int MaxRowSelection;
        int MaxColumnSelection;

        private class ShopEntry
        {
            public string Name;
            public Image Img;
            public ShopEntry(string n, Image i)
            {
                Name = n;
                Img = i;
            }
        }

        public ShopPanel(Form1 fh)
        {
            this.FlowDirection = FlowDirection.TopDown;

            FormHandle = fh;
            ShopTablePanel = new TableLayoutPanel();
            //TablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            BackColor = Color.DarkBlue;
            ExitLabel = new Label() { Text = "Exit Shop", Font = MenusConfig.DefaultFont, ForeColor=Color.White, AutoSize = true, Anchor=AnchorStyles.None };
            ItemDescriptionLabel = new Label() { Text = "abc", Font = MenusConfig.DefaultFont, ForeColor = Color.White, AutoSize = true, Anchor = AnchorStyles.None };

            ShopTabTable = new TableLayoutPanel();
            ShopTabTable.ColumnStyles.Clear();
            ShopTabTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            ShopTabTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            ShopTabTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            ShopTabTable.RowCount = 1;
            ShopTabTable.ColumnCount = 3;
            ShopTabTable.Controls.Add(new PictureBox() { Width = Height = 40, BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.None }, 0, 0);
            ShopTabTable.Controls.Add(new Label() { Text = "Bonuses", Font = MenusConfig.DefaultFont, ForeColor = Color.White, AutoSize = true, Anchor = AnchorStyles.None }, 1, 0);
            ShopTabTable.Controls.Add(new PictureBox() { Width = Height = 40, BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.Zoom, Anchor = AnchorStyles.None }, 2, 0);

            BottomPanel = new TableLayoutPanel();
            BottomPanel.RowCount = 1;
            BottomPanel.ColumnCount = 2;
            BottomPanel.ColumnStyles.Clear();
            BottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            BottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            BottomPanel.Controls.Add(ExitLabel, 0, 0);
            BottomPanel.Controls.Add(ItemDescriptionLabel, 1, 0);
            BottomPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            //((PictureBox)ShopTabTable.GetControlFromPosition(0, 0)).Image = Properties.Resources.RightSelectionMarker;
            ((PictureBox)ShopTabTable.GetControlFromPosition(2, 0)).Image = Properties.Resources.LeftSelectionMarker;

            this.Reposition();

            this.ShopTablePanel.ColumnCount = 4;
            this.ShopTablePanel.RowCount = 1;

            this.ShopTablePanel.ColumnStyles.Clear();
            this.ShopTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            this.ShopTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            this.ShopTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            this.ShopTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            ShopTablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            this.ShopTablePanel.RowStyles.Clear();
            this.ShopTablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, this.ShopTablePanel.Width / this.ShopTablePanel.ColumnCount));
            ShopTabTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            AddSelections();
            this.ShopTablePanel.RowCount++;


            FormHandle.Controls.Add(this);
            this.Controls.Add(ShopTabTable);
            this.Controls.Add(ShopTablePanel);
            this.Controls.Add(BottomPanel);
            //this.Controls.Add(ExitLabel);
            this.BringToFront();

            this.KeyDown += ShopPanel_KeyPress;
            this.Focus();

            this.SetSelection();
            Console.WriteLine($"Max COL: {MaxColumnSelection}");
            Console.WriteLine($"Max ROW: {MaxRowSelection}");
        }
        public void Reposition()
        {
            this.Width = FormHandle.Width;
            this.Height = FormHandle.Height;

            ShopTabTable.Width = this.Width;
            BottomPanel.Width = this.Width;

            this.ShopTablePanel.Width = this.Width;
            this.ShopTablePanel.Height = this.Height - BottomPanel.Height - ShopTabTable.Height - 50;

            this.ShopTablePanel.RowStyles.Clear();
            for (int i=0; i<this.ShopTablePanel.RowCount; i++)
                this.ShopTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        }
        private void AddSelections()
        {
            int ColumnIndex = 0;
            foreach (ShopEntry s in BonusesSelections)
            {

                Console.WriteLine($"Adding selection {s.Name}");

                this.ShopTablePanel.Controls.Add(new PictureBox()
                {
                    Image = s.Img,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.Transparent,
                    Width = 70,
                    Height = 70,
                    Anchor = AnchorStyles.None
                },
                    ColumnIndex++, this.ShopTablePanel.RowCount - 1);

                if (ColumnIndex >= 4)
                {
                    ColumnIndex = 0;
                    this.ShopTablePanel.RowCount++;
                    this.ShopTablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, this.ShopTablePanel.Width / this.ShopTablePanel.ColumnCount));
                }

                this.ShopTablePanel.RowStyles.Clear();
                for (int i=0; i<this.ShopTablePanel.RowCount; i++)
                    this.ShopTablePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, this.ShopTablePanel.Width / this.ShopTablePanel.ColumnCount));
            }

            MaxColumnSelection = this.ShopTablePanel.ColumnCount - 1;
            MaxRowSelection = this.ShopTablePanel.RowCount - 1;
        }

        private void SetSelection()
        {
            Control hParent = ShopTablePanel.GetControlFromPosition(CurrentColumnSelection, CurrentRowSelection);
            ShopSelectionMarker?.Dispose();
            ShopSelectionMarker = new PictureBox() { Image=Properties.Resources.ShopSelectionMarkerPic, BackColor=Color.Transparent, SizeMode=PictureBoxSizeMode.Zoom };
            ShopSelectionMarker.Parent = hParent;
            ShopSelectionMarker.Width = hParent.Width;
            ShopSelectionMarker.Height = hParent.Height;
        }

        private void ChangeTab()
        {
            
            ((Label)ShopTabTable.GetControlFromPosition(1,0)).Text = ShopTabs[1];
            ((PictureBox)ShopTabTable.GetControlFromPosition(0, 0)).Image.Dispose(); ;
            ((PictureBox)ShopTabTable.GetControlFromPosition(2, 0)).Image = Properties.Resources.LeftSelectionMarker;
        }

        private void ShopPanel_KeyPress(object sender, KeyEventArgs e)
        {

                if(e.KeyCode == Keys.Left)
                {
                    if (CurrentColumnSelection > 0 && ShopTablePanel.GetControlFromPosition(CurrentColumnSelection - 1, CurrentRowSelection) != null)
                        CurrentColumnSelection--;
                }
                    

                if (e.KeyCode == Keys.Right)
                    {
//                Console.WriteLine("Pressing right in shop...");
                        if (CurrentColumnSelection < MaxColumnSelection && ShopTablePanel.GetControlFromPosition(CurrentColumnSelection + 1, CurrentRowSelection) != null)
                            CurrentColumnSelection++;
                    }
                    

                 if (e.KeyCode == Keys.Down)
                    {
                        if (CurrentRowSelection < MaxRowSelection && ShopTablePanel.GetControlFromPosition(CurrentColumnSelection, CurrentRowSelection + 1) != null)
                            CurrentRowSelection++;
                    }


                if (e.KeyCode == Keys.Up)
                    {
                        if (CurrentRowSelection > 0 && ShopTablePanel.GetControlFromPosition(CurrentColumnSelection, CurrentRowSelection - 1) != null)
                            CurrentRowSelection--;
                    }

            Console.WriteLine($"COL: {CurrentColumnSelection}");
            Console.WriteLine($"ROW: {CurrentRowSelection}");
            SetSelection();
        }
    }
}
