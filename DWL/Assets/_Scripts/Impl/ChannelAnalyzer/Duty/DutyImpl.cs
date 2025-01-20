using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChannelAnalyzers
{
    public class DutyImpl : IDuty
    {
        List<int> _levels;
        public List<int> levels => _levels;

        List<int> _prelevels;
        public List<int> prelevels => _prelevels;

        public DutyImpl() { }
        public DutyImpl(List<int> levels)
        {
            SetLevels(levels);
        }

        public void SetLevels(List<int> levels)
        {
            if (null == _prelevels)
                _prelevels = new List<int>(levels);
            else
                _prelevels = new List<int>(_levels);

            _levels = levels;
        }

        public bool TryChangeLevel(int newLevel, int currentLevel)
        {   
            int currentIndex = -1;
            for (int i = 0; i < _levels.Count; i++)
            {
                if (_levels[i] == currentLevel)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1)
            {
                return false;
            }

            int prevIndex = currentIndex - 1;
            int nextIndex = currentIndex + 1;
            int prevLevel = (prevIndex >= 0) ? _levels[prevIndex] : 0;
            int nextLevel = (nextIndex < _levels.Count) ? _levels[nextIndex] : 100;

            if (newLevel >= prevLevel && newLevel <= nextLevel)
            {
                _prelevels[currentIndex] = _levels[currentIndex];
                _levels[currentIndex] = newLevel;
                return true;
            }
                
            return false;
        }

        public int GetAdjcentLevel(int value)
        {
            return ClosestFinder.FindClosestPoint(value, _levels);
        }

        public int GetPanelRatioLevel(int extendHeight, List<int> dutiesByPanelRatios)
        {
            if (dutiesByPanelRatios == null || dutiesByPanelRatios.Count == 0)
            {
                throw new ArgumentException("dutiesByPanelRatios is null or empty.");
            }

            int closestValue = dutiesByPanelRatios[0];
            int closestIndex = 0;
            int minDifference = Mathf.Abs(extendHeight - closestValue);

            for (int i = 1; i < dutiesByPanelRatios.Count; i++)
            {
                int currentDifference = Mathf.Abs(extendHeight - dutiesByPanelRatios[i]);

                if (currentDifference < minDifference)
                {
                    closestValue = dutiesByPanelRatios[i];
                    closestIndex = i;
                    minDifference = currentDifference;
                }
            }

            return _levels[closestIndex];
        }

        public int GetlevelByIndex(int dutyIdx)
        {
            return _levels[dutyIdx];
        }

        public int GetPreLevlByIndex(int value)
        {
            return _prelevels[value];
        }

        public int GetDutyIndex(int value)
        {
            var level = GetAdjcentLevel(value);
            return levels.IndexOf(level);
        }
    }
}
