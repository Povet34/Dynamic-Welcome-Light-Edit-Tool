using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ChannelAnalyzers
{
    /// <summary>
    /// �ֳζ����� ä����Ʈ�ѷ� ����. (List<AChanelInfo> ����������.)
    /// </summary>
    public class AChannelsControllerInfo
    {
        public Action<AChannelInfo, bool> showGraphCallback;
        public List<AChannelInfo> channelInfos;
        public Action<string> errorCallback;
    }

    /// <summary>
    /// �ֳζ������� ä�� ����. (List<ASectionInfo> ������ ����.)
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
        /// ���Ǵ� ��ġ 0 ~ 100����. index�� ����
        /// </summary>
        List<int> levels { get; }

        /// <summary>
        /// levels�� ����Ǵ� 
        /// </summary>
        List<int> prelevels { get; }

        void SetLevels(List<int> levels);
        /// <summary>
        /// Level ���� �õ�.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>���� ���� ��ȯ</returns>
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