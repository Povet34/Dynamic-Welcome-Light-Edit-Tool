using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ChannelAnalyzers
{
    public class DataEditPopup : MonoBehaviour
    {
        private const int HANDLE_GRAPH_SPACING_X = 50;

        [SerializeField] GraphBuilderImpl_WithCompareView viewGraph;
        [SerializeField] GraphBuilderImpl_UsingHandle handleGraph;

        [SerializeField] TMP_InputField compareChannelInputField;
        [SerializeField] UIButton saveButton;
        [SerializeField] UIButton cancelButton;

        [SerializeField] UIImage currentViewingRectAreaNotifier;

        Action<AChannelInfo> refreshCallback;
        AChannelInfo editingCellInfo;
        AChannelInfo clone;

        //Only Compare
        List<AChannelInfo> channelInfos;

        private void Awake()
        {
            saveButton.SetCallback(null, OnSave);
            cancelButton.SetCallback(null, OnCancelEdit);

            compareChannelInputField.onEndEdit.AddListener(ChangeCompareGraph);
        }

        private void Update()
        {
            UpdateViewingRectPos();
        }

        public void StartEdit(AChannelInfo info, Action<AChannelInfo> refreshCallback, List<AChannelInfo> channelInfos, List<int> excelDuties)
        {
            //이거 일단 할당만.. 
            this.channelInfos = channelInfos;
            this.refreshCallback = refreshCallback;

            if (null == info || info.sectionInfos.Count == 0)
            {
                EndEdit();
                return;
            }

            editingCellInfo = info;
            clone = new AChannelInfo(editingCellInfo);

            //동적인 길이를 파악해서, RectScroll의 범위에딱 맞게 들어가게 하려고한다.
            float frame = editingCellInfo.GetTotalFrame();
            float width = 2;
            if (frame != -1)
                width = viewGraph.GetGraphBGWidth() / frame;

            //View만 있는 그래프 Init
            var vGInfo = new GraphGridInfo();
            vGInfo.spacingX = width;
            vGInfo.channelIndex = info.channelIndex;
            vGInfo.onRefreshSectionInfo = RefreshSectionInfo;
            vGInfo.savedDuties = excelDuties;
            vGInfo.sectionInfos = editingCellInfo.sectionInfos;
            viewGraph.InitGraph(vGInfo);

            //Handle이 있는 그래프 Init
            var hGInfo = new GraphGridInfo();
            hGInfo.spacingX = HANDLE_GRAPH_SPACING_X;
            hGInfo.channelIndex = info.channelIndex;
            hGInfo.savedDuties = excelDuties;
            hGInfo.onRefreshSectionInfo = RefreshSectionInfo;
            hGInfo.sectionInfos = editingCellInfo.sectionInfos;
            handleGraph.InitGraph(hGInfo);

            InitCurrentViewingRectAreaNotifier();
        }

        private void OnSave(string buttonData)
        {
            EndEdit();
            refreshCallback?.Invoke(editingCellInfo);
        }

        private void OnCancelEdit(string buttonData)
        {
            EndEdit();
        }

        private void EndEdit()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Handle이 변경될 때마다, 해당 값을 변경 또는 생성해서 정렬, 만약 완전히 겹친다면, 없애야한다.
        /// </summary>
        /// <param name="newInfo"></param>
        private void RefreshSectionInfo(ASectionInfo newInfo, bool isRemove)
        {
            if (null != editingCellInfo)
            {
                bool isExist = false;
                for (int i = 0; i < editingCellInfo.sectionInfos.Count; i++)
                {
                    var info = editingCellInfo.sectionInfos[i];
                    if (info.order == newInfo.order)
                    {
                        isExist = true;

                        //제거하기
                        if (isRemove)
                        {
                            _Remove(info);
                            _RefreshAndRebuild();
                            break;
                        }
                        //수정하기
                        else
                        {
                            info.Update(newInfo.order, newInfo.time, newInfo.level);
                            break;
                        }
                    }
                }

                //추가하기
                if (!isExist)
                {
                    _Add(newInfo);
                    _RefreshAndRebuild();
                }

                viewGraph.UpdateGraph(editingCellInfo.sectionInfos);
                handleGraph.UpdateGraph(editingCellInfo.sectionInfos);

                //리빌드 과정
                void _RefreshAndRebuild()
                {
                    //time을 순서대로 오름차순 정렬
                    editingCellInfo.sectionInfos = editingCellInfo.sectionInfos.OrderBy(x => x.time).ToList();

                    //정렬된 sections에 order재부여
                    for (int i = 0; i < editingCellInfo.sectionInfos.Count; i++)
                    {
                        editingCellInfo.sectionInfos[i].order = i;
                    }
                }

                //삭제
                void _Remove(ASectionInfo info)
                {
                    editingCellInfo.sectionInfos.Remove(info);
                }

                //추가
                void _Add(ASectionInfo info)
                {
                    editingCellInfo.sectionInfos.Add(info);
                }
            }
        }

        private void InitCurrentViewingRectAreaNotifier()
        {
            //Edit의 길이/컨텐츠의 길이 ratio
            float handleGpWidth = handleGraph.GetContentRt().rect.width;
            var ratio = handleGpWidth / handleGraph.GetLastHandlePosX();
            var rt = currentViewingRectAreaNotifier.rectTransform;

            rt.sizeDelta = new Vector2(viewGraph.GetLastHandlePosX() * ratio + 3, rt.rect.height);

            UpdateViewingRectPos();
        }

        private void UpdateViewingRectPos()
        {
            var rt = currentViewingRectAreaNotifier.rectTransform;
            var posRatio = -(handleGraph.GetContentRt().anchoredPosition.x /handleGraph.GetLastHandlePosX());

            rt.anchoredPosition = new Vector2(viewGraph.GetLastHandlePosX() * posRatio, rt.anchoredPosition.y);
        }

        private void ChangeCompareGraph(string compareChannelTxt)
        {
            if (string.IsNullOrEmpty(compareChannelTxt))
                return;

            if(int.TryParse(compareChannelTxt, out int compareChannel))
            {
                foreach(var ch in channelInfos)
                {
                    if(ch.channelIndex == compareChannel)
                    {
                        viewGraph.BuildComparewGraph(ch.sectionInfos);
                        handleGraph.BuildComparewGraph(ch.sectionInfos);
                    }
                }
            }
        }
    }
}