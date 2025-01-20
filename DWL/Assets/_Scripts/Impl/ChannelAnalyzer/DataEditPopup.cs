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
            //�̰� �ϴ� �Ҵ縸.. 
            this.channelInfos = channelInfos;
            this.refreshCallback = refreshCallback;

            if (null == info || info.sectionInfos.Count == 0)
            {
                EndEdit();
                return;
            }

            editingCellInfo = info;
            clone = new AChannelInfo(editingCellInfo);

            //������ ���̸� �ľ��ؼ�, RectScroll�� �������� �°� ���� �Ϸ����Ѵ�.
            float frame = editingCellInfo.GetTotalFrame();
            float width = 2;
            if (frame != -1)
                width = viewGraph.GetGraphBGWidth() / frame;

            //View�� �ִ� �׷��� Init
            var vGInfo = new GraphGridInfo();
            vGInfo.spacingX = width;
            vGInfo.channelIndex = info.channelIndex;
            vGInfo.onRefreshSectionInfo = RefreshSectionInfo;
            vGInfo.savedDuties = excelDuties;
            vGInfo.sectionInfos = editingCellInfo.sectionInfos;
            viewGraph.InitGraph(vGInfo);

            //Handle�� �ִ� �׷��� Init
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
        /// Handle�� ����� ������, �ش� ���� ���� �Ǵ� �����ؼ� ����, ���� ������ ��ģ�ٸ�, ���־��Ѵ�.
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

                        //�����ϱ�
                        if (isRemove)
                        {
                            _Remove(info);
                            _RefreshAndRebuild();
                            break;
                        }
                        //�����ϱ�
                        else
                        {
                            info.Update(newInfo.order, newInfo.time, newInfo.level);
                            break;
                        }
                    }
                }

                //�߰��ϱ�
                if (!isExist)
                {
                    _Add(newInfo);
                    _RefreshAndRebuild();
                }

                viewGraph.UpdateGraph(editingCellInfo.sectionInfos);
                handleGraph.UpdateGraph(editingCellInfo.sectionInfos);

                //������ ����
                void _RefreshAndRebuild()
                {
                    //time�� ������� �������� ����
                    editingCellInfo.sectionInfos = editingCellInfo.sectionInfos.OrderBy(x => x.time).ToList();

                    //���ĵ� sections�� order��ο�
                    for (int i = 0; i < editingCellInfo.sectionInfos.Count; i++)
                    {
                        editingCellInfo.sectionInfos[i].order = i;
                    }
                }

                //����
                void _Remove(ASectionInfo info)
                {
                    editingCellInfo.sectionInfos.Remove(info);
                }

                //�߰�
                void _Add(ASectionInfo info)
                {
                    editingCellInfo.sectionInfos.Add(info);
                }
            }
        }

        private void InitCurrentViewingRectAreaNotifier()
        {
            //Edit�� ����/�������� ���� ratio
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