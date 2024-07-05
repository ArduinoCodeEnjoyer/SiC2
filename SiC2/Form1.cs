using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static SiC2.Form1;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SiC2
{
    public partial class Form1 : Form
    {
        private volatile bool keepRunning;
        private TcpClient client;
        private NetworkStream stream;
        private Image originalImage;
        private float angle = 0f;

        bool[] checkbool = new bool[35];

        public Form1()
        {
            InitializeComponent();
            
            // Set up the PictureBox
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void tableLayoutPanel10_Paint(object sender, PaintEventArgs e)
        {

        }

        //debounce


        bool isConnected = false;
        bool keyIsPressed = false;

        private void tcpConnect_Click(object sender, EventArgs e)
        {
            if (!keepRunning)
            {
                try
                {
                    // Replace "arduino_server_ip" and "port_number" with the actual IP address and port number of your Arduino server
                    string serverIP = ipAddress.Text;
                    int port = int.Parse(portTxt.Text);
                    client = new TcpClient(serverIP, port);
                    stream = client.GetStream();
                    output.Text += "\n---------Connected to Arduino server.---------\n";
                    ipAddress.Enabled = false;
                    portTxt.Enabled = false;

                    keepRunning = true;
                    this.KeyPreview = true; // Ensure the form receives key events
                    this.KeyDown += new KeyEventHandler(Form1_KeyDown);
                    this.KeyUp += new KeyEventHandler(Form1_KeyUp);
                }
                catch
                {
                    output.Text += "\n Error: Could not connect to Arduino Server";
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                break;
                case Keys.W:
                    if (checkbool[0] == false)
                    {
                        output.Text += "Forward: " + e.KeyCode + "\n";
                        SendMessage("W\n\r");
                        checkbool[0] = true;
                    }
                break;
                case Keys.A:
                    if (checkbool[1] == false)
                    {
                        output.Text += "Left: " + e.KeyCode + "\n";
                        SendMessage("A\n\r");
                        checkbool[1] = true;
                    }
                break;
                case Keys.S:
                    if (checkbool[2] == false)
                    {
                        output.Text += "Back: " + e.KeyCode + "\n";
                        SendMessage("S\n\r");
                        checkbool[2] = true;
                    }
               break;
                case Keys.D:
                    if (checkbool[3] == false)
                    {
                        output.Text += "Right: " + e.KeyCode + "\n";
                        SendMessage("D\n\r");
                        checkbool[3] = true;
                    }
                break;
                case Keys.Up:
                    if (checkbool[4] == false)
                    {
                        output.Text += "Robot Arm Forward: " + e.KeyCode + "\n";
                        SendMessage("Up\n\r");
                        checkbool[4] = true;
                    }
                break;
                case Keys.Down:
                    if (checkbool[5] == false)
                    {
                        output.Text += "Robot Arm Backward: " + e.KeyCode + "\n";
                        SendMessage("Down\n\r");
                        checkbool[5] = true;
                    }
                break;
                case Keys.Left:
                    if (checkbool[6] == false)
                    {
                        output.Text += "Wrist Left: " + e.KeyCode + "\n";
                        SendMessage("Left\n\r");
                        checkbool[6] = true;
                    }
                break;
                case Keys.Right:
                    if (checkbool[7] == false)
                    {
                        output.Text += "Wrist Right: " + e.KeyCode + "\n";
                        SendMessage("Right\n\r");
                        checkbool[7] = true;
                    }
                break;
                case Keys.U:
                    if (checkbool[8] == false)
                    {
                        output.Text += "Front Flipper Forward: " + e.KeyCode + "\n";
                        SendMessage("U\n\r");
                        checkbool[8] = true;
                    }
                break;
                case Keys.J:
                    if (checkbool[9] == false)
                    {
                        output.Text += "Front Flipper Backward: " + e.KeyCode + "\n";
                        SendMessage("J\n\r");
                        checkbool[9] = true;
                    }
                break;
                case Keys.I:
                    if (checkbool[10] == false)
                    {
                        output.Text += "Back Flipper Forward: " + e.KeyCode + "\n";
                        SendMessage("I\n\r");
                        checkbool[10] = true;
                    }
                break;
                case Keys.K:
                    if (checkbool[11] == false)
                    {
                        output.Text += "Back Flipper Backward: " + e.KeyCode + "\n";
                        SendMessage("K\n\r");
                        checkbool[11] = true;
                    }
                break;
                case Keys.C:
                    if (checkbool[12] == false)
                    {
                        output.Text += "Front Flipper Home: " + e.KeyCode + "\n";
                        SendMessage("C\n\r");
                        checkbool[12] = true;
                    }
                break;
                case Keys.V:
                    if (checkbool[13] == false)
                    {
                        output.Text += "Back Flipper Home: " + e.KeyCode + "\n";
                        SendMessage("V\n\r");
                        checkbool[13] = true;
                    }
                break;
                case Keys.NumPad8:
                    if (checkbool[14] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad8\n\r");
                        checkbool[14] = true;
                    }
                break;
                case Keys.NumPad2:
                    if (checkbool[15] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad2\n\r");
                        checkbool[15] = true;
                    }
                break;
                case Keys.OemPeriod:
                    if (checkbool[16] == false)
                    {
                        output.Text += "Robot Arm Home: " + e.KeyCode + "\n";
                        SendMessage("OemPeriod\n\r");
                        checkbool[16] = true;
                    }
                break;
                case Keys.Space:
                    if (checkbool[17] == false)
                    {
                        output.Text += "All Servos Home: " + e.KeyCode + "\n";
                        SendMessage("Space\n\r");
                        checkbool[17] = true;
                    }
                break;
                case Keys.Z:
                    if (checkbool[18] == false)
                    {
                        output.Text += "Front Flipper Dog: " + e.KeyCode + "\n";
                        SendMessage("Z\n\r");
                        checkbool[18] = true;
                    }
                break;
                case Keys.X:
                    if (checkbool[19] == false)
                    {
                        output.Text += "Back Flipper Dog: " + e.KeyCode + "\n";
                        SendMessage("X\n\r");
                        checkbool[19] = true;
                    }
                break;
                case Keys.F:
                    if (checkbool[20] == false)
                    {
                        output.Text += "Front Flipper Stretch: " + e.KeyCode + "\n";
                        SendMessage("F\n\r");
                        checkbool[20] = true;
                    }
                break;
                case Keys.G:
                    if (checkbool[21] == false)
                    {
                        output.Text += "Back Flipper Stretch: " + e.KeyCode + "\n";
                        SendMessage("G\n\r");
                        checkbool[21] = true;
                    }
                break;
                case Keys.R:
                    if (checkbool[22] == false)
                    {
                        output.Text += "Front Flipper Z: " + e.KeyCode + "\n";
                        SendMessage("R\n\r");
                        checkbool[22] = true;
                    }
                break;
                case Keys.T:
                    if (checkbool[23] == false)
                    {
                        output.Text += "Back Flipper Z: " + e.KeyCode + "\n";
                        SendMessage("T\n\r");
                        checkbool[23] = true;
                    }
                break;
                case Keys.NumPad7:
                    if (checkbool[24] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad7\n\r");
                        checkbool[24] = true;
                    }
                break;
                case Keys.NumPad1:
                    if (checkbool[25] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad1\n\r");
                        checkbool[25] = true;
                    }
                break;
                case Keys.NumPad9:
                    if (checkbool[26] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad9\n\r");
                        checkbool[26] = true;
                    }
                break;
                case Keys.NumPad3:
                    if (checkbool[27] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad3\n\r");
                        checkbool[27] = true;
                    }
                break;
                case Keys.NumPad0:
                    if (checkbool[28] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad0\n\r");
                        checkbool[28] = true;
                    }
                break;
                case Keys.NumPad4:
                    if (checkbool[29] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad4\n\r");
                        checkbool[29] = true;
                    }
                break;
                case Keys.NumPad6:
                    if (checkbool[30] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("NumPad6\n\r");
                        checkbool[30] = true;
                    }
                break;

            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                break;
                case Keys.W:
                    if (checkbool[0] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[0] = false;
                    }
                break;
                case Keys.A:
                    if (checkbool[1] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[1] = false;
                    }
                break;
                case Keys.S:
                    if (checkbool[2] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[2] = false;
                    }
                break;
                case Keys.D:
                    if (checkbool[3] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[3] = false;
                    }
                break;
                case Keys.Up:
                    if (checkbool[4] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[4] = false;
                    }
                break;
                case Keys.Down:
                    if (checkbool[5] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[5] = false;
                    }
                break;
                case Keys.Left:
                    if (checkbool[6] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[6] = false;
                    }
                break;
                case Keys.Right:
                    if (checkbool[7] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[7] = false;
                    }
                break;
                case Keys.U:
                    if (checkbool[8] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[8] = false;
                    }
                break;
                case Keys.J:
                    if (checkbool[9] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[9] = false;
                    }
                break;
                case Keys.I:
                    if (checkbool[10] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[10] = false;
                    }
                break;
                case Keys.K:
                    if (checkbool[11] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[11] = false;
                    }
                break;
                case Keys.C:
                    if (checkbool[12] == true)
                    {
                        checkbool[12] = false;
                    }
                break;
                case Keys.V:
                    if (checkbool[13] == true)
                    {
                        checkbool[13] = false;
                    }
                break;
                case Keys.NumPad8:
                    if (checkbool[14] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[14] = false;
                    }
                break;
                case Keys.NumPad2:
                    if (checkbool[15] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[15] = false;
                    }
                break;
                case Keys.OemPeriod:
                    if (checkbool[16] == true)
                    {
                        checkbool[16] = false;
                    }
                break;
                case Keys.Space:
                    if (checkbool[17] == true)
                    {
                        checkbool[17] = false;
                    }
                break;
                case Keys.Z:
                    if (checkbool[18] == true)
                    {
                        checkbool[18] = false;
                    }
                break;
                case Keys.X:
                    if (checkbool[19] == true)
                    {
                        checkbool[19] = false;
                    }
                break;
                case Keys.F:
                    if (checkbool[20] == true)
                    {
                        checkbool[20] = false;
                    }
                break;
                case Keys.G:
                    if (checkbool[21] == true)
                    {
                        checkbool[21] = false;
                    }
                break;
                case Keys.R:
                    if (checkbool[22] == true)
                    {
                        checkbool[22] = false;
                    }
                break;
                case Keys.T:
                    if (checkbool[23] == true)
                    {
                        checkbool[23] = false;
                    }
                break;
                case Keys.NumPad7:
                    if (checkbool[24] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[24] = false;
                    }
                break;
                case Keys.NumPad1:
                    if (checkbool[25] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[25] = false;
                    }
                break;
                case Keys.NumPad9:
                    if (checkbool[26] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[26] = false;
                    }
                break;
                case Keys.NumPad3:
                    if (checkbool[27] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[27] = false;
                    }
                break;
                case Keys.NumPad0:
                    if (checkbool[28] == true)
                    {
                        checkbool[28] = false;
                    }
                break;
                case Keys.NumPad4:
                    if (checkbool[29] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[29] = false;
                    }
                break;
                case Keys.NumPad6:
                    if (checkbool[30] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[30] = false;
                    }
                break;
            }
        }

        private void SendMessage(string message)
        {
            if (stream != null && stream.CanWrite)
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }

        private void output_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            output.SelectionStart = output.Text.Length;
            // scroll it automatically
            output.ScrollToCaret();
        }

        private Image RotateImage(Image img, float rotationAngle)
        {
            // Create a new empty bitmap to hold rotated image
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // Make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(bmp);

            // Move rotation point to center of image
            g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);

            // Rotate
            g.RotateTransform(rotationAngle);

            // Move image back
            g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);

            // Draw passed in image onto graphics object
            g.DrawImage(img, new Point(0, 0));

            return bmp;
        }

        private void ipAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            output.Text = "";
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_2(object sender, PaintEventArgs e)
        {

        }

        private void gyroOutput_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
