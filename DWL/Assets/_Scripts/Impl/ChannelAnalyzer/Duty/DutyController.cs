using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChannelAnalyzers
{ 
    public class DutyController : MonoBehaviour
    {
        [SerializeField] private GameObject dutyInputPrefab;
        private Action<int> onRefreshDuties;
        private List<GameObject> dutyGos = new List<GameObject>();

        public void Init(Action<int> onRefreshDuties)
        {
            foreach(var go in dutyGos)
                Destroy(go);

            dutyGos.Clear();

            var duty = Provider.Instance.GetDuty();
            if (duty.levels.Count > 1)
            {
                this.onRefreshDuties = onRefreshDuties;

                for (int i = duty.levels.Count - 1;  i >= 0; i--)
                {
                    var go = Instantiate(dutyInputPrefab, transform);
                    var ip = go.GetComponent<TMP_InputField>();
                    if (ip)
                    {
                        int index = i;
                        ip.text = duty.levels[i].ToString();
                        ip.onSubmit.AddListener(delegate (string value) { OnEndEdit(index, value); });
                    }

                    dutyGos.Add(go);
                }
            }
        }

        private void OnEndEdit(int index, string data)
        {
            var newlevel = Convert.ToInt32(data);
            var duty = Provider.Instance.GetDuty();

            int prelevel = duty.GetlevelByIndex(index);
            duty.TryChangeLevel(newlevel, prelevel);

            onRefreshDuties?.Invoke(index);
        }
    }
}

