﻿using AutoIt;
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
        Rectangle rectangle;
        private List<Queue<bool>> partyMemberRefreshTimer;
        private List<Queue<bool>> partyMemberHasteIITimer;
        private Queue<bool> temperTimer;
        private Queue<bool> enwaterIITimer;
        private Queue<bool> followQueue;
        private Queue<bool> followPartyMember2Queue;
        private Queue<bool> engagedQueue;
        private string weaponskill = "";

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
            if (str == "Weaponskill")
            {
                useWeaponSkill();

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
            else if (str == "Composure")
            {
                const int DELAY = 100;
                const int DELAY_AFTER_USE = 2000;
                stopFollow();
                Thread.Sleep(DELAY);
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
                const int DELAY_AFTER_USE = 2000;

                bool shouldUse = false;

                if (isMPLow(1, partySize) == true)
                {
                    shouldUse = true;
                }
                else
                {
                    shouldUse = false;
                }

                if (shouldUse == true)
                {
                    stopFollow();
                    Thread.Sleep(DELAY);
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
                }
            }
            else if (str == "Cure")
            {
                const int DELAY = 100;
                const int CURE_CAST_TIME = 2500;
                const int DELAY_AFTER_CAST = 2000;
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
                    Thread.Sleep(CURE_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Cure II")
            {
                const int DELAY = 100;
                const int CURE_II_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 2000;

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
                    Thread.Sleep(CURE_II_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Cure III")
            {
                const int DELAY = 100;
                const int CURE_III_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 2000;

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
                    appendText("Casting cure III on " + target);
                    AutoItX.Send("/ma \"Cure III\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(CURE_III_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Cure IV")
            {
                const int DELAY = 100;
                const int CURE_IV_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 2000;

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
                    appendText("Casting cure IV on " + target);
                    AutoItX.Send("/ma \"Cure IV\" " + target);
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(CURE_IV_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Refresh")
            {
                const int DELAY = 100;
                const int REFRESH_CAST_TIME = 5000;
                const int DELAY_AFTER_CAST = 2000;

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
                const int DELAY_AFTER_CAST = 2000;

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
                    Thread.Sleep(REFRESH_II_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Stoneskin")
            {
                const int DELAY = 100;
                const int STONESKIN_CAST_TIME = 7000;
                const int DELAY_AFTER_CAST = 2000;
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
                    Thread.Sleep(STONESKIN_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Utsusemi: Ichi")
            {
                const int DELAY = 100;
                const int UTSUSEMI_ICHI_CAST_TIME = 4000;
                const int DELAY_AFTER_CAST = 2000;
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
                    Thread.Sleep(UTSUSEMI_ICHI_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Utsusemi: Ni")
            {
                const int DELAY = 100;
                const int UTSUSEMI_NI_CAST_TIME = 1500;
                const int DELAY_AFTER_CAST = 2000;
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
                    Thread.Sleep(UTSUSEMI_NI_CAST_TIME + DELAY_AFTER_CAST);

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
                const int DELAY_AFTER_CAST = 2000;
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
                    Thread.Sleep(HASTE_II_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Haste II Party Member")
            {
                const int DELAY = 100;
                const int HASTE_II_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 2000;

                appendText("Casting Haste II on " + target);
                stopFollow();
                Thread.Sleep(DELAY);
                AutoItX.Send("/ma \"Haste II\" " + target);
                Thread.Sleep(DELAY);
                AutoItX.Send("{Enter}");
                Thread.Sleep(HASTE_II_CAST_TIME + DELAY_AFTER_CAST);

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
                const int DELAY_AFTER_CAST = 2000;
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
                    Thread.Sleep(ENWATER_II_CAST_TIME + DELAY_AFTER_CAST);

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
            else if (str == "Temper")
            {
                const int DELAY = 100;
                const int TEMPER_CAST_TIME = 3000;
                const int DELAY_AFTER_CAST = 2000;
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
                    Thread.Sleep(TEMPER_CAST_TIME + DELAY_AFTER_CAST);

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
                        followTarget(2, partySize);
                    }
                    else if (followPartyMember2Queue.Count == 0)
                    {
                        followTarget(2, partySize);
                    }
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
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/assist");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(1000);

                    if (isTargetingKikunachi(rectangle) == true)
                    {
                        AutoItX.Send("{ESC}");
                    }
                }
            }
            else if (str == "Check Engaged")
            {
                const int DELAY = 100;
                Rectangle rectangle = getTargetRectangle(partySize);
                if (isEngaged(rectangle) == true)
                {
                    appendText("Going to fight a monster");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("/attack on");
                    Thread.Sleep(DELAY);
                    AutoItX.Send("{Enter}");
                    Thread.Sleep(2000);
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
        }

        private bool isPartyMemberMPToppedOff(Rectangle rectangle)
        {
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "30", @".\images\player-green-mp-bar-pixel.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool isPartyMemberRedHP(Rectangle rectangle)
        {
            string[] redHPImages = { @".\images\player-red-hp-bar-pixel.png", @".\images\player-red-hp-bar-pixel2.png", @".\images\player-red-hp-bar-pixel3.png" };

            bool found = false;

            for (int i = 0; i < redHPImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "10", redHPImages[i]);
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

        private bool isPartyMemberOrangeHP(Rectangle rectangle)
        {
            string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "13", @".\images\player-orange-hp-bar-pixel.png");
            if (results == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool isPartyMemberYellowHP(Rectangle rectangle)
        {
            string[] yellowHpImages = { @".\images\player-yellow-hp-bar-pixel.png", @"/\images\player-yellow-hp-bar-pixel-2.png" };
            bool found = false;
            for (int i = 0; i < yellowHpImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "7", yellowHpImages[i]);
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
            string[] partyMemberDeadImages = { @".\images\player_dead_2_not_targetted.png", @".\images\player_dead_2_targetted.png", @".\images\player_dead_2_not_targetted_2.png", @".\images\player_dead_2_targetted_2.png", @".\images\player_dead_3_not_targetted.png", @".\images\player_dead_3_targetted.png", @".\images\player_dead_3_not_targetted_2.png", @".\images\player_dead_3_targetted_2.png", @".\images\player_dead_4_not_targetted.png", @".\images\player_dead_4_targetted.png", @".\images\player_dead_5_not_targetted.png", @".\images\player_dead_5_targetted.png", @".\images\player_dead_6_not_targetted.png", @".\images\player_dead_6_targetted.png" };

            bool found = false;

            for (int i = 0; i < partyMemberDeadImages.Length; i++)
            {
                string[] results = UseImageSearch(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "5", partyMemberDeadImages[i]);
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
            string[] assistImages = { @".\images\engaged_monster_name_pixel.png", @".\images\engaged_monster_name_pixel3.png", @".\images\engaged_monster_name_pixel4.png", @".\images\engaged_monster_name_pixel5.png", @".\images\target_monster_name_pixel.png", @".\images\target_monster_name_pixel_2.png" };

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
            else if (weaponskill == "Savage Blade")
            {
                appendText("Using savage blade...");
                AutoItX.Send("/ws \"Savage Blade\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Requiescat")
            {
                appendText("Using requiescat...");
                AutoItX.Send("/ws \"Requiescat\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Knights of Round")
            {
                appendText("Using knights of round...");
                AutoItX.Send("/ws \"Knights of Round\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
            }
            else if (weaponskill == "Chant du Cygne")
            {
                appendText("Using chant du cygne...");
                AutoItX.Send("/ws \"Chant du Cygne\" <t>");
                Thread.Sleep(delay);
                AutoItX.Send("{Enter}");
                Thread.Sleep(2000);
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

        private void appendText(string s)
        {
            control.BeginInvoke((MethodInvoker)delegate ()
            {
                DateTime timestamp = DateTime.Now;
                textBox1.AppendText(timestamp.ToString() + " " + s);
                textBox1.AppendText(Environment.NewLine);
            });
        }
    }
}