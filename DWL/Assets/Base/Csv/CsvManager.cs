/*
 * Copyright (c) 2016 NEOFECT Co., Ltd.
 * 
 * All rights reserved. Used by permission.
 * 
 * @author bada8130@neofect.com
*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neofect.BodyChecker.Language
{
    public class JointSetting
    {
        public float max_angle;
        public bool is_minus_allowed;
        public float protractorStartAngle;
    }

    public class CsvManager : AbstractSingleton<CsvManager>
    {
        private Dictionary<int, float> pressureDic = new Dictionary<int, float>();
        public const string PRESSURE_FILE_NAME = "pressure";

        private Dictionary<string, JointSetting> jointSettingDic = new Dictionary<string, JointSetting>();
        public const string JOINT_SETTING_NAME = "joint_setting";

        public void Initialize()
        {
            LoadPressureFile();
            LoadJointMaxAngleFile();
        }

        #region pressure
        private void LoadPressureFile()
        {
            pressureDic.Clear();

            TextAsset csv = Resources.Load<TextAsset>(PRESSURE_FILE_NAME);
            string localeText = (csv == null) ? string.Empty : csv.text;

            if (localeText.Length > 0)
            {
                var csvReader = new CsvParser();
                csvReader.ReadStream(localeText);

                for (int i = 1; i < csvReader.GetRowCount(); i++)
                {
                    var rowList = csvReader.GetRow(i);
                    var key = int.Parse(rowList[0]);
                    var value = float.Parse(rowList[3]);
                    pressureDic.Add(key, value);
                }
            }
            else
            {
                NDebug.Log($"no {PRESSURE_FILE_NAME} file!");
            }
        }

        public float GetPressure(int key)
        {
            if (pressureDic.ContainsKey(key))
                return pressureDic[key];
            else
                return 5f;
        }
        #endregion

        #region joint_max_angle
        private void LoadJointMaxAngleFile()
        {
            jointSettingDic.Clear();

            TextAsset csv = Resources.Load<TextAsset>(JOINT_SETTING_NAME);
            //TextAsset csv = Resources.Load<TextAsset>("CSV/joint_setting");
            string localeText = (csv == null) ? string.Empty : csv.text;

            if (localeText.Length > 0)
            {
                var csvReader = new CsvParser();
                csvReader.ReadStream(localeText);

                for (int i = 1; i < csvReader.GetRowCount(); i++)
                {
                    var rowList = csvReader.GetRow(i);
                    var key = rowList[0];
                    var jma = new JointSetting();
                    jma.max_angle = float.Parse(rowList[1]);
                    jma.is_minus_allowed = bool.Parse(rowList[2]);
                    jma.protractorStartAngle = float.Parse(rowList[4]);
                    jointSettingDic.Add(key, jma);
                }
            }
            else
            {
                NDebug.Log($"no {JOINT_SETTING_NAME} file!");
            }
        }

        public JointSetting GetJointSetting(string key)
        {
            if (jointSettingDic.ContainsKey(key))
                return jointSettingDic[key];
            else
                return null;
        }

        public float GetJointSettingMaxAngle(string key)
        {
            if (jointSettingDic.ContainsKey(key))
                return jointSettingDic[key].max_angle;
            else
                return 0f;
        }

        #endregion

        public string GetGrade(int score)
        {
            if (0 <= score && score <= 10)
                return "A";
            else if (11 <= score && score <= 20)
                return "B";
            else if (21 <= score && score <= 30)
                return "C";
            else if (31 <= score && score <= 40)
                return "D";
            else if (41 <= score && score <= 50)
                return "E";
            else
                return "F";
        }
    }
}