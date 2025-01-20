using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ChannelAnalyzers
{
    /// <summary>
    /// 애널라이저 채널컨트롤러 정보. (List<AChanelInfo> 가지고있음.)
    /// </summary>
    public class AChannelsControllerInfo
    {
        public Action<AChannelInfo, bool> showGraphCallback;
        public List<AChannelInfo> channelInfos;
        public Action<string> errorCallback;
    }

    /// <summary>
    /// 애널라이저의 채널 정보. (List<ASectionInfo> 가지고 있음.)
    /// </summary>
    public class AChannelInfo
    {
        public Vector2Int pos;
        public int channelIndex;
        public List<ASectionInfo> sectionInfos;

        public int GetTotalFrame() => sectionInfos?.Last().time ?? -1;

        public AChannelInfo() { }

        public AChannelInfo(Vector2Int pos, int channelIndex, List<ASectionInfo> sectionInfos)
        {
            this.pos = pos;
            this.channelIndex = channelIndex;
            this.sectionInfos = sectionInfos;
        }

        public AChannelInfo(AChannelInfo duplicate)
        {
            pos = duplicate.pos;
            channelIndex = duplicate.channelIndex;
            sectionInfos = new List<ASectionInfo>(duplicate.sectionInfos);
        }


        public static List<int> GetSectionLevels(AChannelInfo info)
        {
            List<int> result = new List<int>();
            if (null != info)
            {
                foreach (var sec in info.sectionInfos)
                    result.Add(sec.level);
            }
            return result;
        }
    }

    public class ASectionInfo
    {
        public int channelIndex;
        public int order;
        public int time;
        public int level;

        public ASectionInfo() { }

        public ASectionInfo(int channelIndex, int order, int time, int level)
        {
            this.channelIndex = channelIndex;
            this.order = order;
            this.time = time;
            this.level = level;
        }

        public ASectionInfo(ASectionInfo copy)
        {
            channelIndex = copy.channelIndex;
            order = copy.order;
            time = copy.time;
            level = copy.level;
        }

        public void Update(int order, int time, int level)
        {
            this.order = order;
            this.time = time;
            this.level = level;
        }
    }

    public interface IDuty
    {
        /// <summary>
        /// 사용되는 수치 0 ~ 100사이. index는 순서
        /// </summary>
        List<int> levels { get; }

        /// <summary>
        /// levels가 변경되는 
        /// </summary>
        List<int> prelevels { get; }

        void SetLevels(List<int> levels);
        /// <summary>
        /// Level 조절 시도.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>성공 여부 반환</returns>
        bool TryChangeLevel(int newlevel, int currentLevel);
        int GetAdjcentLevel(int value);
        int GetPanelRatioLevel(int extendHeight, List<int> dutiesByPanelRatios);
        int GetlevelByIndex(int dutyIdx);
        int GetPreLevlByIndex(int value);
        int GetDutyIndex(int value);

        public static List<int> GetFixedDutyGap()
        {
            return new List<int> { 60, 120, 190, 255 };
        }

        public static List<int> GetSavedDuties()
        {
            return Provider.Instance.GetDuty().levels;
        }
    }
}