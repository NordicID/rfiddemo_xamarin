using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NurApiDotNet;
using NurApiDotNet.Commands;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin
{
    /// <summary>
    /// Locating tag.<br/>
    /// Pinpoint specific tag using handheld reader equipped with Crossdipole antenna.<br/>
    /// Locating info (0-100%) will be received via <see cref="OnLocateTag"/> event.
    /// </summary>
    public class LocateTag
    {
        private NurApiDotNet.NurApi mApi;
        private TxLevel mBackupTxLevel;
        private int mBackupInvSession;
        private uint mBackupAntennaMask;
        private AntennaId mBackupSelectedAntenna;

        AverageBuffer mSignalAverage = new AverageBuffer(3, 0);
        SmoothingBuffer mSmoothingBuffer = new SmoothingBuffer(5);

        uint mCrossDipoleAntMask = 0;
        uint mProximityAntMask = 0;
        uint mCircularAntMask = 0;

        /// <summary>
        /// Locate tag event. <see cref="LocateTagEventArgs"/>
        /// </summary>
        public event EventHandler<LocateTagEventArgs> OnLocateTag;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nurHandle">NurApi handle <see cref="NurApiDotNet.NurApi"/></param>
        public LocateTag(NurApiDotNet.NurApi nurHandle)
        {
            mApi = nurHandle;
        }

        /// <summary>
        /// Start locating tag by EPC.<br/>
        /// Use readers equipped with Crossdipole antenna.<br/>
        /// Location events received in <see cref="OnLocateTag"/>
        /// </summary>
        /// <param name="epc">EPC of tag to locate as (HEX)string.</param>
        public void Start(string epc)
        {
            byte[] byteTag = HexStringToBin(epc);
            Start(byteTag);
        }

        /// <summary>
        /// Start locating tag by EPC.<br/>
        /// Use readers equipped with Crossdipole antenna.<br/>
        /// Location events received in <see cref="OnLocateTag"/>
        /// </summary>
        /// <param name="epc">EPC of tag to locate as byte array.</param>
        public void Start(byte[] epc)
        {
            mApi.StopContinuous();

            SetupAntenna();
            mApi.TraceTagEvent += MApi_TraceTagEvent;

            var flags = TraceTagFlag.NoEPC | TraceTagFlag.StartContinuous;
            mApi.TraceTag((byte)Bank.EPC, 32, epc.Length * 8, epc, (int)flags);
        }

        private void MApi_TraceTagEvent(object sender, TraceTagEventArgs e)
        {
            int pros = LocateTagCalculateResult(e.data.scaledRssi);
            LocateTagEventArgs args = new LocateTagEventArgs(mApi.GetTimeStamp(), pros);
            OnLocateTag?.Invoke(this, args);
        }

        /// <summary>
        /// Stop Locating tag
        /// </summary>
        public void Stop()
        {
            if (mApi.IsConnected()) // && mApi.IsTraceRunning())
            {
                mApi.StopContinuous();
                mApi.TraceTagEvent -= MApi_TraceTagEvent;
                RestoreAntenna();
            }
        }

        int LocateTagCalculateResult(int scaledRssi)
        {
            int sRssi = Adjust(scaledRssi);
            return mSmoothingBuffer.Add(sRssi);
        }
        
        private int Adjust(int locateSignal)
        {
            int newSignal;

            if (mApi.AntennaMaskEx != mProximityAntMask)
            {
                
                if(mProximityAntMask > 0)
                    newSignal = (int)(locateSignal * 0.93f); // rescale 0-100 to 0-95 as proximity makes up last 5%
                else newSignal = locateSignal; //No proximity onboard so use full scale
            }
            else
            {
                // It's proximity. Rescale 0-70 to 95-100
                newSignal = 93 + (int)((float)locateSignal / 14);

                if (newSignal > 100)                
                    newSignal = 100;                
            }            

            mSignalAverage.Add(newSignal,0);

            int avgSignal = (int)mSignalAverage.avgValue;

            //Debug.WriteLine("LOCATE", "locateSignal " + locateSignal + " NewSignal=" + newSignal + " Avg=" + avgSignal);

            if (newSignal == 0 && mCrossDipoleAntMask > 0)
            {
                mApi.AntennaMaskEx = (uint)mCrossDipoleAntMask;
                return avgSignal;
            }

            if (mApi.AntennaMaskEx == mCrossDipoleAntMask)
            {
                // If we get over 40% switch to Circular.
                // It is faster since there is only one antenna to do inventory on,
                // but The crossdipole has slightly better range.
                if (newSignal > 40 && mCircularAntMask > 0)                
                    mApi.AntennaMaskEx = mCircularAntMask; 
            }
            else if (mApi.AntennaMaskEx == mCircularAntMask)
            {
                // If we get under 35% switch to CrossDP
                if (newSignal < 35 && mCrossDipoleAntMask > 0)                
                    mApi.AntennaMaskEx = (uint)mCrossDipoleAntMask;   
                // If Circular gets over 90% we have ran out of sensitivity on that
                // antenna and it the proximity antenna is now useful.
                else if (newSignal >= 90 && mProximityAntMask > 0)                
                    mApi.AntennaMaskEx = mProximityAntMask;                
            }
            else if (mApi.AntennaMaskEx == mProximityAntMask)
            {
                // Set Circular back on for the next pass
                if (newSignal <= 93 && mCircularAntMask > 0)                
                    mApi.AntennaMaskEx = mCircularAntMask;                
            }

            return avgSignal;
        }

        private void SetupAntenna()
        {
            List<AntennaMapping> antresp = mApi.GetAntennaList();

            foreach (AntennaMapping item in antresp)
            {                
                if (item.Name.Contains("CrossDipole"))
                {                   
                    mCrossDipoleAntMask |= (uint)(1 << item.AntennaId);
                }
                else if (item.Name.Contains("CrossDipole.Y"))
                {                    
                    mCrossDipoleAntMask |= (uint)(1 << item.AntennaId);
                }
                else if (item.Name.Contains("Proximity"))
                {                   
                    mProximityAntMask = (uint)(1 << (int)item.AntennaId);
                }
                else if (item.Name.Contains("Circular"))
                {                   
                    mCircularAntMask = (uint)(1 << (int)item.AntennaId);
                }
            }


            mBackupTxLevel = (TxLevel)mApi.TxLevel; // (TxLevel)Setup.txLevel;
            mBackupInvSession = mApi.InventorySession;
            mBackupAntennaMask = (uint)mApi.AntennaMaskEx;
            mBackupSelectedAntenna = (AntennaId)mApi.SelectedAntenna;

            // set the tx level and antenna to auto select
            mApi.InventorySession = 0;
            mApi.TxLevel = 0;
            mApi.SelectedAntenna = ANTENNAID_AUTOSELECT; // AntennaId.AutoSelect;         
            if(mCrossDipoleAntMask > 0)
                mApi.AntennaMaskEx = (uint)mCrossDipoleAntMask;

        }

        void RestoreAntenna()
        {
            // restore old settings
            mApi.SelectedAntenna = (int)mBackupSelectedAntenna;
            mApi.AntennaMaskEx = (uint)mBackupAntennaMask;
            mApi.TxLevel = (int)mBackupTxLevel;
            mApi.InventorySession = mBackupInvSession;
        }
                      
    }

    /// <summary>
    /// Locate tag event arguments
    /// </summary>
    public class LocateTagEventArgs
    {
        /// <summary>
        /// Log event arguments
        /// </summary>
        /// <param name="timestamp">milliseconds after NurApi initialized</param>
        /// <param name="pros">Value in percentage how near tag is. 0% not seen.</param>

        public LocateTagEventArgs(uint timestamp, int pros)
        {
            this.timestamp = timestamp;
            this.pros = pros;
        }

        /// <summary>
        /// milliseconds after NurApi initialized
        /// </summary>
        public uint timestamp;

        /// <summary>
        /// Value in percentage how near tag is. 0% not seen.
        /// </summary>            
        public int pros;

    }

    class SmoothingBuffer
    {

        List<int> values;
        int count;

        public SmoothingBuffer(int size)
        {
            values = new List<int>();
            count = size;
        }

        public int Add(int value)
        {

            if (values.Count > count)
            {
                values.RemoveAt(0);
            }

            values.Add(value);

            return values.Sum() / values.Count();

        }
    }
}
