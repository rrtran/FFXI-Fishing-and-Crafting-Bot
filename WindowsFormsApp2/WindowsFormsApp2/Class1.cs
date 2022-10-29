using AutoIt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    class Class1
    {
        private string str = "";
        private int cooldownTime = 0;
        private string target = "";
        int partyMember;
        int partySize;
        Control control;
        TextBox textBox1;
        TextBox textBox2;
        Rectangle rectangle;
        private List<Queue<bool>> partyMemberRefreshTimer;
        private List<Queue<bool>> partyMemberHasteIITimer;
        private Queue<bool> temperTimer;
        private Queue<bool> enwaterIITimer;
        private Queue<bool> followQueue;
        private Queue<bool> followPartyMember2Queue;
        private Queue<bool> engagedQueue;
        private string weaponskill = "";
        private bool shouldFollow = false;

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

        public Class1(string str, int cooldownTime, Rectangle rectangle, Control control, TextBox textBox)
        {
            this.str = str;
            this.cooldownTime = cooldownTime;
            this.rectangle = rectangle;
            this.control = control;
            this.textBox1 = textBox;
            partyMemberRefreshTimer = null;
        }

        public Class1(string str, int cooldownTime, Rectangle rectangle, Queue<bool> followQueue, Queue<bool> followPartyMember2Queue, Control control, TextBox textBox)
        {
            this.str = str;
            this.cooldownTime = cooldownTime;
            this.rectangle = rectangle;
            this.control = control;
            this.textBox1 = textBox;
            partyMemberRefreshTimer = null;
            this.followQueue = followQueue;
            this.followPartyMember2Queue = followPartyMember2Queue;
        }

        public Class1(string str, int cooldownTime, Rectangle rectangle, Queue<bool> followQueue, Queue<bool> followPartyMember2Queue, Control control, TextBox textBox, TextBox textBox2)
        {
            this.str = str;
            this.cooldownTime = cooldownTime;
            this.rectangle = rectangle;
            this.control = control;
            this.textBox1 = textBox;
            this.textBox2 = textBox2;
            partyMemberRefreshTimer = null;
            this.followQueue = followQueue;
            this.followPartyMember2Queue = followPartyMember2Queue;
        }

        public Class1(string str, int cooldownTime, Rectangle rectangle, Queue<bool> followQueue, Queue<bool> followPartyMember2Queue, Queue<bool> engagedQueue, Control control, TextBox textBox)
        {
            this.str = str;
            this.cooldownTime = cooldownTime;
            this.rectangle = rectangle;
            this.control = control;
            this.textBox1 = textBox;
            partyMemberRefreshTimer = null;
            this.followQueue = followQueue;
            this.followPartyMember2Queue = followPartyMember2Queue;
            this.engagedQueue = engagedQueue;
        }

        public Class1(string str, int cooldownTime, Rectangle rectangle, Queue<bool> followQueue, Queue<bool> followPartyMember2Queue, Queue<bool> engagedQueue, Control control, TextBox textBox, TextBox textBox2)
        {
            this.str = str;
            this.cooldownTime = cooldownTime;
            this.rectangle = rectangle;
            this.control = control;
            this.textBox1 = textBox;
            this.textBox2 = textBox2;
            partyMemberRefreshTimer = null;
            this.followQueue = followQueue;
            this.followPartyMember2Queue = followPartyMember2Queue;
            this.engagedQueue = engagedQueue;
        }

        public void setTarget(string target)
        {
            this.target = target;
        }

        public void setRefreshTimerReference(List<Queue<bool>> list)
        {
            partyMemberRefreshTimer = list;
        }

        public void setHasteIITimerReference(List<Queue<bool>> list)
        {
            partyMemberHasteIITimer = list;
        }

        public void setTemperTimerReference(Queue<bool> queue)
        {
            temperTimer = queue;
        }

        public void setEnwaterIITimerReference(Queue<bool> queue)
        {
            enwaterIITimer = queue;   
        }

        public void setFollowQueue(Queue<bool> queue)
        {
            this.followQueue = queue;
        }

        public void setFollowPartyMember2Queue(Queue<bool> queue)
        {
            this.followPartyMember2Queue = queue;
        }

        public void setWeaponskill(string weaponskill)
        {
            this.weaponskill = weaponskill;
        }

        public void setFollow(bool shouldFollow)
        {
            this.shouldFollow = shouldFollow;
        }

        public string getAction()
        {
            return str;
        }

        public int getCooldownTime()
        {
            return cooldownTime;
        }

        public void setPartyMember(int partyMember)
        {
            this.partyMember = partyMember;
        }

        public void setPartySize(int partySize)
        {
            this.partySize = partySize;
        }

        public void function1()
        {
            if (str == "Start Skillchain")
            {
                bool shouldUseWeaponSkill = false;

                if (hasTP() == true && isSkillchainActive() == false)
                {
                    shouldUseWeaponSkill = true;
                }

                if (shouldUseWeaponSkill == true)
                {
                    useWeaponSkill();

                    if (shouldFollow == true)
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
            else if (str == "Close Skillchain")
            {
                bool shouldUseWeaponSkill = false;

                if (hasTP() == true && isSkillchainActive() == true)
                {
                    shouldUseWeaponSkill = true;
                }

                if (shouldUseWeaponSkill == true)
                {
                    useWeaponSkill();

                    if (shouldFollow == true) {
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
            else if (str == "Composure")
            {
                const int DELAY = 100;
                const int DELAY_AFTER_USE = 1000;
                
                appendText("Using composure");
                AutoItX.Send("/ja Composure <me>");
                Thread.Sleep(DELAY);
                AutoItX.Send("{Enter}");
                Thread.Sleep(DELAY_AFTER_USE);

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
            else if (str == "Convert")
            {
                const int DELAY = 100;
                const int DELAY_AFTER_USE = 1000;

                appendText("Using convert");
                AutoItX.Send("/ja Convert <me>");
                Thread.Sleep(DELAY);
                AutoItX.Send("{Enter}");
                Thread.Sleep(DELAY_AFTER_USE);

                if (followQueue.Count > 0)
                {
                    followQueue.Dequeue();
                    followTarget(2, partySize);
                }
                else if (followPartyMember2Queue.Count == 0)
                {
                    followTarget(2, partySize);
                }

                //bool shouldUse = false;

                //if (isMPLow(1, partySize) == true
                //        && isPartyMemberOrangeHP(rectangle) == false
                //        && isPartyMemberRedHP(rectangle) == false)
                //{
                //    shouldUse = true;
                //}
                //else
                //{
                //    shouldUse = false;
                //}

                //if (shouldUse == true)
                //{
                //    stopFollow();
                //    Thread.Sleep(DELAY);
                //    appendText("Using convert");
                //    AutoItX.Send("/ja Convert <me>");
                //    Thread.Sleep(DELAY);
                //    AutoItX.Send("{Enter}");
                //    Thread.Sleep(DELAY_AFTER_USE);

                //    if (followQueue.Count > 0)
                //    {
                //        followQueue.Dequeue();
                //        followTarget(2, partySize);
                //    }
                //    else if (followPartyMember2Queue.Count == 0)
                //    {
                //        followTarget(2, partySize);
                //    }
                //}
            }
            else if (str == "Cure")
            {
                const int DELAY = 100;
                const int CURE_CAST_TIME = 2500;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;
                if (isPartyMemberRedHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberOrangeHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberYellowHP(rectangle) == true)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
                    appendText("Casting cure on " + target);
                    AutoItX.Send("/ma Cure " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize, CURE_CAST_TIME);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize, CURE_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Cure II")
            {
                const int DELAY = 100;
                const int CURE_II_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;

                bool shouldCast = false;
                if (isPartyMemberRedHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberOrangeHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberYellowHP(rectangle) == true)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
                    appendText("Casting cure II on " + target);
                    AutoItX.Send("/ma \"Cure II\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize, CURE_II_CAST_TIME);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize, CURE_II_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Cure III")
            {
                const int DELAY = 100;
                const int CURE_III_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;

                bool shouldCast = false;
                if (isHPToppedOff(rectangle) == false)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberRedHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberOrangeHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberYellowHP(rectangle) == true)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
                    appendText("Casting cure III on " + target);
                    AutoItX.Send("/ma \"Cure III\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, CURE_III_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, CURE_III_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(CURE_III_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
                else
                {
                    appendText2("NOT GOING TO CURE III");
                }
            }
            else if (str == "Cure IV")
            {
                const int DELAY = 100;
                const int CURE_IV_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;

                bool shouldCast = false;
                if (isHPToppedOff(rectangle) == false)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberRedHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberOrangeHP(rectangle) == true)
                {
                    shouldCast = true;
                }
                else if (isPartyMemberYellowHP(rectangle) == true)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
                    appendText("Casting cure IV on " + target);
                    AutoItX.Send("/ma \"Cure IV\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, CURE_IV_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, CURE_IV_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(CURE_IV_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
                else
                {
                    appendText2("***** NOT GOING TO CURE IV ******");
                }
            }
            else if (str == "Refresh")
            {
                const int DELAY = 100;
                const int REFRESH_CAST_TIME = 5000;
                const int DELAY_AFTER_CAST = 1000;

                bool shouldCast = false;
                if (isPartyMemberMPToppedOff(rectangle) == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
                    appendText("Casting Refresh on " + target);
                    AutoItX.Send("/ma \"Refresh\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(REFRESH_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Refresh II")
            {
                const int DELAY = 100;
                const int REFRESH_II_CAST_TIME = 5000;
                const int DELAY_AFTER_CAST = 1000;

                bool shouldCast = false;
                if (isPartyMemberMPToppedOff(rectangle) == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
                    appendText("Casting Refresh II on " + target);
                    AutoItX.Send("/ma \"Refresh II\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

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

                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize, REFRESH_II_CAST_TIME);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize, REFRESH_II_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Refresh III")
            {
                const int DELAY = 100;
                const int REFRESH_III_CAST_TIME = 5000;
                const int DELAY_AFTER_CAST = 1000;

                bool shouldCast = false;
                if (isPartyMemberMPToppedOff(rectangle) == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
                    appendText("Casting Refresh III on " + target);
                    AutoItX.Send("/ma \"Refresh III\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

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

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, REFRESH_III_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, REFRESH_III_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(REFRESH_III_CAST_TIME);
                    }
                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Stoneskin")
            {
                const int DELAY = 100;
                const int STONESKIN_CAST_TIME = 7000;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;

                if (isStoneskinActive() == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting stoneskin");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ma Stoneskin <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, STONESKIN_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, STONESKIN_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(STONESKIN_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Utsusemi: Ichi")
            {
                const int DELAY = 100;
                const int UTSUSEMI_ICHI_CAST_TIME = 4000;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;

                if (isUtsusemiActive() == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting utsusemi: ichi");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/nin \"Utsusemi: Ichi\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, UTSUSEMI_ICHI_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, UTSUSEMI_ICHI_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(UTSUSEMI_ICHI_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Utsusemi: Ni")
            {
                const int DELAY = 100;
                const int UTSUSEMI_NI_CAST_TIME = 1500;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;

                if (isUtsusemiActive() == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting utsusemi: ni");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/nin \"Utsusemi: Ni\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, UTSUSEMI_NI_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, UTSUSEMI_NI_CAST_TIME);
                        }
                    }
                    else 
                    { 
                        Thread.Sleep(UTSUSEMI_NI_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Haste")
            {
                const int DELAY = 100;
                const int HASTE_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 2000;
                bool shouldCast = false;

                if (isHasteActive() == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting haste");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ma Haste <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(HASTE_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Haste II")
            {
                const int DELAY = 100;
                const int HASTE_II_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;

                if (isHasteActive() == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting haste II");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ma \"Haste II\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, HASTE_II_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, HASTE_II_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(HASTE_II_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Haste II Party Member")
            {
                const int DELAY = 100;
                const int HASTE_II_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;

                appendText("Casting Haste II on " + target);
                stopFollow();
                Thread.Sleep(DELAY);
                AutoItX.Send("/ma \"Haste II\" " + target);
                Thread.Sleep(DELAY);
                AutoItX.Send("{Enter}");

                partyMemberHasteIITimer[partyMember].Enqueue(true);
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
                        Thread.Sleep(180000);
                    }
                    if (partyMemberHasteIITimer[partyMember].Count > 0)
                    {
                        partyMemberHasteIITimer[partyMember].Dequeue();
                    }
                }).Start();

                if (shouldFollow == true)
                {
                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize, HASTE_II_CAST_TIME);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize, HASTE_II_CAST_TIME);
                    }
                }
                Thread.Sleep(DELAY_AFTER_CAST);
            }
            else if (str == "Haste Samba")
            {
                const int DELAY = 100;
                const int HASTE_SAMBA_CAST_TIME = 1000;
                const int DELAY_AFTER_CAST = 2000;
                bool shouldUse = false;

                if (isHasteSambaActive() == false)
                {
                    shouldUse = true;
                }

                if (shouldUse == true)
                {
                    appendText("Using haste samba");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ja \"Haste Samba\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(HASTE_SAMBA_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Advancing March")
            {
                const int DELAY = 100;
                const int ADVANCING_MARCH_CAST_TIME = 10000;
                const int DELAY_AFTER_CAST = 2000;
                bool shouldCast = false;

                if (isAdvancingMarchActive() == false)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Using advancing march");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ma \"Advancing March\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(ADVANCING_MARCH_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Enwater II")
            {
                const int DELAY = 100;
                const int ENWATER_II_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;

                if (enwaterIITimer.Count == 0)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting enwater II");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ma \"Enwater II\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    enwaterIITimer.Enqueue(true);
                    new Thread(() =>
                    {
                        if (isComposureActive() == true)
                        {
                            appendText("Composure recognized");
                            Thread.Sleep(540000);
                        }
                        else
                        {
                            appendText("Composure not recognized");
                            Thread.Sleep(180000);
                        }
                        if (enwaterIITimer.Count > 0)
                        {
                            enwaterIITimer.Dequeue();
                        }
                    }).Start();

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, ENWATER_II_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, ENWATER_II_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(ENWATER_II_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Temper")
            {
                const int DELAY = 100;
                const int TEMPER_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;

                if (temperTimer.Count == 0)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting temper");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ma \"Temper\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    temperTimer.Enqueue(true);
                    new Thread(() =>
                    {
                        if (isComposureActive() == true)
                        {
                            appendText("Composure recognized");
                            Thread.Sleep(540000);
                        }
                        else
                        {
                            appendText("Composure not recognized");
                            Thread.Sleep(180000);
                        }
                        if (temperTimer.Count > 0)
                        {
                            temperTimer.Dequeue();
                        }
                    }).Start();

                    if (followQueue.Count > 0)
                    {
                        followQueue.Dequeue();
                        followTarget(2, partySize, TEMPER_CAST_TIME);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize, TEMPER_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Temper II")
            {
                const int DELAY = 100;
                const int TEMPER_II_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 1000;
                bool shouldCast = false;

                if (temperTimer.Count == 0)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    appendText("Casting temper ii");
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ma \"Temper II\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    temperTimer.Enqueue(true);
                    new Thread(() =>
                    {
                        if (isComposureActive() == true)
                        {
                            appendText("Composure recognized");
                            Thread.Sleep(540000);
                        }
                        else
                        {
                            appendText("Composure not recognized");
                            Thread.Sleep(180000);
                        }
                        if (temperTimer.Count > 0)
                        {
                            temperTimer.Dequeue();
                        }
                    }).Start();

                    if (shouldFollow == true)
                    {
                        if (followQueue.Count > 0)
                        {
                            followQueue.Dequeue();
                            followTarget(2, partySize, TEMPER_II_CAST_TIME);
                        }
                        else if (followPartyMember2Queue.Count == 0)
                        {
                            followTarget(2, partySize, TEMPER_II_CAST_TIME);
                        }
                    }
                    else
                    {
                        Thread.Sleep(TEMPER_II_CAST_TIME);
                    }

                    Thread.Sleep(DELAY_AFTER_CAST);
                }
            }
            else if (str == "Assist")
            {
                const int DELAY = 100;
                string partyMemberString = getPartyMemberHotkey(2, partySize);
                Rectangle rectangle = getTargetRectangle(partySize);

                AutoItX.Send(partyMemberString);
                Thread.Sleep(500);
                if (isTargetingKikunachi(rectangle) == true)
                {
                    AutoItX.Send("/assist");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    if (isTargetingKikunachi(rectangle) == true)
                    {
                        AutoItX.Send("{ESC}");
                    }
                    else if (isTargettingAMonster(rectangle) == true || isEngaged(rectangle) == true)
                    {
                        appendText2("CLASS1: Going to fight a monster - Assist");
                        AutoItX.Send("/attack on");
                        Thread.Sleep(DELAY);
                        AutoItX.Send("{Enter}");
                        stopFollow();
                        followTarget(2, partySize);
                    }
                }
            }
            else if (str == "Check Engaged")
            {
                const int DELAY = 100;
                Rectangle rectangle = getTargetRectangle(partySize);
                if (isTargettingAMonster(rectangle) == true || isEngaged(rectangle) == true)
                {
                    appendText2("CLASS1: Going to fight a monster - Check Engaged");
                    AutoItX.Send("/attack on");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    stopFollow();
                    followTarget(2, partySize);
                }
            }
            else if (str == "Follow Party Member 2")
            {
                Rectangle partyMember2Rectangle = getPartyMemberDeadRectangle(2, partySize);
                if (isPartyMemberDead(partyMember2Rectangle) == true)
                {
                    stopFollow();
                }
                else if (followQueue.Count > 0)
                {
                    stopFollow();
                    followTarget(2, partySize);
                }
                else if (followPartyMember2Queue.Count == 0)
                {
                    followTarget(2, partySize);
                }
                else
                {
                    appendText("Already following party member 2");
                }
            }
            else if (str == "Stop Follow")
            {
                stopFollow();
            }
            else if (str == "Raise")
            {
                bool shouldCast = false;
                if (isPartyMemberDead(rectangle) == true)
                {
                    shouldCast = true;
                }

                if (shouldCast == true)
                {
                    const int DELAY = 100;
                    const int RAISE_CAST_TIME = 15000;
                    const int DELAY_AFTER_CAST = 2000;
                    appendText("Casting raise on " + target);
                    stopFollow();
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/ja \"Spontaneity\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(1000);
                    AutoItX.Send("/ma Raise " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(RAISE_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Echo Drops")
            {
                const int DELAY = 100;
                const int ECHO_DROPS_USE_TIME = 1000;
                const int DELAY_AFTER_USE = 0;
                bool shouldUse = false;

                if (isSilenced() == true)
                {
                    shouldUse = true;
                }

                if (shouldUse == true)
                {
                    appendText("Using echo drops...");
                    stopFollow();
                    AutoItX.Send("/item \"Echo Drops\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    Thread.Sleep(ECHO_DROPS_USE_TIME + DELAY_AFTER_USE);

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
            else if (str == "Remedy")
            {
                const int DELAY = 100;
                const int REMEDY_USE_TIME = 1000;
                const int DELAY_AFTER_USE = 0;
                bool shouldUse = false;

                if (isParalyzed() == true)
                {
                    shouldUse = true;
                }

                if (shouldUse == true)
                {
                    appendText("Using remedy...");
                    stopFollow();
                    AutoItX.Send("/item \"Remedy\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    Thread.Sleep(REMEDY_USE_TIME + DELAY_AFTER_USE);

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
            else if (str == "Holy Water")
            {
                const int DELAY = 100;
                const int HOLY_WATER_USE_TIME = 1000;
                const int DELAY_AFTER_USE = 0;
                bool shouldUse = false;

                if (isCursed() == true)
                {
                    shouldUse = true;
                }

                if (shouldUse == true)
                {
                    appendText("Using holy water...");
                    stopFollow();
                    AutoItX.Send("/item \"Holy Water\" <me>");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");

                    Thread.Sleep(HOLY_WATER_USE_TIME + DELAY_AFTER_USE);

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
            else if (str == "Pull Mob")
            {
                Rectangle rectangle = getTargetRectangle(partySize);
                const int DELAY = 100;
                bool foundMonster = false;

                for (int i = 0; i < 10; i++)
                {
                    AutoItX.Send("{TAB}");

                    if (isTargettingAMonster(rectangle) == true || isEngaged(rectangle) == true)
                    {
                        AutoItX.Send("/ma Dia <t>");
                        Thread.Sleep(DELAY);
                        AutoItX.Send("{Enter}");
                        Thread.Sleep(1000);

                        foundMonster = true;
                        if (isEngaged(rectangle) == true)
                        {
                            AutoItX.Send("/attack on");
                            Thread.Sleep(DELAY);
                            AutoItX.Send("{Enter}");
                            Thread.Sleep(DELAY);
                            stopFollow();
                            Thread.Sleep(4000);
                            AutoItX.Send("w");
                            break;
                        }
                    }
                }

                if (foundMonster == false)
                {
                    for (int i = 0; i < 40; i++)
                        AutoItX.Send("d");
                }
                

            }
        }

        private bool isPartyMemberMPToppedOff(Rectangle rectangle)
        {
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
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "3", greenMpPixelImages[i]);
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

        private bool isPartyMemberRedHP(Rectangle rectangle)
        {
            string[] redHPImages = { @".\images\player-red-hp-bar-pixel.png",
                @".\images\player-red-hp-bar-pixel2.png",
                @".\images\player-red-hp-bar-pixel3.png",
                @".\images\player-red-hp-bar-pixel4.png",
                @".\images\player-red-hp-bar-pixel6.png",
                @".\images\player-red-hp-bar-pixel8.png",
                @".\images\player-red-hp-bar-pixel10.png" };

            bool found = false;

            for (int i = 0; i < redHPImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "3", redHPImages[i]);
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
                appendText("Party member hp is red");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isPartyMemberOrangeHP(Rectangle rectangle)
        {
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
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "3", orangeHpImages[i]);
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

        private bool isPartyMemberYellowHP(Rectangle rectangle)
        {
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
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0", yellowHpImages[i]);
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

        private bool isPartyMemberDead(Rectangle rectangle)
        {
            string[] partyMemberDeadImages = { @".\images\player_dead_hp_bar.png", @".\images\player_dead_2_not_targetted.png", @".\images\player_dead_2_targetted.png", @".\images\player_dead_2_not_targetted_2.png", @".\images\player_dead_2_targetted_2.png", @".\images\player_dead_3_not_targetted.png", @".\images\player_dead_3_targetted.png", @".\images\player_dead_3_not_targetted_2.png", @".\images\player_dead_3_targetted_2.png", @".\images\player_dead_4_not_targetted.png", @".\images\player_dead_4_targetted.png", @".\images\player_dead_5_not_targetted.png", @".\images\player_dead_5_targetted.png", @".\images\player_dead_6_not_targetted.png", @".\images\player_dead_6_targetted.png" };

            bool found = false;

            for (int i = 0; i < partyMemberDeadImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "0" +
                    "", partyMemberDeadImages[i]);
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

        bool isComposureActive()
        {
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
            }

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
            else
            {
                AutoItX.Send("{r}");
            }
        }

        void followTarget()
        {
            if (followPartyMember2Queue.Count > 0)
            {
                followPartyMember2Queue.Dequeue();
                followQueue.Enqueue(true);
                AutoItX.Send("/follow");
                Thread.Sleep(750);
                AutoItX.Send("{Enter}");
                Thread.Sleep(100);
            }
            else if (followQueue.Count == 0)
            {
                followQueue.Enqueue(true);
                AutoItX.Send("/follow");
                Thread.Sleep(750);
                AutoItX.Send("{Enter}");
                Thread.Sleep(100);
            }
            else
            {
                // Do nothing because I'm already following the party member
            }

        }

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

            if (followQueue.Count > 0)
            {
                followQueue.Dequeue();
                followPartyMember2Queue.Enqueue(true);
                AutoItX.Send("/follow " + partyMemberString);
                Thread.Sleep(750);
                AutoItX.Send("{Enter}");
                Thread.Sleep(100);
            }
            else if (followPartyMember2Queue.Count == 0)
            {
                followPartyMember2Queue.Enqueue(true);
                AutoItX.Send("/follow " + partyMemberString);
                Thread.Sleep(750);
                AutoItX.Send("{Enter}");
                Thread.Sleep(100);
            }
            else
            {
                // Do nothing because I'm already following the party member
            }
        }

        void followTarget(int partyMember, int partySize, int waitTime)
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

            if (followQueue.Count > 0)
            {
                followQueue.Dequeue();
                followPartyMember2Queue.Enqueue(true);
                AutoItX.Send("/follow " + partyMemberString);
                Thread.Sleep(waitTime);
                AutoItX.Send("{Enter}");
                Thread.Sleep(100);
            }
            else if (followPartyMember2Queue.Count == 0)
            {
                followPartyMember2Queue.Enqueue(true);
                AutoItX.Send("/follow " + partyMemberString);
                Thread.Sleep(waitTime);
                AutoItX.Send("{Enter}");
                Thread.Sleep(100);
            }
            else
            {
                // Do nothing because I'm already following the party member
            }
        }

        bool isStoneskinActive()
        {

            if (File.Exists(@".\images\stoneskin.png") == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int status_x = 0;
            int status_y = 0;
            int status_width = (int)(windowDimensionInfo.Width * 0.30);
            int status_height = (int)(windowDimensionInfo.Height * 0.30);
            string[] results = UseImageSearch(status_x, status_y, status_width, status_height, "30", @".\images\stoneskin.png");
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

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int status_x = 0;
            int status_y = 0;
            int status_width = (int)(windowDimensionInfo.Width * 0.30);
            int status_height = (int)(windowDimensionInfo.Height * 0.30);
            string[] results = UseImageSearch(status_x, status_y, status_width, status_height, "3", utsusemiImages[0]);
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
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
            }

            string[] hasteImages = { @".\images\haste.png" };
            if (File.Exists(hasteImages[0]) == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int status_x = 0;
            int status_y = 0;
            int status_width = (int)(windowDimensionInfo.Width * 0.30);
            int status_height = (int)(windowDimensionInfo.Height * 0.30);
            string[] results = UseImageSearch(status_x, status_y, status_width, status_height, "10", hasteImages[0]);
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
            if (AutoItX.WinExists("[CLASS:FFXiClass]") == 1)
            {
                if (AutoItX.WinActive("[CLASS:FFXiClass]") == 0)
                {
                    AutoItX.WinActivate("[CLASS:FFXiClass]");
                }
            }

            string[] enwater2Images = { @".\images\enwater.png" };
            if (File.Exists(enwater2Images[0]) == false)
            {
                return false;
            }

            Rectangle windowDimensionInfo = AutoItX.WinGetPos("[CLASS:FFXiClass]");
            int status_x = 0;
            int status_y = 0;
            int status_width = (int)(windowDimensionInfo.Width * 0.40);
            int status_height = (int)(windowDimensionInfo.Height * 0.30);
            string[] results = UseImageSearch(status_x, status_y, status_width, status_height, "20", enwater2Images[0]);
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

        private string getPartyMemberHotkey(int partyMember, int partySize)
        {
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
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    partyMemberString = "{F1}";
                }
            }

            return partyMemberString;
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

        public bool isTargetingKikunachi(Rectangle rectangle)
        {
            string[] kikunachiImages = { @".\images\kikunachi_2.png", @".\images\kikunachi_3.png", @".\images\kikunachi_4.png", @".\images\kikunachi_5.png", @".\images\kikunachi_6.png" };  
            bool found = false;
            for (int i = 0; i < kikunachiImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", kikunachiImages[i]);
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
                appendText("I am not targeting Kikunachi");
                return false;
            }
            else
            {
                appendText("I am targeting Kikunachi");
                return true;
            }
        }

        public bool isEngaged(Rectangle rectangle)
        {
            string[] assistImages = { @".\images\engaged_monster_name_pixel.png" };

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
                appendText("I am targeting an engaged monster to fight");
                return true;
            }
            else
            {
                appendText("I am not targeting a engaged monster to fight");
                return false;
            }
        }

        public bool isTargettingAMonster(Rectangle rectangle)
        {
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

        void useWeaponSkill()
        {
            int delay = 100;
            if (weaponskill == "Fast Blade")
            {
                appendText("Using fast blade...");
                stopFollow();
                Thread.Sleep(100);
                AutoItX.Send("/ws \"Fast Blade\" <t>");
                Thread.Sleep(1000);
                AutoItX.Send("{Enter}");
                Thread.Sleep(3000);
            }
            else if (weaponskill == "Burning Blade")
            {
                appendText("Using burning blade...");
                AutoItX.Send("/ws \"Burning Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(3000);
            }
            else if (weaponskill == "Red Lotus Blade")
            {
                appendText("Using red lotus blade...");
                stopFollow();
                Thread.Sleep(100);
                AutoItX.Send("/ws \"Red Lotus Blade\" <t>");
                Thread.Sleep(1000);
                AutoItX.Send("{Enter}");
                Thread.Sleep(3000);
            }
            else if (weaponskill == "Wasp Sting")
            {
                appendText("Using wasp sting...");
                AutoItX.Send("/ws \"Wasp Sting\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(3000);
            }
            else if (weaponskill == "Shining Blade")
            {
                appendText("Using shining blade...");
                AutoItX.Send("/ws \"Shining Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(3000);
            }
            else if (weaponskill == "Seraph Blade")
            {
                appendText("Using seraph blade...");
                AutoItX.Send("/ws \"Seraph Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
            }
            else if (weaponskill == "Vorpal Blade")
            {
                appendText("Using vorpal blade...");
                stopFollow();
                Thread.Sleep(100);
                AutoItX.Send("/ws \"Vorpal Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(3000);
            }
            else if (weaponskill == "Flat Blade")
            {
                appendText("Using flat blade...");
                AutoItX.Send("/ws \"Flat Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(3000);
            }
            else if (weaponskill == "Circle Blade")
            {
                appendText("Using circle blade...");
                AutoItX.Send("/ws \"Circle Blade\" <t>");

                if (canIUseNextWeaponskill() == true)
                {
                    if (willCircleBladeMakeLevel2Fragmentation() == true)
                    {
                        AutoItX.Send("{Enter}");
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling circle blade");
                    }
                }
                else if (isSkillchainActive() == false)
                {
                    AutoItX.Send("{Enter}");
                }
                else
                {
                    if (canIUseNextWeaponskillInLessThan1Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 1 second");
                        Thread.Sleep(1000);
                    }
                    else if (canIUseNextWeaponskillInLessThan2Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 2 second");
                        Thread.Sleep(2000);
                    }
                    else if (canIUseNextWeaponskillInLessThan3Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 3 second");
                        Thread.Sleep(3000);
                    }

                    if (canIUseNextWeaponskill() == true)
                    {
                        if (willCircleBladeMakeLevel2Fragmentation() == true)
                        {
                            AutoItX.Send("{Enter}");
                        }
                        else
                        {
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            appendText2("Cancelling circle blade");
                        }
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling circle blade - no skillchain with cicle blade is possible");
                    }
                }
            }
            else if (weaponskill == "Savage Blade")
            {
                appendText("Using savage blade...");
                AutoItX.Send("/ws \"Savage Blade\" <t>");

                if (canIUseNextWeaponskill() == true)
                {
                    if (willSavageBladeMakeLevel3Light() == true)
                    {
                        AutoItX.Send("{Enter}");
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling savage blade");
                    }
                }
                else if (isSkillchainActive() == false)
                {
                    AutoItX.Send("{Enter}");
                }
                else
                {
                    if (canIUseNextWeaponskillInLessThan1Second() == true)
                    {
                        appendText("*** SLEEPING FOR 1 second");
                        Thread.Sleep(1000);
                    }
                    else if (canIUseNextWeaponskillInLessThan2Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 2 second");
                        Thread.Sleep(2000);
                    }
                    else if (canIUseNextWeaponskillInLessThan3Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 3 second");
                        Thread.Sleep(3000);
                    }

                    if (canIUseNextWeaponskill() == true)
                    {
                        if (willSavageBladeMakeLevel3Light() == true)
                        {
                            AutoItX.Send("{Enter}");
                        }
                        else
                        {
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            appendText2("Cancelling savage blade");
                        }
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling savage blade - no skillchain with savage blade is possible");
                    }
                }
            }
            else if (weaponskill == "Requiescat")
            {
                appendText("Using requiescat...");
                AutoItX.Send("/ws \"Requiescat\" <t>");

                if (canIUseNextWeaponskill() == true)
                {
                    if (willRequiescatMakeLevel3Darkness() == true)
                    {
                        AutoItX.Send("{Enter}");
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling requiescat");
                    }
                }
                else if (isSkillchainActive() == false)
                {
                    AutoItX.Send("{Enter}");
                }
                else
                {
                    if (canIUseNextWeaponskillInLessThan1Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 1 second");
                        Thread.Sleep(1000);
                    }
                    else if (canIUseNextWeaponskillInLessThan2Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 2 second");
                        Thread.Sleep(2000);
                    }
                    else if (canIUseNextWeaponskillInLessThan3Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 3 second");
                        Thread.Sleep(3000);
                    }

                    if (canIUseNextWeaponskill() == true)
                    {
                        if (willRequiescatMakeLevel3Darkness() == true)
                        {
                            AutoItX.Send("{Enter}");
                        }
                        else
                        {
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            appendText2("Cancelling requiescat");
                        }
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling requiescat - no skillchain with requiescat is possible");
                    }
                }
            }
            else if (weaponskill == "Knights of Round")
            {
                appendText("Using knights of round...");
                AutoItX.Send("/ws \"Knights of Round\" <t>");

                //if (canIUseNextWeaponskillInLessThan6Second() == true)
                //{
                //    appendText2("*** SLEEPING FOR 4 3/4 second");
                //    Thread.Sleep(4750);
                //}
                //else if (canIUseNextWeaponskillInLessThan5Second() == true)
                //{
                //    appendText2("*** SLEEPING FOR 3 3/4 second");
                //    Thread.Sleep(3750);
                //}
                //else if (canIUseNextWeaponskillInLessThan4Second() == true)
                //{
                //    appendText2("*** SLEEPING FOR 2 3/4 second");
                //    Thread.Sleep(2750);
                //}
                //else if (canIUseNextWeaponskillInLessThan3Second() == true)
                //{
                //    appendText2("*** SLEEPING FOR 1 3/4 second");
                //    Thread.Sleep(1750);
                //}
                //else if (canIUseNextWeaponskillInLessThan2Second() == true)
                //{
                //    appendText2("*** SLEEPING FOR 3/4 second");
                //    Thread.Sleep(750);
                //}
                //else if (canIUseNextWeaponskillInLessThan1Second() == true)
                //{
                //    appendText2("*** SLEEPING FOR 0 second");
                //    Thread.Sleep(0);
                //}

                if (canIUseNextWeaponskill() == true)
                {
                    if (willKnightsOfRoundMakeLevel4Light() == true)
                    {
                        AutoItX.Send("{Enter}");
                    }
                    else if (willKnightsOfRoundMakeLevel3Light() == true)
                    {
                        AutoItX.Send("{Enter}");
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling knights of round");
                    }
                }
                else if (isSkillchainActive() == false)
                {
                    AutoItX.Send("{Enter}");
                }
                else
                {
                    if (canIUseNextWeaponskillInLessThan1Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 1 second");
                        Thread.Sleep(1000);
                    }
                    else if (canIUseNextWeaponskillInLessThan2Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 2 second");
                        Thread.Sleep(2000);
                    }
                    else if (canIUseNextWeaponskillInLessThan3Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 3 second");
                        Thread.Sleep(3000);
                    }

                    if (canIUseNextWeaponskill() == true)
                    {
                        if (willKnightsOfRoundMakeLevel4Light() == true)
                        {
                            AutoItX.Send("{Enter}");
                        }
                        else if (willKnightsOfRoundMakeLevel3Light() == true)
                        {
                            AutoItX.Send("{Enter}");
                        }
                        else
                        {
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            appendText2("Cancelling knights of round");
                        }

                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling knights of round - no skillchain with knights of round is possible");
                    }
                }
                
                //if (canIUseNextWeaponskill() == true)
                //{
                //    if (willKnightsOfRoundMakeLevel4Light() == true)
                //    {
                //        AutoItX.Send("{Enter}");
                //        Thread.Sleep(2000);
                //    }
                //    else if (willKnightsOfRoundMakeLevel3Light() == true)
                //    {
                //        AutoItX.Send("{Enter}");
                //        Thread.Sleep(2000);
                //    }
                //    else
                //    {
                //        AutoItX.Send("{ESC}");
                //        Thread.Sleep(delay);
                //        AutoItX.Send("{ESC}");
                //        Thread.Sleep(delay);
                //        appendText2("Cancelling knights of round - no skillchain with knights of round is possible");
                //    }
                //}
                //else if (isSkillchainActive() == false)
                //{
                //    AutoItX.Send("{Enter}");
                //    Thread.Sleep(2000);
                //}
                //else
                //{
                //    AutoItX.Send("{ESC}");
                //    Thread.Sleep(delay);
                //    AutoItX.Send("{ESC}");
                //    Thread.Sleep(delay);
                //    appendText2("Cancelling knights of round");
                //}
            }
            else if (weaponskill == "Chant du Cygne")
            {
                appendText("Using chant du cygne...");
                AutoItX.Send("/ws \"Chant du Cygne\" <t>");

                if (canIUseNextWeaponskill() == true)
                {
                    if (willChantDuCygneMakeLevel3Darkness() == true)
                    {
                        AutoItX.Send("{Enter}");
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling chant du cygne");
                    }
                }
                else if (isSkillchainActive() == false)
                {
                    AutoItX.Send("{Enter}");
                }
                else
                {
                    if (canIUseNextWeaponskillInLessThan1Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 1 second");
                        Thread.Sleep(1000);
                    }
                    else if (canIUseNextWeaponskillInLessThan2Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 2 second");
                        Thread.Sleep(2000);
                    }
                    else if (canIUseNextWeaponskillInLessThan3Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 3 second");
                        Thread.Sleep(3000);
                    }

                    if (canIUseNextWeaponskill() == true)
                    {
                        if (willChantDuCygneMakeLevel3Darkness() == true)
                        {
                            AutoItX.Send("{Enter}");
                        }
                        else
                        {
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            appendText2("Cancelling chant du cygne");
                        }
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling chant du cygne - no skillchain with chant du cygne is possible");
                    }
                }
            }
            else if (weaponskill == "Death Blossom")
            {
                appendText("Using death blossom...");
                AutoItX.Send("/ws \"Death Blossom\" <t>");

                if (canIUseNextWeaponskill() == true)
                {
                    if (willDeathBlossomMakeLevel3Light() == true)
                    {
                        AutoItX.Send("{Enter}");
                    }
                    else
                    {
                        appendText2("TODO: DEATH BLOSSOM");
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling death blossom");
                    }
                }
                else if (isSkillchainActive() == false)
                {
                    AutoItX.Send("{Enter}");
                }
                else 
                {
                    if (canIUseNextWeaponskillInLessThan1Second() == true)
                    {
                        appendText("*** SLEEPING FOR 1 second");
                        Thread.Sleep(1000);
                    }
                    else if (canIUseNextWeaponskillInLessThan2Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 2 second");
                        Thread.Sleep(2000);
                    }
                    else if (canIUseNextWeaponskillInLessThan3Second() == true)
                    {
                        appendText2("*** SLEEPING FOR 3 second");
                        Thread.Sleep(3000);
                    }

                    if (canIUseNextWeaponskill() == true)
                    {
                        if (willDeathBlossomMakeLevel3Light() == true)
                        {
                            AutoItX.Send("{Enter}");
                        }
                        else
                        {
                            appendText2("Cancelling death blossom");
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                            AutoItX.Send("{ESC}");
                            Thread.Sleep(delay);
                        }
                    }
                    else
                    {
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        AutoItX.Send("{ESC}");
                        Thread.Sleep(delay);
                        appendText2("Cancelling death blossom - no skillchain with death blossom is possible");
                    }
                }
            }
            else if (weaponskill == "Gust Slash")
            {
                appendText("Using gust slash...");
                AutoItX.Send("/ws \"Gust Slash\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Viper Bite")
            {
                appendText("Using viper bite...");
                AutoItX.Send("/ws \"Viper Bite\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else
            {
                appendText("Select a weapon skill from the dropdown list...");
            }
        }

        bool isWeaponDrawn()
        {
            string[] weaponDrawnImages = { @".\images\weapon_drawn.png" };

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
                    rectangle.Y = 690;
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
                    rectangle.Y = 690;
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
                    rectangle.Y = 690;
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
                    rectangle.Y = 690;
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
                    rectangle.Y = 685;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }
            else if (partySize == 1)
            {
                if (partyMember == 1)
                {
                    rectangle.X = 1320;
                    rectangle.Y = 690;
                    rectangle.Width = 1340;
                    rectangle.Height = 705;
                }
            }

            return rectangle;
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

        bool isSkillchainActive()
        {
            string[] skillchainImages = { @".\images\windower_skillchain_background.png" };

            Rectangle rectangle = new Rectangle(135, 355, 485, 445);
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
                @".\images\windower_skillchain_go_2.png" };

            bool found = false;
            Rectangle rectangle = new Rectangle(130, 350, 165, 380);

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
                appendText2("*** USING WEAPONSKILL IN 1 SECONDS");
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
                appendText2("*** USING WEAPONSKILL IN 2 SECONDS");
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
                appendText2("*** USING WEAPONSKILL IN 3 SECONDS");
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
                appendText2("*** USING WEAPONSKILL IN 4 SECONDS");
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
                appendText2("*** USING WEAPONSKILL IN 5 seconds");
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
                appendText2("CLASS1: I can use next weaponskill in less than 6 second");
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
                appendText("****CLASS1: Level 3 light is possible - Death Blossom****");
                return true;
            }
            else
            {
                appendText2("****CLASS1: Level 3 light is not possible - Death Blossom ****");
                return false;
            }
        }

        bool willKnightsOfRoundMakeLevel4Light()
        {
            string[] skillchainKnightsOfRoundImages = { @".\images\windower_skillchain_knights_of_round_level_4_light_2.png" };

            Rectangle rectangle = new Rectangle(130, 350, 490, 530);
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
                appendText("****CLASS 1: Level 4 light is possible - Knights of Round****");
                return true;
            }
            else
            {
                appendText("****CLASS 1: Level 4 light is not possible - Knights of Round****");
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
                appendText2("****CLASS1: Level 3 light is possible - Knights of Round***");
                return true;
            }
            else
            {
                appendText2("****CLASS1: Level 3 light is NOT possible - Knights of Round***");
                return false;
            }
        }

        bool willSavageBladeMakeLevel3Light()
        {
            string[] windowerSkillchainImages = { @".\images\windower_skillchain_savage_blade_level_3_light.png", 
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
                appendText2("****CLASS1: Level 3 light is possible - Savage Blade****");
                return true;
            }
            else
            {
                appendText2("****CLASS1: Level 3 light is not possible - Savage Blade****");
                return false;
            }
        }

        bool willChantDuCygneMakeLevel3Darkness()
        {
            string[] windowerSkillchainImages = { @".\images\windower_skillchain_chant_du_cygne_level_3_darkness.png" };

            Rectangle rectangle = new Rectangle(130, 415, 480, 485);

            bool found = false;

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
                appendText2("***CLASS1: Level 3 darkness is possible - Chant du Cygne***");
                return true;
            }
            else
            {
                appendText2("***CLASS1: Level 3 darkness is not possible - Chant du Cygne***");
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
                appendText("***Level 3 darkness is possible - Requiescat    ***");
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
                appendText("***Level 2 fragmentation is possible - CIRCLE BLADE    ***");
                return true;
            }
        }

        bool almostHasTP()
        {
            string[] tpImages = { @".\images\tp_text_700.png", @".\images\tp_text_800.png", @".\images\tp_text_900.png" };

            bool found = false;
            for (int i = 0; i < tpImages.Length; i++)
            {
                string[] results = UseImageSearch(116, 90, 130, 110, "3", tpImages[i]);
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
                appendText2("CLASS1: I almost have TP");
                return true;
            }
            else
            {
                appendText2("CLASS1: I do not almost have TP");
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

        bool isHPToppedOff(Rectangle rectangle)
        {
            string[] hpPixelImages = { @".\images\pink-hp-pixel.png",
                @".\images\pink-hp-pixel-2.png",
                @".\images\pink-hp-pixel-3.png",
                @".\images\pink-hp-pixel-4.png" };

            bool found = false;

            for (int i = 0; i < hpPixelImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "3", hpPixelImages[i]);
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
    }
}