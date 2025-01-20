using ChannelAnalyzers;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class ExcelDataConverter
{
    public enum SectionType
    {
        None,
        Flicker, //���ҵ� : ������ ���簡 ��� index
        FadeIn,  //�����ϴٰ� ��ȭ : ������ index ���簡 255
        FadeOut, //��ȭ�ϴٰ� ���� : ������ 255 ���簡 index
        Maintain,
    }
    public class Inference
    {
        public SectionType type;
        public int startFrame;
        public int endFrame;
        public int duty;

        public Inference() { }

        public Inference(SectionType type, int startFrame, int endFrame, int duty)
        {
            this.type = type;
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.duty = duty;
        }

        public Inference(int startFrame, int endFrame, int duty)
        {
            this.startFrame = startFrame;
            this.endFrame = endFrame;
            this.duty = duty;
        }

        public static List<int> GetToIntList(List<Inference> inputs)
        {
            List<int> result = new List<int>();
            foreach (var input in inputs)
                result.Add(input.duty);

            return result;
        }
    }

    /// <summary>
    /// Graph datas to excel cell datas [List<AChannelInfo> -> Dictionary<RecordKey, List<RecordValue>>]
    /// </summary>
    /// <param name="infos"></param>
    /// <returns></returns>
    public static Dictionary<RecordKey, List<RecordValue>> ConvertToSimplifiedRecordMap(List<AChannelInfo> infos)
    {
        Dictionary<RecordKey, List<RecordValue>> recordDictionary = new Dictionary<RecordKey, List<RecordValue>>();
        for (int i = 0; i < infos.Count; i++)
        {
            var ch = infos[i];

            var key = new RecordKey(ch.channelIndex, ch.pos);
            var inferList = GetInferences_BySections(ch.sectionInfos);
            recordDictionary[key] = _MakeRecordList(inferList);
        }

        return recordDictionary;

        List<RecordValue> _MakeRecordList(List<Inference> inferences)
        {
            Dictionary<int, RecordValue> duplicatedPreventMap = new Dictionary<int, RecordValue>();
            duplicatedPreventMap[0] = new RecordValue(0);

            for (int ii = 0; ii < inferences.Count; ii++)
            {
                var inference = inferences[ii];

                if (inference.type == SectionType.Flicker)
                {
                    duplicatedPreventMap[inference.startFrame + 1] = new RecordValue(inference.duty);
                    if (ii != inferences.Count - 1)
                    {
                        var nextInference = inferences[ii + 1];
                        nextInference.startFrame = inference.startFrame + 2;
                    }
                }
                else
                {
                    for (int oo = inference.startFrame; oo <= inference.endFrame; oo++)
                    {
                        duplicatedPreventMap[oo] = new RecordValue(inference.duty);
                    }
                }
            }

            ModifyDictionary(duplicatedPreventMap);
            return duplicatedPreventMap.Values.ToList();
        }
    }

    /// <summary>
    /// Excel cell datas to Graph datas [Dictionary<RecordKey, List<RecordValue>> -> List<AChannelInfo>]
    /// </summary>
    /// <param name="maps"></param>
    /// <returns></returns>
    public static List<AChannelInfo> ConvertToChannelCellInfo(Dictionary<RecordKey, List<RecordValue>> maps)
    {
        List<AChannelInfo> convertedCellInfos = new List<AChannelInfo>();
        foreach (var key in maps.Keys)
        {
            var origin = RecordValue.GetIntList(maps[key]);
            var changeMap = _FindChangingPoints(origin);
            var typeMap = _OverridePointType(changeMap, origin);
            var sections = _MakeSectionFlow(key.index, typeMap, origin);

            AChannelInfo channelCellInfo = new AChannelInfo(key.pos, key.index, sections);
            convertedCellInfos.Add(channelCellInfo);
        }

        return convertedCellInfos;

        Dictionary<int, int> _FindChangingPoints(List<int> origin)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            int previous = 0;
            result[0] = previous;

            for (int i = 1; i < origin.Count; i++)
            {
                int cur = origin[i];
                if (previous != cur)
                {
                    result[i] = cur;
                }

                previous = cur;
            }

            result[origin.Count - 1] = origin.Last();

            return result;
        }

        Dictionary<int, SectionType> _OverridePointType(Dictionary<int, int> changeMap, List<int> origin)
        {
            Dictionary<int, SectionType> result = new Dictionary<int, SectionType>();

            foreach (var changer in changeMap)
            {
                //0
                if (changer.Key == 0)
                {
                    result[changer.Key] = SectionType.None;
                    continue;
                }

                //Maintain
                if (origin[changer.Key - 1] == changer.Value)
                {
                    result[changer.Key] = SectionType.Maintain;
                }
                //Filcker
                else if (origin[changer.Key - 1] != Definitions.FADE_VALUE && changer.Value != Definitions.FADE_VALUE)
                {
                    result[changer.Key] = SectionType.Flicker;
                }
                //FadeIn
                else if (origin[changer.Key - 1] != Definitions.FADE_VALUE && changer.Value == Definitions.FADE_VALUE)
                {
                    result[changer.Key - 1] = SectionType.FadeIn;
                }
                //FadeOut
                else if (origin[changer.Key - 1] == Definitions.FADE_VALUE && changer.Value != Definitions.FADE_VALUE)
                {
                    result[changer.Key] = SectionType.FadeOut;
                }
            }

            return result;
        }

        List<ASectionInfo> _MakeSectionFlow(int chIdx, Dictionary<int, SectionType> typeMap, List<int> origin)
        {
            List<ASectionInfo> result = new List<ASectionInfo>();
            var keys = typeMap.Keys.ToList();
            var duty = Provider.Instance.GetDuty();

            var values = typeMap.Values.ToList();

            int order = 0;
            for (int i = 0; i < typeMap.Count; i++)
            {
                int frame = keys[i];
                int index = origin[frame];

                if (Definitions.IS_FADE_VALUE(index))
                {
                    index = origin[keys[i - 1]];
                    Debug.Log($"Has Fade value on {i}th key.. Can't access index.. ");
                }

                int bright = duty.GetlevelByIndex(index);

                switch (values[i])
                {
                    case SectionType.None:
                    case SectionType.Maintain:
                        {
                            ASectionInfo sectionInfo = new ASectionInfo(chIdx, order++, frame, bright);
                            result.Add(sectionInfo);
                        }
                        break;
                    case SectionType.FadeIn:
                    case SectionType.FadeOut:
                        {
                            ASectionInfo sectionInfo = new ASectionInfo(chIdx, order++, frame + 2, bright);
                            result.Add(sectionInfo);
                        }
                        break;
                    case SectionType.Flicker:
                        {
                            int preBright = duty.GetlevelByIndex(origin[frame - 1]);
                            ASectionInfo first = new ASectionInfo(chIdx, order++, frame + 1, preBright);
                            result.Add(first);

                            ASectionInfo second = new ASectionInfo(chIdx, order++, frame + 1, bright);
                            result.Add(second);
                        }
                        break;
                }
            }

            _CheckStartIsFade(result);

            return result;
        }

        void _CheckStartIsFade(List<ASectionInfo> result)
        {
            if (result.Count > 2)
            {
                var first = result[0];
                var second = result[1];

                if (first.order == 0 && second.order == 1)
                {
                    //IF not same.. this section is [Fade] section.
                    if (first.level != second.level)
                    {
                        Provider.Instance.ShowErrorPopup(Definitions.ERROR_EXIST_CHANNEL_FAIL_SHOT_OF_MINIMUM_INTERVALS_DUTY_ON_START);
                    }
                }
            }
        }
    }

    public static List<Inference> GetInferences_ByReordValue(List<RecordValue> recordValues)
    {
        List<Inference> inferences = new List<Inference>();
        RecordValue currentRecord = recordValues[0];
        RecordValue preRecord = currentRecord;

        int lastFrame = recordValues.Count - 1;
        int startFrame = 0;

        for (int ii = 0; ii < recordValues.Count; ii++)
        {
            if (recordValues[ii].Brightness_Int != currentRecord.Brightness_Int)
            {
                if (Definitions.IS_FLICKERING(preRecord.Brightness_Int, currentRecord.Brightness_Int))
                {
                    if(startFrame != 0)
                    {
                        inferences.Add(new Inference
                        {
                            startFrame = startFrame - 1,
                            endFrame = startFrame - 1,
                            duty = currentRecord.Brightness_Int
                        });
                    }
                }

                inferences.Add(new Inference
                {
                    startFrame = startFrame,
                    endFrame = ii - 1 + Definitions.ADD_FADE_TIME_OFFSET(currentRecord.Brightness_Int),
                    duty = currentRecord.Brightness_Int
                });

                preRecord = currentRecord;
                currentRecord = recordValues[ii];
                startFrame = ii;
            }
        }

        if(inferences.Count == 0)
        {
            inferences.Add(new Inference
            {
                startFrame = startFrame,
                endFrame = lastFrame,
                duty = 0
            });
        }

        // ������ ���� �߰�
        var lastDuty = recordValues[lastFrame].Brightness_Int == Definitions.FADE_VALUE ? (inferences.Last().duty == Definitions.LAST_DUTY_INDEX ? Definitions.FIRST_DUTY : Definitions.LAST_DUTY_INDEX) : recordValues[lastFrame].Brightness_Int;

        //�������� Flicker ���..
        if (inferences.Last().duty != Definitions.FADE_VALUE)
        {
            inferences.Add(new Inference(startFrame, startFrame, lastDuty));
        }

        inferences.Add(new Inference(startFrame, lastFrame, lastDuty));


        //255��� Fade�����̸�, Fade�� ������, ������ ���۵Ǵ°��� Duty�� �����Ѵ�.
        for(int ii = 0; ii < inferences.Count; ii++)
        {
            if(Definitions.IS_FADE_VALUE(inferences[ii].duty))
            {
                inferences[ii].duty = inferences[ii + 1].duty;
            }
        }

        return inferences;
    }

    public static Dictionary<int, List<Inference>> GetInferencesDic(Dictionary<RecordKey, List<RecordValue>> dic)
    {
        Dictionary<int, List<Inference>> result = new Dictionary<int, List<Inference>>();
        foreach(var element in dic)
            result[element.Key.index] = GetInferences_ByReordValue(element.Value);

        return result;
    }

    /// <summary>
    /// �� ������ � Ÿ������ �߷��Ͽ�, ���ǰ� ������ �����Ѵ�.
    /// </summary>
    /// <param name="sectionInfos"></param>
    /// <returns></returns>
    private static List<Inference> GetInferences_BySections(List<ASectionInfo> sectionInfos)
    {
        List<Inference> sectionTypeList = new List<Inference>();
        var duty = Provider.Instance.GetDuty();

        for (int i = 1; i < sectionInfos.Count; i++)
        {
            var curSection = sectionInfos[i];
            var preSection = (i > 0) ? sectionInfos[i - 1] : curSection;

            if (null == preSection || null == curSection)
            {
                return null;
            }

            int start = preSection.time;
            int end = curSection.time;

            int index = duty.GetDutyIndex(curSection.level);

            if (preSection.time != curSection.time)
            {
                if (preSection.level != curSection.level)
                {
                    sectionTypeList.Add(new Inference(SectionType.FadeIn, start, end, 255));
                }
                else if (preSection.level == curSection.level)
                {
                    sectionTypeList.Add(new Inference(SectionType.Maintain, start - 1, end - 1, index));
                }
            }
            else if (preSection.level != curSection.level)
            {
                sectionTypeList.Add(new Inference(SectionType.Flicker, start - 1, start - 1, index));
            }
        }

        return sectionTypeList;
    }

    public static Dictionary<RecordKey, List<RecordValue>> ConvertToBrightnessRecordMap(List<AChannelInfo> infos, List<int> savedDuties)
    {
        Dictionary<RecordKey, List<RecordValue>> recordDictionary = new Dictionary<RecordKey, List<RecordValue>>();
        for (int i = 0; i < infos.Count; i++)
        {
            var ch = infos[i];

            var key = new RecordKey(ch.channelIndex, ch.pos);
            var inferList = GetInferences_BySections(ch.sectionInfos);

            var map = _MakeLinearBrightnessMap(inferList);
            if (null != map)
            {
                recordDictionary[key] = map;
            }
            else
            {
                return null;
            }
        }

        return recordDictionary;

        List<RecordValue> _MakeLinearBrightnessMap(List<Inference> inferences)
        {
            var levels = savedDuties;
            if (levels == null || levels.Count == 0)
                levels = Provider.Instance.GetDuty().levels;

            Dictionary<int, RecordValue> duplicatedPreventMap = new Dictionary<int, RecordValue>
            {
                [0] = new RecordValue(0)
            };

            for (int ii = 0; ii < inferences.Count; ii++)
            {
                var inference = inferences[ii];

                if (inference.type == SectionType.Flicker)
                {
                    duplicatedPreventMap[inference.startFrame + 1] = new RecordValue(levels[inference.duty]);
                    if (ii != inferences.Count - 1)
                    {
                        var nextInference = inferences[ii + 1];
                        nextInference.startFrame = inference.startFrame + 2;
                    }
                }
                else
                {
                    if (inference.duty == Definitions.FADE_VALUE)
                    {
                        int preOffset = 1;
                        int nexOffset = 1;
                        try
                        {
                            // front , current, next duties are all fade.. if occured this case, access ealier and later duties level
                            if (inferences[ii - preOffset].duty == Definitions.FADE_VALUE)
                            {
                                for (int pp = ii - 1; pp > 0; pp--)
                                {
                                    if (inferences[pp].duty != Definitions.FADE_VALUE)
                                    {
                                        break;
                                    }
                                    preOffset++;
                                }
                            }
                            if (inferences[ii + nexOffset].duty == Definitions.FADE_VALUE)
                            {
                                for (int nn = ii + 1; nn < inferences.Count - 1; nn++)
                                {
                                    if (inferences[nn].duty != Definitions.FADE_VALUE)
                                    {
                                        break;
                                    }
                                    nexOffset++;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Provider.Instance.ShowErrorPopup(Definitions.ERROR_EXIST_CHANNEL_STARTED_FADE_ON_ZERO_FRAME);
                            return null;
                        }

                        int preIndex = Mathf.Max(0, ii - preOffset);
                        int nextIndex = Mathf.Min(inferences.Count - 1, ii + nexOffset);

                        int preLevel = levels[inferences[preIndex].duty];
                        int nextLevel = levels[inferences[nextIndex].duty];

                        if (nextIndex - preIndex != 2) //when result is not -2 indices sub .. this is nested fade region.
                        {
                            int newCurrentIndex = nextIndex - 1;
                            Debug.Log($"Nested Fade! Set New Current Index {newCurrentIndex}");
                            float subAverage = (float)(nextLevel - preLevel) / (inferences[newCurrentIndex].endFrame - inferences[preIndex].endFrame);
                            
                            int count = 0;
                            //Current는 next의 -1일 뿐이기에, PreEnd -> NextStart까지로 해야한다.
                            for (int oo = inferences[preIndex].endFrame; oo <= inferences[newCurrentIndex].endFrame; oo++)
                            {
                                duplicatedPreventMap[oo] = new RecordValue(Mathf.RoundToInt(preLevel + (subAverage * count++)));
                            }
                            ii = newCurrentIndex;
                        }
                        else // not nested fade..
                        {
                            float subAverage = (float)(nextLevel - preLevel) / (inference.endFrame - inference.startFrame);

                            int count = 0;
                            for (int oo = inference.startFrame; oo <= inference.endFrame; oo++)
                            {
                                duplicatedPreventMap[oo] = new RecordValue(Mathf.RoundToInt(preLevel + (subAverage * count++)));
                            }
                        }
                    }
                    else
                    {
                        for (int oo = inference.startFrame; oo <= inference.endFrame; oo++)
                        {
                            duplicatedPreventMap[oo] = new RecordValue(levels[inference.duty]);
                        }
                    }
                }
            }

            return duplicatedPreventMap.Values.ToList();
        }
    }

    static void ModifyDictionary(Dictionary<int, RecordValue> map)
    {
        if (map.Count < 2)
        {
            return; // Not enough elements to perform the operation
        }

        // Step 1: Duplicate the last two values and add them to the end
        int maxKey = -1;
        foreach (var key in map.Keys)
        {
            if (key > maxKey)
            {
                maxKey = key;
            }
        }

        if (maxKey >= 1)
        {
            map[maxKey + 1] = map[maxKey - 1];
            map[maxKey + 2] = map[maxKey];
        }

        // Step 2: Remove the first two elements
        if (map.ContainsKey(1)) map.Remove(1);
        if (map.ContainsKey(2)) map.Remove(2);

        // Step 3: Shift the keys down by two positions
        Dictionary<int, RecordValue> newMap = new Dictionary<int, RecordValue>();
        foreach (var kvp in map)
        {
            newMap[kvp.Key - 2] = kvp.Value;
        }

        // Update the original map
        map.Clear();
        foreach (var kvp in newMap)
        {
            map[kvp.Key] = kvp.Value;
        }
    }

    #region Find Tables

    public static Dictionary<int, int> GetModeTable(Dictionary<RecordKey, List<RecordValue>> orign)
    {
        Dictionary<int, int> result = new Dictionary<int, int>();

        foreach (var element in orign)
        {
            int preIndex = 255;
            bool isExist = false;
            foreach (var section in element.Value)
            {
                int curIndex = section.Brightness_Int;
                if (preIndex != curIndex)
                {
                    //�ø�Ŀ�� �ѹ� �ѹ� �� �ִ´�.,
                    if(Definitions.IS_FLICKERING(preIndex, curIndex))
                    {
                        _AddMode();
                    }

                    _AddMode();

                    isExist = true;
                    preIndex = curIndex;
                }
            }

            if(!isExist)
            {
                result[element.Key.index] = 0;
            }

            void _AddMode()
            {
                if (result.ContainsKey(element.Key.index))
                    result[element.Key.index]++;
                else
                    result[element.Key.index] = 0; // ModeTable�� �� ������ �ƴ϶�, Section�� ������ �ε������� �ʿ���ϱ⿡, 0���� �����Ѵ�.
            }
        }

        return result;
    }

    public static Dictionary<int, List<int>> GetTimeTable(Dictionary<int, List<Inference>> inferDic)
    {
        Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
        foreach (var element in inferDic)
        {
            result[element.Key] = new List<int>();
            for (int i = 0; i < element.Value.Count; i++)
            {
                var infer = element.Value[i];
                result[element.Key].Add(infer.endFrame - infer.startFrame);
            }
        }

        return result;
    }

    public static Dictionary<int, List<int>> GetDutyTable(Dictionary<int, List<Inference>> inferDic)
    {
        Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
        foreach (var element in inferDic)
        {
            result[element.Key] = new List<int>();
            for (int i = 0; i < element.Value.Count; i++)
            {
                var infer = element.Value[i];
                result[element.Key].Add(infer.duty);
            }
        }
        return result;
    }

    #endregion
}
