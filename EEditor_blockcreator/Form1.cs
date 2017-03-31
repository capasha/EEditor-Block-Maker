using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerIOClient;
using System.IO;

namespace EEditor_blockcreator
{
    public partial class Form1 : Form
    {
        private Bitmap foreground = new Bitmap(16, 16);
        private Dictionary<string, List<data>> blocks = new Dictionary<string, List<data>>();
        private Dictionary<string, List<data>> action = new Dictionary<string, List<data>>();
        private Dictionary<string, List<data>> decos = new Dictionary<string, List<data>>();
        private Dictionary<string, List<data>> bgs = new Dictionary<string, List<data>>();
        private ImageList il = new ImageList();
        private data blockData = new data();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string pathToIMGBlocks = Directory.GetCurrentDirectory() + "\\blocks\\foreground.png";
            string pathToBlocks = Directory.GetCurrentDirectory() + "\\blocks\\blocks.json";
            if (File.Exists(pathToIMGBlocks) && File.Exists(pathToBlocks))
            {
                foreground = new Bitmap(Bitmap.FromFile(pathToIMGBlocks));
                //blockData = JsonConvert.DeserializeObject<data>(File.ReadAllText(Directory.GetCurrentDirectory() + "\\blocks.json"));
                var output = JObject.Parse(File.ReadAllText(pathToBlocks));
                foreach (var property in output)
                {
                    if (!treeView1.Nodes.ContainsKey(property.Key)) treeView1.Nodes.Add(property.Key.ToString(), property.Key.ToString());
                    foreach (var value in property.Value)
                    {
                        if (!blocks.ContainsKey(property.Key.ToString()))
                        {
                            //int bid = value[0].ToString().Contains("blockID") ? Convert.ToInt32(value[0].ToString()) : 0;
                            //int iid = value[1].ToString().Contains("blockImage") ? Convert.ToInt32(value[1].ToString()) : 0;
                            //string color = value[2].ToString().Contains("blockColor") ? value[2].ToString() : "0x00000";
                            blocks.Add(property.Key.ToString(), new List<data>() {
                                new data()
                                {

                                    blockID = value["blockID"].ToString() != null ? Convert.ToInt32(value["blockID"].ToString()):0,
                                    blockImage = value["blockImage"].ToString() != null ? Convert.ToInt32(value["blockImage"].ToString()):0,
                                    blockColor = value["blockColor"].ToString() != null ? value["blockColor"].ToString():"0x00000",
                                    method = value["method"].ToString() != null ? Convert.ToInt32(value["method"].ToString()):0
                                }
                            });

                        }
                        else
                        {

                            blocks[property.Key.ToString()].Add(new data()
                            {

                                blockID = value["blockID"].ToString() != null ? Convert.ToInt32(value["blockID"].ToString()) : 0,
                                blockImage = value["blockImage"].ToString() != null ? Convert.ToInt32(value["blockImage"].ToString()) : 0,
                                blockColor = value["blockColor"].ToString() != null ? value["blockColor"].ToString() : "0x00000",
                                method = value["method"].ToString() != null ? Convert.ToInt32(value["method"].ToString()) : 0
                            });

                        }
                        treeView1.Nodes[property.Key.ToString()].Nodes.Add(value["blockID"].ToString());
                    }
                }

            }
            else
            {
                label6.ForeColor = Color.DarkBlue;
                label6.Text = "The Folder 'blocks' doesn't exist. The Folder is now created.";
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\blocks\\");
            }
            il.Images.Add(Properties.Resources.eeditor_block);
            il.Images.Add(Properties.Resources.eeditor_action);
            il.Images.Add(Properties.Resources.eeditor_decor);
            il.Images.Add(Properties.Resources.eeditor_bg);
            tabControl1.ImageList = il;
            tabPage1.ImageIndex = 0;
            tabPage2.ImageIndex = 1;
            tabPage3.ImageIndex = 2;
            tabPage4.ImageIndex = 3;
        }

        private void UpdateBlocks(string pack, string block, bool clear)
        {
            if (clear) treeView1.Nodes.Clear();
            flowLayoutPanel1.Controls.Clear();

            ToolStrip ts = new ToolStrip();
            ts.Name = "Blocks";
            ts.Anchor = AnchorStyles.None;
            ts.Location = new Point(16, 16);
            flowLayoutPanel1.Controls.Add(ts);

            ToolStrip ts1 = new ToolStrip();
            ts1.Name = "Colors";
            ts1.Anchor = AnchorStyles.None;
            ts1.Location = new Point(16, 46);
            flowLayoutPanel1.Controls.Add(ts1);
            int incr = 0;
            for (int i = 0; i < blocks[pack].Count; i++)
            {
                if (blocks[pack][i].blockImage * 16 <= foreground.Width)
                {
                    if (incr == 0 && !treeView1.Nodes.ContainsKey(pack))
                    {
                        treeView1.Nodes.Add(pack, pack);
                    }
                    if (clear && treeView1.Nodes.ContainsKey(pack))
                    {
                        treeView1.Nodes[pack].Nodes.Add(blocks[pack][i].blockID.ToString());
                    }
                    if (block == null)
                    {

                        ToolStripButton tsb = new ToolStripButton();
                        tsb.ToolTipText = blocks[pack][i].blockID.ToString();
                        ToolStripButton tsb1 = new ToolStripButton();
                        tsb1.ToolTipText = blocks[pack][i].blockColor;
                        tsb.Image = foreground.Clone(new Rectangle(blocks[pack][i].blockImage * 16, 0, 16, 16), foreground.PixelFormat);
                        Bitmap col = new Bitmap(24, 24);
                        using (Graphics gr = Graphics.FromImage(col))
                        {
                            gr.Clear(ColorTranslator.FromHtml(blocks[pack][i].blockColor.ToString().Replace("0x", "#")));
                            //gr.Clear(Color.Red);
                        }

                        tsb1.Image = col;
                        ts.Items.Add(tsb);
                        ts1.Items.Add(tsb1);

                    }
                    else
                    {
                        if (blocks[pack][i].blockID.ToString() == block)
                        {
                            ToolStripButton tsb = new ToolStripButton();
                            tsb.ToolTipText = blocks[pack][i].blockID.ToString();
                            ToolStripButton tsb1 = new ToolStripButton();
                            tsb1.ToolTipText = blocks[pack][i].blockColor;
                            tsb.Image = foreground.Clone(new Rectangle(blocks[pack][i].blockImage * 16, 0, 16, 16), foreground.PixelFormat);
                            Bitmap col = new Bitmap(24, 24);
                            using (Graphics gr = Graphics.FromImage(col))
                            {
                                gr.Clear(ColorTranslator.FromHtml(blocks[pack][i].blockColor.ToString().Replace("0x", "#")));
                                //gr.Clear(Color.Red);
                            }

                            tsb1.Image = col;
                            ts.Items.Add(tsb);
                            ts1.Items.Add(tsb1);
                            break;
                        }
                    }
                }
                else
                {

                    if (!clear)
                    {
                        label6.ForeColor = Color.DarkBlue;
                        label6.Text = "The current block you tried to add\nDoesn't exist as an image.";
                    }
                    break;
                }
                incr++;
            }
            if (clear && treeView1.Nodes.ContainsKey(pack) && !treeView1.Nodes[pack].IsExpanded)
            {
                treeView1.Nodes[pack].Expand();
            }
            if (clear && treeView1.Nodes.ContainsKey(pack) && treeView1.Nodes[pack].IsExpanded)
            {
                treeView1.Nodes[pack].Collapse();
            }

        }
        private void RadioButton(int value)
        {
            switch (value)
            {
                case 0:
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                    radioButton3.Checked = false;
                    radioButton4.Checked = false;
                    break;
                case 1:
                    radioButton1.Checked = false;
                    radioButton2.Checked = true;
                    radioButton3.Checked = false;
                    radioButton4.Checked = false;
                    break;
                case 2:
                    radioButton1.Checked = false;
                    radioButton2.Checked = false;
                    radioButton3.Checked = true;
                    radioButton4.Checked = false;
                    break;
                case 3:
                    radioButton1.Checked = false;
                    radioButton2.Checked = false;
                    radioButton3.Checked = false;
                    radioButton4.Checked = true;
                    break;
            }
        }
        private void AddBlocks(string packName, int blockID, int blockImagePosition, string blockColor, int method)
        {
            if (IsHex(blockColor))
            {


                if (!blocks.ContainsKey(packName))
                {
                    blocks.Add(packName,
                        new List<data>()
                        {
                            new data()
                            {
                                blockID = blockID,
                                blockImage = blockImagePosition,
                                blockColor = blockColor,
                                method = method

                            }
                        });
                    if (Directory.Exists(Directory.GetCurrentDirectory() + "\\blocks"))
                    {

                        File.WriteAllText(Directory.GetCurrentDirectory() + "\\blocks\\blocks.json", JsonConvert.SerializeObject(blocks, Newtonsoft.Json.Formatting.Indented));
                        UpdateBlocks(packName, null, true);
                    }
                    else
                    {
                        label6.ForeColor = Color.DarkBlue;
                        label6.Text = "The Folder 'blocks' doesn't exist. The Folder is now created.";
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\blocks");
                        if (Directory.Exists(Directory.GetCurrentDirectory() + "\\blocks"))
                        {

                            File.WriteAllText(Directory.GetCurrentDirectory() + "\\blocks\\blocks.json", JsonConvert.SerializeObject(blocks, Newtonsoft.Json.Formatting.Indented));
                            UpdateBlocks(packName, null, true);
                        }
                    }
                }
                else
                {
                    var blockExist = false;
                    for (int i = 0; i < blocks[packName].Count; i++)
                    {
                        if (blocks[packName][i].blockID == blockID)
                        {
                            blockExist = true;
                            break;
                        }
                    }
                    if (!blockExist)
                    {
                        blocks[packName].Add(
                            new data()
                            {
                                blockID = blockID,
                                blockImage = blockImagePosition,
                                blockColor = blockColor,
                                method = method
                            }
                            );


                        if (Directory.Exists(Directory.GetCurrentDirectory() + "\\blocks"))
                        {

                            File.WriteAllText(Directory.GetCurrentDirectory() + "\\blocks\\blocks.json", JsonConvert.SerializeObject(blocks, Newtonsoft.Json.Formatting.Indented));
                            UpdateBlocks(packName, null, true);
                        }
                        else
                        {
                            label6.ForeColor = Color.DarkBlue;
                            label6.Text = "The Directory 'blocks' doesn't exist. The Folder is now created.";
                            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\blocks");
                            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\blocks"))
                            {

                                File.WriteAllText(Directory.GetCurrentDirectory() + "\\blocks\\blocks.json", JsonConvert.SerializeObject(blocks, Newtonsoft.Json.Formatting.Indented));
                                UpdateBlocks(packName, null, true);
                            }
                        }
                    }


                    else
                    {
                        label6.ForeColor = Color.DarkBlue;
                        label6.Text = "You can't add more than 1 of the same block twice.";

                    }

                }
            }
            else
            {
                label6.ForeColor = Color.DarkBlue;
                label6.Text = "This isn't a correct color format.\nIt should look like this: 0xFF00FF";
            }

        }
        private Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }

        private bool IsHex(IEnumerable<char> chars)
        {
            bool isHex;
            if (chars.ToString().StartsWith("0x"))
            {
                chars = chars.ToString().Replace("0x", "");
                foreach (var c in chars)
                {
                    isHex = ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));

                    if (!isHex)
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PackNameBox1.Text))
            {
                if (!string.IsNullOrEmpty(BlockColorBox2.Text))
                {
                    if (BlockImageUpDown2.Value < 3000)
                    {
                        if (BlockIDUpDown1.Value < 3000)
                        {
                            int method = 0;
                            if (radioButton1.Checked) method = 0;
                            if (radioButton2.Checked) method = 1;
                            if (radioButton3.Checked) method = 2;
                            if (radioButton4.Checked) method = 3;
                            AddBlocks(PackNameBox1.Text, Convert.ToInt32(BlockIDUpDown1.Value), Convert.ToInt32(BlockImageUpDown2.Value), BlockColorBox2.Text, method);
                        }
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            //Console.WriteLine(e.Node.Text);
            if (blocks.ContainsKey(e.Node.Text) && e.Node.Parent == null)
            {
                UpdateBlocks(e.Node.Text, null, false);
                PackNameBox1.Text = e.Node.Text;
                RadioButton(0);
            }
            else if (blocks.ContainsKey(e.Node.Parent.ToString().Replace("TreeNode: ", "")) && e.Node.Parent != null)
            {
                UpdateBlocks(e.Node.Parent.ToString().Replace("TreeNode: ", ""), e.Node.Text, false);
                PackNameBox1.Text = e.Node.Parent.ToString().Replace("TreeNode: ", "");
                BlockIDUpDown1.Value = Convert.ToInt32(e.Node.Text);
                RadioButton(0);
                for (int i = 0; i < blocks[e.Node.Parent.ToString().Replace("TreeNode: ", "")].Count; i++)
                {
                    if (blocks[e.Node.Parent.ToString().Replace("TreeNode: ", "")][i].blockID.ToString() == e.Node.Text)
                    {
                        BlockColorBox2.Text = blocks[e.Node.Parent.ToString().Replace("TreeNode: ", "")][i].blockColor;
                        BlockImageUpDown2.Value = blocks[e.Node.Parent.ToString().Replace("TreeNode: ", "")][i].blockImage;
                        RadioButton(blocks[e.Node.Parent.ToString().Replace("TreeNode: ", "")][i].method);
                        break;
                    }
                }
            }
            label6.Text = "";
        }
    }
    public class data
    {
        public int blockID { get; set; }
        public int blockImage { get; set; }
        public int method { get; set; }
        public string blockColor { get; set; }
    }

}
