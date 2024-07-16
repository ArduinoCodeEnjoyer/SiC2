using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
        private Image originalImage2;

        private const int bufferSize = 1024;

        private Dictionary<string, (int row, int col)> prefixToIndexMap;

        private System.Windows.Forms.Timer blinkTimer;

        string msgTemp;

        string speed = "Fast";

        bool[] checkbool = new bool[45];
        String[,] ThermalPrefix = {
            {"A1:","B1:","C1:","D1:","E1:","F1:","G1:","H1:"},
            {"A2:","B2:","C2:","D2:","E2:","F2:","G2:","H2:"},
            {"A3:","B3:","C3:","D3:","E3:","F3:","G3:","H3:"},
            {"A4:","B4:","C4:","D4:","E4:","F4:","G4:","H4:"},
            {"A5:","B5:","C5:","D5:","E5:","F5:","G5:","H5:"},
            {"A6:","B6:","C6:","D6:","E6:","F6:","G6:","H6:"},
            {"A7:","B7:","C7:","D7:","E7:","F7:","G7:","H7:"},
            {"A8:","B8:","C8:","D8:","E8:","F8:","G8:","H8:"},
        };

        Label[,] TempLabel = new Label[8, 8];

        float[,] ThermalValue = new float[8,8];

        string convertedMsg;
        string prefix = "F:";
        string prefix2 = "B:";
        string prefix3 = "G:";
        string prefix4 = "S:";

        int i;
        int j;

        private bool isLabelVisible = true;

        public Form1()
        {
            InitializeComponent();

            originalImage = Image.FromFile("C:\\Users\\use\\Downloads\\WheelRescueGraphic2.png");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = new Bitmap(originalImage);

            originalImage2 = Image.FromFile("C:\\Users\\use\\Downloads\\WheelRescueGraphic1.png");
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = new Bitmap(originalImage2);

            InitializeTemperatureLabels();
            //InitializeThermalValues();
            float minTemp, maxTemp;
            GetMinMaxTemperature(out minTemp, out maxTemp);

            float averageTemp = CalculateAverageTemperature();

            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 500; // Blinking interval in milliseconds
            blinkTimer.Tick += BlinkTimer_Tick;

            //UpdateAllLabels();
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

                    Task.Run(() => ReceiveDataAsync(stream));
                }
                catch
                {
                    output.Text += "\n Error: Could not connect to Arduino Server";
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //output.Text += e.KeyCode;
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    keepRunning = false;
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
                        output.Text += "Shoulder Up: " + e.KeyCode + "\n";
                        SendMessage("NumPad7\n\r");
                        checkbool[24] = true;
                    }
                break;
                case Keys.NumPad1:
                    if (checkbool[25] == false)
                    {
                        output.Text += "Shoulder Down: " + e.KeyCode + "\n";
                        SendMessage("NumPad1\n\r");
                        checkbool[25] = true;
                    }
                break;
                case Keys.NumPad9:
                    if (checkbool[26] == false)
                    {
                        output.Text += "Wrist Up: " + e.KeyCode + "\n";
                        SendMessage("NumPad9\n\r");
                        checkbool[26] = true;
                    }
                break;
                case Keys.NumPad3:
                    if (checkbool[27] == false)
                    {
                        output.Text += "Wrist Down: " + e.KeyCode + "\n";
                        SendMessage("NumPad3\n\r");
                        checkbool[27] = true;
                    }
                break;
                case Keys.NumPad0:
                    if (checkbool[28] == false)
                    {
                        output.Text += "Home Robot Arm: " + e.KeyCode + "\n";
                        SendMessage("NumPad0\n\r");
                        checkbool[28] = true;
                    }
                break;
                case Keys.NumPad4:
                    if (checkbool[29] == false)
                    {
                        output.Text += "Wrist Left: " + e.KeyCode + "\n";
                        SendMessage("NumPad4\n\r");
                        checkbool[29] = true;
                    }
                break;
                case Keys.NumPad6:
                    if (checkbool[30] == false)
                    {
                        output.Text += "Wrist Right: " + e.KeyCode + "\n";
                        SendMessage("NumPad6\n\r");
                        checkbool[30] = true;
                    }
                break;
                case Keys.Tab:
                    if (checkbool[31] == false)
                    { 
                        output.Text += e.KeyCode + "\n";
                        SendMessage("Tab\n\r");
                        checkbool[31] = true;
                    }
                break;
                case Keys.VolumeUp:
                    if (checkbool[32] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("VolumeUp\n\r");
                        checkbool[32] = true;
                    }
                break;
                case Keys.VolumeDown:
                    if (checkbool[33] == false)
                    {
                        output.Text += e.KeyCode + "\n";
                        SendMessage("VolumeDown\n\r");
                        checkbool[33] = true;
                    }
                break;
                case Keys.ControlKey:
                    output.Text += "Enable/Disable Thermistor: " + e.KeyCode + "\n";
                    SendMessage("Control\n\r");
                break;
                case Keys.Add:
                    if (checkbool[35] == false)
                    {
                        output.Text += "Gripping: " + e.KeyCode + "\n";
                        SendMessage("Add\n\r");
                        checkbool[35] = true;
                    }
                break;
                case Keys.Subtract:
                    if (checkbool[36] == false)
                    {
                        output.Text += "Releasing: " + e.KeyCode + "\n";
                        SendMessage("Subtract\n\r");
                        checkbool[36] = true;
                    }
                break;
                case Keys.Decimal:
                    if (checkbool[37] == false)
                    {
                        output.Text += "Gripper Stick: " + e.KeyCode + "\n";
                        SendMessage("Decimal\n\r");
                        checkbool[37] = true;
                    }
                break;
                case Keys.VolumeMute:
                    if (checkbool[38] == false)
                    {
                        output.Text += "Change Speed" + speed + e.KeyCode + "\n";
                        SendMessage("VolumeMute\n\r");
                        checkbool[38] = true;
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
                case Keys.Tab:
                    if (checkbool[31] == true)
                    {
                        checkbool[31] = false;
                    }
                break;
                case Keys.VolumeUp:
                    if (checkbool[32] == true)
                    {
                        output.Text += "VolumeUp\n";
                        checkbool[32] = false;
                    }
                break;
                case Keys.VolumeDown:
                    if (checkbool[33] == true)
                    {
                        output.Text += "VolumeDown\n";
                        checkbool[33] = false;
                    }
                break;
                case Keys.ControlKey:
                    if (checkbool[34] == true)
                    {
                        checkbool[34] = false;
                    }
                break;
                case Keys.Add:
                    if (checkbool[35] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[35] = false;
                    }
                break;
                case Keys.Subtract:
                    if (checkbool[36] == true)
                    {
                        output.Text += "KeyUp\n";
                        SendMessage("KeyUp\n\r");
                        checkbool[36] = false;
                    }
                break;
                case Keys.Decimal:
                    if (checkbool[37] == true)
                    {
                        checkbool[37] = false;
                    }
                break;
                case Keys.VolumeMute:
                    if (checkbool[38] == true)
                    {
                        checkbool[38] = false;
                    }
                break;
            }
        }

        private void SendMessage(string message)
        {
            if (stream != null && stream.CanWrite)
            {
                try
                {
                    byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
                catch {
                    output.Text += "Error Cannot Comunicate with Arduino\n";
                }
            }
        }
        private async Task ReceiveDataAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            try
            {
                while (keepRunning && (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Invoke(new Action(() => decodeReceived(response)));
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => output.Text += $"\nError: {ex.Message}"));
            }
        }

        private void output_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            output.SelectionStart = output.Text.Length;
            // scroll it automatically
            output.ScrollToCaret();
        }
        private void decodeReceived(string msg)
        {
            //output.Text += $"\nReceived: {msg}\n";
            if (msg.StartsWith(prefix))
            {
                string removePrefix = msg.Substring(prefix.Length).Trim();
                if(float.TryParse(removePrefix, out float angle))
                {
                    pictureBox2.Image = RotateImage2(originalImage2, (angle - 90) * -1);
                    frontAngle.Text = string.Format("Front Angle: {0:F1}", angle);
                }
            }
            else if (msg.StartsWith(prefix2))
            {
                string removePrefix = msg.Substring(prefix2.Length).Trim();
                if (float.TryParse(removePrefix, out float angle))
                {
                    pictureBox1.Image = RotateImage(originalImage, angle - 90);
                    backAngle.Text = string.Format("Front Angle: {0:F1}", angle);
                }
            }
            else if (msg.StartsWith(prefix3))
            {
                string removePrefix = msg.Substring(prefix3.Length).Trim();
                IMUout.Text = removePrefix;
                IMUout.TextChanged += IMUout_TextChanged;
            }
            else
            {
                string[] messages = msg.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var message in messages)
                {
                    bool matched = false;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (message.StartsWith(ThermalPrefix[i, j]))
                            {
                                string tempString = message.Substring(ThermalPrefix[i, j].Length).Trim();
                                tempString = RemoveNonNumeric(tempString); // Ensure no non-numeric characters are left

                                if (float.TryParse(tempString, out float temp))
                                {
                                    ThermalValue[i, j] = temp;
                                    UpdateLabel(TempLabel[i, j], temp, float.MinValue, float.MaxValue); // update with dummy min/max first
                                }
                                matched = true;
                                break;
                            }
                        }
                        if (matched)
                        {
                            break;
                        }
                    }
                }

                // Recalculate and update all labels after processing the data
                UpdateAllLabels();
            }
        }

        private void IMUout_TextChanged(object sender, EventArgs e)
        {
            if(IMUout.Text == "OK")
            {
                IMUout.ForeColor = Color.Green;
                blinkTimer.Stop();
            }
            else if(IMUout.Text == "Push Front Flipper")
            {
                IMUout.ForeColor = Color.Red;
                blinkTimer.Start();
            }
            else if(IMUout.Text == "Push Back Flipper")
            {
                IMUout.ForeColor = Color.Red;
                blinkTimer.Start();
            }
        }

        private string RemoveNonNumeric(string input)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char c in input)
            {
                if (char.IsDigit(c) || c == '.' || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private Image RotateImage(Image img, float rotationAngle)
        {
            // Create a new empty bitmap to hold the rotated image
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // Make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(bmp);

            // Move rotation point to the center of the image
            g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);

            // Rotate the image
            g.RotateTransform(rotationAngle);

            // Move the image back
            g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);

            // Draw the passed-in image onto the graphics object
            g.DrawImage(img, new Point(0, 0));

            return bmp;
        }
        private Image RotateImage2(Image img, float rotationAngle)
        {
            // Create a new empty bitmap to hold the rotated image
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            // Make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(bmp);

            // Move rotation point to the center of the image
            g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);

            // Rotate the image
            g.RotateTransform(rotationAngle);

            // Move the image back
            g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);

            // Draw the passed-in image onto the graphics object
            g.DrawImage(img, new Point(0, 0));

            return bmp;
        }

        private void InitializeTemperatureLabels()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Label label = new Label
                    {
                        Size = new Size(60, 30),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Microsoft Sans Serif", 7, FontStyle.Regular),
                        Dock = DockStyle.Fill,
                        Margin = new Padding(1)
                    };

                    tableLayoutPanel1.Controls.Add(label, j, i);
                    TempLabel[i, j] = label;
                }
            }
        }
        private void UpdateLabel(Label label, float temperature, float minTemp, float maxTemp)
        {
            Color color = MapTemperatureToColor(temperature, minTemp, maxTemp);
            MinTemp.Text = "Min Temp : " + minTemp.ToString() + "°C";
            MaxTemp.Text = "Max Temp : " + maxTemp.ToString() + "°C";

            label.BackColor = color;
            label.Text = $"{temperature:F1}°C"; // Display temperature with one decimal point
        }

        private void UpdateAllLabels()
        {
            // Calculate the min and max temperature values
            GetMinMaxTemperature(out float minTemp, out float maxTemp);

            // Update each label with the corresponding temperature value
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    float temperature = ThermalValue[i, j];
                    UpdateLabel(TempLabel[i, j], temperature, minTemp, maxTemp);
                }
            }
        }

        public static Color MapTemperatureToColor(double temperature, double minTemp, double maxTemp)
        {
            Color minColor = Color.Blue;
            Color maxColor = Color.Red;

            temperature = Math.Max(minTemp, Math.Min(maxTemp, temperature));

            double range = maxTemp - minTemp;
            double relativePosition = (temperature - minTemp) / range;

            int red = (int)(minColor.R + (maxColor.R - minColor.R) * relativePosition);
            int green = (int)(minColor.G + (maxColor.G - minColor.G) * relativePosition);
            int blue = (int)(minColor.B + (maxColor.B - minColor.B) * relativePosition);

            return Color.FromArgb(red, green, blue);
        }
        private void InitializeThermalValues()
        {
            // Sample initialization with random values for demonstration
            Random rand = new Random();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    float temp = (float)(rand.NextDouble() * 80 - 30); // Random values between -30 and 50
                    ThermalValue[i, j] = temp;
                }
            }
        }

        private void GetMinMaxTemperature(out float minTemp, out float maxTemp)
        {
            minTemp = float.MaxValue;
            maxTemp = float.MinValue;

            foreach (float temp in ThermalValue)
            {
                if (temp < minTemp) minTemp = temp;
                if (temp > maxTemp) maxTemp = temp;
            }
        }

        private float CalculateAverageTemperature()
        {
            float sum = 0;
            int count = 0;

            foreach (float temp in ThermalValue)
            {
                sum += temp;
                count++;
            }

            return sum / count;
        }

        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            // Toggle the visibility of the label
            isLabelVisible = !isLabelVisible;
            IMUout.Visible = isLabelVisible;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            keepRunning = false;
            base.OnFormClosing(e);
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

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_3(object sender, PaintEventArgs e)
        {

        }

        private void label51_Click(object sender, EventArgs e)
        {

        }

        private void TG4_Click(object sender, EventArgs e)
        {

        }

        private void TF4_Click(object sender, EventArgs e)
        {

        }

        private void TE4_Click(object sender, EventArgs e)
        {

        }

        private void TD4_Click(object sender, EventArgs e)
        {

        }

        private void TC3_Click(object sender, EventArgs e)
        {

        }

        private void TB4_Click(object sender, EventArgs e)
        {

        }

        private void TA4_Click(object sender, EventArgs e)
        {

        }

        private void TA5_Click(object sender, EventArgs e)
        {

        }

        private void TB5_Click(object sender, EventArgs e)
        {

        }

        private void TB6_Click(object sender, EventArgs e)
        {

        }

        private void TA6_Click(object sender, EventArgs e)
        {

        }

        private void TA7_Click(object sender, EventArgs e)
        {

        }

        private void TB7_Click(object sender, EventArgs e)
        {

        }

        private void TC7_Click(object sender, EventArgs e)
        {

        }

        private void TD7_Click(object sender, EventArgs e)
        {

        }

        private void TD6_Click(object sender, EventArgs e)
        {

        }

        private void TF5_Click(object sender, EventArgs e)
        {

        }

        private void TG5_Click(object sender, EventArgs e)
        {

        }

        private void TH5_Click(object sender, EventArgs e)
        {

        }

        private void TH6_Click(object sender, EventArgs e)
        {

        }

        private void TG6_Click(object sender, EventArgs e)
        {

        }

        private void TF7_Click(object sender, EventArgs e)
        {

        }

        private void TG7_Click(object sender, EventArgs e)
        {

        }

        private void MinTemp_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
