using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChannelAnalyzers
{
    public class GraphBuildersController : MonoBehaviour
    {
        const float CELL_SIZE_Y = 291;

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform panel;
        [SerializeField] private RectTransform content;

        [SerializeField] private GameObject dataEditPopupPrefab;


        private List<GraphBuilderImpl_OnlyView> graphBuilders = new List<GraphBuilderImpl_OnlyView>();
        private ScrollHolder scrollHolder = new ScrollHolder(ScrollHolder.Axis.Y);

        [Header("Prefab")]
        [SerializeField] GameObject graphPrefab;

        GraphControllerInfo info;

        private void Update()
        {
            scrollHolder.UpdateHolder();
        }

        public void Init(GraphControllerInfo info)
        {
            this.info = info;
            InitGraphBuildersController(info.channelInfos, info.savedDuties);
        }

        void InitGraphBuildersController(List<AChannelInfo> channelInfos, List<int> savedDuties)
        {
            DestroyAllGraphBuilders();

            for (int i = 0; i < channelInfos.Count; i++)
            {
                var go = Instantiate(graphPrefab, content);
                var gh = go.GetComponent<GraphBuilderImpl_OnlyView>();

                //동적인 길이를 파악해서, RectScroll의 범위에딱 맞게 들어가게 하려고한다.
                float frame = channelInfos.First().GetTotalFrame();
                float width = 2;
                if (frame != -1)
                    width = gh.GetGraphBGWidth() / frame;

                var info = new GraphGridInfo();
                info.spacingX = width;
                info.channelIndex = channelInfos[i].channelIndex;
                info.savedDuties = savedDuties;
                info.sectionInfos = channelInfos[i].sectionInfos;
                info.startEditCallback = StartEditGraph;

                gh.InitGraph(info);
                graphBuilders.Add(gh);
            }

            UpdateContentHolder();
        }


        /// <summary>
        /// 채널 컨트롤러의 채널 토글들을 키고 끄는 용도로 사용된다.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isShow"></param>
        public void ShowTargetGraph(AChannelInfo info, bool isShow)
        {
            if (null != info)
            {
                foreach (var gh in graphBuilders)
                {
                    if (gh.graphGridInfo.channelIndex == info.channelIndex)
                    {
                        gh.Show(isShow);
                        break;
                    }
                }

                UpdateContentHolder();
            }
        }

        void UpdateContentHolder()
        {
            int activingCount = 0;
            foreach (var gh in graphBuilders)
            {
                if (gh.gameObject.activeSelf)
                    activingCount++;
            }

            content.sizeDelta = new Vector2(content.sizeDelta.x, activingCount * CELL_SIZE_Y);
            scrollHolder.UpdateScrollHolderData(content, panel, 0, content.rect.height);
        }

        public void DestroyAllGraphBuilders()
        {
            if (null != graphBuilders)
            {
                foreach (var gh in graphBuilders)
                    Destroy(gh.gameObject);

                graphBuilders.Clear();
            }
        }

        #region Edit : --------------------------------------------------------

        private void StartEditGraph(int index, List<int> excelDuties)
        {
            var dataEditPopup = Instantiate(dataEditPopupPrefab, transform).GetComponent<DataEditPopup>();

            if (null != dataEditPopup)
            {
                foreach (var cell in info.channelInfos)
                {
                    if (cell.channelIndex == index)
                    {
                        dataEditPopup.StartEdit(new AChannelInfo(cell), SaveEditedData, new List<AChannelInfo>(info.channelInfos), excelDuties);
                        break;
                    }
                }
            }
        }

        private void SaveEditedData(AChannelInfo newInfo)
        {
            //Verification
            _Verification();

            //Combine & Save
            _CombineAndSave();

            void _Verification()
            {
                //About Index 0
                {
                    if (null != newInfo)
                    {
                        //Fade on 0 Index
                        if (newInfo.sectionInfos.Count > 2)
                        {
                            var first = newInfo.sectionInfos[0];
                            var second = newInfo.sectionInfos[1];

                            if (first.level != second.level)
                            {
                                Provider.Instance.ShowErrorPopup(Definitions.ERROR_EXIST_CHANNEL_STARTED_FADE_ON_ZERO_FRAME);
                                return;
                            }
                        }
                    }
                }
            }

            void _CombineAndSave()
            {
                if (null != info)
                {
                    for (int i = 0; i < info.channelInfos.Count; i++)
                    {
                        if (info.channelInfos[i].channelIndex == newInfo.channelIndex)
                        {
                            info.channelInfos[i] = newInfo;
                            break;
                        }
                    }

                    info.saveCallback?.Invoke(info.channelInfos);
                    StartCoroutine(DelayApply());
                }
            }
        }

        IEnumerator DelayApply()
        {
            yield return new WaitForSeconds(1f);
            InitGraphBuildersController(info.channelInfos, info.savedDuties);
        }

        #endregion
    }
}
