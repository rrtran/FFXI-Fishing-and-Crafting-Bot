using AutoIt;
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
                    else if (isFishingGaugeOnScreen() == true)
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

        bool isFishingGaugeOnScreen()
        {
            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int width = windowDimensionInfo.Width;
            int height = windowDimensionInfo.Height;
            string[] fishGaugeResults = UseImageSearch((int)(width / 3), 0, width - (int)(width / 3), (int)(height / 2), "10", @".\images\fish-gauge-pixel.png");
            if (fishGaugeResults == null)
            {
                appendText("Did not see fishing gauge");
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
                    x = 1310;
                    y = 595;
                    width = 1340;
                    height = 610;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 5)
                {
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 6)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 5)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
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
                    x = 1310;
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

            string[] redHPImages = { @".\images\player-red-hp-bar-pixel.png", @".\images\player-red-hp-bar-pixel2.png", @".\images\player-red-hp-bar-pixel3.png" };

            bool found = false;

            for (int i = 0; i < redHPImages.Length; i++) {
                string[] results = UseImageSearch(x, y, width, height, "10", redHPImages[i]);
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
                    x = 1310;
                    y = 595;
                    width = 1340;
                    height = 610;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 5)
                {
                    x = 1310;
                    y = 67;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 6)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 5)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 665;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 672;
                    width = 1340;
                    height = 687;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
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
                    x = 1310;
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

            string[] results = UseImageSearch(x, y, width, height, "13", @".\images\player-orange-hp-bar-pixel.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                return true;
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
                    x = 1310;
                    y = 595;
                    width = 1340;
                    height = 610;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 5)
                {
                    x = 1310;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 6)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 615;
                    width = 1340;
                    height = 630;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 5)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 635;
                    width = 1340;
                    height = 650;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 670;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 4)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 655;
                    width = 1340;
                    height = 665;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 3)
                {
                    x = 1310;
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
                    x = 1310;
                    y = 670;
                    width = 1340;
                    height = 685;
                }
                else if (partyMember == 2)
                {
                    x = 1310;
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
                    x = 1310;
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

            string[] yellowHpImages = { @".\images\player-yellow-hp-bar-pixel.png", @"/\images\player-yellow-hp-bar-pixel-2.png" };

            bool found = false;
            for (int i = 0; i < yellowHpImages.Length; i++)
            {
                string[] results = UseImageSearch(x, y, width, height, "7", yellowHpImages[i]);
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
                return true;
            }
            
        }

        bool isPartyMemberDead(int partyMember, int partySize)
        {
            Rectangle rectangle = getPartyMemberDeadRectangle(partyMember, partySize);

            string[] partyMemberDeadImages = { @".\images\player_dead_2_not_targetted.png", @".\images\player_dead_2_targetted.png", @".\images\player_dead_2_not_targetted_2.png", @".\images\player_dead_2_targetted_2.png", @".\images\player_dead_3_not_targetted.png", @".\images\player_dead_3_targetted.png", @".\images\player_dead_3_not_targetted_2.png", @".\images\player_dead_3_targetted_2.png", @".\images\player_dead_4_not_targetted.png", @".\images\player_dead_4_targetted.png", @".\images\player_dead_5_not_targetted.png", @".\images\player_dead_5_targetted.png", @".\images\player_dead_6_not_targetted.png", @".\images\player_dead_6_targetted.png" };

            bool found = false;
            for (int i = 0; i < partyMemberDeadImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "10", partyMemberDeadImages[i]);
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
                appendText("Player " + partyMember + " is dead");
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

            string[] results = UseImageSearch(left, top, right, bottom, "30", @".\images\player-green-mp-bar-pixel.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool isMPLow(int partyMember, int partySize)
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
                    top = 605;
                    right = 1350;
                    bottom = 625;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 630;
                    right = 1350;
                    bottom = 640;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 4)
                {
                    left = 1300;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 5)
                {
                    left = 1300;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 6)
                {
                    left = 1300;
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
                    left = 1300;
                    top = 630;
                    right = 1350;
                    bottom = 640;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 4)
                {
                    left = 1300;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 5)
                {
                    left = 1300;
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
                    left = 1300;
                    top = 650;
                    right = 1350;
                    bottom = 660;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 4)
                {
                    left = 1300;
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
                    left = 1300;
                    top = 668;
                    right = 1350;
                    bottom = 678;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 3)
                {
                    left = 1300;
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
                    left = 1300;
                    top = 685;
                    right = 1350;
                    bottom = 695;
                }
                else if (partyMember == 2)
                {
                    left = 1300;
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
                    left = 1300;
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

            string[] mpPixelImages = { @".\images\player-green-mp-bar-pixel.png" };

            string[] results = UseImageSearch(left, top, right, bottom, "30", mpPixelImages[0]);
            if (results == null)
            {
                return true;
            }
            else
            {
                return false;
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

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int status_x = 0;
            int status_y = 0;
            int status_width = (int)(windowDimensionInfo.Width * 0.30);
            int status_height = (int)(windowDimensionInfo.Height * 0.30);
            string[] results = UseImageSearch(status_x, status_y, status_width, status_height, "30", @".\images\composure.png");
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
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "10", hasteSambaImages[i]);
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
        private int cure3CooldownTime = 5000;
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
        private int cure4CooldownTime = 6000;
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
        private int stoneskinCooldownTime = 33000;
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
            appendText("Window width: " + windowRectangle.Width);
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
                rectangle.X = 1250;
                rectangle.Y = 550;
                rectangle.Width = 1340;
                rectangle.Height = 575;
            }
            else if (partySize == 5)
            {
                rectangle.X = 1250;
                rectangle.Y = 580;
                rectangle.Width = 1340;
                rectangle.Height = 590;
            }
            else if (partySize == 4)
            {
                rectangle.X = 1250;
                rectangle.Y = 590;
                rectangle.Width = 1340;
                rectangle.Height = 610;
            }
            else if (partySize == 3)
            {
                rectangle.X = 1250;
                rectangle.Y = 610;
                rectangle.Width = 1340;
                rectangle.Height = 630;
            }
            else if (partySize == 2)
            {
                rectangle.X = 1250;
                rectangle.Y = 630;
                rectangle.Width = 1340;
                rectangle.Height = 650;
            }
            else if (partySize == 1)
            {
            }

            return rectangle;
        }

        bool isEngaged(int partySize)
        {
            Rectangle rectangle = getTargetRectangle(partySize);

            string[] assistImages = { @".\images\engaged_monster_name_pixel.png", @".\images\target_monster_name_pixel.png", @".\images\target_monster_name_pixel_2.png" };

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
                appendText("Weapon is not out");
            }
            else
            {
                appendText("Found weapon is drawn");
            }
            return weaponDrawn;
        }

        bool hasTP()
        {
            string[] tpImages = { @".\images\tp_text_1000.png", @".\images\tp_text_2000.png", @".\images\tp_text_3000.png" };

            bool found = false;
            for (int i = 0; i < tpImages.Length; i++)
            {
                string[] results = UseImageSearch(110, 90, 120, 110, "5", tpImages[i]);
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
                appendText("I have TP");
                return true;
            }
            else
            {
                return false;
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
                    rectangle.X = 1320;
                    rectangle.Y = 590;
                    rectangle.Width = 1340;
                    rectangle.Height = 605;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 610;
                    rectangle.Width = 1340;
                    rectangle.Height = 625;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 630;
                    rectangle.Width = 1340;
                    rectangle.Height = 645;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 650;
                    rectangle.Width = 1340;
                    rectangle.Height = 665;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 6)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 687;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 610;
                    rectangle.Width = 1340;
                    rectangle.Height = 625;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 630;
                    rectangle.Width = 1340;
                    rectangle.Height = 645;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 650;
                    rectangle.Width = 1340;
                    rectangle.Height = 665;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 687;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 630;
                    rectangle.Width = 1340;
                    rectangle.Height = 645;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 650;
                    rectangle.Width = 1340;
                    rectangle.Height = 665;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 687;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 650;
                    rectangle.Width = 1340;
                    rectangle.Height = 665;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 687;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 687;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 687;
                    rectangle.Width = 1340;
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
                    rectangle.X = 1310;
                    rectangle.Y = 595;
                    rectangle.Width = 1340;
                    rectangle.Height = 610;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 615;
                    rectangle.Width = 1340;
                    rectangle.Height = 630;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 635;
                    rectangle.Width = 1340;
                    rectangle.Height = 650;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 655;
                    rectangle.Width = 1340;
                    rectangle.Height = 670;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 6)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 690;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 5)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 615;
                    rectangle.Width = 1340;
                    rectangle.Height = 630;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 635;
                    rectangle.Width = 1340;
                    rectangle.Height = 650;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 655;
                    rectangle.Width = 1340;
                    rectangle.Height = 670;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 5)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 690;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 4)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 635;
                    rectangle.Width = 1340;
                    rectangle.Height = 650;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 655;
                    rectangle.Width = 1340;
                    rectangle.Height = 670;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 4)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 690;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 3)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 655;
                    rectangle.Width = 1340;
                    rectangle.Height = 665;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 3)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 690;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 2)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 670;
                    rectangle.Width = 1340;
                    rectangle.Height = 685;
                }
                else if (partyMember == 2)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 690;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1310;
                    rectangle.Y = 690;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }

            return rectangle;
        }

        private Hashtable actionTable = new Hashtable();
        private Queue<Class1> actionQueue = new Queue<Class1>();
        private PriorityQueue actionPriorityQueue = new PriorityQueue();
        private bool battle = false;
        private Queue<bool> weaponDrawnQueue = new Queue<bool>();
        private Queue<bool> temperTimer = new Queue<bool>();
        private Queue<bool> enwaterIITimer = new Queue<bool>();
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
            }

            new Thread(() =>
            {
                while (battle == true)
                {
                    if (actionPriorityQueue.size() > 0)
                    {
                        Class1 action = actionPriorityQueue.getData();
                        actionPriorityQueue.remove();
                        action.function1();
                        new Thread(() =>
                        {
                            Thread.Sleep(action.getCooldownTime());
                            actionTable.Remove(action.getAction());
                        }).Start();
                    }
                }
            }).Start();

            const int PARTY_MEMBER_ONE = 1;
            const int PARTY_MEMBER_TWO = 2;
            const int PARTY_MEMBER_THREE = 3;
            const int PARTY_MEMBER_FOUR = 4;
            const int PARTY_MEMBER_FIVE = 5;
            const int PARTY_MEMBER_SIX = 6;
            const int COMPOSURE_PRIORITY = 25;
            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    appendText("Party Size: " + partySize);
                    if (isComposureActive() == false && checkBox21.Checked == true)
                    {
                        if (actionTable.Contains("Composure") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 composure = new Class1("Composure", composureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            composure.setPartySize(partySize);
                            actionTable.Add(composure.getAction(), true);
                            actionPriorityQueue.insert(COMPOSURE_PRIORITY, composure);
                        }
                        else
                        {
                            appendText("Composure is already queued");
                        }
                    }
                    if (isUtsusemiActive() == false && checkBox20.Checked == true)
                    {
                        const int UTSUSEMI_NI_PRIORITY = 15;
                        const int UTSUSEMI_ICHI_PRIORITY = 14;

                        if (isComposureActive() == false)
                        {
                            if (isHasteActive() == false)
                            {
                                utsusemiIchiCooldownTime = 20000;
                                utsusemiNiCooldownTime = 30000;
                            }
                            else
                            {
                                if (checkBox22.Checked == true)
                                {
                                    utsusemiIchiCooldownTime = 16000;
                                    utsusemiNiCooldownTime = 24000;
                                }
                                else if (checkBox32.Checked == true)
                                {
                                    utsusemiIchiCooldownTime = 12000;
                                    utsusemiNiCooldownTime = 18000;
                                }
                            }
                        }
                        else
                        {
                            if (isHasteActive() == false)
                            {
                                utsusemiIchiCooldownTime = 23000;
                                utsusemiNiCooldownTime = 34000;
                            }
                            else
                            {
                                if (checkBox22.Checked == true)
                                {
                                    utsusemiIchiCooldownTime = 18000;
                                    utsusemiNiCooldownTime = 27000;
                                }
                                else if (checkBox32.Checked == true)
                                {
                                    utsusemiIchiCooldownTime = 13000;
                                    utsusemiNiCooldownTime = 20000;
                                }
                            }
                        }

                        if (actionTable.Contains("Utsusemi: Ni") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 utsusemiNi = new Class1("Utsusemi: Ni", utsusemiNiCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            utsusemiNi.setPartySize(partySize);
                            actionTable.Add(utsusemiNi.getAction(), true);
                            actionPriorityQueue.insert(UTSUSEMI_NI_PRIORITY, utsusemiNi);
                        }
                        else
                        {
                            appendText("Utsusemi: Ni is queued");
                        }
                        if (actionTable.Contains("Utsusemi: Ichi") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 utsusemiIchi = new Class1("Utsusemi: Ichi", utsusemiIchiCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            utsusemiIchi.setPartySize(partySize);
                            actionTable.Add(utsusemiIchi.getAction(), true);
                            actionPriorityQueue.insert(UTSUSEMI_ICHI_PRIORITY, utsusemiIchi);
                        }
                        else
                        {
                            appendText("Utsusemi: Ichi is queued");
                        }
                    }
                    if (amIStoneskined() == false && checkBox16.Checked == true)
                    {
                        const int STONESKIN_PRIORITY = 13;
                        if (actionTable.Contains("Stoneskin") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 stoneskin = new Class1("Stoneskin", stoneskinCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            stoneskin.setPartySize(partySize);
                            actionTable.Add(stoneskin.getAction(), true);
                            actionPriorityQueue.insert(STONESKIN_PRIORITY, stoneskin);
                        }
                        else
                        {
                            appendText("Stoneskin is already queued");
                        }
                    }

                    if (checkBox1.Checked == true)
                    {
                        const int FOLLOW_PARTY_MEMBER_TWO_PRIORITY = 90;
                        const int FOLLOW_DEAD_PARTY_MEMBER_TWO_PRIORITY = 100;
                        if (actionTable.Contains("Follow Party Member 2") == false)
                        {
                            if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == true)
                            {
                                Rectangle rectangle = new Rectangle();
                                Class1 followPartyMember = new Class1("Follow Party Member 2", 1000, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                followPartyMember.setPartySize(partySize);
                                actionTable.Add(followPartyMember.getAction(), true);
                                actionPriorityQueue.insert(FOLLOW_DEAD_PARTY_MEMBER_TWO_PRIORITY, followPartyMember);
                            }
                            else if (followPartyMember2Queue.Count == 0)
                            {
                                Rectangle rectangle = new Rectangle();
                                Class1 followPartyMember = new Class1("Follow Party Member 2", 1000, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
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
                    
                    maintainPartyMP(partySize);
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    if (checkBox23.Checked == true)
                    {
                        if (isWeaponDrawn() == true)
                        {
                            weaponDrawnQueue.Enqueue(true);
                            new Thread(() =>
                            {
                                Thread.Sleep(6000);
                                weaponDrawnQueue.Dequeue();
                            }).Start();

                            if (hasTP() == true)
                            {
                                appendText("I have TP");
                                if (actionTable.Contains("Weaponskill") == false)
                                {
                                    const int WEAPONSKILL_PRIORITY = 99;
                                    Rectangle rectangle = new Rectangle();
                                    Class1 weaponskill = new Class1("Weaponskill", 3000, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                    weaponskill.setPartySize(partySize);
                                    weaponskill.setWeaponskill(this.weaponskill);
                                    actionTable.Add(weaponskill.getAction(), true);
                                    actionPriorityQueue.insert(WEAPONSKILL_PRIORITY, weaponskill);
                                }
                            }
                            else if (enwaterIITimer.Count == 0 && checkBox24.Checked == true)
                            {
                                const int ENWATER_II_PRIORITY = 13;
                                if (actionTable.Contains("Enwater II") == false)
                                {
                                    Rectangle rectangle = getStatusRectangle();
                                    Class1 enwaterII = new Class1("Enwater II", enwater2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
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
                            else if (temperTimer.Count == 0 && checkBox33.Checked == true)
                            {
                                const int TEMPER_PRIORITY = 13;
                                if (actionTable.Contains("Temper") == false)
                                {
                                    Rectangle rectangle = getStatusRectangle();
                                    Class1 temper = new Class1("Temper", temperCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
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
                            else if (isEnwaterActive() == false && checkBox24.Checked == true)
                            {
                                while (temperTimer.Count > 0) {
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
                        else if (isEngaged(partySize) == true && weaponDrawnQueue.Count == 0)
                        {
                            if (actionTable.Contains("Check Engaged") == false)
                            {
                                const int ENGAGED_PRIORITY = 17;
                                Rectangle rectangle = new Rectangle();
                                Class1 engaged = new Class1("Check Engaged", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1);
                                engaged.setPartySize(partySize);
                                actionTable.Add(engaged.getAction(), true);
                                actionPriorityQueue.insert(ENGAGED_PRIORITY, engaged);
                            }
                        }
                        else if (actionTable.Contains("Assist") == false && weaponDrawnQueue.Count == 0)
                        {
                            if (isPartyMemberDead(2, partySize) == false)
                            {
                                const int ASSIST_PRIORITY = 16;
                                Rectangle rectangle = new Rectangle();
                                Class1 assist = new Class1("Assist", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1);
                                assist.setPartySize(partySize);
                                actionTable.Add(assist.getAction(), true);
                                actionPriorityQueue.insert(ASSIST_PRIORITY, assist);
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
                        const int HASTE_II_PRIORITY = 16;
                        if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                        {
                            Rectangle rectangle = getStatusRectangle();
                            Class1 haste2 = new Class1("Haste II", hasteCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            haste2.setPartySize(partySize);
                            actionTable.Add(haste2.getAction(), true);
                            actionPriorityQueue.insert(HASTE_II_PRIORITY, haste2);
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
                            const int HASTE_II_PARTY_PRIORITY = 16;
                            if (partyMemberHasteIITimer[PARTY_MEMBER_TWO].Count == 0)
                            {
                                if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                {
                                    Rectangle rectangle = new Rectangle();
                                    Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                    hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                    string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                                    hasteIIPartyMember.setPartyMember(PARTY_MEMBER_TWO);
                                    hasteIIPartyMember.setTarget(target);
                                    hasteIIPartyMember.setPartySize(partySize);
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
                            const int HASTE_II_PARTY_PRIORITY = 16;
                            if (partyMemberHasteIITimer[PARTY_MEMBER_THREE].Count == 0)
                            {
                                if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                {
                                    Rectangle rectangle = new Rectangle();
                                    Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                    hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                    string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                                    hasteIIPartyMember.setPartyMember(PARTY_MEMBER_THREE);
                                    hasteIIPartyMember.setTarget(target);
                                    hasteIIPartyMember.setPartySize(partySize);
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
                            const int HASTE_II_PARTY_PRIORITY = 16;
                            if (partyMemberHasteIITimer[PARTY_MEMBER_FOUR].Count == 0)
                            {
                                if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                {
                                    Rectangle rectangle = new Rectangle();
                                    Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                    hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                    string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                                    hasteIIPartyMember.setPartyMember(PARTY_MEMBER_FOUR);
                                    hasteIIPartyMember.setTarget(target);
                                    hasteIIPartyMember.setPartySize(partySize);
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
                            const int HASTE_II_PARTY_PRIORITY = 16;
                            if (partyMemberHasteIITimer[PARTY_MEMBER_FIVE].Count == 0)
                            {
                                if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                {
                                    Rectangle rectangle = new Rectangle();
                                    Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                    hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                    string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                                    hasteIIPartyMember.setPartyMember(PARTY_MEMBER_FIVE);
                                    hasteIIPartyMember.setTarget(target);
                                    hasteIIPartyMember.setPartySize(partySize);
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
                            const int HASTE_II_PARTY_PRIORITY = 16;
                            if (partyMemberHasteIITimer[PARTY_MEMBER_SIX].Count == 0)
                            {
                                if (actionTable.Contains("Haste II") == false && actionTable.Contains("Haste II Party Member") == false)
                                {
                                    Rectangle rectangle = new Rectangle();
                                    Class1 hasteIIPartyMember = new Class1("Haste II Party Member", hasteIICooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                    hasteIIPartyMember.setHasteIITimerReference(partyMemberHasteIITimer);
                                    string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                                    hasteIIPartyMember.setPartyMember(PARTY_MEMBER_SIX);
                                    hasteIIPartyMember.setTarget(target);
                                    hasteIIPartyMember.setPartySize(partySize);
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
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    maintainPartyHP(partySize);
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    raiseParty(partySize);
                }
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
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
                                Class1 echoDrops = new Class1("Echo Drops", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1);
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
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
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
                                Class1 remedy = new Class1("Remedy", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1);
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
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
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
                                Class1 remedy = new Class1("Remedy", 0, rectangle, followQueue, followPartyMember2Queue, engagedQueue, control, textBox1);
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
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
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
            }).Start();

            new Thread(() =>
            {
                while (battle == true)
                {
                    int partySize = getPartySize();
                    if (checkBox44.Checked == true)
                    {
                        if (isMPLow(PARTY_MEMBER_ONE, partySize) == true)
                        {
                            int CONVERT_PRIORITY = 98;
                            if (actionTable.Contains("Convert") == false)
                            {
                                Rectangle rectangle = new Rectangle();
                                Class1 convert = new Class1("Convert", 500000, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                                convert.setPartySize(partySize);
                                actionTable.Add(convert.getAction(), true);
                                actionPriorityQueue.insert(CONVERT_PRIORITY, convert);
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
            }).Start();
        }

        private List<Queue<bool>> partyMemberHasteIITimer = new List<Queue<bool>> { new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>(), new Queue<bool>() };

        private void raiseParty(int partySize)
        {
            const int RAISE_COOLDOWN = 37000;
            const int PARTY_MEMBER_ONE = 1;
            const int PARTY_MEMBER_TWO = 2;
            const int PARTY_MEMBER_THREE = 3;
            const int PARTY_MEMBER_FOUR = 4;
            const int PARTY_MEMBER_FIVE = 5;
            const int PARTY_MEMBER_SIX = 6;
            const int RAISE_PRIORITY = 15;

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
                        appendText("Raise is already queued");
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
            const int REFRESH_ME_PRIORITY = 22;
            const int REFRESH_PARTY_PRIORITY = 16;
            const int REFRESH_II_ME_PRIORITY = 22;
            const int REFRESH_II_PARTY_PRIORITY = 16;
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
                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_SIX, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_SIX, partySize) == true && checkBox14.Checked == true)
                {
                    appendText("Party member 6 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_SIX, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_SIX, partySize) == true && checkBox14.Checked == true)
                {
                    appendText("Party member 6 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_SIX, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_SIX, partySize) == true && checkBox14.Checked == true)
                {
                    appendText("Party member 6 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_SIX, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_SIX, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
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
                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FIVE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_FIVE, partySize) == true && checkBox13.Checked == true)
                {
                    appendText("Party member 5 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FIVE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FIVE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
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
                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_FOUR, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_FOUR, partySize) == true && checkBox12.Checked == true)
                {
                    appendText("Party member 4 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_FOUR, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_FOUR, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
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
                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_THREE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_THREE, partySize) == true && checkBox11.Checked == true)
                {
                    appendText("Party member 3 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_THREE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_THREE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
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
                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add("Cure", true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }
                else if (isPartyMemberDead(PARTY_MEMBER_TWO, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_TWO, partySize) == true && checkBox10.Checked == true)
                {
                    appendText("Party member 2 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_TWO, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_TWO, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
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
                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberRedHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has red hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cure2CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberOrangeHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has orange hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
                            actionTable.Add(cure.getAction(), true);
                            actionPriorityQueue.insert(CURE_PRIORITY, cure);
                        }
                        else
                        {
                            appendText("Cure is already queued");
                        }
                    }
                }

                if (isPartyMemberDead(PARTY_MEMBER_ONE, partySize) == false && isPartyMemberYellowHP(PARTY_MEMBER_ONE, partySize) == true && checkBox9.Checked == true)
                {
                    appendText("Party member 1 has yellow hp");
                    if (checkBox19.Checked == true)
                    {
                        if (actionTable.Contains("Cure IV") == false)
                        {
                            Rectangle rectangle = getPartyMemberHPRectangle(PARTY_MEMBER_ONE, partySize);
                            Class1 cureIV = new Class1("Cure IV", cure4CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIV.setTarget(target);
                            cureIV.setPartySize(partySize);
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
                            Class1 cureIII = new Class1("Cure III", cure3CooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureIII.setTarget(target);
                            cureIII.setPartySize(partySize);
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
                            Class1 cureII = new Class1("Cure II", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cureII.setTarget(target);
                            cureII.setPartySize(partySize);
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
                            Class1 cure = new Class1("Cure", cureCooldownTime, rectangle, followQueue, followPartyMember2Queue, control, textBox1);
                            string target = getTargetString(PARTY_MEMBER_ONE, partySize);
                            cure.setTarget(target);
                            cure.setPartySize(partySize);
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

            actionTable.Clear();
            while (actionPriorityQueue.size() > 0)
            {
                actionPriorityQueue.remove();
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
    }
}