using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NurApiDotNet;
using static NurApiDotNet.NurApi;

namespace nur_tools_rfiddemo_xamarin
{
    class Stats
    {
        Stopwatch stopWatch = new Stopwatch();

        private const double TAGS_PER_SEC_OVERTIME = 2.0;

		private AverageBuffer mTagsPerSecBuffer = new AverageBuffer(1000, (int)(TAGS_PER_SEC_OVERTIME * 1000));

		private long mTagsReadTotal = 0;
		private double mTagsPerSec = 0;
		private double mAvgTagsPerSec = 0;
		private double mMaxTagsPerSec = 0;
		private int mInventoryRounds = 0;		
		private double mTagsFoundInTime = 0;

		public void UpdateStats(int roundsDone, int tagsaddedAndUpdated)
		{			
            mTagsPerSecBuffer.Add(tagsaddedAndUpdated, stopWatch.ElapsedMilliseconds);

            mTagsReadTotal += tagsaddedAndUpdated;

			mTagsPerSec = mTagsPerSecBuffer.sumValue / TAGS_PER_SEC_OVERTIME;
			if (GetElapsedSecs() > 1)
				mAvgTagsPerSec = mTagsReadTotal / GetElapsedSecs();
			else
				mAvgTagsPerSec = mTagsPerSec;

			if (mTagsPerSec > mMaxTagsPerSec)
				mMaxTagsPerSec = mTagsPerSec;

			mInventoryRounds += roundsDone;
		}

		public void Start()
		{
            Clear();
            stopWatch.Restart();           
		}

        public void Stop()
        {
            stopWatch.Stop();                        
        }

        public void Clear()
		{
			mTagsPerSecBuffer.Clear();
			mTagsReadTotal = 0;
			mTagsPerSec = 0;
			mAvgTagsPerSec = 0;
			mMaxTagsPerSec = 0;
			mInventoryRounds = 0;			
			mTagsFoundInTime = 0;            
		}

		public double GetElapsedSecs()
		{
            if (stopWatch.IsRunning == false) return 0;
            return stopWatch.ElapsedMilliseconds / 1000.0;
		}

		public long GetTagsReadTotal()
		{
			return mTagsReadTotal;
		}

		public int GetTagsPerSec()
		{
			return (int)mTagsPerSec;
		}

		public int GetAvgTagsPerSec()
		{
			return (int)mAvgTagsPerSec;
		}

		public int GetMaxTagsPerSec()
		{
			return (int)mMaxTagsPerSec;
		}

		public int GetInventoryRounds()
		{
			return mInventoryRounds;
		}

		public double GetTagsFoundInTimeSecs()
		{
			return mTagsFoundInTime;
		}

		public void SetTagsFoundInTimeSecs()
		{
			mTagsFoundInTime = GetElapsedSecs();
		}
	}

    class AverageBuffer
    {
        class Entry
        {
            public long time;
            public double val;
        }

        List<Entry> values;
        int maxAge;
        int maxSize;
        public double avgValue;
        public double sumValue;

        public AverageBuffer(int maxSize, int maxAge)
        {
            values = new List<Entry>();
            this.maxSize = maxSize;
            this.maxAge = maxAge;
            avgValue = 0;
            sumValue = 0;
        }

        public void Add(double value,long ms)
        {
            RemoveOld(ms);

            while (values.Count >= maxSize)
            {
                values.RemoveAt(0);
            }

            Entry entry = new Entry();
            entry.time = ms;
            entry.val = value;
            values.Add(entry);

            CalcAvg();
        }

        public void Clear()
        {
            values.Clear();
            avgValue = 0;
            sumValue = 0;
        }

        public double GetAvgValue()
        {
            return avgValue;
        }

        public double GetSumValue()
        {
            return sumValue;
        }

        public void CalcAvg()
        {
            if (values.Count == 0)
            {
                avgValue = 0;
                sumValue = 0;
                return;
            }

            double avgVal = 0;

            foreach (Entry tmp in values)
            {
                avgVal += tmp.val;
            }

            sumValue = avgVal;
            avgValue = avgVal / (double)values.Count;
        }

        public bool RemoveOld(long ms)
        {
            if (maxAge == 0)
            {
                return false;
            }

            bool ret = false;
                       
            int index = 0;
            while (index < values.Count)
            {
                Entry e = values[index];
                if (ms - e.time > maxAge)
                {
                    values.RemoveAt(index);
                    ret = true;
                }
                else
                {
                    index++;
                }
            }

            return ret;
        }

    }
}
