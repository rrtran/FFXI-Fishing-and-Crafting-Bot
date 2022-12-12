using AutoIt;
using AutoItX3Lib;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        AutoItX3 au3 = new AutoItX3();
        private LibPcapLiveDeviceList devices = null;
        private int packetIndex = 0;
        private int packetIndex2 = 0;
        private int port = 54090;
        private int port2 = 54091;
        private Queue<int> queue = new Queue<int>(); // When port # is the destination port
        private Queue<List<byte>> udpPayloadDataQueue = new Queue<List<byte>>();
        private Queue<int> queue2 = new Queue<int>(); // When port # is the source port
        private Stack<int> outOfBaitStack = new Stack<int>();
        private Stack<int> fishAgainStack = new Stack<int>();
        private Queue<bool> startedFishAgainQueue = new Queue<bool>();
        private Queue<bool> playedFishingGameQueue = new Queue<bool>();
        private Queue<bool> idleQueue = new Queue<bool>();
        private Queue<bool> equippedBaitQueue = new Queue<bool>();
        private LibPcapLiveDevice device = null;
        private Control control;
        private Control control2;
        private Control control3;
        private Control control4;
        private Control control5;
        private Control control6;
        private string outputFile = "temp";
        private int cin = 4;
        private List<int> mogHouseCompletedCraftValues = new List<int> { 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 138, 164, 165, 166, 182, 183, 184, 185, 186, 187, 188, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 311 };
        private List<int> startCraftValues = new List<int> { 181, 183, 184, 185, 186, 187, 188, 189, 190 };
        private List<int> completeCraftValues = null;
        private List<int> passedCraftValue = new List<int> { 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 255 };
        private List<int> failedCraftValues = new List<int> { 163, 164, 165, 166 };
        private List<int> valueHistory = new List<int>();
        private List<int> windurstWatersFishHookedValues = new List<int> { 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 99, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 367,  381, 445};
        private List<int> windurstWatersPutFishingRodAwayValues = new List<int> { 49, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228,289 };
        private List<int> windurstWatersOutOfBaitValues = new List<int> { 65, 66, 67, 68, 69 };

        private List<int> windurstWoodsFishHookedValues = new List<int> { 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 312, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 418, 419, 420, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 517, 518, 519, 520, 521, 522, 523, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552, 553, 554, 555 };
        private List<int> windurstWoodsPutFishingRodAwayValues = new List<int> { 148, 161, 162, 163, 164, 165, 166, 167, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 239, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390 };
        private List<int> windurstWoodsOutOfBaitValues = new List<int> { };

        private List<int> saromugueChampaignFishHookedValues = new List<int> { 49, 204, 205, 208 };
        private List<int> saromugueChampaignPutFishingRodAwayValues = new List<int> { 146, 147, 159 };
        private List<int> saromugueChampaignOutOfBaitValues = new List<int> { };

        private List<int> westRonfaureFishHookedValues = new List<int> { 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 441, 441, 442, 443, 444, 445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 470, 471, 472, 473, 474, 475, 476, 477, 478, 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 495, 496, 497, 498, 499, 500, 501, 502, 503, 504, 505 };
        private List<int> westRonfaurePutFishingRodAwayValues = new List<int> { 145, 146, 147, 148, 149, 182, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359, 360, 361, 362, 363, 364, 365, 366, 367, 368, 369, 370, 371, 372, 373, 374, 375, 376, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386, 387, 388, 389 };
        private List<int> westRonfaureOutOfBaitValues = new List<int> { };

        private List<int> cureCastedOnMeValues = new List<int> { 181, 182, 183, 184 };
        private List<int> regen2CastedOnMeValues = new List<int> { 332, 333, 334, 335, 336, 337, 338 };

        private List<int> actionMessageValues = new List<int> { 76, 88 };
        private List<int> eastSarubarutaCureCastedOnMeValues = new List<int> { 164, 185, 186, 187, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280, 281, 282, 283, 284, 285, 286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297, 298, 299, 300, 301, 302, 303, 304, 305, 343, 344, 345, 346, 347, 348, 349, 350, 351, 352, 353, 354, 355, 356, 357, 358, 359 };
        private List<int> eastSarubarutaRegen2CastedOnMeValues = new List<int> { };
        private Queue<string> lastSpellCasted = new Queue<string>();

        private List<int> fishHookedValues = null;
        private List<int> putFishingRodAwayValues = null;
        private List<int> outOfBaitValues = null;
        private List<int> fishMessageValues = new List<int> { 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93 };
        private List<int> craftMessageValues = new List<int> { 80, 81, 82, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150};
        private bool logData = false;

        private string command = @"C:\Program Files\AutoHotkey\AutoHotkey.exe";
        private string craftArgs = @"Autohotkey\StartCraft.exe";
        private string craftArgs2 = @"Autohotkey\StartCraft2.exe";
        private string fishArgs = @"Autohotkey\ThrowFishingLine.exe";
        private string fishArgs2 = @"Autohotkey\FishingGame.exe";
        private string fishArgs3 = @"Autohotkey\EquipBait.exe";
        private string fishArgs4 = @"Autohotkey\AltMacro1Warp.exe";

        bool macroStarted = false;
        int macroDuration = 7000;
        private int count = 0;
        private Random randomNumberGenerator = new Random();
        Process process = null;

        List<string> badFeelingImages = new List<string>();

        public Form1()
        {
            InitializeComponent();
            badFeelingImages.Add(@".\images\1920_1080-bad-feeling-2.png");
            badFeelingImages.Add(@".\images\1920_1080-bad-feeling-3.png");
            badFeelingImages.Add(@".\images\1920_1080-bad-feeling-4.png");
            badFeelingImages.Add(@".\images\1920_1080-bad-feeling-5.png");
            badFeelingImages.Add(@".\images\1920_1080-bad-feeling-6.png");
            badFeelingImages.Add(@".\images\1920_1080-bad-feeling-7.png");
            badFeelingImages.Add(@".\images\1920_1080-bad-feeling-8.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-1.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-2.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-3.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-4.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-5.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-6.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-7.png");
            badFeelingImages.Add(@".\images\1600_900-bad-feeling-8.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-1.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-2.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-3.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-4.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-5.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-6.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-7.png");
            badFeelingImages.Add(@".\images\1280_720-bad-feeling-8.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-1.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-2.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-3.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-4.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-5.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-6.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-7.png");
            badFeelingImages.Add(@".\images\1366_768-bad-feeling-8.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-1.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-2.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-3.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-4.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-5.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-6.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-7.png");
            badFeelingImages.Add(@".\images\1680_1050-bad-feeling-8.png");
            control = textBox1;
            control2 = textBox2;
            control3 = button5;
            control4 = checkBox1;
            control5 = checkBox57;
            control6 = checkBox58;

            devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count < 1)
            {
                textBox1.AppendText("No devices were found on this machine.");
                return;
            }

            textBox1.AppendText("Select your wireless or wired network adapter from the dropdown menu and click the 'Capture Packet' button...");
            textBox1.AppendText(Environment.NewLine);

            for (int i = 0; i < devices.Count; i++)
            {
                comboBox1.Items.Add(devices[i].Description);
            }
        }

        private void appendText(string s)
        {
            control.BeginInvoke((MethodInvoker)delegate ()
            {
                DateTime timestamp = DateTime.Now;
                textBox1.AppendText(timestamp.ToString() + " " + s);
                textBox1.AppendText(Environment.NewLine);
            });
        }

        private void appendText2(string s)
        {
            control.BeginInvoke((MethodInvoker)delegate ()
            {
                DateTime timestamp = DateTime.Now;
                textBox2.AppendText(timestamp.ToString() + " " + s);
                textBox2.AppendText(Environment.NewLine);
            });
        }

        public void setCheckBox1(bool booleanVariable)
        {
            control.BeginInvoke((MethodInvoker)delegate ()
            {
                checkBox1.Checked = booleanVariable;
            });
        }

        public bool getCheckBox1()
        {
            return checkBox1.Checked;
        }

        public bool getCheckBox58()
        {
            return checkBox58.Checked;
        }

        public void setCheckBox57(bool noCheckmarkOrCheckmark)
        {
           control5.BeginInvoke((MethodInvoker)delegate ()
           {
               checkBox57.Checked = noCheckmarkOrCheckmark;
           });
            
        }

        public void setCheckBox58(bool noCheckmarkOrCheckmark)
        {
           control6.BeginInvoke((MethodInvoker)delegate ()
           {
               checkBox58.Checked = noCheckmarkOrCheckmark;
           }); 
        }

        private void stopFishing()
        {
            control3.BeginInvoke((MethodInvoker)delegate ()
            {
                button5.PerformClick();
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void device_OnPacketArrival(object sender, PacketCapture e)
        {
            RawCapture rawPacket = e.GetPacket();
            if (rawPacket.LinkLayerType == PacketDotNet.LinkLayers.Ethernet)
            {
                Packet packet = PacketDotNet.Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                EthernetPacket ethernetPacket = (EthernetPacket)packet;
                IPv4Packet ipv4Packet = ethernetPacket.Extract<IPv4Packet>();
                UdpPacket udpPacket = ipv4Packet.Extract<UdpPacket>();
                if (((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).DestinationPort == port ||
                    ((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).DestinationPort == port2)
                {
                    int length = ((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).PayloadData.Length;

                    queue.Enqueue(length);
                    List<byte> list = new List<byte>();
                    for (int i = 0; i < udpPacket.PayloadData.Length; i++)
                    {
                        list.Add(udpPacket.PayloadData[i]);
                    }
                    udpPayloadDataQueue.Enqueue(list);
                    if (udpPayloadDataQueue.Count > 50)
                    {
                        udpPayloadDataQueue.Dequeue();
                    }
                    if (queue.Count > 50)
                    {
                        queue.Dequeue();
                    }
                }

                if (((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).SourcePort == port ||
                    ((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).SourcePort == port2)
                {
                    int length = ((UdpPacket)(ethernetPacket.PayloadPacket.PayloadPacket)).PayloadData.Length;

                    queue.Enqueue(length);
                    List<byte> list = new List<byte>();
                    for (int i = 0; i < udpPacket.PayloadData.Length; i++)
                    {
                        list.Add(udpPacket.PayloadData[i]);
                    }
                    udpPayloadDataQueue.Enqueue(list);
                    if (udpPayloadDataQueue.Count > 50)
                    {
                        udpPayloadDataQueue.Dequeue();
                    }
                    if (queue.Count > 50)
                    {
                        queue.Dequeue();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (device == null)
            {
                // do nothing
            }
            else if (device.Started == true)
            {
                device.StopCapture();
                if (device.Opened == true)
                {
                    device.Close();
                    device.Dispose();
                }
                else
                {
                    device.Dispose();
                }
            }
            else if (device.Opened == true)
            {
                device.Close();
                device.Dispose();
            }
            else
            {
                device.Dispose();
            }

            if (comboBox1.SelectedItem == null)
            {
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                textBox1.AppendText("Nothing selected in dropdown menu, choose an option and then click the button!");
                textBox1.AppendText(Environment.NewLine);
            }
            else
            {
                cin = comboBox1.SelectedIndex;
                device = devices[cin];
                device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);

                int readTimeoutInMilliseconds = 1000;
                device.Open(DeviceModes.Promiscuous | DeviceModes.DataTransferUdp | DeviceModes.NoCaptureLocal, readTimeoutInMilliseconds);

                string filter = "ip and udp";
                device.Filter = filter;

                appendText("Listening on " + device.Name + " " + device.Description);
                device.StartCapture();

                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = false;
                button4.Enabled = true;
                button5.Enabled = false;
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private bool craft = false;
        private Queue<bool> completedCraftQueue = new Queue<bool>();
        private Queue<bool> startedCraftQueue = new Queue<bool>();
        private int startedCraftCooldown = 15000;
        private int craftIdleCooldown = 40000;
        private void button2_Click(object sender, EventArgs e)
        {
            if (craft == false)
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = true;
                button4.Enabled = false;
                button5.Enabled = false;

                appendText("Start craft bot");

                if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
                {
                    if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                    {
                        AutoItX.WinActivate("[CLASS:FFXiClass]");
                    }
                }

                craft = true;
            }

            new Thread(() =>
            {
                while (craft == true)
                {
                    if (isReadyToCraft() == true)
                    {
                        const int START_CRAFT_PRIORITY = 1;
                        if (craftTable.Contains("Start Craft") == false)
                        {
                            Class3 craft = new Class3("Start Craft", control, textBox1);
                            craftTable.Add(craft.getAction(), true);
                            craftPriorityQueue.insert(START_CRAFT_PRIORITY, craft);
                        }
                        else
                        {
                            appendText("Start craft action is queued");
                        }
                    }
                    else if (isCraftResultsOnScreen() == true)
                    {
                        const int DISMISS_CRAFT_RESULTS = 2;
                        if (craftTable.Contains("Start Craft") == false)
                        {
                            if (craftTable.Contains("Dismiss Results") == false)
                            {
                                Class3 dismissResults = new Class3("Dismiss Results", control, textBox1);
                                craftTable.Add(dismissResults.getAction(), true);
                                craftPriorityQueue.insert(DISMISS_CRAFT_RESULTS, dismissResults);
                            }
                            else
                            {
                                appendText("Dismiss results action is queued");
                            }
                        }
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (craft == true)
                {
                    if (craftPriorityQueue.size() > 0)
                    {
                        Class3 action = craftPriorityQueue.getData();
                        craftPriorityQueue.remove();
                        action.function1();
                        craftTable.Remove(action.getAction());
                    }
                }
            }).Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = true;
            button5.Enabled = false;
            appendText("Stop craft bot");
            craft = false;
        }

        [DllImport(@".\lib\ImageSearchDLL.dll")]
        public static extern IntPtr ImageSearch(int x, int y, int right, int bottom, [MarshalAs(UnmanagedType.LPStr)] string imagePath);

        public static string[] UseImageSearch(int left, int top, int right, int bottom, string tolerance, string imgPath)
        {
            imgPath = "*" + tolerance + " " + imgPath;

            IntPtr result = ImageSearch(left, top, right, bottom, imgPath);
            string res = Marshal.PtrToStringAnsi(result);

            if (res[0] == '0') return null;

            string[] data = res.Split('|');

            int x; int y;
            int.TryParse(data[1], out x);
            int.TryParse(data[2], out y);

            return data;
        }

        bool fish = false;
        int equippedBaitMacroDuration = 22000;
        int startedFishAgainMacroDuration = 27500;
        int PlayedFishingGameMacroDuration = 3000;
        private int minigameCooldownAfterStartedFishing = 9000;
        int fishAgainAfterMiniGameCooldown = 7000;
        int idleCooldown = 35000;
        string lastAction = "";
        bool fight = false;
        bool wait = false;
        bool move = false;
        int camera360DegreeRotationInMilliseoncds = 3800;
        Rectangle windowRectangle = new Rectangle();
        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = true;
            button6.Enabled = false;
            button7.Enabled = false;

            if (fish == false)
            {
                appendText("Starting fish bot");
                if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
                {
                    if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                    {
                        AutoItX.WinActivate("[CLASS:FFXiClass]");
                    }
                }
                windowRectangle = AutoItX.WinGetPos("[CLASS:FFXiClass]");
                fish = true;
            }

            new Thread(() =>
            {
                while (fish == true)
                {
                    if (fishPriorityQueue.size() > 0)
                    {
                        Class2 action = fishPriorityQueue.getData();
                        fishPriorityQueue.remove();
                        action.function1();
                        fishTable.Remove(action.getAction());
                    }
                    else
                    {
                        appendText("Queue is empty");
                    }
                    Thread.Sleep(1000);
                }
            }).Start();

            new Thread(() =>
            {
                while (fish == true)
                {
                    if (isReadyToFish() == true)
                    {
                        const int START_FISH_PRIORITY = 1;
                        if (fishTable.Contains("Start Fish") == false)
                        {
                            appendText("Ready to fish");
                            Class2 fish = new Class2("Start Fish", control, textBox1, windowRectangle);
                            fishTable.Add(fish.getAction(), true);
                            fishPriorityQueue.insert(START_FISH_PRIORITY, fish);
                        }
                        else
                        {
                            appendText("Start fishing action is queued");
                        }
                    }
                    else if (isFishingGaugeOnScreen() == true || isSomethingPullingAtLine() == true || isSomethingClampsOntoYourLineFerociously() == true)
                    {
                        const int FISH_GAME_PRIORITY = 2;
                        if (fishTable.Contains("Fish Game") == false)
                        {
                            appendText("Play fish mini-game");
                            Class2 fishGame = new Class2("Fish Game", control, textBox1, windowRectangle);
                            fishTable.Add(fishGame.getAction(), true);
                            fishPriorityQueue.insert(FISH_GAME_PRIORITY, fishGame);
                        }
                        else
                        {
                            appendText("Play fishing game action is queued");
                        }
                    }
                    Thread.Sleep(1000);
                }
            }).Start();


            //new Thread(() =>
            //{
            //    while (fish == true)
            //    {
            //        if (isFishingCapReached() == true)
            //        {
            //            fish = false;
            //            control.BeginInvoke((MethodInvoker)delegate ()
            //            {
            //                button4.Enabled = true;
            //                button5.Enabled = false;
            //            });
            //        }
            //    }
            //}).Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = true;
            button5.Enabled = false;
            button6.Enabled = true;
            button7.Enabled = false;
            appendText("Stopping fish bot");
            fish = false;
        }

        void press_ESC()
        {
            AutoItX.Send("{ESC}");
        }

        void press_F7()
        {
            AutoItX.Send("{F7}");
        }

        void press_Down()
        {
            AutoItX.Send("{Down}");
        }

        void press_Enter()
        {
            AutoItX.Send("{Enter}");
        }

        void press_d()
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                    AutoItX.Send("{d}");
                }
                else
                {
                    AutoItX.Send("{d}");
                }
            }
        }

        void press_a()
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                    AutoItX.Send("{a}");
                }
                else
                {
                    AutoItX.Send("{a}");
                }
            }
        }

        void startFishing()
        {
            press_ESC();
            Thread.Sleep(250);
            press_ESC();
            Thread.Sleep(250);
            press_ESC();
            Thread.Sleep(250);
            press_ESC();

            press_F7();
            Thread.Sleep(250);
            press_Enter();
            Thread.Sleep(1000);

            press_Down();
            Thread.Sleep(100);
            press_Down();
            Thread.Sleep(100);
            press_Down();
            Thread.Sleep(100);
            press_Down();
            Thread.Sleep(100);
            press_Down();
            Thread.Sleep(100);
            press_Down();
            Thread.Sleep(100);
            press_Enter();
        }

        public bool isFishingCapReached()
        {
            string[] fishingLimitReached = { @".\images\fishing_cap_reached.png" };
            bool found = false;
            Rectangle rectangle = new Rectangle(20, 590, 235, 710);

            for (int i = 0; i < fishingLimitReached.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", fishingLimitReached[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isSomethingClampsOntoYourLineFerociously()
        {
            string[] fishImages = { "" };
            Rectangle fishRectangle = new Rectangle();
            if (windowRectangle.Width == 1382)
            {
                fishImages[0] = @".\images\something_clamps_onto_your_line_ferociously.png";
                //caughtHookImage[1] = @".\images\something_pulling_at_the_line.png";
                fishRectangle = new Rectangle(0, 665, 650, 715);
            }
            else if (windowRectangle.Width == 1936)
            {
                fishImages[0] = @".\images\something_pulling_at_the_line_1920_1080.png";
                //caughtHookImage[1] = @".\images\something_pulling_at_the_line_1920_1080.png";
                fishRectangle = new Rectangle(0, 990, 260, 1015);
            }
            else
            {
                return false;
            }

            bool found = false;

            for (int i = 0; i < fishImages.Length; i++)
            {
                string[] results = UseImageSearch(fishRectangle.X, fishRectangle.Y, fishRectangle.Width, fishRectangle.Height, "30", fishImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isSomethingPullingAtLine()
        {
            string[] somethingPullingAtLineImage = { "" };
            Rectangle somethingPullingAtLineRectangle = new Rectangle();
            if (windowRectangle.Width == 1382)
            {
                somethingPullingAtLineImage[0] = @".\images\something_pulling_at_the_line.png";
                //caughtHookImage[1] = @".\images\something_pulling_at_the_line.png";
                somethingPullingAtLineRectangle = new Rectangle(0, 665, 650, 715);
            }
            else if (windowRectangle.Width == 1936)
            {
                somethingPullingAtLineImage[0] = @".\images\something_pulling_at_the_line_1920_1080.png";
                //caughtHookImage[1] = @".\images\something_pulling_at_the_line_1920_1080.png";
                somethingPullingAtLineRectangle = new Rectangle(0, 990, 260, 1015);
            }
            else
            {
                return false;
            }

            bool found = false;

            for (int i = 0; i < somethingPullingAtLineImage.Length; i++)
            {
                string[] results = UseImageSearch(somethingPullingAtLineRectangle.X, somethingPullingAtLineRectangle.Y, somethingPullingAtLineRectangle.Width, somethingPullingAtLineRectangle.Height, "30", somethingPullingAtLineImage[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isFishingGaugeOnScreen()
        {
            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int width = windowDimensionInfo.Width;
            int height = windowDimensionInfo.Height;
            string[] fishGaugeResults = UseImageSearch((int)(width / 3), 0, width - (int)(width / 3), (int)(height / 2), "10", @".\images\fish-gauge-pixel.png");
            if (fishGaugeResults == null)
            {
                return false;
            }
            else
            {
                appendText("Found fishing gauge");
                return true;
            }
        }

        void doFishingGame()
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");

                }
                Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
                int width = windowDimensionInfo.Width;
                int height = windowDimensionInfo.Height;
                string[] fishGaugeResults = UseImageSearch((int)(width / 3), 0, width - (int)(width / 3), (int)(height / 2), "10", @".\images\fish-gauge-pixel.png");
                if (fishGaugeResults == null)
                {
                    appendText("Did not see fishing gague.");
                    appendText2("Did not see fishing gague.");
                }
                else
                {
                    while (fishGaugeResults != null)
                    {
                        string[] arrowResults = UseImageSearch((int)(width * .21), (int)(height * .29), (int)(width * .37), (int)(height * .53), "3", @".\images\gold-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            press_a();
                        }

                        arrowResults = UseImageSearch((int)(width * .6), (int)(height * .29), (int)(width * .8), (int)(height * .53), "3", @".\images\gold-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            press_d();
                        }

                        arrowResults = UseImageSearch((int)(width * .21), (int)(height * .29), (int)(width * .37), (int)(height * .53), "3", @".\images\silver-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            press_a();
                        }

                        arrowResults = UseImageSearch((int)(width * .6), (int)(height * .29), (int)(width * .8), (int)(height * .53), "3", @".\images\silver-arrow-pixel.png");
                        if (arrowResults == null)
                        {
                            // Nothing to process
                        }
                        else
                        {
                            press_d();
                        }
                        fishGaugeResults = UseImageSearch((int)(width / 3), 0, width - (int)(width / 3), (int)(height / 2), "10", @".\images\fish-gauge-pixel.png");
                    }
                }
                press_Enter();
            }
        }

        bool isItABadFeeling()
        {
            bool isBadFeeling = false;
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
                Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
                Thread.Sleep(1000);

                string[] badFeelingResults = UseImageSearch(0, (int)(windowDimensionInfo.Height * 0.95), (int)(windowDimensionInfo.Width * .5), windowDimensionInfo.Height, "30", badFeelingImages[0]);
                int i = 1;
                while (badFeelingResults == null && i < badFeelingImages.Count())
                {
                    badFeelingResults = UseImageSearch(0, (int)(windowDimensionInfo.Height * 0.95), (int)(windowDimensionInfo.Width * .5), windowDimensionInfo.Height, "30", badFeelingImages[i]);
                    if (badFeelingResults == null)
                    {

                    }
                    else
                    {
                        appendText(badFeelingImages[i]);
                        isBadFeeling = true;
                    }
                    i++;
                }

                return isBadFeeling;
            }
            else
            {
                appendText("Exited for no reason...");
                return true;
            }
        }

        bool findEnemy(ref int enemyX, ref int enemyY)
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }

                string[] results = UseImageSearch(990, 80, 1240, 320, "30", @".\images\mini-map-enemy.png");
                if (results == null)
                {
                    return false;
                }
                else
                {
                    enemyX = int.Parse(results[1]);
                    enemyY = int.Parse(results[2]);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        bool findPlayer(ref int playerX, ref int playerY)
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }

                string[] results = UseImageSearch(990, 80, 1240, 320, "30", @".\images\mini-map-player.png");
                if (results == null)
                {
                    return false;
                }
                else
                {
                    playerX = int.Parse(results[1]);
                    playerY = int.Parse(results[2]);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        bool findBook(ref int bookX, ref int bookY)
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }

                string[] results = UseImageSearch(990, 80, 1240, 320, "30", @".\images\mini-map-book.png");
                if (results == null)
                {
                    return false;
                }
                else
                {
                    bookX = int.Parse(results[1]);
                    bookY = int.Parse(results[2]);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        bool selectSynthesis()
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            string[] results = UseImageSearch((int)(windowDimensionInfo.Width / 2), 0, windowDimensionInfo.Width, (int)(windowDimensionInfo.Height / 2), "3", @".\images\main-menu-pixel.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                results = UseImageSearch((int)(windowDimensionInfo.Width / 2), 0, windowDimensionInfo.Width, (int)(windowDimensionInfo.Height / 2), "30", @".\images\synthesis.png");
                if (results == null)
                {
                    int i = 0;
                    while (results == null && i < 11)
                    {
                        AutoItX.Send("{Down}");
                        Thread.Sleep(50);
                        results = UseImageSearch((int)(windowDimensionInfo.Width / 2), 0, windowDimensionInfo.Width, (int)(windowDimensionInfo.Height / 2), "3", @".\images\synthesis.png");
                        i++;
                    }

                    if (results == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        bool isPartyMemberRedHP(int partyMember, int partySize)
        {
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 595;
                    width = 1340;
                    height = 610;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 5)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 6)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 5)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 665;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] redHPImages = { @".\images\player-red-hp-bar-pixel.png", 
                @".\images\player-red-hp-bar-pixel2.png", 
                @".\images\player-red-hp-bar-pixel3.png", 
                @".\images\player-red-hp-bar-pixel4.png",
                @".\images\player-red-hp-bar-pixel5.png",
                @".\images\player-red-hp-bar-pixel6.png",
                @".\images\player-red-hp-bar-pixel7.png",
                @".\images\player-red-hp-bar-pixel8.png",
                @".\images\player-red-hp-bar-pixel9.png",
                @".\images\player-red-hp-bar-pixel10.png" };

            bool found = false;

            for (int i = 0; i < redHPImages.Length; i++) {
                string[] results = UseImageSearch(x, y, width, height, "0", redHPImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            return found;
        }

        bool isPartyMemberOrangeHP(int partyMember, int partySize)
        {
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 595;
                    width = 1340;
                    height = 610;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 5)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 6)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 5)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 665;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] orangeHpImages = { @".\images\player-orange-hp-bar-pixel.png",
                @".\images\player-orange-hp-bar-pixel2.png",
                @".\images\player-orange-hp-bar-pixel3.png",
                @".\images\player-orange-hp-bar-pixel4.png",
                @".\images\player-orange-hp-bar-pixel5.png",
                @".\images\player-orange-hp-bar-pixel6.png",
                @".\images\player-orange-hp-bar-pixel7.png",
                @".\images\player-orange-hp-bar-pixel8.png",
                @".\images\player-orange-hp-bar-pixel9.png" };

            bool found = false;

            for (int i = 0; i < orangeHpImages.Length; i++)
            {
                string[] results = UseImageSearch(x, y, width, height, "0", orangeHpImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isPartyMemberYellowHP(int partyMember, int partySize)
        {
            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 595;
                    width = 1340;
                    height = 610;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 5)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 6)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 5)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 4)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 655;
                    width = 1340;
                    height = 665;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 3)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 2)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    x = 1290;
                    y = 690;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] yellowHpImages = { @".\images\player-yellow-hp-bar-pixel.png", 
                @".\images\player-yellow-hp-bar-pixel-2.png", 
                @".\images\player-yellow-hp-bar-pixel-3.png",
                @".\images\player-yellow-hp-bar-pixel-4.png",
                @".\images\player-yellow-hp-bar-pixel-5.png",
                @".\images\player-yellow-hp-bar-pixel-6.png",
                @".\images\player-yellow-hp-bar-pixel-7.png",
                @".\images\player-yellow-hp-bar-pixel-8.png",
                @".\images\player-yellow-hp-bar-pixel-9.png",
                @".\images\player-yellow-hp-bar-pixel-10.png",
                @".\images\player-yellow-hp-bar-pixel-11.png" };

            bool found = false;
            for (int i = 0; i < yellowHpImages.Length; i++)
            {
                string[] results = UseImageSearch(x, y, width, height, "0", yellowHpImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == false)
            {
                return false;
            }
            else
            {
                appendText("********** Party member " + partyMember + " is in yellow hp ********");
                return true;
            }
            
        }

        bool isHpLow()
        {
            string[] myHPImages = { @".\images\self-pink-hp-pixel.png", @".\images\self-pink-hp-pixel-2.png" };

            Rectangle rectangle = new Rectangle(90, 75, 110, 85);
            bool found = false;

            for (int i = 0; i < myHPImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", myHPImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isBlackScreen()
        {
            string[] blackScreenImages = { @".\images\black_screen.png" };

            bool found = false;

            for (int i = 0; i < blackScreenImages.Length; i++)
            {
                string[] results = UseImageSearch(0, 0, 500, 500, "0", blackScreenImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("Changing zones");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isDead()
        {
            string[] deadImages = { @".\images\time_left.png" };

            bool found = false;
            for (int i = 0; i < deadImages.Length; i++)
            {
                string[] results = UseImageSearch(0, 0, 171, 168, "30", deadImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                appendText("I died");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isPartyMemberDead(int partyMember, int partySize)
        {
            Rectangle rectangle = getPartyMemberDeadRectangle(partyMember, partySize);

            string[] partyMemberDeadImages = { @".\images\player-dead-hp-bar-pixel-2a.png", @".\images\player-dead-hp-bar-pixel-3a.png", @".\images\player-dead-hp-bar-pixel-4a.png", @".\images\player-dead-hp-bar-pixel-5a.png", @".\images\player-dead-hp-bar-pixel-6a.png" };

            bool found = false;
            for (int i = 0; i < partyMemberDeadImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", partyMemberDeadImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Player " + partyMember + " is dead");
                return true;
            }
            else
            {
                return false;
            }
        }

        int getPartySize()
        {
            int left = 1240;
            int top = 590;
            int right = 1280;
            int bottom = 610;
            string[] results = UseImageSearch(left, top, right, bottom, "30", @".\images\partylist6_plum.png");
            if (results == null)
            {
            }
            else
            {
                return 6;
            }

            left = 1240;
            top = 610;
            right = 1280;
            bottom = 630;
            results = UseImageSearch(left, top, right, bottom, "50", @".\images\partylist5_plum.png");
            if (results == null)
            {
            }
            else
            {
                return 5;
            }

            left = 1240;
            top = 630;
            right = 1280;
            bottom = 650;
            results = UseImageSearch(left, top, right, bottom, "50", @".\images\partylist4_plum.png");
            if (results == null)
            {

            }
            else
            {
                return 4;
            }

            left = 1240;
            top = 650;
            right = 1280;
            bottom = 670;
            results = UseImageSearch(left, top, right, bottom, "50", @".\images\partylist3_plum.png");
            if (results == null)
            {

            }
            else
            {
                return 3;
            }

            left = 1240;
            top = 670;
            right = 1280;
            bottom = 690;
            results = UseImageSearch(left, top, right, bottom, "50", @".\images\partylist2_plum.png");
            if (results == null)
            {

            }
            else
            {
                return 2;
            }

            return 1;
        }

        bool isPartyMemberToppedOff(int partyMember, int partySize)
        {
            if (File.Exists(@".\images\player-pink-hp-bar-pixel.png") == false)
            {
                appendText(@".\images\player-pink-hp-bar-pixel.png does not exists");
                return false;
            }

            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    x = 1325;
                    y = 600;
                    width = 1340;
                    height = 610;
                }
                else if (partyMember == 2)
                {
                    x = 1325;
                    y = 620;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 3)
                {
                    x = 1325;
                    y = 640;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 4)
                {
                    x = 1325;
                    y = 660;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 5)
                {
                    x = 1325;
                    y = 675;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 6)
                {
                    x = 1325;
                    y = 695;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    x = 1325;
                    y = 620;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 2)
                {
                    x = 1325;
                    y = 640;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 3)
                {
                    x = 1325;
                    y = 660;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 4)
                {
                    x = 1325;
                    y = 675;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 5)
                {
                    x = 1325;
                    y = 695;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    x = 1325;
                    y = 640;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 2)
                {
                    x = 1325;
                    y = 660;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 3)
                {
                    x = 1325;
                    y = 675;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 4)
                {
                    x = 1325;
                    y = 695;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    x = 1325;
                    y = 660;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 2)
                {
                    x = 1325;
                    y = 675;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 3)
                {
                    x = 1325;
                    y = 695;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    x = 1325;
                    y = 675;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 2)
                {
                    x = 1325;
                    y = 695;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    x = 1325;
                    y = 695;
                    width = 1340;
                    height = 705;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] results = UseImageSearch(x, y, width, height, "50", @".\images\player-pink-hp-bar-pixel.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        bool maintainPartyMemberMP(int partyMember, int partySize, bool partyMemberChecked)
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
            }

            if (isPartyMemberMPToppedOff(partyMember, partySize) == false && partyMemberChecked == true)
            {
                string message = "Party member " + partyMember + " is not at full MP";
                appendText(message);
                if (refresh2CooldownQueue.Count == 0 && checkBox26.Checked == true)
                {
                    if (partyMemberRefresh2Timer[partyMember].Count == 0)
                    {
                        castRefresh2(partyMember, partySize);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (refreshCooldownQueue.Count == 0 && checkBox18.Checked == true)
                {
                    if (partyMemberRefreshTimer[partyMember].Count == 0)
                    {
                        castRefresh(partyMember, partySize);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        bool isPartyMemberMPToppedOff(int partyMember, int partySize)
        {
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    left = 1330;
                    top = 605;
                    right = 1350;
                    bottom = 625;
                }
                else if (partyMember == 2)
                {
                    left = 1330;
                    top = 630;
                    right = 1350;
                    bottom = 640;
                }
                else if (partyMember == 3)
                {
                    left = 1330;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 4)
                {
                    left = 1330;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 5)
                {
                    left = 1330;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 6)
                {
                    left = 1330;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    left = 1330;
                    top = 630;
                    right = 1350;
                    bottom = 640;
                }
                else if (partyMember == 2)
                {
                    left = 1330;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 3)
                {
                    left = 1330;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 4)
                {
                    left = 1330;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 5)
                {
                    left = 1330;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    left = 1330;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 2)
                {
                    left = 1330;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 3)
                {
                    left = 1330;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 4)
                {
                    left = 1330;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    left = 1330;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 2)
                {
                    left = 1330;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 3)
                {
                    left = 1330;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    left = 1330;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 2)
                {
                    left = 1330;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    left = 1330;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] greenMpPixelImages = { @".\images\player-green-mp-bar-pixel.png",
                @".\images\player-green-mp-bar-pixel-2.png",
                @".\images\player-green-mp-bar-pixel-3.png",
                @".\images\player-green-mp-bar-pixel-4.png",
                @".\images\player-green-mp-bar-pixel-5.png",
                @".\images\player-green-mp-bar-pixel-6.png",
                @".\images\player-green-mp-bar-pixel-7.png",
                @".\images\player-green-mp-bar-pixel-8.png" };

            bool found = false;
            for (int i = 0; i < greenMpPixelImages.Length; i++)
            {
                string[] results = UseImageSearch(left, top, right, bottom, "3", greenMpPixelImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                appendText("***** PARTY MEMBER MP IS NOT TOPPED OFF ********");
                return false;
            }
        }

        bool isMPLow(int partyMember, int partySize)
        {
            string[] mpPixelImages = { @".\images\self-pink-mp-pixel.png", @".\images\self-pink-mp-pixel-2.png" };
            
            Rectangle rectangle = new Rectangle(55, 83, 110, 94);
            bool found = false;
            for (int i = 0; i < mpPixelImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", mpPixelImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isMPLow2(int partyMember, int partySize)
        {
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    left = 1295;
                    top = 605;
                    right = 1350;
                    bottom = 625;
                }
                else if (partyMember == 2)
                {
                    left = 1295;
                    top = 630;
                    right = 1350;
                    bottom = 640;
                }
                else if (partyMember == 3)
                {
                    left = 1295;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 4)
                {
                    left = 1295;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 5)
                {
                    left = 1295;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 6)
                {
                    left = 1295;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    left = 1295;
                    top = 630;
                    right = 1350;
                    bottom = 640;
                }
                else if (partyMember == 2)
                {
                    left = 1295;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 3)
                {
                    left = 1295;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 4)
                {
                    left = 1295;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 5)
                {
                    left = 1295;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    left = 1295;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 2)
                {
                    left = 1295;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 3)
                {
                    left = 1295;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 4)
                {
                    left = 1295;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    left = 1295;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 2)
                {
                    left = 1295;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 3)
                {
                    left = 1295;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    left = 1295;
                    top = 680;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 2)
                {
                    left = 1295;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    left = 1295;
                    top = 705;
                    right = 1350;
                    bottom = 713;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] mpPixelImages = { @".\images\player-green-mp-bar-pixel.png",
                @".\images\player-green-mp-bar-pixel-2.png",
                @".\images\player-green-mp-bar-pixel-3.png",
                @".\images\player-green-mp-bar-pixel-4.png",
                @".\images\player-green-mp-bar-pixel-5.png",
                @".\images\player-green-mp-bar-pixel-6.png",
                @".\images\player-green-mp-bar-pixel-7.png",
                @".\images\player-green-mp-bar-pixel-8.png" };

            bool found = false;

            for (int i = 0; i < mpPixelImages.Length; i++)
            {
                string[] results = UseImageSearch(left, top, right, bottom, "3", mpPixelImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isHPToppedOff(int partyMember, int partySize)
        {
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    left = 1326;
                    top = 600;
                    right = 1350;
                    bottom = 610;
                }
                else if (partyMember == 2)
                {
                    left = 1326;
                    top = 617;
                    right = 1350;
                    bottom = 627;
                }
                else if (partyMember == 3)
                {
                    left = 1326;
                    top = 634;
                    right = 1350;
                    bottom = 644;
                }
                else if (partyMember == 4)
                {
                    left = 1326;
                    top = 655;
                    right = 1350;
                    bottom = 665;
                }
                else if (partyMember == 5)
                {
                    left = 1326;
                    top = 672;
                    right = 1350;
                    bottom = 682;
                }
                else if (partyMember == 6)
                {
                    left = 1326;
                    top = 692;
                    right = 1350;
                    bottom = 702;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    left = 1326;
                    top = 617;
                    right = 1350;
                    bottom = 627;
                }
                else if (partyMember == 2)
                {
                    left = 1326;
                    top = 634;
                    right = 1350;
                    bottom = 644;
                }
                else if (partyMember == 3)
                {
                    left = 1326;
                    top = 655;
                    right = 1350;
                    bottom = 665;
                }
                else if (partyMember == 4)
                {
                    left = 1326;
                    top = 672;
                    right = 1350;
                    bottom = 682;
                }
                else if (partyMember == 5)
                {
                    left = 1326;
                    top = 692;
                    right = 1350;
                    bottom = 702;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    left = 1326;
                    top = 634;
                    right = 1350;
                    bottom = 644;
                }
                else if (partyMember == 2)
                {
                    left = 1326;
                    top = 655;
                    right = 1350;
                    bottom = 665;
                }
                else if (partyMember == 3)
                {
                    left = 1326;
                    top = 672;
                    right = 1350;
                    bottom = 682;
                }
                else if (partyMember == 4)
                {
                    left = 1326;
                    top = 692;
                    right = 1350;
                    bottom = 702;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    left = 1326;
                    top = 655;
                    right = 1350;
                    bottom = 665;
                }
                else if (partyMember == 2)
                {
                    left = 1326;
                    top = 672;
                    right = 1350;
                    bottom = 682;
                }
                else if (partyMember == 3)
                {
                    left = 1326;
                    top = 692;
                    right = 1350;
                    bottom = 702;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    left = 1326;
                    top = 672;
                    right = 1350;
                    bottom = 682;
                }
                else if (partyMember == 2)
                {
                    left = 1326;
                    top = 692;
                    right = 1350;
                    bottom = 702;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    left = 1326;
                    top = 692;
                    right = 1350;
                    bottom = 702;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] hpPixelImages = { @".\images\pink-hp-pixel.png",
                @".\images\pink-hp-pixel-2.png",
                @".\images\pink-hp-pixel-3.png",
                @".\images\pink-hp-pixel-4.png"};

            bool found = false;

            for (int i = 0; i < hpPixelImages.Length; i++)
            {
                string[] results = UseImageSearch(left, top, right, bottom, "5", hpPixelImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                appendText("HP IS NOT TOPPED OFF FOR PARTY MEMBER " + partyMember);
                return false;
            }
        }

        public bool isHPRoughlyHalfEmpty(int partyMember, int partySize)
        {
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    left = 1300;
                    top = 600;
                    right = 1350;
                    bottom = 612;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 620;
                    right = 1350;
                    bottom = 632;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
                    top = 638;
                    right = 1350;
                    bottom = 650;
                }
                else if (partyMember == 4)
                {
                    left = 1300;
                    top = 656;
                    right = 1350;
                    bottom = 668;
                }
                else if (partyMember == 5)
                {
                    left = 1300;
                    top = 674;
                    right = 1350;
                    bottom = 686;
                }
                else if (partyMember == 6)
                {
                    left = 1300;
                    top = 692;
                    right = 1350;
                    bottom = 704;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    left = 1300;
                    top = 620;
                    right = 1350;
                    bottom = 632;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 638;
                    right = 1350;
                    bottom = 650;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
                    top = 656;
                    right = 1350;
                    bottom = 668;
                }
                else if (partyMember == 4)
                {
                    left = 1300;
                    top = 674;
                    right = 1350;
                    bottom = 686;
                }
                else if (partyMember == 5)
                {
                    left = 1300;
                    top = 692;
                    right = 1350;
                    bottom = 704;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    left = 1300;
                    top = 638;
                    right = 1350;
                    bottom = 650;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 656;
                    right = 1350;
                    bottom = 668;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
                    top = 674;
                    right = 1350;
                    bottom = 686;
                }
                else if (partyMember == 4)
                {
                    left = 1300;
                    top = 692;
                    right = 1350;
                    bottom = 704;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    left = 1300;
                    top = 656;
                    right = 1350;
                    bottom = 668;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 674;
                    right = 1350;
                    bottom = 686;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
                    top = 692;
                    right = 1350;
                    bottom = 704;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    left = 1300;
                    top = 674;
                    right = 1350;
                    bottom = 686;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 692;
                    right = 1350;
                    bottom = 704;
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    left = 1300;
                    top = 692;
                    right = 1350;
                    bottom = 704;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            string[] hpPixelImages = { 
                @".\images\roughly_half_empty_hp_bar_pixel-party-member-1a.png", 
                @".\images\roughly_half_empty_hp_bar_pixel-party-member-2a.png", 
                @".\images\roughly_half_empty_hp_bar_pixel-party-member-3a.png",
                @".\images\roughly_half_empty_hp_bar_pixel-party-member-4a.png",
                @".\images\roughly_half_empty_hp_bar_pixel-party-member-5a.png",
                @".\images\roughly_half_empty_hp_bar_pixel-party-member-6a.png" };

            bool found = false;

            for (int i = 0; i < hpPixelImages.Length; i++)
            {
                string[] results = UseImageSearch(left, top, right, bottom, "6", hpPixelImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                return false;
            }
            else
            {
                appendText2("HP IS HALF EMPTY OFF FOR PARTY MEMBER " + partyMember);
                return true;
            }
        }

        bool isTargetSleeping()
        {
            if (File.Exists(@".\images\target_sleep.png") == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int target_status_x = 0;
            int target_status_y = (int)(windowDimensionInfo.Height * 0.5);
            int target_status_width = (int)(windowDimensionInfo.Width * 0.5);
            int target_status_height = windowDimensionInfo.Height;
            bool sleeping_status = false;
            string[] results = UseImageSearch(target_status_x, target_status_y, target_status_width, target_status_height, "50", @".\images\target_sleep.png");
            if (results == null)
            {

            }
            else
            {
                sleeping_status = true;
            }
            return sleeping_status;
        }

        bool isTargetRefresh()
        {
            if (File.Exists(@".\images\target_refresh.png") == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int target_status_x = 0;
            int target_status_y = (int)(windowDimensionInfo.Height * 0.5);
            int target_status_width = (int)(windowDimensionInfo.Width * 0.5);
            int target_status_height = windowDimensionInfo.Height;
            bool status = false;
            string[] results = UseImageSearch(target_status_x, target_status_y, target_status_width, target_status_height, "70", @".\images\target_refresh.png");
            if (results == null)
            {

            }
            else
            {
                status = true;
            }
            return status;
        }

        bool isMeRefresh()
        {
            if (File.Exists(@".\images\refresh.png") == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int status_x = 0;
            int status_y = 0;
            int status_width = (int)(windowDimensionInfo.Width * 0.25);
            int status_height = (int)(windowDimensionInfo.Height * 0.25);
            bool status = false;
            string[] results = UseImageSearch(status_x, status_y, status_width, status_height, "30", @".\images\refresh.png");
            if (results == null)
            {

            }
            else
            {
                status = true;
            }
            return status;
        }


        private Queue<bool> composureQueue = new Queue<bool>();
        bool isComposureActive()
        {
            if (File.Exists(@".\images\composure.png") == false)
            {
                return false;
            }

            Rectangle rectangle = getStatusRectangle();
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", @".\images\composure.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool amIStoneskined()
        {
            if (File.Exists(@".\images\stoneskin.png") == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", @".\images\stoneskin.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isUtsusemiActive()
        {
            string[] utsusemiImages = { @".\images\utsusemi.png" };
            if (File.Exists(utsusemiImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "3", utsusemiImages[0]);
            if (results == null)
            {
                appendText("Utsusemi status is not found");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isHasteActive()
        {
            string[] hasteImages = { @".\images\haste.png" };
            if (File.Exists(hasteImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "10", hasteImages[0]);
            if (results == null)
            {
                appendText("Haste status is not found");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isRefreshActive()
        {
            string[] refreshImages = { @".\images\refresh.png", @".\images\refresh-3.png", @".\images\refresh-6.png" };

            Rectangle rectangle = getStatusRectangle();

            bool found = false;
            for (int i = 0; i < refreshImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "10", refreshImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                appendText2("Refresh on me is not found");
                return false;
            }
        }

        bool isSkillchainActive()
        {
            string[] skillchainImages = { @".\images\windower_skillchain_background.png" };

            Rectangle rectangle = new Rectangle(400, 355, 485, 445);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainImages[0]);
            if (results == null)
            {
                return false;
            }
            else
            {
                appendText2("Skillchain is active");
                return true;
            }
        }

        bool isSkillchainActive2()
        {
            string[] skillchainImages = { @".\images\windower_skillchain_active_pixel.png" };

            Rectangle rectangle = new Rectangle(400, 355, 485, 445);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainImages[0]);
            if (results == null)
            {
                appendText("Skillchain is not active");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool canIUseNextWeaponskill()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go.png", 
                @".\images\windower_skillchain_go_2.png"};

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 490, 530);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }
            
            if (found == true)
            {
                appendText2("I can use next weaponskill");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool canIUseNextWeaponskill2()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go_pixel.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(135, 362, 165, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("CLASS1: I can use next weaponskill");
                return true;
            }
            else
            {
                appendText2("CLASS1: I cannot use next weaponskill");
                return false;
            }
        }

        bool amIWaitingForNextWeaponskill()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_wait_pixel.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(135, 362, 165, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("CLASS1: I can use next weaponskill");
                return true;
            }
            else
            {
                appendText2("CLASS1: I cannot use next weaponskill");
                return false;
            }
        }

        bool canIUseNextWeaponskillInLessThan1Second()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go_3.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 205, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I can use next weaponskill in less than 1 second");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool canIUseNextWeaponskillInLessThan2Second()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go_4.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 205, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I can use next weaponskill in less than 2 second");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool canIUseNextWeaponskillInLessThan3Second()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go_5.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 205, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I can use next weaponskill in less than 3 second");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool canIUseNextWeaponskillInLessThan4Second()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go_6.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 205, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I can use next weaponskill in less than 4 second");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool canIUseNextWeaponskillInLessThan5Second()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go_7.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 205, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I can use next weaponskill in less than 5 second");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool canIUseNextWeaponskillInLessThan6Second()
        {
            string[] skillchainGoImages = { @".\images\windower_skillchain_go_8.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 205, 380);

            for (int i = 0; i < skillchainGoImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainGoImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I can use next weaponskill in less than 6 second");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool willDeathBlossomMakeLevel3Light()
        {
            string[] skillchainDeathBlossomImages = { @".\images\windower_skillchain_death_blossom_level_3_light.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            bool found = false;

            for (int i = 0; i < skillchainDeathBlossomImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainDeathBlossomImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("****Level 3 light is possible - Death Blossom****");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool willKnightsOfRoundMakeLevel4Light()
        {
            string[] skillchainKnightsOfRoundImages = { @".\images\windower_skillchain_knights_of_round_level_4_light_2.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            bool found = false;

            for (int i = 0; i < skillchainKnightsOfRoundImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainKnightsOfRoundImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }
            
            if (found == true)
            {
                appendText("****Level 4 light is possible - Knights of Round****");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool willKnightsOfRoundMakeLevel3Light()
        {
            string[] skillchainKnightsOfRoundImages = { @".\images\windower_skillchain_knights_of_round_level_3_light.png", 
                @".\images\windower_skillchain_knights_of_round_level_3_light_2.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            bool found = false;

            for (int i = 0; i < skillchainKnightsOfRoundImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainKnightsOfRoundImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("****Level 3 light is possible - Knights of Round***");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool willKnightsOfRoundMakeLevel2Fusion()
        {
            string[] skillchainKnightsOfRoundImages = { @".\images\windower_skillchain_knights_of_round_level_2_fusion_3.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            bool found = false;

            for (int i = 0; i < skillchainKnightsOfRoundImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainKnightsOfRoundImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("****Level 2 fusion is possible - Knights of Round***");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool willEmpyrealArrowMakeLevel3Light()
        {
            string[] skillchainImages = { @".\images\windower_skillchain_empyreal_arrow_level_3_light.png", @".\images\windower_skillchain_empyreal_arrow_level_3_light-2.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            bool found = false;

            for (int i = 0; i < skillchainImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", skillchainImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("****Level 3 light is possible - Empyreal Arrow***");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool willSavageBladeMakeLevel3Light()
        {
            string[] windowerSkillchainImages = { @".\images\windower_skillchain_savage_blade_level_3_light.png",
                @".\images\windower_skillchain_savage_blade_level_3_light_2.png",
                @".\images\windower_skillchain_savage_blade_level_3_light_3.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            for (int i = 0; i < windowerSkillchainImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", windowerSkillchainImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("****Level 3 light is possible - Savage Blade****");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool willChantDuCygneMakeLevel4Light()
        {
            string[] windowerSkillchainImages = { @".\images\windower_skillchain_chant_du_cygne_level_4_light.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);

            bool found = false;

            for (int i = 0; i < windowerSkillchainImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", windowerSkillchainImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("***Level 4 light is possible - Chant du Cygne***");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool willChantDuCygneMakeLevel3Darkness()
        {
            string[] windowerSkillchainImages = { @".\images\windower_skillchain_chant_du_cygne_level_3_darkness.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);

            bool found = false;

            for (int i = 0; i < windowerSkillchainImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", windowerSkillchainImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }
            
            if (found == true)
            {
                appendText("***Level 3 darkness is possible - Chant du Cygne***");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool willRequiescatMakeLevel3Darkness()
        {
            string[] windowerSkillchainImages = { @".\images\windower_skillchain_requiescat_level_3_darkness.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", windowerSkillchainImages[0]);
            if (results == null)
            {
                return false;
            }
            else
            {
                appendText2("***Level 3 darkness is possible - Requiescat    ***");
                return true;
            }
        }

        bool willCircleBladeMakeLevel2Fragmentation()
        {
            string[] windowerSkillchainImages = { @".\images\windower_skillchain_circle_blade_level_2_fragmentation.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", windowerSkillchainImages[0]);
            if (results == null)
            {
                return false;
            }
            else
            {
                appendText2("***Level 2 fragmentation is possible - CIRCLE BLADE***");
                return true;
            }
        }

        bool isHasteSambaActive()
        {
            string[] hasteSambaImages = { @".\images\haste_samba.png", @".\images\haste_samba_2.png" };
            if (File.Exists(hasteSambaImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            bool found = false;

            for (int i = 0; i < hasteSambaImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", hasteSambaImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == false)
            {
                appendText("Haste samba status is not found");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isAdvancingMarchActive()
        {
            string[] advancingMarchImages = { @".\images\advancing_march.png", @".\images\advancing_march_2.png" };
            if (File.Exists(advancingMarchImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            bool found = false;

            for (int i = 0; i < advancingMarchImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "15", advancingMarchImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == false)
            {
                appendText("Advancing march status is not found");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isParalyzed()
        {
            string[] paralyzedImages = { @".\images\paralysis.png" };
            if (File.Exists(paralyzedImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            bool found = false;
            for (int i = 0; i < paralyzedImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "15", paralyzedImages[i]);
                if (results == null)
                {
                }
                else
                {
                    appendText("Paralysis status found");
                    found = true;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isSilenced()
        {
            string[] silencedImages = { @".\images\silenced.png", @".\images\silenced2.png" };
            if (File.Exists(silencedImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            bool found = false;
            for (int i = 0; i < silencedImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "15", silencedImages[i]);
                if (results == null)
                {
                }
                else
                {
                    appendText("Silence status found");
                    found = true;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isCursed()
        {
            string[] cursedImages = { @".\images\curse.png" };
            if (File.Exists(cursedImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            bool found = false;
            for (int i = 0; i < cursedImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "15", cursedImages[i]);
                if (results == null)
                {
                }
                else
                {
                    appendText("Curse status found");
                    found = true;
                }
            }

            if (found == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isEnwaterActive()
        {
            string[] hasteImages = { @".\images\enwater.png" };
            if (File.Exists(hasteImages[0]) == false)
            {
                return false;
            }

            Rectangle rectangle = new Rectangle(0, 0, 420, 150);
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", hasteImages[0]);
            if (results == null)
            {
                appendText("Enwater status is not found");
                return false;
            }
            else
            {
                appendText("Enwater status is found");
                return true;
            }
        }


        bool selectFish()
        {
            string[] fishImages = { @".\images\fish.png", @".\images\fish_1920_1080.png" };
            Rectangle windowRectangle = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            Rectangle rectangle = new Rectangle();
            if (windowRectangle.Width == 1382)
            {
                rectangle = new Rectangle(0, 450, 200, 465);
            }
            else if (windowRectangle.Width == 1936)
            {
                rectangle = new Rectangle(0, 750, 200, 780);
            }
            else
            {
                appendText("Could not assign rectangle for isFishingReady()");
                return false;
            }
            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", fishImages[0]);
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && fish == true && i < 7)
                {
                    AutoItX.Send("{down}");
                    Thread.Sleep(250);
                    results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", fishImages[0]);
                    if (results == null)
                    {

                    }
                    else
                    {
                        found = true;
                    }

                    results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", fishImages[1]);
                    if (results == null)
                    {

                    }
                    else
                    {
                        found = true;
                    }

                    i++;
                }
                if (found == true)
                {
                    appendText("Found fish in menu");
                }
                else
                {
                    appendText("Did not find fish in menu");
                }
                return found;
            }
            else
            {
                // Nothing to process, the action menu is in the state I want it in
                appendText("Found fish in menu");
                return true;
            }
        }

        bool selectMagic()
        {
            if (File.Exists(@".\images\engaged_menu_magic_selected.png") == false)
            {
                return false;
            }

            if (File.Exists(@".\images\action_menu_item_pixel.png") == false)
            {
                return false;
            }

            if (File.Exists(@".\images\action_menu_magic_selected.png") == false)
            {
                return false;
            }
            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            string[] results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.4), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "0", @".\images\action_menu_item_pixel.png");
            if (results == null)
            {
                return false;
            }

            results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.4), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "100", @".\images\engaged_menu_magic_selected.png");
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && fight == true && i < 6)
                {
                    AutoItX.Send("{down}");
                    Thread.Sleep(50);
                    results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "100", @".\images\engaged_menu_magic_selected.png");
                    if (results == null)
                    {
                        results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "100", @".\images\action_menu_magic_selected.png");
                        if (results == null)
                        {

                        }
                        else
                        {
                            return true;
                        }

                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }
                return found;
            }
            else
            {
                // Nothing to process, the action menu is in the state I want it in
                return true;
            }
        }

        bool selectMagic2()
        {
            string[] magicImages = { @".\images\magic.png", @".\images\magic_when_weapon_is_out.png" };
            if (File.Exists(magicImages[0]) == false)
            {
                return false;
            }

            if (File.Exists(magicImages[1]) == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            string[] results = UseImageSearch(0, 350, (int)(windowDimensionInfo.Width * 0.33), 470, "70", @".\images\magic.png");
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && heal == true && i < 7)
                {
                    AutoItX.Send("{down}");
                    Thread.Sleep(100);
                    results = UseImageSearch(0, 350, (int)(windowDimensionInfo.Width * 0.33), 470, "70", magicImages[0]);
                    if (results == null)
                    {
                    }
                    else
                    {
                        found = true;
                    }

                    results = UseImageSearch(0, 350, (int)(windowDimensionInfo.Width * 0.33), 470, "70", magicImages[1]);
                    if (results == null)
                    {
                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }
                return found;
            }
            else
            {
                // Nothing to process, the action menu is in the state I want it in
                return true;
            }
        }

        bool selectAbilities()
        {
            if (File.Exists(@".\images\engaged_menu_abilities_selected.png") == false)
            {
                return false;
            }

            if (File.Exists(@".\images\action_menu_item_pixel.png") == false)
            {
                return false;
            }

            if (File.Exists(@".\images\action_menu_abilities_selected.png") == false)
            {
                return false;
            }
            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            string[] results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "0", @".\images\action_menu_item_pixel.png");
            if (results == null)
            {
                return false;
            }

            results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "0", @".\images\engaged_menu_abilities_selected.png");
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && fight == true && i < 6)
                {
                    AutoItX.Send("{down}");
                    Thread.Sleep(50);
                    results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "30", @".\images\engaged_menu_abilities_selected.png");
                    if (results == null)
                    {
                        results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "30", @".\images\action_menu_abilities_selected.png");
                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }
                return found;
            }
            else
            {
                // Nothing to process, the action menu is in the state I want it in
                return true;
            }
        }

        bool selectAbilities2()
        {
            string[] abilitiesImages = { @".\images\abilities_when_weapon_is_out.png" };
            if (File.Exists(abilitiesImages[0]) == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            string[] results = UseImageSearch(0, 380,  200, 405, "10", abilitiesImages[0]);
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && heal == true && i < 8)
                {
                    AutoItX.Send("{down}");
                    Thread.Sleep(500);
                    results = UseImageSearch(0, 380, 200, 405, "10", abilitiesImages[0]);
                    if (results == null)
                    {
                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }
                if (found == false)
                {
                    appendText("Did not find abilities in menu");
                }
                else
                {
                    appendText("Found abilities in menu");
                }
                return found;
            }
            else
            {
                // Nothing to process, the action menu is in the state I want it in
                appendText("Found abilities in menu");
                return true;
            }
        }

        bool selectWeaponSkills()
        {
            if (File.Exists(@".\images\action_menu_item_pixel.png") == false)
            {
                return false;
            }

            if (File.Exists(@".\images\action_menu_weapon_skills_selected.png") == false)
            {
                return false;
            }
            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            string[] results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "0", @".\images\action_menu_item_pixel.png");
            if (results == null)
            {
                return false;
            }

            results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "0", @".\images\action_menu_weapon_skills_selected.png");
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && fight == true && i < 4)
                {
                    AutoItX.Send("{down}");
                    Thread.Sleep(50);
                    results = UseImageSearch(0, (int)(windowDimensionInfo.Y * 0.5), (int)(windowDimensionInfo.Width * 0.33), windowDimensionInfo.Height, "30", @".\images\action_menu_weapon_skills_selected.png");
                    if (results == null)
                    {
                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }
                // The action menu should have 'Abilities' selected after the loop exits
                return found;
            }
            else
            {
                // Nothing to process, the action menu is in the state I want it in
                return true;
            }
        }

        bool selectWeaponskill2()
        {
            string[] weaponskillImages = { @".\images\weaponskills.png" };
            if (File.Exists(weaponskillImages[0]) == false)
            {
                return false;
            }

            string[] results = UseImageSearch(0, 408, 200, 416, "3", weaponskillImages[0]);
            if (results == null)
            {
                int i = 0;
                bool found = false;
                while (results == null && heal == true && i < 5)
                {
                    AutoItX.Send("{Down}");
                    Thread.Sleep(100);
                    results = UseImageSearch(0, 408, 200, 416, "3", weaponskillImages[0]);
                    if (results == null)
                    {

                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }

                if (found == true)
                {
                    appendText("Found weaponskills in menu");
                }
                else
                {
                    appendText("Did not find weaponskills in menu");
                }

                return found;
            }
            else
            {
                appendText("Found weaponskills in menu");
                return true;
            }
        }

        private List<Queue<bool>> partyMemberRefreshTimer = new List<Queue<bool>> { new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>() };
        private Queue<bool> refreshCooldownQueue = new Queue<bool>();
        private int refreshCastTime = 7000;
        private int refreshCooldownTime = 16000;
        void castRefresh(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else
                {
                    return;
                }
            }

            lastSpellCasted.Enqueue("Refresh");
            new Thread(() =>
            {
                Thread.Sleep(refreshCastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            refreshCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(refreshCooldownTime + refreshCastTime + 1550);
                refreshCooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Refresh\" ");
            AutoItX.Send(partyMemberString);
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(refreshCastTime + 1000);

            partyMemberRefreshTimer[partyMember].Enqueue(true);
            new Thread(() =>
            {
                if (partyMember == 1)
                {
                    if (isComposureActive() == true)
                    {
                        appendText("Composure recognized");
                        Thread.Sleep(405000);
                    }
                    else
                    {
                        appendText("Composure not recognized");
                        Thread.Sleep(135000);
                    }
                }
                else
                {
                    appendText("Party member not 1");
                    Thread.Sleep(135000);
                }
                if (partyMemberRefreshTimer[partyMember].Count > 0)
                {
                    partyMemberRefreshTimer[partyMember].Dequeue();
                }
            }).Start();

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private List<Queue<bool>> partyMemberRefresh2Timer = new List<Queue<bool>> { new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>() };
        private Queue<bool> refresh2CooldownQueue = new Queue<bool>();
        private int refresh2CastTime = 5000;
        private int refresh2CooldownTime = 27000;
        private int refresh3CooldownTime = 17000;
        void castRefresh2(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else
                {
                    return;
                }
            }

            lastSpellCasted.Enqueue("Refresh II");
            new Thread(() =>
            {
                Thread.Sleep(refresh2CastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            refresh2CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(refresh2CooldownTime + refresh2CastTime + 1550);
                refresh2CooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Refresh II\" ");
            AutoItX.Send(partyMemberString);
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(refreshCastTime + 1000);

            partyMemberRefresh2Timer[partyMember].Enqueue(true);
            new Thread(() =>
            {
                if (partyMember == 1)
                {
                    if (isComposureActive() == true)
                    {
                        appendText("Composure recognized");
                        Thread.Sleep(405000);
                    }
                    else
                    {
                        appendText("Composure not recognized");
                        Thread.Sleep(135000);
                    }
                }
                else
                {
                    appendText("Party member not 1");
                    Thread.Sleep(135000);
                }
                if (partyMemberRefresh2Timer[partyMember].Count > 0)
                {
                    partyMemberRefresh2Timer[partyMember].Dequeue();
                }
            }).Start();

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private Queue<bool> cureCooldownQueue = new Queue<bool>();
        private int cureCastTime = 2000;
        private int cureCooldownTime = 5000;
        void castCure(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "{F6}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }

            lastSpellCasted.Enqueue("Cure");
            new Thread(() =>
            {
                Thread.Sleep(cureCastTime + 1400);
                lastSpellCasted.Dequeue();
            }).Start();
            cureCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cureCooldownTime + cureCastTime + 1400);
                cureCooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(250);
            AutoItX.Send(partyMemberString);
            Thread.Sleep(250);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cureCastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        void castCure()
        {
            lastSpellCasted.Enqueue("Cure");
            new Thread(() =>
            {
                Thread.Sleep(cureCastTime + 750);
                lastSpellCasted.Dequeue();
            }).Start();
            cureCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cureCooldownTime + cureCastTime + 750);
                cureCooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{r}");
            Thread.Sleep(50);
            AutoItX.Send("{Enter}");
            Thread.Sleep(50);
            AutoItX.Send("{Enter}");
            Thread.Sleep(cureCastTime + 1000);

            if (checkBox1.Checked == true)
            {
                AutoItX.Send("/follow <p1>");
                Thread.Sleep(100);
                AutoItX.Send("{Enter}");
            }
        }

        private Queue<bool> cure2CooldownQueue = new Queue<bool>();
        private int cure2CastTime = 2500;
        private int cure2CooldownTime = 4000;
        void castCure2()
        {
            lastSpellCasted.Enqueue("Cure II");
            new Thread(() =>
            {
                Thread.Sleep(cure2CastTime + 1700);
                lastSpellCasted.Dequeue();
            }).Start();
            cure2CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure2CooldownTime + cureCastTime + 1700);
                cure2CooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(1000);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Enter}");
        }

        void castCure2(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "{F6}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else
                {
                    return;
                }
            }
            lastSpellCasted.Enqueue("Cure II");
            new Thread(() =>
            {
                Thread.Sleep(cure2CastTime + 1450);
                lastSpellCasted.Dequeue();
            }).Start();
            cure2CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure2CooldownTime + cure2CastTime + 1450);
                cure2CooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(250);
            AutoItX.Send(partyMemberString);
            Thread.Sleep(250);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cure2CastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private Queue<bool> cure3CooldownQueue = new Queue<bool>();
        private int cure3CooldownTime = 2000;
        private int cure3CastTime = 2500;
        void castCure3()
        {
            lastSpellCasted.Enqueue("Cure III");
            new Thread(() =>
            {
                Thread.Sleep(cure3CastTime + 1750);
                lastSpellCasted.Dequeue();
            }).Start();
            cure3CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure3CooldownTime + cureCastTime + 1750);
                cure3CooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(750);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Left}");
            Thread.Sleep(50);
            AutoItX.Send("{Down}");
            Thread.Sleep(50);
            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Enter}");
        }

        void castCure3(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "{F6}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else
                {
                    return;
                }
            }
            lastSpellCasted.Enqueue("Cure III");
            new Thread(() =>
            {
                Thread.Sleep(cure3CastTime + 1500);
                lastSpellCasted.Dequeue();
            }).Start();
            cure3CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure3CooldownTime + cure3CastTime + 1500);
                cure3CooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send(partyMemberString);
            Thread.Sleep(500);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cure3CastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private Queue<bool> cure4CooldownQueue = new Queue<bool>();
        private int cure4CooldownTime = 3000;
        private int cure4CastTime = 2500;
        void castCure4(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "{F6}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else
                {
                    return;
                }
            }
            lastSpellCasted.Enqueue("Cure IV");
            new Thread(() =>
            {
                Thread.Sleep(cure4CastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            cure4CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure4CooldownTime + cure4CastTime + 1550);
                cure4CooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send(partyMemberString);
            Thread.Sleep(500);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cure4CastTime + 1000);
            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        void castCure4_2(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else
                {
                    return;
                }
            }
            lastSpellCasted.Enqueue("Cure IV");
            new Thread(() =>
            {
                Thread.Sleep(cure4CastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            cure4CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure4CooldownTime + cure4CastTime + 1550);
                cure4CooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Cure IV\" ");
            AutoItX.Send(partyMemberString);
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cure4CastTime + 1000);
            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        void castCure3_2(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else
                {
                    return;
                }
            }
            lastSpellCasted.Enqueue("Cure III");
            new Thread(() =>
            {
                Thread.Sleep(cure3CastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            cure3CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure3CooldownTime + cure3CastTime + 1550);
                cure3CooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Cure III\" ");
            AutoItX.Send(partyMemberString);
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cure3CastTime + 1000);
            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        void castCure2_2(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else
                {
                    return;
                }
            }
            lastSpellCasted.Enqueue("Cure II");
            new Thread(() =>
            {
                Thread.Sleep(cure2CastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            cure2CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cure2CooldownTime + cure2CastTime + 1550);
                cure2CooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Cure II\" ");
            AutoItX.Send(partyMemberString);
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cure2CastTime + 1000);
            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        void castCure_2(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else
                {
                    return;
                }
            }
            lastSpellCasted.Enqueue("Cure");
            new Thread(() =>
            {
                Thread.Sleep(cureCastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            cureCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(cureCooldownTime + cureCastTime + 1550);
                cureCooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Cure\" ");
            AutoItX.Send(partyMemberString);
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(cureCastTime + 1000);
            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private Queue<bool> stoneskinCooldownQueue = new Queue<bool>();
        private int stoneskinCastTime = 7000;
        private int stoneskinCooldownTime = 13000;
        void castStoneskin()
        {
            int delay = 100;
            lastSpellCasted.Enqueue("Stoneskin");
            new Thread(() =>
            {
                Thread.Sleep(stoneskinCastTime + 1350);
                lastSpellCasted.Dequeue();
            }).Start();
            stoneskinCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(stoneskinCooldownTime + stoneskinCastTime + 1350);
                stoneskinCooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Left}");
            Thread.Sleep(100);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{r}");
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(250);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(stoneskinCastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    AutoItX.Send("/follow");
                    Thread.Sleep(100);
                    AutoItX.Send("{Enter}");
                }
                else
                {
                    AutoItX.Send("/follow <p1>");
                    Thread.Sleep(100);
                    AutoItX.Send("{Enter}");
                }
            }
        }

        void castStoneskin(int partyMember, int partySize)
        {
            int delay = 100;
            lastSpellCasted.Enqueue("Stoneskin");
            new Thread(() =>
            {
                Thread.Sleep(stoneskinCastTime + 1350);
                lastSpellCasted.Dequeue();
            }).Start();
            stoneskinCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(stoneskinCooldownTime + stoneskinCastTime + 1350);
                stoneskinCooldownQueue.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(500);
            AutoItX.Send("{Left}");
            Thread.Sleep(100);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Left}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            AutoItX.Send("{Down}");
            Thread.Sleep(delay);
            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(250);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(stoneskinCastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private int utsusemiIchiCastTime = 5000;
        private int utsusemiIchiCooldownTime = 31000;
        private Queue<bool> utsusemiIchiCooldownQueue = new Queue<bool>();
        void castUtsusemiIchi(int partyMember, int partySize)
        {
            int delay = 100;
            lastSpellCasted.Enqueue("Utsusemi: Ichi");
            new Thread(() =>
            {
                Thread.Sleep(utsusemiIchiCastTime + 1350);
                lastSpellCasted.Dequeue();
            }).Start();
            utsusemiIchiCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(utsusemiIchiCooldownTime + utsusemiIchiCastTime + 1350);
                utsusemiIchiCooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/nin \"Utsusemi: Ichi\" <me>");
            Thread.Sleep(1500);
            AutoItX.Send("{Enter}");
            Thread.Sleep(utsusemiIchiCastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private int utsusemiNiCastTime = 3000;
        private int utsusemiNiCooldownTime = 46000;
        private Queue<bool> utsusemiNiCooldownQueue = new Queue<bool>();
        void castUtsusemiNi(int partyMember, int partySize)
        {
            int delay = 100;
            lastSpellCasted.Enqueue("Utsusemi: Ni");
            new Thread(() =>
            {
                Thread.Sleep(utsusemiNiCastTime + 1350);
                lastSpellCasted.Dequeue();
            }).Start();
            utsusemiNiCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(utsusemiNiCooldownTime + utsusemiNiCastTime + 1350);
                utsusemiNiCooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/nin \"Utsusemi: Ni\" <me>");
            Thread.Sleep(1500);
            AutoItX.Send("{Enter}");
            Thread.Sleep(utsusemiNiCastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private int hasteCastTime = 3000;
        private int hasteCooldownTime = 16000;
        private int hasteIICooldownTime = 9000;
        private Queue<bool> hasteCooldownQueue = new Queue<bool>();
        void castHaste(int partyMember, int partySize)
        {
            int delay = 100;
            hasteCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(hasteCooldownTime + hasteCastTime + 1350);
                hasteCooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Haste\" <me>");
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(hasteCastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private int enwater2CastTime = 3000;
        private int enwater2CooldownTime = 26000;
        private int temperCooldownTime = 23000;
        private int temper2CooldownTime = 33000;
        private Queue<bool> enwater2CooldownQueue = new Queue<bool>();
        void castEnwater2(int partyMember, int partySize)
        {
            int delay = 100;
            enwater2CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(enwater2CooldownTime + enwater2CastTime + 1350);
                enwater2CooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Enwater II\" <me>");
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(enwater2CastTime + 1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private int paralyzeCastTime = 3000;
        private int paralyzeCooldownTime = 10000;
        private Queue<bool> paralyzeCooldownQueue = new Queue<bool>();
        private Queue<bool> paralyzeMonsterQueue = new Queue<bool>();
        private Thread paralyzeThread = null;
        void castParalyze(int partyMember, int partySize)
        {
            int delay = 100;
            paralyzeCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(paralyzeCooldownTime + paralyzeCastTime + 1000);
                paralyzeCooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Paralyze\" <t>");
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(paralyzeCastTime + 1000);
            paralyzeMonsterQueue.Enqueue(true);
            paralyzeThread = new Thread(() =>
            {
                Thread.Sleep(60000);
                if (paralyzeMonsterQueue.Count > 0)
                {
                    paralyzeMonsterQueue.Dequeue();
                }
            });
            paralyzeThread.Start();

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private int composureCooldownTime = 300000;
        private Queue<bool> composureCooldownQueue = new Queue<bool>();
        void useComposure(int partyMember, int partySize)
        {
            int delay = 100;
            composureCooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(composureCooldownTime + 1350);
                composureCooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ja \"Composure\" <me>");
            Thread.Sleep(1500);
            AutoItX.Send("{Enter}");
            Thread.Sleep(1000);

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private List<Queue<bool>> partyMemberRegen2Timer = new List<Queue<bool>> { new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>() };
        private Queue<bool> regen2CooldownQueue = new Queue<bool>();
        private int regen2CastTime = 3000;
        private int regen2CooldownTime = 16000;
        void castRegen2(int partyMember, int partySize)
        {
            int delay = 100;
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else
                {
                    return;
                }
            }

            lastSpellCasted.Enqueue("Regen 2");
            new Thread(() =>
            {
                Thread.Sleep(regen2CastTime + 1550);
                lastSpellCasted.Dequeue();
            }).Start();
            regen2CooldownQueue.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(regen2CooldownTime + regen2CastTime + 1550);
                regen2CooldownQueue.Dequeue();
            }).Start();

            stopFollow();
            Thread.Sleep(delay);
            AutoItX.Send("/ma \"Regen II\" ");
            AutoItX.Send(partyMemberString);
            Thread.Sleep(1000);
            AutoItX.Send("{Enter}");
            Thread.Sleep(delay);
            AutoItX.Send("{ESC}");
            Thread.Sleep(regen2CastTime + 1000);

            partyMemberRegen2Timer[partyMember].Enqueue(true);
            new Thread(() =>
            {
                if (partyMember == 1)
                {
                    if (isComposureActive() == true)
                    {
                        appendText("Composure recognized");
                        Thread.Sleep(165000);
                    }
                    else
                    {
                        appendText("Composure not recognized");
                        Thread.Sleep(55000);
                    }
                }
                else
                {
                    appendText("Party member not 1");
                    Thread.Sleep(55000);
                }
                if (partyMemberRegen2Timer[partyMember].Count > 0)
                {
                    partyMemberRegen2Timer[partyMember].Dequeue();
                }
            }).Start();

            if (checkBox1.Checked == true)
            {
                if (isWeaponDrawn() == true)
                {
                    if (followPartyMember2Queue.Count > 0)
                    {
                        followPartyMember2Queue.Dequeue();
                        followTarget();
                    }
                    else if (followQueue.Count == 0)
                    {
                        followTarget();
                    }
                }
                else
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
                }
            }
        }

        private Queue<string> lastAbilitiesUsed = new Queue<string>();
        private int weaponSkillActivationTime = 3000;
        void useSeraphStrike()
        {
            lastAbilitiesUsed.Enqueue("Seraph Strike");
            new Thread(() =>
            {
                Thread.Sleep(weaponSkillActivationTime + 2600);
                lastAbilitiesUsed.Dequeue();
            }).Start();

            AutoItX.Send("{Enter}");
            Thread.Sleep(1000);
            if (selectWeaponSkills() == true)
            {
                AutoItX.Send("{Enter}");
                Thread.Sleep(1000);
                AutoItX.Send("{Left}");
                Thread.Sleep(50);
                AutoItX.Send("{Down}");
                Thread.Sleep(50);
                AutoItX.Send("{Enter}");
                Thread.Sleep(500);
                AutoItX.Send("{Enter}");
            }
        }

        void useFastBlade()
        {
            AutoItX.Send("/ws \"Fast Blade\" <t>");
            Thread.Sleep(500);
            AutoItX.Send("{Enter}");
            Thread.Sleep(2000);
        }

        void useWeaponSkill()
        {
            int delay = 5000;
            if (weaponskill == "Fast Blade")
            {
                appendText("Using fast blade...");
                stopFollow();
                Thread.Sleep(100);
                AutoItX.Send("/ws \"Fast Blade\" <t>");
                Thread.Sleep(1000);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Burning Blade")
            {
                appendText("Using burning blade...");
                AutoItX.Send("/ws \"Burning Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Red Lotus Blade")
            {
                appendText("Using red lotus blade...");
                stopFollow();
                Thread.Sleep(100);
                AutoItX.Send("/ws \"Red Lotus Blade\" <t>");
                Thread.Sleep(1000);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Wasp Sting")
            {
                appendText("Using wasp sting...");
                AutoItX.Send("/ws \"Wasp Sting\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Shining Blade")
            {
                appendText("Using shining blade...");
                AutoItX.Send("/ws \"Shining Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Seraph Blade")
            {
                appendText("Using seraph blade...");
                AutoItX.Send("/ws \"Seraph Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Vorpal Blade")
            {
                appendText("Using vorpal blade...");
                stopFollow();
                Thread.Sleep(100);
                AutoItX.Send("/ws \"Vorpal Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Flat Blade")
            {
                appendText("Using flat blade...");
                AutoItX.Send("/ws \"Flat Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Circle Blade")
            {
                appendText("Using circle blade...");
                AutoItX.Send("/ws \"Circle Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else
            {
                appendText("Select a weapon skill from the dropdown list...");
            }
        }
        bool maintainPartyMemberHP(int partyMember, int partySize, bool partyMemberChecked)
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
            }

            if (isPartyMemberRedHP(partyMember, partySize) == true && partyMemberChecked == true)
            {
                string message = "Party member " + partyMember + " is in red";
                appendText(message);
                if (cure4CooldownQueue.Count == 0 && checkBox19.Checked == true)
                {
                    castCure4_2(partyMember, partySize);
                    return true;
                }
                else if (cure3CooldownQueue.Count == 0 && checkBox2.Checked == true)
                {
                    castCure3_2(partyMember, partySize);
                    return true;
                }
                else if (cure2CooldownQueue.Count == 0 && checkBox15.Checked == true)
                {
                    castCure2_2(partyMember, partySize);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (isPartyMemberOrangeHP(partyMember, partySize) == true && partyMemberChecked == true)
            {
                string message = "Party member " + partyMember + " is in orange";
                appendText(message);
                if (cure4CooldownQueue.Count == 0 && checkBox19.Checked == true)
                {
                    castCure4_2(partyMember, partySize);
                    return true;
                }
                else if (cure3CooldownQueue.Count == 0 && checkBox2.Checked == true)
                {
                    castCure3_2(partyMember, partySize);
                    return true;
                }
                else if (cure2CooldownQueue.Count == 0 && checkBox15.Checked == true)
                {
                    castCure2_2(partyMember, partySize);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (isPartyMemberYellowHP(partyMember, partySize) == true && partyMemberChecked == true)
            {
                string message = "Party member " + partyMember + " is in yellow";
                appendText(message);
                if (partyMemberRegen2Timer[partyMember].Count == 0 && checkBox25.Checked == true)
                {
                    if (regen2CooldownQueue.Count == 0)
                    {
                        castRegen2(partyMember, partySize);
                        return true;
                    }
                }

                if (cure3CooldownQueue.Count == 0 && checkBox2.Checked == true)
                {
                    castCure3_2(partyMember, partySize);
                    return true;
                }
                else if (cure2CooldownQueue.Count == 0 && checkBox15.Checked == true)
                {
                    castCure2_2(partyMember, partySize);
                    return true;
                }
                else if (cureCooldownQueue.Count == 0 && checkBox17.Checked == true)
                {
                    castCure_2(partyMember, partySize);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        Hashtable fishTable = new Hashtable();
        FishPriorityQueue fishPriorityQueue = new FishPriorityQueue();
        private bool watchEgg = false;
        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;
            button7.Enabled = true;
            
            if (watchEgg == false)
            {
                appendText("Watching chocobo egg");
                watchEgg = true;

                if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
                {
                    if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                    {
                        AutoItX.WinActivate("[CLASS:FFXiClass]");
                    }
                }
            }

            new Thread(() =>
            {
                while (watchEgg == true)
                {
                    if (AutoItX.WinActive("[CLASS:FFXiClass]") == 1)
                    {
                        AutoItX.Send("{Enter}");
                        appendText("Watch over chocobo");
                        Thread.Sleep(1000);
                        AutoItX.Send("{Enter}");
                        Thread.Sleep(1000);
                        AutoItX.Send("{Up}");
                        Thread.Sleep(100);
                        AutoItX.Send("{Enter}");
                        appendText("Yes");
                        Thread.Sleep(6000);
                        AutoItX.Send("{Enter}");
                        appendText("Return to Watch over chocobo");
                        Thread.Sleep(6000);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                }
            }).Start();
        }

        private bool isReadyToFish()
        {
            string[] dayImages = new string[8];
            Rectangle rectangle = new Rectangle();
            if (windowRectangle.Width == 1382)
            {
                rectangle = new Rectangle(20, 450, 40, 470);
                dayImages[0] = @".\images\lightning_day.png";
                dayImages[1] = @".\images\lightsday.png";
                dayImages[2] = @".\images\ice_day.png";
                dayImages[3] = @".\images\dark_day.png";
                dayImages[4] = @".\images\firesday.png";
                dayImages[5] = @".\images\earthsday.png";
                dayImages[6] = @".\images\watersday.png";
                dayImages[7] = @".\images\windsday.png";
            }
            else if (windowRectangle.Width == 1936)
            {
                rectangle = new Rectangle(20, 755, 50, 780);
                dayImages[0] = @".\images\lightningsday_1080_1920.png";
                dayImages[1] = @".\images\lightsday_1920_1080.png";
                dayImages[2] = @".\images\icesday_1920_1080.png";
                dayImages[3] = @".\images\darksday_1920_1080.png";
                dayImages[4] = @".\images\firesday_1920_1080.png";
                dayImages[5] = @".\images\earthsday_1920_1080.png";
                dayImages[6] = @".\images\watersday_1920_1080.png";
                dayImages[7] = @".\images\windsday_1920_1080.png";
            }
            else
            {
                appendText("Could not assign rectangle for isFishingReady()");
                return false;
            }
            // string[] dayImages = {@".\images\firesday.png", @".\images\earthsday.png", @".\images\watersday.png", @".\images\windsday.png", @".\images\ice_day.png", @".\images\lightning_day.png", @".\images\dark_day.png", @".\images\lightsday.png" };
            bool found = false;
            for (int i = 0; i < dayImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "3", dayImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                appendText("Found day icon");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button6.Enabled = true;
            button7.Enabled = false;
            appendText("Stop watching chocobo egg");
            watchEgg = false;
        }

        private bool isCraftResultsOnScreen()
        {
            bool found = false;
            string[] results = UseImageSearch(0, 430, 160, 470, "15", @".\images\craft_results.png");
            if (results == null)
            {

            }
            else
            {
                found = true;
            }

            if (found == true)
            {
                appendText("Found craft results");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool isReadyToCraft()
        {
            bool found = false;
            string[] results = UseImageSearch(1250, 130, 1350, 160, "30", @".\images\synthesis.png");
            if (results == null)
            {

            }
            else
            {
                found = true;
            }

            if (found == true)
            {
                appendText("Found synthesis button");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool heal = false;
        private string weaponskill = "";
        private Hashtable craftTable = new Hashtable();
        private CraftPriorityQueue craftPriorityQueue = new CraftPriorityQueue();
        private void button8_Click(object sender, EventArgs e)
        {
            button8.Enabled = false;
            button9.Enabled = true;
            
            if (craft == false)
            {
                appendText("Start craft bot");

                if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
                {
                    if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                    {
                        AutoItX.WinActivate("[CLASS:FFXiClass]");
                    }
                }

                craft = true;
            }

            new Thread(() =>
            {
                while (craft == true)
                {
                    if (isReadyToCraft() == true)
                    {
                        const int START_CRAFT_PRIORITY = 1;
                        if (craftTable.Contains("Start Craft") == false)
                        {
                            Class3 craft = new Class3("Start Craft", control, textBox1);
                            craftTable.Add(craft.getAction(), true);
                            craftPriorityQueue.insert(START_CRAFT_PRIORITY, craft);
                        }
                        else
                        {
                            appendText("Start craft action is queued");
                        }
                    }
                    else if (isCraftResultsOnScreen() == true)
                    {
                        const int DISMISS_CRAFT_RESULTS = 2;
                        if (craftTable.Contains("Dismiss Results") == false)
                        {
                            Class3 dismissResults = new Class3("Dismiss Results", control, textBox1);
                            craftTable.Add(dismissResults.getAction(), true);
                            craftPriorityQueue.insert(DISMISS_CRAFT_RESULTS, dismissResults);
                        }
                        else
                        {
                            appendText("Dismiss results action is queued");
                        }
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (craft == true)
                {
                    if (craftPriorityQueue.size() > 0)
                    {
                        Class3 action = craftPriorityQueue.getData();
                        craftPriorityQueue.remove();
                        action.function1();
                        craftTable.Remove(action.getAction());
                    }
                }
            }).Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            appendText("Stop craft bot");
            craft = false;

            button8.Enabled = true;
            button9.Enabled = false;

        }

        private Queue<bool> assistCooldownQueue = new Queue<bool>();
        bool partyMemberIsEngaged(int partyMember, int partySize)
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
            }

            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "{F6}";
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "{F5}";
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "{F4}";
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "{F3}";
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "{F2}";
                }
                else
                {
                    return false;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            int x = 0;
            int y = 0;
            int width = 0;
            int height = 0;
            if (partySize == 6)
            {
                x = 1245;
                y = 550;
                width = 1350;
                height = 575;
            }
            else if (partySize == 5)
            {
                x = 1245;
                y = 570;
                width = 1350;
                height = 595;
            }
            else if (partySize == 4)
            {
                x = 1245;
                y = 590;
                width = 1350;
                height = 610;
            }
            else if (partySize == 3)
            {
                x = 1245;
                y = 610;
                width = 1350;
                height = 630;
            }
            else if (partySize == 2)
            {
                x = 1245;
                y = 630;
                width = 1350;
                height = 650;
            }
            else if (partySize == 1)
            {
            }
            else
            {
                return false;
            }

            if (File.Exists(@".\images\engaged_monster_name_pixel.png") == false)
            {
                appendText("engaged_monster_name_pixel.png does not exist");
                return false;
            }

            int delay = 100;

            AutoItX.Send(partyMemberString);
            Thread.Sleep(delay);
            AutoItX.Send("/assist");
            Thread.Sleep(delay);
            AutoItX.Send("{Enter}");
            Thread.Sleep(1000);

            string[] results = UseImageSearch(x, y, width, height, "6", @".\images\engaged_monster_name_pixel.png");
            if (results == null)
            {
                appendText("We do not have enmity with this target");
                return false;
            }
            else
            {
                appendText("We have enmity with this target");
                return true;
            }
        }

        public Rectangle getTargetRectangle(int partySize)
        {
            Rectangle rectangle = new Rectangle();
            if (partySize == 6)
            {
                rectangle.X = 1240;
                rectangle.Y = 550;
                rectangle.Width = 1340;
                rectangle.Height = 575;
            }
            else if (partySize == 5)
            {
                rectangle.X = 1240;
                rectangle.Y = 570;
                rectangle.Width = 1340;
                rectangle.Height = 595;
            }
            else if (partySize == 4)
            {
                rectangle.X = 1240;
                rectangle.Y = 585;
                rectangle.Width = 1340;
                rectangle.Height = 610;
            }
            else if (partySize == 3)
            {
                rectangle.X = 1240;
                rectangle.Y = 605;
                rectangle.Width = 1340;
                rectangle.Height = 630;
            }
            else if (partySize == 2)
            {
                rectangle.X = 1240;
                rectangle.Y = 620;
                rectangle.Width = 1340;
                rectangle.Height = 647;
            }
            else if (partySize == 1)
            {
            }

            return rectangle;
        }

        bool isEngaged(int partySize)
        {
            Rectangle rectangle = getTargetRectangle(partySize);

            string[] assistImages = { @".\images\engaged_monster_name_pixel.png" };

            bool found = false;
            for (int i = 0; i < assistImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "6", assistImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                appendText("I am targeting a monster to fight");
                return true;
            }
            else
            {
                appendText("I am not targeting a monster to fight");
                return false;
            }
        }

        public bool isTargettingAMonster(int partySize)
        {
            Rectangle rectangle = getTargetRectangle(partySize);

            string[] assistImages = { @".\images\target_monster_name_pixel.png", @".\images\target_monster_name_pixel_2.png" };

            bool found = false;
            for (int i = 0; i < assistImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "3", assistImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                }
            }

            if (found == true)
            {
                appendText("I am targeting a monster to fight");
                return true;
            }
            else
            {
                appendText("I am not targeting a monster to fight");
                return false;
            }
        }

        bool selectAttack() { 
            if (File.Exists(@".\images\attack.png") == false)
            {
                return false;
            }

            bool found = false;
            string[] results = UseImageSearch(0, 350, 200, 470, "100", @".\images\attack.png");
            if (results == null)
            {
                int i = 0;
                while (results == null && heal == true && i < 6)
                {
                    AutoItX.Send("{Down}");
                    Thread.Sleep(100);
                    results = UseImageSearch(0, 350, 200, 470, "100", @".\images\attack.png");
                    if (results == null)
                    {

                    }
                    else
                    {
                        found = true;
                    }
                    i++;
                }

                if (found == true)
                {
                    appendText("Found attack in menu");
                }
                else
                {
                    appendText("Did not find attack in menu");
                }

                return found;
            }
            else
            {
                appendText("Found attack in menu");
                return true;
            }
        }

        private Queue<bool> followTargetCooldown = new Queue<bool>();
        private Queue<bool> followPartyMember2Queue = new Queue<bool>();
        void followTarget(int partyMember, int partySize)
        {
            string partyMemberString = "";
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else if (partyMember == 6)
                {
                    partyMemberString = "<p5>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else if (partyMember == 5)
                {
                    partyMemberString = "<p4>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else if (partyMember == 4)
                {
                    partyMemberString = "<p3>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else if (partyMember == 3)
                {
                    partyMemberString = "<p2>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
                else if (partyMember == 2)
                {
                    partyMemberString = "<p1>";
                }
                else
                {
                    return;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "<p0>";
                }
            }
            else
            {
                return;
            }

            followTargetCooldown.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(5000);
                followTargetCooldown.Dequeue();
            }).Start();

            if (followPartyMember2Queue.Count == 0)
            {
                followPartyMember2Queue.Enqueue(true);
                AutoItX.Send("/follow ");
                AutoItX.Send(partyMemberString);
                Thread.Sleep(100);
                AutoItX.Send("{Enter}");
            }
            else
            {
                // Do nothing because I'm already following the party member
            }
        }

        private Queue<bool> followQueue = new Queue<bool>();
        void followTarget()
        {
            followTargetCooldown.Enqueue(true);
            new Thread(() =>
            {
                Thread.Sleep(5000);
                followTargetCooldown.Dequeue();
            }).Start();

            if (followQueue.Count == 0)
            {
                followQueue.Enqueue(true);
                AutoItX.Send("/follow");
                Thread.Sleep(100);
                AutoItX.Send("{Enter}");
            }
            else
            {
                // Do nothing because I'm already following the party member
            }
        }

        void stopFollow()
        {
            if (followQueue.Count > 0)
            {
                followQueue.Dequeue();
                AutoItX.Send("{r}");
                return;
            }
            else if (followPartyMember2Queue.Count > 0)
            {
                followPartyMember2Queue.Dequeue();
                AutoItX.Send("{r}");
                return;
            }
        }

        private Queue<bool> engagedQueue = new Queue<bool>();
        // engageTarget - Player should have the monster targeted before calling this function
        void engageTarget()
        {
            AutoItX.Send("{Enter}");
            Thread.Sleep(1000);
            if (selectAttack() == true)
            {
                appendText("Going to fight a monster");
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
                engagedQueue.Enqueue(true);

                if (followPartyMember2Queue.Count > 0)
                {
                    followPartyMember2Queue.Dequeue();
                    followTarget();
                }
                else if (followQueue.Count == 0)
                {
                    followTarget();
                }
                else
                {
                    // We are already following the target/
                }
            }
            else
            {
            }
        }

        void engageTargetCommand()
        {
            appendText("Going to fight a monster");
            AutoItX.Send("/attack");
            Thread.Sleep(500);
            AutoItX.Send("{Enter}");
            Thread.Sleep(2000);
            engagedQueue.Enqueue(true);

            if (followPartyMember2Queue.Count > 0)
            {
                followPartyMember2Queue.Dequeue();
                followTarget();
            }
            else if (followQueue.Count == 0)
            {
                followTarget();
            }
            else
            {
                // We are already following the target/
            }
        }

        bool isWeaponDrawn()
        {
            string[] weaponDrawnImages = { @".\images\weapon_drawn.png"};

            bool weaponDrawn = false;
            for (int i = 0; i < weaponDrawnImages.Length; i++)
            {
                string[] results = UseImageSearch(0, 0, 50, 110, "30", weaponDrawnImages[i]);
                if (results == null)
                {

                }
                else
                {
                    weaponDrawn = true;
                }
            }

            if (weaponDrawn == false)
            {
                appendText2("Weapon is not out");
            }
            else
            {
                appendText("Found weapon is drawn");
            }
            return weaponDrawn;
        }

        bool almostHasTP()
        {
            string[] tpImages = { @".\images\tp_text_700.png", @".\images\tp_text_800.png", @".\images\tp_text_900.png" };

            bool found = false;
            for (int i = 0; i < tpImages.Length; i++)
            {
                string[] results = UseImageSearch(110, 90, 130, 110, "0", tpImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I almost have TP");
                return true;
            }
            else
            {
                return false;
            }
        }

        bool hasTP()
        {
            string[] tpImages = { @".\images\tp_text_1000.png", @".\images\tp_text_2000.png", @".\images\tp_text_3000.png" };

            bool found = false;
            for (int i = 0; i < tpImages.Length; i++)
            {
                string[] results = UseImageSearch(110, 90, 120, 110, "3", tpImages[i]);
                if (results == null)
                {
                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("I have TP");
                return true;
            }
            else
            {
                return false;
            }
        }

        public Rectangle getTPRectangle()
        {
            return new Rectangle(110, 90, 120, 110);
        }

        public bool has3000TP()
        {
            Rectangle rectangle = getTPRectangle();
            AutoItX3 au3 = new AutoItX3();
            au3.AutoItSetOption("PixelCoordMode", 1);
            // 0xF7F4F7
            Object pixel = au3.PixelSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0xBBBABB);
            if (au3.error == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool has2000TP()
        {
            Rectangle rectangle = getTPRectangle();
            AutoItX3 au3 = new AutoItX3();
            au3.AutoItSetOption("PixelCoordMode", 1);
            // 0x7B797C
            Object pixel = au3.PixelSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0x939294);
            if (au3.error == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool has1000TP()
        {
            Rectangle rectangle = getTPRectangle();
            AutoItX3 au3 = new AutoItX3();
            au3.AutoItSetOption("PixelCoordMode", 1);
            //0x818482
            Object pixel = au3.PixelSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0xAFB0B1);
            if (au3.error == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox18.Checked == true)
            {
                checkBox26.Checked = false;
            }
        }

        private void checkBox26_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox26.Checked == true)
            {
                checkBox18.Checked = false;
            }
        }

        Rectangle getStatusRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Width = 420;
            rectangle.Height = 150;
            return rectangle;
        }

        Rectangle getPartyMemberDeadRectangle(int partyMember, int partySize)
        {
            Rectangle rectangle = new Rectangle();
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 602;
                    rectangle.Width = 1310;
                    rectangle.Height = 612;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 620;
                    rectangle.Width = 1310;
                    rectangle.Height = 631;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 640;
                    rectangle.Width = 1310;
                    rectangle.Height = 650;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 658;
                    rectangle.Width = 1310;
                    rectangle.Height = 668;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 674;
                    rectangle.Width = 1310;
                    rectangle.Height = 687;
                }
                else if (partyMember == 6)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 692;
                    rectangle.Width = 1310;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 620;
                    rectangle.Width = 1310;
                    rectangle.Height = 631;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 640;
                    rectangle.Width = 1310;
                    rectangle.Height = 650;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 658;
                    rectangle.Width = 1310;
                    rectangle.Height = 668;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 674;
                    rectangle.Width = 1310;
                    rectangle.Height = 687;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 692;
                    rectangle.Width = 1310;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1370;
                    rectangle.Y = 640;
                    rectangle.Width = 1310;
                    rectangle.Height = 650;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 658;
                    rectangle.Width = 1310;
                    rectangle.Height = 668;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 674;
                    rectangle.Width = 1310;
                    rectangle.Height = 687;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 692;
                    rectangle.Width = 1310;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 658;
                    rectangle.Width = 1310;
                    rectangle.Height = 668;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 674;
                    rectangle.Width = 1310;
                    rectangle.Height = 687;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 692;
                    rectangle.Width = 1310;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 674;
                    rectangle.Width = 1310;
                    rectangle.Height = 687;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 692;
                    rectangle.Width = 1310;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1270;
                    rectangle.Y = 692;
                    rectangle.Width = 1310;
                    rectangle.Height = 705;
                }
            }

            return rectangle;
        }

        Rectangle getPartyMemberMPRectangle(int partyMember, int partySize)
        {
            Rectangle rectangle = new Rectangle();
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 605;
                    rectangle.Width = 1350;
                    rectangle.Height = 625;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 630;
                    rectangle.Width = 1350;
                    rectangle.Height = 640;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 650;
                    rectangle.Width = 1350;
                    rectangle.Height = 660;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 668;
                    rectangle.Width = 1350;
                    rectangle.Height = 678;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 685;
                    rectangle.Width = 1350;
                    rectangle.Height = 695;
                }
                else if (partyMember == 6)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 705;
                    rectangle.Width = 1350;
                    rectangle.Height = 713;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 630;
                    rectangle.Width = 1350;
                    rectangle.Height = 640;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 650;
                    rectangle.Width = 1350;
                    rectangle.Height = 660;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 668;
                    rectangle.Width = 1350;
                    rectangle.Height = 678;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 685;
                    rectangle.Width = 1350;
                    rectangle.Height = 695;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 705;
                    rectangle.Width = 1350;
                    rectangle.Height = 713;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 650;
                    rectangle.Width = 1350;
                    rectangle.Height = 660;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 668;
                    rectangle.Width = 1350;
                    rectangle.Height = 678;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 685;
                    rectangle.Width = 1350;
                    rectangle.Height = 695;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 705;
                    rectangle.Width = 1350;
                    rectangle.Height = 713;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 668;
                    rectangle.Width = 1350;
                    rectangle.Height = 678;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 685;
                    rectangle.Width = 1350;
                    rectangle.Height = 695;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 705;
                    rectangle.Width = 1350;
                    rectangle.Height = 713;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 685;
                    rectangle.Width = 1350;
                    rectangle.Height = 695;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 705;
                    rectangle.Width = 1350;
                    rectangle.Height = 713;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1330;
                    rectangle.Y = 705;
                    rectangle.Width = 1350;
                    rectangle.Height = 713;
                }
            }

            return rectangle;
        }

        Rectangle getPartyMemberHPRectangle(int partyMember, int partySize)
        {
            Rectangle rectangle = new Rectangle();
            if (partySize == 6)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 600;
                    rectangle.Width = 1350;
                    rectangle.Height = 610;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 617;
                    rectangle.Width = 1350;
                    rectangle.Height = 627;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 634;
                    rectangle.Width = 1350;
                    rectangle.Height = 644;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 655;
                    rectangle.Width = 1350;
                    rectangle.Height = 665;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 672;
                    rectangle.Width = 1350;
                    rectangle.Height = 682;
                }
                else if (partyMember == 6)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 692;
                    rectangle.Width = 1350;
                    rectangle.Height = 702;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 617;
                    rectangle.Width = 1350;
                    rectangle.Height = 627;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 634;
                    rectangle.Width = 1350;
                    rectangle.Height = 644;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 655;
                    rectangle.Width = 1350;
                    rectangle.Height = 665;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 672;
                    rectangle.Width = 1350;
                    rectangle.Height = 682;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 692;
                    rectangle.Width = 1350;
                    rectangle.Height = 702;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 634;
                    rectangle.Width = 1350;
                    rectangle.Height = 644;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 655;
                    rectangle.Width = 1350;
                    rectangle.Height = 665;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 672;
                    rectangle.Width = 1350;
                    rectangle.Height = 682;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 692;
                    rectangle.Width = 1350;
                    rectangle.Height = 702;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 655;
                    rectangle.Width = 1350;
                    rectangle.Height = 665;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 672;
                    rectangle.Width = 1350;
                    rectangle.Height = 682;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 692;
                    rectangle.Width = 1350;
                    rectangle.Height = 702;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 672;
                    rectangle.Width = 1350;
                    rectangle.Height = 682;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 692;
                    rectangle.Width = 1350;
                    rectangle.Height = 702;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1326;
                    rectangle.Y = 692;
                    rectangle.Width = 1350;
                    rectangle.Height = 702;
                }
            }

            return rectangle;
        }

        private Hashtable actionTable = new Hashtable();
        private Queue<Class1> actionQueue = new Queue<Class1>();
        private PriorityQueue actionPriorityQueue = new PriorityQueue();
        private PriorityQueue weaponskillPriorityQueue = new PriorityQueue();
        private PriorityQueue assistPriorityQueue = new PriorityQueue();
        private PriorityQueue curePriorityQueue = new PriorityQueue();
        private PriorityQueue priorityQueueOne = new PriorityQueue();
        private PriorityQueue priorityQueueTwo = new PriorityQueue();
        private PriorityQueue priorityQueueThree = new PriorityQueue();
        private PriorityQueue priorityQueueFour = new PriorityQueue();
        private PriorityQueue priorityQueueFive = new PriorityQueue();
        private bool battle = false;
        private Queue<bool> weaponDrawnQueue = new Queue<bool>();
        private Queue<bool> temperTimer = new Queue<bool>();
        private Queue<bool> enwaterIITimer = new Queue<bool>();
        private Queue<bool> phalanxIITimer = new Queue<bool>();
        private Queue<bool> barfiraTimer = new Queue<bool>();
        const int CURE_IV_PRIORITY = 50;
        const int CURE_III_PRIORITY = 49;
        const int CURE_II_PRIORITY = 18;
        const int CURE_PRIORITY = 17;
        const int WEAPONSKILL_ACTIVATION_TIME = 0;
        private bool shouldPrioritizeWeaponskill = false;
        private bool hasCastedDistractIII = false;
        private bool hasCastedFrazzleIII = false;
        private bool hasCastedParalyze = false;
        private bool hasCastedAddleII = false;
        private bool hasCastedInundation = false;

        public void setInundation(bool hasCastedInundation)
        {
            this.hasCastedInundation = hasCastedInundation;
        }

        public void setDistractIII(bool hasCastedDistractIII)
        {
            this.hasCastedDistractIII = hasCastedDistractIII;
        }

        public void setFrazzleIII(bool hasCastedFrazzleIII)
        {
            this.hasCastedFrazzleIII = hasCastedFrazzleIII;
        }

        public void setAddleII(bool param)
        {
            this.hasCastedAddleII = param;
        }

        public void setParalyze(bool hasCastedParalyze)
        {
            this.hasCastedParalyze = hasCastedParalyze;
        }

        const int PHALANX_II_COOLDOWN = 4000;
        bool utsusemiVariable = false;

        public void setUtsusemiVariable(bool booleanInput)
        {
            utsusemiVariable = booleanInput;
        }

        bool shouldDoStuff = true;

        private void button10_Click(object sender, EventArgs e)
        {
            button10.Enabled = false;
            button11.Enabled = true;
            if (battle == false)
            {
                if ((string)comboBox4.SelectedItem == "Fast Blade")
                {
                    weaponskill = "Fast Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Circle Blade")
                {
                    weaponskill = "Circle Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Flat Blade")
                {
                    weaponskill = "Flat Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Burning Blade")
                {
                    weaponskill = "Burning Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Red Lotus Blade")
                {
                    weaponskill = "Red Lotus Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Wasp Sting")
                {
                    weaponskill = "Wasp Sting";
                }
                else if ((string)comboBox4.SelectedItem == "Gust Slash")
                {
                    weaponskill = "Gust Slash";
                }
                else if ((string)comboBox4.SelectedItem == "Viper Bite")
                {
                    weaponskill = "Viper Bite";
                }
                else if ((string)comboBox4.SelectedItem == "Shining Blade")
                {
                    weaponskill = "Shining Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Seraph Blade")
                {
                    weaponskill = "Seraph Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Vorpal Blade")
                {
                    weaponskill = "Vorpal Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Savage Blade")
                {
                    weaponskill = "Savage Blade";
                }
                else if ((string)comboBox4.SelectedItem == "Requiescat")
                {
                    weaponskill = "Requiescat";
                }
                else if ((string)comboBox4.SelectedItem == "Knights of Round")
                {
                    weaponskill = "Knights of Round";
                }
                else if ((string)comboBox4.SelectedItem == "Chant du Cygne")
                {
                    weaponskill = "Chant du Cygne";
                }
                else if ((string)comboBox4.SelectedItem == "Death Blossom")
                {
                    weaponskill = "Death Blossom";
                }
                else if ((string)comboBox4.SelectedItem == "Empyreal Arrow")
                {
                    weaponskill = "Empyreal Arrow";
                }
                else
                {
                    weaponskill = "";
                }

                if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
                {
                    if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                    {
                        AutoItX.WinActivate("[CLASS:FFXiClass]");
                    }
                }
                battle = true;
                shouldDoStuff = true;
            }

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        if (weaponskillPriorityQueue.size() > 0)
                        {
                            Class1 action = weaponskillPriorityQueue.getData();
                            appendText2("Weaponskill priority queue size is: " + weaponskillPriorityQueue.size());
                            appendText2("****NEXT ACTIION IS " + action.getAction() + "****");
                            weaponskillPriorityQueue.remove();

                            action.function1();
                            new Thread(() =>
                            {
                                Thread.Sleep(action.getCooldownTime());
                                actionTable.Remove(action.getAction());
                            }).Start();
                        }
                        else if (assistPriorityQueue.size() > 0)
                        {
                            Class1 action = assistPriorityQueue.getData();
                            appendText2("Assist priority queue size is: " + assistPriorityQueue.size());
                            appendText2("****NEXT ACTIION IS " + action.getAction() + "****");
                            assistPriorityQueue.remove();

                            action.function1();
                            new Thread(() =>
                            {
                                Thread.Sleep(action.getCooldownTime());
                                actionTable.Remove(action.getAction());
                            }).Start();
                        }
                        else if (priorityQueueOne.size() > 0)
                        {
                            Class1 action = priorityQueueOne.getData();
                            appendText2("action priority queue size is: " + priorityQueueOne.size());
                            appendText2("****NEXT ACTIION IS " + action.getAction() + "****");
                            priorityQueueOne.remove();

                            action.function1();
                            new Thread(() =>
                            {
                                Thread.Sleep(action.getCooldownTime());
                                actionTable.Remove(action.getAction());
                            }).Start();
                        }
                        else if (priorityQueueTwo.size() > 0)
                        {
                            Class1 action = priorityQueueTwo.getData();
                            appendText2("action priority queue size is: " + priorityQueueTwo.size());
                            appendText2("****NEXT ACTIION IS " + action.getAction() + "****");
                            priorityQueueTwo.remove();

                            action.function1();
                            new Thread(() =>
                            {
                                Thread.Sleep(action.getCooldownTime());
                                actionTable.Remove(action.getAction());
                            }).Start();
                        }
                        else if (curePriorityQueue.size() > 0)
                        {
                            Class1 action = curePriorityQueue.getData();
                            appendText2("action priority queue size is: " + curePriorityQueue.size());
                            appendText2("****NEXT ACTIION IS " + action.getAction() + "****");
                            curePriorityQueue.remove();

                            action.function1();
                            new Thread(() =>
                            {
                                Thread.Sleep(action.getCooldownTime());
                                actionTable.Remove(action.getAction());
                            }).Start();
                        }
                        else if (actionPriorityQueue.size() > 0)
                        {
                            if (hasTP() == false)
                            {
                                Class1 action = actionPriorityQueue.getData();
                                appendText2("action priority queue size is: " + actionPriorityQueue.size());
                                appendText2("****NEXT ACTIION IS " + action.getAction() + "****");
                                actionPriorityQueue.remove();

                                action.function1();
                                new Thread(() =>
                                {
                                    Thread.Sleep(action.getCooldownTime());
                                    actionTable.Remove(action.getAction());
                                }).Start();
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    if (checkBox23.Checked == true)
                    {
                        if (hasReceivedDisengageTell() == true)
                        {
                            if (actionTable.Contains("Disengage") == false)
                            {
                                const int DISENGAGE_PRIORTY = 110;
                                Rectangle rectangle = new Rectangle();
                                Class1 disengage = new Class1("Disengage", 0, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                disengage.setPartySize(partySize);
                                disengage.setPartyMember(partySize);
                                actionTable.Add(disengage.getAction(), true);
                                assistPriorityQueue.insert(DISENGAGE_PRIORTY, disengage);
                            }
                        }
                    }

                    if (shouldDoStuff == true && hasReceivedStopDoingStuffTell() == true)
                    {
                        shouldDoStuff = false;
                        setParalyze(false);
                        setDistractIII(false);
                        setFrazzleIII(false);
                        setAddleII(false);
                        setInundation(false);
                        setUtsusemiVariable(false);

                        for (int i = 0; i < partyMemberRefreshTimer.Count; i++)
                        {
                            while (partyMemberRefreshTimer[i].Count > 0)
                            {
                                partyMemberRefreshTimer[i].Dequeue();
                            }
                        }

                        for (int i = 0; i < partyMemberHasteIITimer.Count; i++)
                        {
                            while (partyMemberHasteIITimer[i].Count > 0)
                            {
                                partyMemberHasteIITimer[i].Dequeue();
                            }
                        }

                        for (int i = 0; i < partyMemberPhalanxIITimer.Count; i++)
                        {
                            while (partyMemberPhalanxIITimer[i].Count > 0)
                            {
                                partyMemberPhalanxIITimer[i].Dequeue();
                            }
                        }

                        while (temperTimer.Count > 0)
                        {
                            temperTimer.Dequeue();
                        }
                    }
                    else if (shouldDoStuff == false && hasReceivedStartDoingStuffTell() == true)
                    {
                        shouldDoStuff = true;
                    }
                }
            }).Start();

            new Thread(() => 
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    new Thread(() =>
                    {
                        if (hasReceivedGravityIITell() == true)
                        {
                            if (actionTable.Contains("Gravity II") == false)
                            {
                                const int GRAVITY_II_COOLDOWN = 1000;
                                const int GRAVITY_II_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 gravityII = new Class1("Gravity II", GRAVITY_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                gravityII.setPartySize(partySize);
                                actionTable.Add(gravityII.getAction(), true);
                                assistPriorityQueue.insert(GRAVITY_II_PRIORITY, gravityII);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedBindTell() == true)
                        {
                            if (actionTable.Contains("Bind") == false)
                            {
                                const int BIND_COOLDOWN = 1000;
                                const int BIND_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 bind = new Class1("Bind", BIND_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                bind.setPartySize(partySize);
                                actionTable.Add(bind.getAction(), true);
                                assistPriorityQueue.insert(BIND_PRIORITY, bind);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedParalyzeIITell() == true)
                        {
                            if (actionTable.Contains("Paralyze II") == false)
                            {
                                const int PARALYZE_II_COOLDOWN = 1000;
                                const int PARALYZE_II_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 paralyzeII = new Class1("Paralyze II", PARALYZE_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                paralyzeII.setPartySize(partySize);
                                actionTable.Add(paralyzeII.getAction(), true);
                                assistPriorityQueue.insert(PARALYZE_II_PRIORITY, paralyzeII);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedSlowIITell() == true)
                        {
                            if (actionTable.Contains("Slow II") == false)
                            {
                                const int SLOW_II_COOLDOWN = 1000;
                                const int SLOW_II_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 slowII = new Class1("Slow II", SLOW_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                slowII.setPartySize(partySize);
                                actionTable.Add(slowII.getAction(), true);
                                assistPriorityQueue.insert(SLOW_II_PRIORITY, slowII);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedBlindIITell() == true)
                        {
                            if (actionTable.Contains("Blind II") == false)
                            {
                                const int BLIND_II_COOLDOWN = 1000;
                                const int BLIND_II_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 blindII = new Class1("Blind II", BLIND_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                blindII.setPartySize(partySize);
                                actionTable.Add(blindII.getAction(), true);
                                assistPriorityQueue.insert(BLIND_II_PRIORITY, blindII);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedSilenceTell() == true)
                        {
                            if (actionTable.Contains("Silence") == false)
                            {
                                const int SILENCE_COOLDOWN = 1000;
                                const int SILENCE_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 silence = new Class1("Silence", SILENCE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                silence.setPartySize(partySize);
                                actionTable.Add(silence.getAction(), true);
                                assistPriorityQueue.insert(SILENCE_PRIORITY, silence);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedBioIIITell() == true)
                        {
                            if (actionTable.Contains("Bio III") == false)
                            {
                                const int BIO_III_COOLDOWN = 1000;
                                const int BIO_III_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 bioIII = new Class1("Bio III", BIO_III_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                bioIII.setPartySize(partySize);
                                actionTable.Add(bioIII.getAction(), true);
                                assistPriorityQueue.insert(BIO_III_PRIORITY, bioIII);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedFrazzleIIITell() == true)
                        {
                            if (actionTable.Contains("Frazzle III") == false)
                            {
                                const int FRAZZLE_III_COOLDOWN = 1000;
                                const int FRAZZLE_III_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 frazzleIII = new Class1("Frazzle III", FRAZZLE_III_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                frazzleIII.setPartySize(partySize);
                                actionTable.Add(frazzleIII.getAction(), true);
                                assistPriorityQueue.insert(FRAZZLE_III_PRIORITY, frazzleIII);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedDistractIIITell() == true)
                        {
                            if (actionTable.Contains("Distract III") == false)
                            {
                                const int DISTRACT_III_COOLDOWN = 1000;
                                const int DISTRACT_III_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 distractIII = new Class1("Distract III", DISTRACT_III_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                distractIII.setPartySize(partySize);
                                actionTable.Add(distractIII.getAction(), true);
                                assistPriorityQueue.insert(DISTRACT_III_PRIORITY, distractIII);
                            }
                        }
                    }).Start();

                    new Thread(() =>
                    {
                        if (hasReceivedAddleIITell() == true)
                        {
                            if (actionTable.Contains("Addle III") == false)
                            {
                                const int ADDLE_II_COOLDOWN = 1000;
                                const int ADDLE_II_PRIORITY = 99;
                                Rectangle rectangle = new Rectangle();
                                Class1 addleII = new Class1("Addle III", ADDLE_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                addleII.setPartySize(partySize);
                                actionTable.Add(addleII.getAction(), true);
                                assistPriorityQueue.insert(ADDLE_II_PRIORITY, addleII);
                            }
                        }
                    }).Start();

                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (hasReceivedSavageBladeTell() == true)
                    {
                        if (this.weaponskill != "Savage Blade") this.weaponskill = "Savage Blade";
                    }
                    else if (hasReceivedChantDuCygneTell() == true)
                    {
                        if (this.weaponskill != "Chant du Cygne") this.weaponskill = "Chant du Cygne";
                    }
                    else if (hasReceivedDeathBlossomTell() == true)
                    {
                        if (this.weaponskill != "Death Blossom") this.weaponskill = "Death Blossom";
                    }
                    else if (hasReceivedRequiescatTell() == true)
                    {
                        if (this.weaponskill != "Requiescat") this.weaponskill = "Requiescat";
                    }
                    else if (hasReceivedKnightsOfRoundTell() == true)
                    {
                        if (this.weaponskill != "Knights of Round") this.weaponskill = "Knights of Round";
                    }
                    else if (hasReceivedRedLotusBladeTell() == true)
                    {
                        if (this.weaponskill != "Red Lotus Blade") this.weaponskill = "Red Lotus Blade";
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (hasReceivedDoNotEngageTell() == true)
                    {
                        setCheckBox58(false);
                    }
                    else if (hasReceivedTurnOnEngageTell() == true)
                    {
                        setCheckBox58(true);
                    }
                    else if (hasReceivedTurnOnDispelTell() == true)
                    {
                        setCheckBox57(true);
                    }
                    else if (hasReceivedTurnOffDispelTell() == true)
                    {
                        setCheckBox57(false);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    if (shouldDoStuff == true)
                    {
                        if (checkBox74.Checked == true)
                        {
                            if (isWeaponDrawn() == true)
                            {
                                if (actionTable.Contains("Ranged Attack") == false)
                                {
                                    const int RANGED_ATTACK_PRIORITY = 44;
                                    Rectangle rectangle = new Rectangle();
                                    Class1 rangedAttack = new Class1("Ranged Attack", 4000, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    rangedAttack.setPartySize(partySize);
                                    actionTable.Add(rangedAttack.getAction(), true);
                                    weaponskillPriorityQueue.insert(RANGED_ATTACK_PRIORITY, rangedAttack);
                                }
                                else
                                {
                                    appendText("Ranged attack is queued");
                                }
                            }
                        }
                    }
                }
            }).Start();

            const int PARTY_MEMBER_ONE = 1;
            const int PARTY_MEMBER_TWO = 2;
            const int PARTY_MEMBER_THREE = 3;
            const int PARTY_MEMBER_FOUR = 4;
            const int PARTY_MEMBER_FIVE = 5;
            const int PARTY_MEMBER_SIX = 6;
            const int COMPOSURE_PRIORITY = 80;
            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        appendText("Party Size: " + partySize);
                        if (isComposureActive() == false && checkBox21.Checked == true)
                        {
                            if (actionTable.Contains("Composure") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 composure = new Class1("Composure", composureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                composure.setPartySize(partySize);
                                actionTable.Add(composure.getAction(), true);
                                priorityQueueOne.insert(COMPOSURE_PRIORITY, composure);
                            }
                            else
                            {
                                appendText("Composure is already queued");
                            }
                        }
                        if (checkBox20.Checked == true && isUtsusemiActive() == false)
                        {
                            const int UTSUSEMI_NI_PRIORITY = 61;
                            const int UTSUSEMI_ICHI_PRIORITY = 60;

                            if (isComposureActive() == false)
                            {
                                if (isHasteActive() == false)
                                {
                                    utsusemiIchiCooldownTime = 23000;
                                    utsusemiNiCooldownTime = 35000;
                                }
                                else
                                {
                                    if (checkBox22.Checked == true)
                                    {
                                        utsusemiIchiCooldownTime = 19000;
                                        utsusemiNiCooldownTime = 28000;
                                    }
                                    else if (checkBox32.Checked == true)
                                    {
                                        utsusemiIchiCooldownTime = 14000;
                                        utsusemiNiCooldownTime = 21000;
                                    }
                                }
                            }
                            else
                            {
                                if (isHasteActive() == false)
                                {
                                    utsusemiIchiCooldownTime = 18000;
                                    utsusemiNiCooldownTime = 28000;
                                }
                                else
                                {
                                    if (checkBox22.Checked == true)
                                    {
                                        utsusemiIchiCooldownTime = 15000;
                                        utsusemiNiCooldownTime = 23000;
                                    }
                                    else if (checkBox32.Checked == true)
                                    {
                                        utsusemiIchiCooldownTime = 11000;
                                        utsusemiNiCooldownTime = 17000;
                                    }
                                }
                            }

                            if (utsusemiVariable == false)
                            {
                                if (actionTable.Contains("Utsusemi: Ni") == false)
                                {
                                    Rectangle rectangle = getStatusRectangle();
                                    Class1 utsusemiNi = new Class1("Utsusemi: Ni", utsusemiNiCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    utsusemiNi.setPartySize(partySize);
                                    actionTable.Add(utsusemiNi.getAction(), true);
                                    priorityQueueOne.insert(UTSUSEMI_NI_PRIORITY, utsusemiNi);

                                    setUtsusemiVariable(true);
                                }
                                else if (actionTable.Contains("Utsusemi: Ichi") == false)
                                {
                                    Rectangle rectangle = getStatusRectangle();
                                    Class1 utsusemiIchi = new Class1("Utsusemi: Ichi", utsusemiIchiCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    utsusemiIchi.setPartySize(partySize);
                                    actionTable.Add(utsusemiIchi.getAction(), true);
                                    priorityQueueOne.insert(UTSUSEMI_ICHI_PRIORITY, utsusemiIchi);

                                    setUtsusemiVariable(true);
                                }
                                else
                                {
                                    appendText("Utsusemi is queued");
                                }
                            }
                        }
                        if (amIStoneskined() == false && checkBox16.Checked == true)
                        {
                            const int STONESKIN_PRIORITY = 47;
                            if (actionTable.Contains("Stoneskin") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 stoneskin = new Class1("Stoneskin", stoneskinCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                stoneskin.setPartySize(partySize);
                                actionTable.Add(stoneskin.getAction(), true);
                                priorityQueueOne.insert(STONESKIN_PRIORITY, stoneskin);
                            }
                            else
                            {
                                appendText("Stoneskin is already queued");
                            }
                        }

                        if (barfiraTimer.Count == 0 && checkBox54.Checked == true)
                        {
                            if (actionTable.Contains("Barfira") == false)
                            {
                                const int BARFIRA_COOLDOWN = 10000;
                                const int BARFIRA_PRIORITY = 49;

                                Rectangle rectangle = new Rectangle();

                                Class1 barfira = new Class1("Barfira", BARFIRA_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                barfira.setPartySize(partySize);
                                barfira.setBarfiraTimerReference(barfiraTimer);
                                actionTable.Add(barfira.getAction(), true);
                                actionPriorityQueue.insert(BARFIRA_PRIORITY, barfira);
                            }
                            else
                            {
                                appendText("Barfira is queued");
                            }
                        }
                        else if (phalanxIITimer.Count == 0 && checkBox55.Checked == true)
                        {
                            if (actionTable.Contains("Phalanx II") == false)
                            {
                                const int PHALANX_II_PRIORITY = 38;

                                Rectangle rectangle = new Rectangle();

                                Class1 phalanxII = new Class1("Phalanx II", PHALANX_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                phalanxII.setPartySize(partySize);
                                phalanxII.setPhalanxIIReference(phalanxIITimer);
                                actionTable.Add(phalanxII.getAction(), true);
                                actionPriorityQueue.insert(PHALANX_II_PRIORITY, phalanxII);
                            }
                            else
                            {
                                appendText("Phalanx II is queued");
                            }
                        }

                        if (isPhalanxIIActive() == false && checkBox55.Checked == true)
                        {
                            while (phalanxIITimer.Count > 0)
                            {
                                phalanxIITimer.Dequeue();
                            }
                        }

                        if (isBarfiraActive() == false && checkBox54.Checked == true)
                        {
                            while (barfiraTimer.Count > 0)
                            {
                                barfiraTimer.Dequeue();
                            }
                        }


                        if (checkBox1.Checked == true)
                        {
                            const int FOLLOW_PARTY_MEMBER_TWO_PRIORITY = 109;
                            const int FOLLOW_DEAD_PARTY_MEMBER_TWO_PRIORITY = 109;
                            if (isWeaponDrawn() == false)
                            {
                                if (actionTable.Contains("Follow Party Member 2") == false)
                                {
                                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == true)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 followPartyMember = new Class1("Follow Party Member 2", 1000, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        followPartyMember.setPartySize(partySize);
                                        actionTable.Add(followPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(FOLLOW_DEAD_PARTY_MEMBER_TWO_PRIORITY, followPartyMember);
                                    }
                                    else if (followPartyMember2Queue.Count == 0)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 followPartyMember = new Class1("Follow Party Member 2", 1000, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        followPartyMember.setPartySize(partySize);
                                        actionTable.Add(followPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(FOLLOW_PARTY_MEMBER_TWO_PRIORITY, followPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Already following party member 2");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();

                        maintainPartyMP3(partySize);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (temperTimer.Count == 0 && checkBox33.Checked == true)
                        {
                            const int TEMPER_PRIORITY = 35;
                            if (actionTable.Contains("Temper") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 temper = new Class1("Temper", temperCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                temper.setPartySize(partySize);
                                temper.setTemperTimerReference(temperTimer);
                                actionTable.Add(temper.getAction(), true);
                                actionPriorityQueue.insert(TEMPER_PRIORITY, temper);
                            }
                            else
                            {
                                appendText("Temper is already queued");
                            }
                        }
                        else if (temperTimer.Count == 0 && checkBox51.Checked == true)
                        {
                            const int TEMPER_II_PRIORITY = 49;
                            if (actionTable.Contains("Temper II") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 temperII = new Class1("Temper II", temper2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                temperII.setPartySize(partySize);
                                temperII.setTemperTimerReference(temperTimer);
                                actionTable.Add(temperII.getAction(), true);
                                actionPriorityQueue.insert(TEMPER_II_PRIORITY, temperII);
                            }
                            else
                            {
                                appendText("Temper II is already queued");
                            }
                        }
                        else if (enwaterIITimer.Count == 0 && checkBox24.Checked == true)
                        {
                            const int ENWATER_II_PRIORITY = 48;
                            if (actionTable.Contains("Enwater II") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 enwaterII = new Class1("Enwater II", enwater2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                enwaterII.setPartySize(partySize);
                                enwaterII.setEnwaterIITimerReference(enwaterIITimer);
                                actionTable.Add(enwaterII.getAction(), true);
                                actionPriorityQueue.insert(ENWATER_II_PRIORITY, enwaterII);
                            }
                            else
                            {
                                appendText("Enwater II is already queued");
                            }
                        }
                        else if (isEnwaterActive() == false && checkBox24.Checked == true)
                        {
                            while (temperTimer.Count > 0)
                            {
                                temperTimer.Dequeue();
                            }

                            while (enwaterIITimer.Count > 0)
                            {
                                enwaterIITimer.Dequeue();
                            }
                        }
                        else if (isHasteSambaActive() == false && checkBox37.Checked == true)
                        {
                            const int HASTE_SAMBA_PRIORITY = 13;
                            if (actionTable.Contains("Haste Samba") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 hasteSamba = new Class1("Haste Samba", 5000, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                hasteSamba.setPartySize(partySize);
                                actionTable.Add(hasteSamba.getAction(), true);
                                actionPriorityQueue.insert(HASTE_SAMBA_PRIORITY, hasteSamba);
                            }
                            else
                            {
                                appendText("Haste Samba is already queued");
                            }
                        }
                        else if (isAdvancingMarchActive() == false && checkBox38.Checked == true)
                        {
                            const int ADVANCING_MARCH_PRIORITY = 13;
                            if (actionTable.Contains("Advancing March") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 advancingMarch = new Class1("Advancing March", 10000, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                advancingMarch.setPartySize(partySize);
                                actionTable.Add(advancingMarch.getAction(), true);
                                actionPriorityQueue.insert(ADVANCING_MARCH_PRIORITY, advancingMarch);
                            }
                            else
                            {
                                appendText("Advancing march is already queued");
                            }
                        }
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (isWeaponDrawn() == false)
                        {
                            if (checkBox23.Checked == true)
                            {
                                if (hasReceivedDisengageTell() == false)
                                {
                                    if (isTargettingAMonster(partySize) == false && isEngaged(partySize) == false)
                                    {
                                        if (actionTable.Contains("Assist2") == false)
                                        {
                                            if (isPartyMemberDead(2, partySize) == false)
                                            {
                                                const int ASSIST_PRIORITY = 46;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 assist = new Class1("Assist2", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1, textBox2, this);
                                                assist.setPartySize(partySize);
                                                actionTable.Add(assist.getAction(), true);
                                                assistPriorityQueue.insert(ASSIST_PRIORITY, assist);
                                            }
                                        }
                                    }
                                }
                            }
                            if (checkBox58.Checked == true)
                            {
                                if (isTargettingAMonster(partySize) == true || isEngaged(partySize) == true)
                                {
                                    if (actionTable.Contains("Check Engaged") == false)
                                    {
                                        const int ENGAGED_PRIORITY = 45;
                                        Rectangle rectangle = new Rectangle();
                                        Class1 engaged = new Class1("Check Engaged", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1, textBox2, this);
                                        engaged.setPartySize(partySize);
                                        engaged.setDistractIIIReference(ref hasCastedDistractIII);
                                        actionTable.Add(engaged.getAction(), true);
                                        assistPriorityQueue.insert(ENGAGED_PRIORITY, engaged);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (hasTP() && checkBox45.Checked == true)
                        {
                            appendText2("I have TP to weaponskill");
                            if (checkBox46.Checked == true)
                            {
                                if (isSkillchainActive() == false)
                                {
                                    if (actionTable.Contains("Start Skillchain") == false && actionTable.Contains("Close Skillchain") == false)
                                    {
                                        const int WEAPONSKILL_PRIORITY = 99;
                                        Rectangle rectangle = new Rectangle();
                                        Class1 weaponskill = new Class1("Start Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        weaponskill.setPartySize(partySize);
                                        weaponskill.setWeaponskill(this.weaponskill);
                                        actionTable.Add(weaponskill.getAction(), true);
                                        weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                    }
                                    else
                                    {
                                        appendText("Start Skillchain is queued");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (hasTP() && checkBox45.Checked == true)
                        {
                            appendText2("I have TP to skillchain");
                            if (checkBox47.Checked == true)
                            {
                                new Thread(() =>
                                {
                                    if (willSavageBladeMakeLevel3Light() == true && checkBox48.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox71.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                weaponskill.setWeaponskill("Savage Blade");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() =>
                                {
                                    if (willDeathBlossomMakeLevel3Light() == true && checkBox48.Checked == true)
                                    {
                                        if (checkBox70.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false && actionTable.Contains("Start Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                weaponskill.setWeaponskill("Death Blossom");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() =>
                                {
                                    if (willKnightsOfRoundMakeLevel4Light() == true && checkBox48.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox68.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                weaponskill.setWeaponskill("Knights of Round");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() =>
                                {
                                    if (willEmpyrealArrowMakeLevel3Light() == true && checkBox48.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox73.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                weaponskill.setFollow(checkBox1.Checked);
                                                weaponskill.setWeaponskill("Empyreal Arrow");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() =>
                                {
                                    if (willKnightsOfRoundMakeLevel3Light() == true && checkBox48.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox68.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                //weaponskill.setFollow(checkBox1.Checked);
                                                weaponskill.setWeaponskill("Knights of Round");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() =>
                                {
                                    if (willKnightsOfRoundMakeLevel2Fusion() == true && checkBox48.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox68.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                //weaponskill.setFollow(checkBox1.Checked);
                                                weaponskill.setWeaponskill("Knights of Round");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() => {
                                    if (willChantDuCygneMakeLevel4Light() == true && checkBox48.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox69.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                weaponskill.setWeaponskill("Chant du Cygne");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() => {
                                    if (willChantDuCygneMakeLevel3Darkness() == true && checkBox49.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox69.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                //weaponskill.setFollow(checkBox1.Checked);
                                                weaponskill.setWeaponskill("Chant du Cygne");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                new Thread(() => {
                                    if (willRequiescatMakeLevel3Darkness() == true && checkBox49.Checked == true && actionTable.Contains("Start Skillchain") == false)
                                    {
                                        if (checkBox72.Checked == true)
                                        {
                                            if (actionTable.Contains("Close Skillchain") == false)
                                            {
                                                const int WEAPONSKILL_PRIORITY = 99;
                                                Rectangle rectangle = new Rectangle();
                                                Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                                weaponskill.setPartySize(partySize);
                                                //weaponskill.setFollow(checkBox1.Checked);
                                                weaponskill.setWeaponskill("Requiescat");
                                                actionTable.Add(weaponskill.getAction(), true);
                                                weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                            }
                                            else
                                            {
                                                appendText("Close Skillchain is queued");
                                            }
                                        }
                                    }
                                }).Start();

                                //new Thread(() => {
                                //    if (willCircleBladeMakeLevel2Fragmentation() == true)
                                //    {
                                //        if (actionTable.Contains("Close Skillchain") == false && actionTable.Contains("Start Skillchain") == false)
                                //        {
                                //            const int WEAPONSKILL_PRIORITY = 99;
                                //            Rectangle rectangle = new Rectangle();
                                //            Class1 weaponskill = new Class1("Close Skillchain", WEAPONSKILL_ACTIVATION_TIME, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                //            weaponskill.setPartySize(partySize);
                                //            weaponskill.setFollow(checkBox1.Checked);
                                //            weaponskill.setWeaponskill("Circle Blade");
                                //            actionTable.Add(weaponskill.getAction(), true);
                                //            weaponskillPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                //        }
                                //        else
                                //        {
                                //            appendText("Close Skillchain is queued");
                                //        }
                                //    }
                                //}).Start();
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (isHasteActive() == false && checkBox22.Checked == true)
                        {
                            const int HASTE_PRIORITY = 16;
                            if (actionTable.Contains("Haste") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 haste = new Class1("Haste", hasteCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                haste.setPartySize(partySize);
                                actionTable.Add(haste.getAction(), true);
                                actionPriorityQueue.insert(HASTE_PRIORITY, haste);
                            }
                            else
                            {
                                appendText("Haste is already queued");
                            }
                        }
                        else if (isHasteActive() == false && checkBox32.Checked == true)
                        {
                            const int HASTE_II_PRIORITY = 63;
                            if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                            {
                                Rectangle rectangle = getStatusRectangle();
                                Class1 haste2 = new Class1("Haste II", hasteCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                haste2.setPartySize(partySize);
                                //haste2.setFollow(checkBox1.Checked);
                                actionTable.Add(haste2.getAction(), true);
                                priorityQueueOne
                                .insert(HASTE_II_PRIORITY, haste2);
                            }
                            else
                            {
                                appendText("Haste II is already queued");
                            }
                        }
                        else if (checkBox32.Checked == true)
                        {
                            if (checkBox39.Checked == true)
                            {
                                const int HASTE_II_PARTY_PRIORITY = 40;
                                if (partyMemberHasteIITimer[PARTY_MEMBER_TWO].Count == 0)
                                {
                                    if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        hasteIIPartyMember.setPartyMember(PARTY_MEMBER_TWO);
                                        hasteIIPartyMember.setTarget(target);
                                        hasteIIPartyMember.setPartySize(partySize);
                                        //hasteIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(hasteIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(HASTE_II_PARTY_PRIORITY, hasteIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Haste II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox40.Checked == true)
                            {
                                const int HASTE_II_PARTY_PRIORITY = 40;
                                if (partyMemberHasteIITimer[PARTY_MEMBER_THREE].Count == 0)
                                {
                                    if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        hasteIIPartyMember.setPartyMember(PARTY_MEMBER_THREE);
                                        hasteIIPartyMember.setTarget(target);
                                        hasteIIPartyMember.setPartySize(partySize);
                                        //hasteIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(hasteIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(HASTE_II_PARTY_PRIORITY, hasteIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Haste II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox41.Checked == true)
                            {
                                const int HASTE_II_PARTY_PRIORITY = 40;
                                if (partyMemberHasteIITimer[PARTY_MEMBER_FOUR].Count == 0)
                                {
                                    if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        hasteIIPartyMember.setPartyMember(PARTY_MEMBER_FOUR);
                                        hasteIIPartyMember.setTarget(target);
                                        hasteIIPartyMember.setPartySize(partySize);
                                        //hasteIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(hasteIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(HASTE_II_PARTY_PRIORITY, hasteIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Haste II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox42.Checked == true)
                            {
                                const int HASTE_II_PARTY_PRIORITY = 40;
                                if (partyMemberHasteIITimer[PARTY_MEMBER_FIVE].Count == 0)
                                {
                                    if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                        hasteIIPartyMember.setPartyMember(PARTY_MEMBER_FIVE);
                                        hasteIIPartyMember.setTarget(target);
                                        hasteIIPartyMember.setPartySize(partySize);
                                        //hasteIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(hasteIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(HASTE_II_PARTY_PRIORITY, hasteIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Haste II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox43.Checked == true)
                            {
                                const int HASTE_II_PARTY_PRIORITY = 40;
                                if (partyMemberHasteIITimer[PARTY_MEMBER_SIX].Count == 0)
                                {
                                    if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                        string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                        hasteIIPartyMember.setPartyMember(PARTY_MEMBER_SIX);
                                        hasteIIPartyMember.setTarget(target);
                                        hasteIIPartyMember.setPartySize(partySize);
                                        //hasteIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(hasteIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(HASTE_II_PARTY_PRIORITY, hasteIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Haste II Party member already queued");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox55.Checked == true)
                        {
                            const int PHALANX_II_PARTY_PRIORITY = 37;
                            if (checkBox60.Checked == true)
                            {
                                if (partyMemberPhalanxIITimer[PARTY_MEMBER_TWO].Count == 0)
                                {
                                    if (actionTable.Contains("Phalanx II") == false && actionTable.Contains("Phalanx II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 phalanxIIPartyMember = new Class1("Phalanx II Party Member", PHALANX_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        phalanxIIPartyMember.setPartyMember(PARTY_MEMBER_TWO);
                                        phalanxIIPartyMember.setTarget(target);
                                        phalanxIIPartyMember.setPartySize(partySize);
                                        //phalanxIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(phalanxIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(PHALANX_II_PARTY_PRIORITY, phalanxIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Phalanx II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox61.Checked == true)
                            {
                                if (partyMemberPhalanxIITimer[PARTY_MEMBER_THREE].Count == 0)
                                {
                                    if (actionTable.Contains("Phalanx II") == false && actionTable.Contains("Phalanx II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 phalanxIIPartyMember = new Class1("Phalanx II Party Member", PHALANX_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        phalanxIIPartyMember.setPartyMember(PARTY_MEMBER_THREE);
                                        phalanxIIPartyMember.setTarget(target);
                                        phalanxIIPartyMember.setPartySize(partySize);
                                        //phalanxIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(phalanxIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(PHALANX_II_PARTY_PRIORITY, phalanxIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Phalanx II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox62.Checked == true)
                            {
                                if (partyMemberPhalanxIITimer[PARTY_MEMBER_FOUR].Count == 0)
                                {
                                    if (actionTable.Contains("Phalanx II") == false && actionTable.Contains("Phalanx II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 phalanxIIPartyMember = new Class1("Phalanx II Party Member", PHALANX_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        phalanxIIPartyMember.setPartyMember(PARTY_MEMBER_FOUR);
                                        phalanxIIPartyMember.setTarget(target);
                                        phalanxIIPartyMember.setPartySize(partySize);
                                        //phalanxIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(phalanxIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(PHALANX_II_PARTY_PRIORITY, phalanxIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Phalanx II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox63.Checked == true)
                            {
                                if (partyMemberPhalanxIITimer[PARTY_MEMBER_FIVE].Count == 0)
                                {
                                    if (actionTable.Contains("Phalanx II") == false && actionTable.Contains("Phalanx II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 phalanxIIPartyMember = new Class1("Phalanx II Party Member", PHALANX_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                        phalanxIIPartyMember.setPartyMember(PARTY_MEMBER_FIVE);
                                        phalanxIIPartyMember.setTarget(target);
                                        phalanxIIPartyMember.setPartySize(partySize);
                                        //phalanxIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(phalanxIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(PHALANX_II_PARTY_PRIORITY, phalanxIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Phalanx II Party member already queued");
                                    }
                                }
                            }

                            if (checkBox64.Checked == true)
                            {
                                if (partyMemberPhalanxIITimer[PARTY_MEMBER_SIX].Count == 0)
                                {
                                    if (actionTable.Contains("Phalanx II") == false && actionTable.Contains("Phalanx II Party Member") == false)
                                    {
                                        Rectangle rectangle = new Rectangle();
                                        Class1 phalanxIIPartyMember = new Class1("Phalanx II Party Member", PHALANX_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                        phalanxIIPartyMember.setPartyMember(PARTY_MEMBER_SIX);
                                        phalanxIIPartyMember.setTarget(target);
                                        phalanxIIPartyMember.setPartySize(partySize);
                                        //phalanxIIPartyMember.setFollow(checkBox1.Checked);
                                        actionTable.Add(phalanxIIPartyMember.getAction(), true);
                                        actionPriorityQueue.insert(PHALANX_II_PARTY_PRIORITY, phalanxIIPartyMember);
                                    }
                                    else
                                    {
                                        appendText("Phalanx II Party member already queued");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() => {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (partySize == 6)
                        {
                            if (checkBox19.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_ONE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_TWO, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_THREE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_THREE);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox12.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_FOUR, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_FOUR);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox13.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_FIVE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_FIVE);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox14.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_SIX, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_SIX);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 5)
                        {
                            if (checkBox19.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_ONE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_TWO, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_THREE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_THREE);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox12.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_FOUR, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_FOUR);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox13.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_FIVE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_FIVE);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 4)
                        {
                            if (checkBox19.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_ONE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_TWO, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_THREE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_THREE);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox12.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_FOUR, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_FOUR);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 3)
                        {
                            if (checkBox19.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_ONE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_TWO, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_THREE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartySize(partySize);
                                        cureIV.setPartyMember(PARTY_MEMBER_THREE);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 2)
                        {
                            if (checkBox19.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_ONE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_TWO, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 1)
                        {
                            if (checkBox19.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPRoughlyHalfEmpty(PARTY_MEMBER_ONE, partySize) == true)
                                {
                                    if (actionTable.Contains("Cure IV") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIV.setTarget(target);
                                        cureIV.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIV.setPartySize(partySize);
                                        //cureIV.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIV.getAction(), true);
                                        curePriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                                    }
                                    else
                                    {
                                        appendText("Cure IV is already queued");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (partySize == 6)
                        {
                            if (checkBox2.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPToppedOff(PARTY_MEMBER_ONE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPToppedOff(PARTY_MEMBER_TWO, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPToppedOff(PARTY_MEMBER_THREE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_THREE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox12.Checked == true && isHPToppedOff(PARTY_MEMBER_FOUR, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_FOUR);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox13.Checked == true && isHPToppedOff(PARTY_MEMBER_FIVE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_FIVE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox14.Checked == true && isHPToppedOff(PARTY_MEMBER_SIX, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartySize(partySize);
                                        cureIII.setPartyMember(PARTY_MEMBER_SIX);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 5)
                        {
                            if (checkBox2.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPToppedOff(PARTY_MEMBER_ONE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPToppedOff(PARTY_MEMBER_TWO, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPToppedOff(PARTY_MEMBER_THREE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_THREE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox12.Checked == true && isHPToppedOff(PARTY_MEMBER_FOUR, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_FOUR);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox13.Checked == true && isHPToppedOff(PARTY_MEMBER_FIVE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_FIVE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 4)
                        {
                            if (checkBox2.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPToppedOff(PARTY_MEMBER_ONE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPToppedOff(PARTY_MEMBER_TWO, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPToppedOff(PARTY_MEMBER_THREE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_THREE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox12.Checked == true && isHPToppedOff(PARTY_MEMBER_FOUR, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_FOUR);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 3)
                        {
                            if (checkBox2.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPToppedOff(PARTY_MEMBER_ONE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPToppedOff(PARTY_MEMBER_TWO, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox11.Checked == true && isHPToppedOff(PARTY_MEMBER_THREE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_THREE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 2)
                        {
                            if (checkBox2.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPToppedOff(PARTY_MEMBER_ONE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                                else if (checkBox10.Checked == true && isHPToppedOff(PARTY_MEMBER_TWO, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_TWO);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                            }
                        }
                        else if (partySize == 1)
                        {
                            if (checkBox2.Checked == true)
                            {
                                if (checkBox9.Checked == true && isHPToppedOff(PARTY_MEMBER_ONE, partySize) == false)
                                {
                                    if (actionTable.Contains("Cure III") == false)
                                    {
                                        Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                        Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                        cureIII.setTarget(target);
                                        cureIII.setPartyMember(PARTY_MEMBER_ONE);
                                        cureIII.setPartySize(partySize);
                                        //cureIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(cureIII.getAction(), true);
                                        curePriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                                    }
                                    else
                                    {
                                        appendText("Cure III is already queued");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            //new Thread(() =>
            //{
            //    while (battle == true)
            //    {
            //        if (shouldDoStuff == true)
            //        {
            //            int partySize = getPartySize();
            //            raiseParty(partySize);
            //        }
            //        else
            //        {
            //            Thread.Sleep(1000);
            //        }
            //    }
            //}).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox35.Checked == true)
                        {
                            if (isSilenced() == true)
                            {
                                if (actionTable.Contains("Echo Drops") == false)
                                {
                                    const int ECHO_DROPS_PRIORITY = 98;
                                    Rectangle rectangle = new Rectangle();
                                    Class1 echoDrops = new Class1("Echo Drops", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1, textBox2, this);
                                    echoDrops.setPartySize(partySize);
                                    actionTable.Add(echoDrops.getAction(), true);
                                    actionPriorityQueue.insert(ECHO_DROPS_PRIORITY, echoDrops);
                                }
                                else
                                {
                                    appendText("Use echo drops is already queued");
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox34.Checked == true)
                        {
                            if (isParalyzed() == true)
                            {
                                if (actionTable.Contains("Remedy") == false)
                                {
                                    const int REMEDY_PRIORITY = 100;
                                    Rectangle rectangle = new Rectangle();
                                    Class1 remedy = new Class1("Remedy", 0, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    remedy.setPartySize(partySize);
                                    actionTable.Add(remedy.getAction(), true);
                                    actionPriorityQueue.insert(REMEDY_PRIORITY, remedy);
                                }
                                else
                                {
                                    appendText("Use remedy already queued");
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox34.Checked == true)
                        {
                            if (isParalyzed() == true)
                            {
                                if (actionTable.Contains("Remedy") == false)
                                {
                                    const int REMEDY_PRIORITY = 101;
                                    Rectangle rectangle = new Rectangle();
                                    Class1 remedy = new Class1("Remedy", 0, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    remedy.setPartySize(partySize);
                                    actionTable.Add(remedy.getAction(), true);
                                    actionPriorityQueue.insert(REMEDY_PRIORITY, remedy);
                                }
                                else
                                {
                                    appendText("Use remedy already queued");
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox36.Checked == true)
                        {
                            if (isCursed() == true)
                            {
                                if (actionTable.Contains("Holy Water") == false)
                                {
                                    const int HOLY_WATER_PRIORITY = 100;
                                    Rectangle rectangle = new Rectangle();
                                    Class1 holyWater = new Class1("Holy Water", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1);
                                    holyWater.setPartySize(partySize);
                                    actionTable.Add(holyWater.getAction(), true);
                                    actionPriorityQueue.insert(HOLY_WATER_PRIORITY, holyWater);
                                }
                                else
                                {
                                    appendText("Use holy water already queued");
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox44.Checked == true)
                        {
                            if (isMPLow2(PARTY_MEMBER_ONE, partySize) == true
                                && isPartyMemberOrangeHP(1, partySize) == false
                                && isPartyMemberRedHP(1, partySize) == false)
                            {
                                int CONVERT_PRIORITY = 98;
                                if (actionTable.Contains("Convert") == false)
                                {
                                    Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                                    Class1 convert = new Class1("Convert", 500000, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    convert.setPartySize(partySize);
                                    //convert.setFollow(checkBox1.Checked);
                                    actionTable.Add(convert.getAction(), true);
                                    priorityQueueOne.insert(CONVERT_PRIORITY, convert);
                                }
                                else
                                {
                                    appendText("Convert is already queued");
                                }
                            }
                            else
                            {
                                appendText("Party member 1 doesn't need to convert");
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();


            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox52.Checked == true)
                        {
                            if (isWeaponDrawn() == false)
                            {
                                if (actionTable.Contains("Pull Mob") == false)
                                {
                                    const int PULL_MOB_PRIORITY = 48;
                                    Rectangle rectangle = new Rectangle();
                                    Class1 pullMob = new Class1("Pull Mob", 1000, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    pullMob.setPartySize(partySize);
                                    actionTable.Add(pullMob.getAction(), true);
                                    actionPriorityQueue.insert(PULL_MOB_PRIORITY, pullMob);
                                }
                                else
                                {
                                    appendText("Pull mob is already queued");
                                }

                                if (followPartyMember2Queue.Count > 0 || followQueue.Count > 0)
                                {
                                    if (actionTable.Contains("Stop Follow") == false)
                                    {
                                        const int STOP_FOLLOW_PRIORITY = 9;
                                        Rectangle rectangle = new Rectangle();
                                        Class1 stopFollowAction = new Class1("Stop Follow", 25000, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                        stopFollowAction.setPartySize(partySize);
                                        actionTable.Add(stopFollowAction.getAction(), true);
                                        actionPriorityQueue.insert(STOP_FOLLOW_PRIORITY, stopFollowAction);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    if (isDead() == true)
                    {
                        battle = false;
                        control.BeginInvoke((MethodInvoker)delegate ()
                        {
                            button10.Enabled = true;
                            button11.Enabled = false;
                        });
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox59.Checked == true)
                        {
                            if (isWeaponDrawn() == true)
                            {
                                if (hasCastedParalyze == false)
                                {
                                    if (actionTable.Contains("Paralyze") == false)
                                    {
                                        const int PARALYZE_COOLDOWM = 4000;
                                        const int PARALYZE_PRIORITY = 89;

                                        Rectangle rectangle = new Rectangle();
                                        Class1 paralyze = new Class1("Paralyze", PARALYZE_COOLDOWM, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        paralyze.setPartySize(partySize);
                                        //paralyze.setFollow(checkBox1.Checked);

                                        actionTable.Add(paralyze.getAction(), true);
                                        actionPriorityQueue.insert(PARALYZE_PRIORITY, paralyze);
                                    }
                                    else
                                    {
                                        appendText("Paralyze is queued");
                                    }
                                }
                            }
                        }
                        if (checkBox53.Checked == true)
                        {
                            if (isWeaponDrawn() == true)
                            {
                                if (hasCastedDistractIII == false)
                                {
                                    if (actionTable.Contains("Distract III") == false)
                                    {
                                        const int DISTRACT_III_COOLDOWN = 6000;
                                        const int DISTRACT_III_PRIORITY = 90;

                                        Rectangle rectangle = new Rectangle();
                                        Class1 distractIII = new Class1("Distract III", DISTRACT_III_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        distractIII.setPartySize(partySize);
                                        //distractIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(distractIII.getAction(), true);
                                        actionPriorityQueue.insert(DISTRACT_III_PRIORITY, distractIII);
                                    }
                                    else
                                    {
                                        appendText("Distract III is queued");
                                    }
                                }
                            }
                        }

                        if (checkBox65.Checked == true)
                        {
                            if (isWeaponDrawn() == true)
                            {
                                if (hasCastedFrazzleIII == false)
                                {
                                    if (actionTable.Contains("Frazzle III") == false)
                                    {
                                        const int FRAZZLE_III_COOLDOWN = 6000;
                                        const int FRAZZLE_III_PRIORITY = 91;

                                        Rectangle rectangle = new Rectangle();
                                        Class1 frazzleIII = new Class1("Frazzle III", FRAZZLE_III_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        frazzleIII.setPartySize(partySize);
                                        //frazzleIII.setFollow(checkBox1.Checked);
                                        actionTable.Add(frazzleIII.getAction(), true);
                                        actionPriorityQueue.insert(FRAZZLE_III_PRIORITY, frazzleIII);
                                    }
                                    else
                                    {
                                        appendText("Frazzle III is queued");
                                    }
                                }
                            }
                        }

                        if (checkBox66.Checked == true)
                        {
                            if (isWeaponDrawn() == true)
                            {
                                if (hasCastedAddleII == false)
                                {
                                    if (actionTable.Contains("Addle II") == false)
                                    {
                                        const int ADDLE_II_COOLDOWN = 12000;
                                        const int ADDLE_II_PRIORITY = 88;

                                        Rectangle rectangle = new Rectangle();
                                        Class1 addleII = new Class1("Addle II", ADDLE_II_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        addleII.setPartySize(partySize);
                                        //addleII.setFollow(checkBox1.Checked);
                                        actionTable.Add(addleII.getAction(), true);
                                        actionPriorityQueue.insert(ADDLE_II_PRIORITY, addleII);
                                    }
                                }
                            }
                        }

                        if (checkBox67.Checked == true)
                        {
                            if (isWeaponDrawn() == true)
                            {
                                if (hasCastedInundation == false)
                                {
                                    if (actionTable.Contains("Inundation") == false)
                                    {
                                        const int INUNDATION_COOLDOWN = 12000;
                                        const int INUNDATION_PRIORITY = 92;

                                        Rectangle rectangle = new Rectangle();
                                        Class1 inundation = new Class1("Inundation", INUNDATION_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                        inundation.setPartySize(partySize);
                                        actionTable.Add(inundation.getAction(), true);
                                        actionPriorityQueue.insert(INUNDATION_PRIORITY, inundation);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (shouldDoStuff == true)
                    {
                        int partySize = getPartySize();
                        if (checkBox56.Checked == true)
                        {
                            if (actionTable.Contains("Thunder") == false)
                            {
                                const int THUNDER_COOLDOWN = 1000;
                                const int THUNDER_PRIORITY = 9;
                                if (isEngaged(partySize) == true)
                                {
                                    Rectangle rectangle = getTargetRectangle(partySize);
                                    Class1 thunder = new Class1("Thunder", THUNDER_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    thunder.setPartySize(partySize);
                                    //thunder.setFollow(checkBox1.Checked);
                                    actionTable.Add(thunder.getAction(), true);
                                    actionPriorityQueue.insert(THUNDER_PRIORITY, thunder);
                                }
                            }
                            else
                            {
                                appendText("Thunder is queued");
                            }
                        }
                        if (checkBox57.Checked == true)
                        {
                            if (actionTable.Contains("Dispel") == false)
                            {
                                const int DISPEL_COOLDOWN = 6000;
                                const int DISPEL_PRIORITY = 40;
                                if (isEngaged(partySize) == true)
                                {
                                    Rectangle rectangle = getTargetRectangle(partySize);
                                    Class1 dispel = new Class1("Dispel", DISPEL_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                    dispel.setPartySize(partySize);
                                    //dispel.setFollow(checkBox1.Checked);
                                    actionTable.Add(dispel.getAction(), true);
                                    actionPriorityQueue.insert(DISPEL_PRIORITY, dispel);
                                }
                            }
                            else
                            {
                                appendText("Dispel is queued");
                            }
                        }
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    if (hasReceivedFollowMeTell() == true)
                    {
                        if (actionTable.Contains("Follow Me") == false)
                        {
                            const int FOLLOW_ME_PRIORITY = 9999;
                            const int FOLLOW_ME_TELL_WINDOW_IN_MILLISECONDS = 3000;
                            Rectangle rectangle = new Rectangle();
                            Class1 followMe = new Class1("Follow Me", FOLLOW_ME_TELL_WINDOW_IN_MILLISECONDS, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            followMe.setPartySize(partySize);
                            actionTable.Add(followMe.getAction(), true);
                            assistPriorityQueue.insert(FOLLOW_ME_PRIORITY, followMe);
                        }
                    }
                    else if (hasReceivedStopRightThereTell() == true)
                    {
                        if (actionTable.Contains("Stop Right There") == false)
                        {
                            const int STOP_RIGHT_THERE_PRIORITY = 9999;
                            const int STOP_RIGHT_THERE_TELL_WINDOW_IN_MILLISECONDS = 3000; // The in-game tell message is open for 3 seconds
                            Rectangle rectangle = new Rectangle();
                            Class1 stopRightThere = new Class1("Stop Right There", STOP_RIGHT_THERE_TELL_WINDOW_IN_MILLISECONDS, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            stopRightThere.setPartySize(partySize);
                            actionTable.Add(stopRightThere.getAction(), true);
                            assistPriorityQueue.insert(STOP_RIGHT_THERE_PRIORITY, stopRightThere);
                        }
                    }
                }
            }).Start();
        }

        private List<Queue<bool>> partyMemberHasteIITimer = new List<Queue<bool>> { new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>() };
        private List<Queue<bool>> partyMemberPhalanxIITimer = new List<Queue<bool>> { new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>() };

        public Queue<bool> getPartyPhalanxIITimer(int partyMember)
        {
            if (partyMember == 1)
            {
                return partyMemberPhalanxIITimer[1];
            }
            else if (partyMember == 2)
            {
                return partyMemberPhalanxIITimer[2];
            }
            else if (partyMember == 3)
            {
                return partyMemberPhalanxIITimer[3];
            }
            else if (partyMember == 4)
            {
                return partyMemberPhalanxIITimer[4];
            }
            else if (partyMember == 5)
            {
                return partyMemberPhalanxIITimer[5];
            }
            else if (partyMember == 6)
            {
                return partyMemberPhalanxIITimer[6];
            }
            else
            {
                return null;
            }
        }

        private void raiseParty(int partySize)
        {
            const int RAISE_COOLDOWN = 37000;
            const int PARTY_MEMBER_ONE = 1;
            const int PARTY_MEMBER_TWO = 2;
            const int PARTY_MEMBER_THREE = 3;
            const int PARTY_MEMBER_FOUR = 4;
            const int PARTY_MEMBER_FIVE = 5;
            const int PARTY_MEMBER_SIX = 6;
            const int RAISE_PRIORITY = 80;

            if (partySize == 6)
            {
                if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == true && checkBox27.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_TWO, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_TWO);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == true && checkBox28.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_THREE, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_THREE);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == true && checkBox29.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_FOUR, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_FOUR);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == true && checkBox30.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_FIVE, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_FIVE);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_SIX, partySize) == true && checkBox31.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_SIX, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_SIX);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
            }
            else if (partySize == 5)
            {
                if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == true && checkBox27.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_TWO, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_TWO);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == true && checkBox28.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_THREE, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_THREE);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == true && checkBox29.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_FOUR, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_FOUR);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == true && checkBox30.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_FIVE, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_FIVE);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
            }
            else if (partySize == 4)
            {
                if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == true && checkBox27.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_TWO, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_TWO);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == true && checkBox28.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_THREE, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_THREE);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == true && checkBox29.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_FOUR, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_FOUR);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
            }
            else if (partySize == 3)
            {
                if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == true && checkBox27.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_TWO, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_TWO);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == true && checkBox28.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_THREE, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_THREE);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText("Raise is already queued");
                    }
                }
            }
            else if (partySize == 2)
            {
                if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == true && checkBox27.Checked == true)
                {
                    if (actionTable.Contains("Raise") == false)
                    {
                        Rectangle rectangle = getPartyMemberDeadRectangle(PARTY_MEMBER_TWO, partySize);
                        Class1 raise = new Class1("Raise", RAISE_COOLDOWN, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                        string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                        raise.setTarget(target);
                        raise.setPartyMember(PARTY_MEMBER_TWO);
                        raise.setPartySize(partySize);
                        actionTable.Add("Raise", true);
                        actionPriorityQueue.insert(RAISE_PRIORITY, raise);
                    }
                    else
                    {
                        appendText2("Raise is already queued");
                    }
                }
            }
        }

        private void maintainPartyMP(int partySize)
        {
            const int PARTY_MEMBER_ONE = 1;
            const int PARTY_MEMBER_TWO = 2;
            const int PARTY_MEMBER_THREE = 3;
            const int PARTY_MEMBER_FOUR = 4;
            const int PARTY_MEMBER_FIVE = 5;
            const int PARTY_MEMBER_SIX = 6;
            const int REFRESH_ME_PRIORITY = 51;
            const int REFRESH_PARTY_PRIORITY = 49;
            const int REFRESH_II_ME_PRIORITY = 51;
            const int REFRESH_II_PARTY_PRIORITY = 49;
            if (checkBox26.Checked == true)
            {
                if (partySize == 6)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_ONE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_ME_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_FOUR);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_FIVE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_SIX, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_SIX, partySize) == false && checkBox8.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_SIX].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_SIX, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_SIX);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                }
                else if (partySize == 5)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_ONE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_ME_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_FOUR);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_FIVE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                }
                else if (partySize == 4)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_ONE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_ME_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_FOUR);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                }
                else if (partySize == 3)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_ONE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_ME_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                }
                else if (partySize == 2)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_ONE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_ME_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_PARTY_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                }
                else if (partySize == 1)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh II") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refreshII = new Class1("Refresh II", refresh2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refreshII.setTarget(target);
                                refreshII.setPartyMember(PARTY_MEMBER_ONE);
                                refreshII.setPartySize(partySize);
                                refreshII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshII.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_II_ME_PRIORITY, refreshII);
                            }
                        }
                        else
                        {
                            appendText("Refresh II is already queued");
                        }
                    }
                }
            }
            else if (checkBox18.Checked == true)
            {
                if (partySize == 6)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FOUR);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FIVE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_SIX, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_SIX, partySize) == false && checkBox8.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_SIX].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_SIX, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_SIX);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                if (partySize == 5)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FOUR);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FIVE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 4)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FOUR);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 3)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 2)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 1)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
            }
        }

        private void maintainPartyMP3(int partySize)
        {
            const int PARTY_MEMBER_ONE = 1;
            const int PARTY_MEMBER_TWO = 2;
            const int PARTY_MEMBER_THREE = 3;
            const int PARTY_MEMBER_FOUR = 4;
            const int PARTY_MEMBER_FIVE = 5;
            const int PARTY_MEMBER_SIX = 6;
            const int REFRESH_ME_PRIORITY = 51;
            const int REFRESH_PARTY_PRIORITY = 42;
            const int REFRESH_III_ME_PRIORITY = 51;
            const int REFRESH_III_PARTY_PRIORITY = 42;
            if (checkBox50.Checked == true)
            {
                if (partySize == 6)
                {
                    //if (checkBox3.Checked == true)
                    //{
                    //    if (actionTable.Contains("Refresh III") == false)
                    //    {
                    //        if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                    //        {
                    //            Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                    //            Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                    //            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                    //            refreshIII.setTarget(target);
                    //            refreshIII.setPartyMember(PARTY_MEMBER_ONE);
                    //            refreshIII.setPartySize(partySize);
                    //            refreshIII.setFollow(checkBox1.Checked);
                    //            refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                    //            actionTable.Add(refreshIII.getAction(), true);
                    //            actionPriorityQueue.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        appendText("Refresh III is already queued");
                    //    }
                    //}

                    if (isRefreshActive() == false)
                    {
                        if (actionTable.Contains("Refresh III Me") == false && actionTable.Contains("Refresh III") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 refreshIII = new Class1("Refresh III Me", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            refreshIII.setPartySize(partySize);
                            //refreshIII.setFollow(checkBox1.Checked);
                            actionTable.Add(refreshIII.getAction(), true);
                            priorityQueueOne.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                        }
                        else
                        {
                            appendText("Refresh III is queued");
                        }
                    }

                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_FOUR);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_FIVE);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_SIX, partySize) == false && checkBox8.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_SIX].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_SIX, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_SIX);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                }
                else if (partySize == 5)
                {
                    //if (checkBox3.Checked == true)
                    //{
                    //    if (actionTable.Contains("Refresh III") == false)
                    //    {
                    //        if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                    //        {
                    //            Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                    //            Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                    //            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                    //            refreshIII.setTarget(target);
                    //            refreshIII.setPartyMember(PARTY_MEMBER_ONE);
                    //            refreshIII.setPartySize(partySize);
                    //            refreshIII.setFollow(checkBox1.Checked);
                    //            refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                    //            actionTable.Add(refreshIII.getAction(), true);
                    //            actionPriorityQueue.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        appendText("Refresh III is already queued");
                    //    }
                    //}

                    if (isRefreshActive() == false)
                    {
                        if (actionTable.Contains("Refresh III Me") == false && actionTable.Contains("Refresh III") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 refreshIII = new Class1("Refresh III Me", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            refreshIII.setPartySize(partySize);
                            //refreshIII.setFollow(checkBox1.Checked);
                            actionTable.Add(refreshIII.getAction(), true);
                            priorityQueueOne.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                        }
                        else
                        {
                            appendText("Refresh III is queued");
                        }
                    }

                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshIII.setPartySize(partySize);
                                // refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_FOUR);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_FIVE);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                }
                else if (partySize == 4)
                {
                    //if (checkBox3.Checked == true)
                    //{
                    //    if (actionTable.Contains("Refresh III") == false)
                    //    {
                    //        if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                    //        {
                    //            Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                    //            Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                    //            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                    //            refreshIII.setTarget(target);
                    //            refreshIII.setPartyMember(PARTY_MEMBER_ONE);
                    //            refreshIII.setPartySize(partySize);
                    //            refreshIII.setFollow(checkBox1.Checked);
                    //            refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                    //            actionTable.Add(refreshIII.getAction(), true);
                    //            actionPriorityQueue.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        appendText("Refresh III is already queued");
                    //    }
                    //}

                    if (isRefreshActive() == false)
                    {
                        if (actionTable.Contains("Refresh III Me") == false && actionTable.Contains("Refresh III") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 refreshIII = new Class1("Refresh III Me", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            refreshIII.setPartySize(partySize);
                            //refreshIII.setFollow(checkBox1.Checked);
                            actionTable.Add(refreshIII.getAction(), true);
                            priorityQueueOne.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                        }
                        else
                        {
                            appendText("Refresh III is queued");
                        }
                    }

                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_FOUR);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                }
                else if (partySize == 3)
                {
                    //if (checkBox3.Checked == true)
                    //{
                    //    if (actionTable.Contains("Refresh III") == false)
                    //    {
                    //        if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                    //        {
                    //            Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                    //            Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                    //            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                    //            refreshIII.setTarget(target);
                    //            refreshIII.setPartyMember(PARTY_MEMBER_ONE);
                    //            refreshIII.setPartySize(partySize);
                    //            refreshIII.setFollow(checkBox1.Checked);
                    //            refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                    //            actionTable.Add(refreshIII.getAction(), true);
                    //            actionPriorityQueue.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        appendText("Refresh III is already queued");
                    //    }
                    //}

                    if (isRefreshActive() == false)
                    {
                        if (actionTable.Contains("Refresh III Me") == false && actionTable.Contains("Refresh III") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 refreshIII = new Class1("Refresh III Me", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            refreshIII.setPartySize(partySize);
                            //refreshIII.setFollow(checkBox1.Checked);
                            actionTable.Add(refreshIII.getAction(), true);
                            priorityQueueOne.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                        }
                        else
                        {
                            appendText("Refresh III is queued");
                        }
                    }

                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshIII.setPartySize(partySize);
                                //refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_THREE);
                                refreshIII.setPartySize(partySize);
                                // refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                }
                else if (partySize == 2)
                {
                    //if (checkBox3.Checked == true)
                    //{
                    //    if (actionTable.Contains("Refresh III") == false)
                    //    {
                    //        if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                    //        {
                    //            Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                    //            Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                    //            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                    //            refreshIII.setTarget(target);
                    //            refreshIII.setPartyMember(PARTY_MEMBER_ONE);
                    //            refreshIII.setPartySize(partySize);
                    //            refreshIII.setFollow(checkBox1.Checked);
                    //            refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                    //            actionTable.Add(refreshIII.getAction(), true);
                    //            actionPriorityQueue.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        appendText("Refresh III is already queued");
                    //    }
                    //}

                    if (isRefreshActive() == false)
                    {
                        if (actionTable.Contains("Refresh III Me") == false && actionTable.Contains("Refresh III") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 refreshIII = new Class1("Refresh III Me", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            refreshIII.setPartySize(partySize);
                            // refreshIII.setFollow(checkBox1.Checked);
                            actionTable.Add(refreshIII.getAction(), true);
                            priorityQueueOne.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                        }
                        else
                        {
                            appendText("Refresh III is queued");
                        }
                    }

                    if (isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh III") == false && actionTable.Contains("Refresh III Me") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refreshIII.setTarget(target);
                                refreshIII.setPartyMember(PARTY_MEMBER_TWO);
                                refreshIII.setPartySize(partySize);
                                // refreshIII.setFollow(checkBox1.Checked);
                                refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refreshIII.getAction(), true);
                                priorityQueueTwo.insert(REFRESH_III_PARTY_PRIORITY, refreshIII);
                            }
                        }
                        else
                        {
                            appendText("Refresh III is already queued");
                        }
                    }
                }
                else if (partySize == 1)
                {
                    //if (checkBox3.Checked == true)
                    //{
                    //    if (actionTable.Contains("Refresh III") == false)
                    //    {
                    //        if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                    //        {
                    //            Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                    //            Class1 refreshIII = new Class1("Refresh III", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                    //            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                    //            refreshIII.setTarget(target);
                    //            refreshIII.setPartyMember(PARTY_MEMBER_ONE);
                    //            refreshIII.setPartySize(partySize);
                    //            refreshIII.setFollow(checkBox1.Checked);
                    //            refreshIII.setRefreshTimerReference(partyMemberRefreshTimer);
                    //            actionTable.Add(refreshIII.getAction(), true);
                    //            actionPriorityQueue.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        appendText("Refresh III is already queued");
                    //    }
                    //}

                    if (isRefreshActive() == false)
                    {
                        if (actionTable.Contains("Refresh III Me") == false && actionTable.Contains("Refresh III") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 refreshIII = new Class1("Refresh III Me", refresh3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            refreshIII.setPartySize(partySize);
                            // refreshIII.setFollow(checkBox1.Checked);
                            actionTable.Add(refreshIII.getAction(), true);
                            priorityQueueOne.insert(REFRESH_III_ME_PRIORITY, refreshIII);
                        }
                        else
                        {
                            appendText("Refresh III is queued");
                        }
                    }
                }
            }
            else if (checkBox18.Checked == true)
            {
                if (partySize == 6)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FOUR);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FIVE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_SIX, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_SIX, partySize) == false && checkBox8.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_SIX].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_SIX, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_SIX);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                if (partySize == 5)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FOUR);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FIVE, partySize) == false && checkBox7.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FIVE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FIVE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 4)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_FOUR, partySize) == false && checkBox6.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_FOUR, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_FOUR);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 3)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_THREE, partySize) == false && checkBox5.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_THREE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_THREE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 2)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                    if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_TWO, partySize) == false && checkBox4.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_TWO, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_TWO);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_PARTY_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
                else if (partySize == 1)
                {
                    if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberMPToppedOff(PARTY_MEMBER_ONE, partySize) == false && checkBox3.Checked == true)
                    {
                        if (actionTable.Contains("Refresh") == false)
                        {
                            if (partyMemberRefreshTimer[PARTY_MEMBER_ONE].Count == 0)
                            {
                                Rectangle rectangle = getPartyMemberMPRectangle(PARTY_MEMBER_ONE, partySize);
                                Class1 refresh = new Class1("Refresh", refreshCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                                refresh.setTarget(target);
                                refresh.setPartyMember(PARTY_MEMBER_ONE);
                                refresh.setPartySize(partySize);
                                refresh.setRefreshTimerReference(partyMemberRefreshTimer);
                                actionTable.Add(refresh.getAction(), true);
                                actionPriorityQueue.insert(REFRESH_ME_PRIORITY, refresh);
                            }
                        }
                        else
                        {
                            appendText("Refresh is already queued");
                        }
                    }
                }
            }
        }


        private void maintainPartyHP(int partySize)
        {
            const int PARTY_MEMBER_ONE = 1;
            const int PARTY_MEMBER_TWO = 2;
            const int PARTY_MEMBER_THREE = 3;
            const int PARTY_MEMBER_FOUR = 4;
            const int PARTY_MEMBER_FIVE = 5;
            const int PARTY_MEMBER_SIX = 6;
            const int CURE_IV_PRIORITY = 20;
            const int CURE_III_PRIORITY = 19;
            const int CURE_II_PRIORITY = 18;
            const int CURE_PRIORITY = 17;
            if (partySize == 6)
            {
                if (isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            // cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_SIX, partySize) == true && checkBox14.Checked == true)
                {
                    appendText("Party member 6 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            // cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            // cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            // cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionQueue.Enqueue(cureIV);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            // cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_SIX, partySize) == true && checkBox14.Checked == true)
                {
                    appendText("Party member 6 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            // cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_SIX, partySize) == true && checkBox14.Checked == true)
                {
                    appendText("Party member 6 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
            }
            else if (partySize == 5)
            {
                if (isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            // cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            // cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            // cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionQueue.Enqueue(cureIV);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            // cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            // cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            // cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            // cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            // cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
            }
            else if (partySize == 4)
            {
                if (isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionQueue.Enqueue(cureIV);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
            }
            else if (partySize == 3)
            {
                if (isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
            }
            else if (partySize == 2)
            {
                if (isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
            }
            else if (partySize == 1)
            {
                if (isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
                            //cureIV.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIV.getAction(), true);
                            actionPriorityQueue.insert(CURE_IV_PRIORITY, cureIV);
                        }
                        else
                        {
                            appendText("Cure IV is already queued");
                        }
                    }
                    if (checkBox2.Checked == true)
                    {
                        if (actionTable.Contains("Cure III") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
                            //cureIII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureIII.getAction(), true);
                            actionPriorityQueue.insert(CURE_III_PRIORITY, cureIII);
                        }
                        else
                        {
                            appendText("Cure III is already queued");
                        }
                    }
                    if (checkBox15.Checked == true)
                    {
                        if (actionTable.Contains("Cure II") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
                            //cureII.setFollow(checkBox1.Checked);
                            actionTable.Add(cureII.getAction(), true);
                            actionPriorityQueue.insert(CURE_II_PRIORITY, cureII);
                        }
                        else
                        {
                            appendText("Cure II is already queued");
                        }
                    }
                    if (checkBox17.Checked == true)
                    {
                        if (actionTable.ContainsKey("Cure") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1, textBox2, this);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            //cure.setFollow(checkBox1.Checked);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
            }
        }

        private string getTargetString(int partyMember, int partySize)
        {
            string targetString = "";
            if (partyMember == 1)
            {
                targetString = "<p0>";
            }
            else if (partyMember == 2)
            {
                targetString = "<p1>";
            }
            else if (partyMember == 3)
            {
                targetString = "<p2>";
            }
            else if (partyMember == 4)
            {
                targetString = "<p3>";
            }
            else if (partyMember == 5)
            {
                targetString = "<p4>";
            }
            else if (partyMember == 6)
            {
                targetString = "<p5>";
            }

            return targetString;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            button10.Enabled = true;
            button11.Enabled = false;
            battle = false;

            for (int i = 0; i < partyMemberRefreshTimer.Count; i++)
            {
                while (partyMemberRefreshTimer[i].Count > 0)
                {
                    partyMemberRefreshTimer[i].Dequeue();
                }
            }

            for (int i = 0; i < partyMemberHasteIITimer.Count; i++)
            {
                while (partyMemberHasteIITimer[i].Count > 0)
                {
                    partyMemberHasteIITimer[i].Dequeue();
                }
            }

            for (int i = 0; i < partyMemberPhalanxIITimer.Count; i++)
            {
                while (partyMemberPhalanxIITimer[i].Count > 0)
                {
                    partyMemberPhalanxIITimer[i].Dequeue();
                }
            }

            setUtsusemiVariable(false);

            while (temperTimer.Count > 0)
            {
                temperTimer.Dequeue();
            }

            actionTable.Clear();
            while (actionPriorityQueue.size() > 0)
            {
                actionPriorityQueue.remove();
            }

            while (assistPriorityQueue.size() > 0)
            {
                assistPriorityQueue.remove();
            }

            while (weaponskillPriorityQueue.size() > 0)
            {
                weaponskillPriorityQueue.remove();
            }
        }

        public bool isBarfiraActive()
        {
            string[] barfiraImages = { @".\images\barfira.png", @".\images\barfira-2.png" };

            bool found = false;

            Rectangle rectangle = getStatusRectangle();
            for (int i = 0; i < barfiraImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", barfiraImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("Barfira is active");
                return true;
            }
            else
            {
                appendText2("Barfira is not active");
                return false;
            }
        }

        public bool isPhalanxIIActive()
        {
            string[] phalanxIIImages = { @".\images\phalanxII.png", @".\images\phalanxII-3.png", @".\images\phalanxII-5.png", @".\images\phalanxII-8.png" };

            bool found = false;

            Rectangle rectangle = getStatusRectangle();
            for (int i = 0; i < phalanxIIImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", phalanxIIImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText("Phalanx II is active");
                return true;
            }
            else
            {
                appendText2("Phalanx II is not active");
                return false;
            }
        }

        public bool isTargettingAMonster2(Rectangle rectangle)
        {
            AutoItX3 au3 = new AutoItX3();
            au3.AutoItSetOption("PixelCoordMode", 1);
            //0x64634C
            Object pixel = au3.PixelSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0xDFDFA9);
            if (au3.error == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox31_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox22.Checked == true)
            {
                checkBox32.Checked = false;
            }
        }

        private void checkBox32_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox32.Checked == true)
            {
                checkBox22.Checked = false;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            AutoItX.WinActivate("[CLASS:FFXiClass]");
            int partySize = getPartySize();
            appendText("Party size: " + partySize);
            Rectangle rectangle = getTargetRectangle(partySize);
            if (has2000TP() == true)
            {
                appendText("Success: I have 2000 TP");
            }
            else if (has1000TP() == true)
            {
                appendText("Success: I have 1000 TP");
            }
            else if (has3000TP() == true)
            {
                appendText("Success: I have 3000 TP");
            }
            else if (isTargettingAMonster2(rectangle) == true)
            {
                appendText("Success: I found an unclaimed monster");
            }
            else if (hasReceivedDisengageTell() == true)
            {
                appendText("Success: I have received a tell");
            }
            else
            {
                appendText("Failure");
            }
        }

        public bool isEngaged2(Rectangle rectangle)
        {
            AutoItX3 au3 = new AutoItX3();
            au3.AutoItSetOption("PixelCoordMode", 1);
            Object pixel = au3.PixelSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0xCC6868);
            if (au3.error == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool hasReceivedAddleIITell()
        {
            string[] disengageImages = { @".\images\addle-ii-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Addle II tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedDistractIIITell()
        {
            string[] disengageImages = { @".\images\distract-iii-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Distract III tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedFrazzleIIITell()
        {
            string[] disengageImages = { @".\images\frazzle-iii-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Frazzle III tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedBioIIITell()
        {
            string[] disengageImages = { @".\images\bio-iii-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Bio III tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedSilenceTell()
        {
            string[] disengageImages = { @".\images\silence-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Silence tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedBlindIITell()
        {
            string[] disengageImages = { @".\images\blind-ii-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Blind II tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedParalyzeIITell()
        {
            string[] disengageImages = { @".\images\paralyze-ii-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Paralyze II tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedSlowIITell()
        {
            string[] disengageImages = { @".\images\slow-ii-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Paralyze II tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedBindTell()
        {
            string[] disengageImages = { @".\images\bind-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Bind tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedGravityIITell()
        {
            string[] disengageImages = { @".\images\gravity_ii_tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Gravity II tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedDisengageTell()
        {
            string[] disengageImages = { @".\images\disengage.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Disengage tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedFollowMeTell()
        {
            string[] disengageImages = { @".\images\follow_me_tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Follow me tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedStopRightThereTell()
        {
            string[] disengageImages = { @".\images\stop_right_there_tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < disengageImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", disengageImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Stop right there tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedStopDoingStuffTell()
        {
            string[] stopDoingStuffImages = { @".\images\stop-doing-stuff-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < stopDoingStuffImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", stopDoingStuffImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Stop doing stuff tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedStartDoingStuffTell()
        {
            string[] startDoingStuffImages = { @".\images\start-doing-stuff-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < startDoingStuffImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", startDoingStuffImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Start doing stuff tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public Rectangle getTellRectangle()
        {
            return new Rectangle(115, 573, 400, 590);
        }

        public bool hasReceivedSavageBladeTell()
        {
            string[] weaponskillTellImages = { @".\images\savage_blade_tell.png" };

            bool found = false;

            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Savage blade tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedChantDuCygneTell()
        {
            string[] weaponskillTellImages = { @".\images\chant_du_cygne_tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Chant du Cygne tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedDeathBlossomTell()
        {
            string[] weaponskillTellImages = { @".\images\death_blossom_tell.png" };

            bool found = false;

            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Death Blossom tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedRequiescatTell()
        {
            string[] weaponskillTellImages = { @".\images\requiescat_tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();

            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Requiescat tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedRedLotusBladeTell()
        {
            string[] weaponskillTellImages = { @".\images\red_lotus_blade_tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Red lotus blade tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedKnightsOfRoundTell()
        {
            string[] weaponskillTellImages = { @".\images\knights_of_round_tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Red lotus blade tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedDoNotEngageTell()
        {
            string[] weaponskillTellImages = { @".\images\do-not-engage-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Shut off engage tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedTurnOnEngageTell()
        {
            string[] weaponskillTellImages = { @".\images\engage-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Turn on engage tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedTurnOnDispelTell()
        {
            string[] weaponskillTellImages = { @".\images\turn-on-dispel-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Turn on dispel tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hasReceivedTurnOffDispelTell()
        {
            string[] weaponskillTellImages = { @".\images\turn-off-dispel-tell.png" };

            bool found = false;
            Rectangle rectangle = getTellRectangle();
            for (int i = 0; i < weaponskillTellImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", weaponskillTellImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("Shut off dispel tell received");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isTargettingPlumkick(int partySize)
        {
            string[] plumkickImages = { @".\images\plumkick-targetted-2.png" };

            bool found = false;
            Rectangle rectangle = getTargetRectangle(partySize);

            for (int i = 0; i < plumkickImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", plumkickImages[i]);
                if (results == null)
                {

                }
                else
                {
                    found = true;
                    break;
                }
            }

            if (found == true)
            {
                appendText2("I am targeting myself");
                return true;
            }
            else
            {
                return false;
            }

        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}