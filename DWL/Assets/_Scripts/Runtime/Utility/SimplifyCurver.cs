using System;
using System.Collections.Generic;

public class SimplifyCurver
{
    public List<int> ExcuteSimplify(List<float> original, float radicalThreshold, float gradualThreshold, int minimumFadeCount, List<int> basis)
    {
        List<int> simplified = new List<int>() { 0 };
        MarkChangingValues();
        ReplaceShortGradualChanges();
        ConvertRadicalValues();
        ConvertDuties();
        ChangeLastMaintainRegion();

        return simplified;

        //이전 값과 현재값을 보고 비약(-1), 점진(-2), 유지(인접한basis로치환), 작업을 수행한다.
        void MarkChangingValues()
        {
            if (original.Count < 2) return; // 원본 데이터가 2개 미만인 경우 처리하지 않음

            float prePreviousValue = original[0]; // 이전의 이전값
            float previousValue = original[0]; // 이전값
            for (int i = 1; i < original.Count; i++) // 0번째 인덱스는 이미 초기값으로 설정했으므로 1부터 시작
            {
                float currentValue = original[i];
                float currentDiff = Math.Abs(currentValue - previousValue) + Definitions.EPSILON;

                //계속 증가 또는 감소 중인지 확인한다. ex) 255 -> 3 -> 255가 너무 빠르면, 3이 씹힐 수 있음.
                bool isIncreasing = previousValue > prePreviousValue && currentValue > previousValue;
                bool isDecreasing = previousValue < prePreviousValue && currentValue < previousValue;

                if (currentDiff > radicalThreshold) // 비약적 변화
                {
                    simplified.Add(isIncreasing || isDecreasing ? -1 : FindClosestBasisValue((int)currentValue, basis));
                    //simplified.Add(-1);
                }
                else if (currentDiff >= gradualThreshold && currentDiff <= radicalThreshold) // 점진적 변화
                {
                    simplified.Add(isIncreasing || isDecreasing ? -2 : FindClosestBasisValue((int)currentValue, basis));
                    //simplified.Add(-2);
                }
                else // 유지
                {
                    simplified.Add(FindClosestBasisValue((int)currentValue, basis));
                }

                // 값을 업데이트
                prePreviousValue = previousValue;
                previousValue = currentValue;
            }
        }
        //인접한 값을 찾는다.
        int FindClosestBasisValue(int value, List<int> basis)
        {
            int closest = basis[0];
            int minDifference = Math.Abs(value - closest);

            foreach (var basisValue in basis)
            {
                int currentDifference = Math.Abs(value - basisValue);
                if (currentDifference < minDifference)
                {
                    closest = basisValue;
                    minDifference = currentDifference;
                }
            }

            return closest;
        }
        //minimumFadeCount 보다 짧은 점진변화를 제거한다.
        void ReplaceShortGradualChanges()
        {
            if (minimumFadeCount == 0) return;

            int startIndex = -1;
            for (int i = 0; i < simplified.Count; i++)
            {
                if (simplified[i] == -2)
                {
                    if (startIndex == -1)
                    {
                        startIndex = i; // -2 구간의 시작 인덱스 저장
                    }
                }
                else
                {
                    if (startIndex != -1)
                    {
                        // -2 구간의 끝을 찾음
                        int endIndex = i - 1;
                        if (endIndex - startIndex + 1 <= minimumFadeCount)
                        {
                            // -2 구간의 길이가 minimumFadeCount 이하일 경우
                            if (startIndex > 0 && i < simplified.Count && simplified[startIndex - 1] == simplified[i])
                            {
                                // 양 끝 값이 같은 경우, 구간을 시작 전 인덱스의 값으로 변경
                                for (int j = startIndex; j <= endIndex; j++)
                                {
                                    simplified[j] = simplified[startIndex - 1];
                                }
                            }
                        }
                        startIndex = -1; // 다음 -2 구간을 찾기 위해 초기화
                    }
                }
            }

            // 리스트 끝 처리
            if (startIndex != -1 && simplified.Count - startIndex < minimumFadeCount && simplified[startIndex - 1] == simplified[simplified.Count - 1])
            {
                for (int j = startIndex; j < simplified.Count; j++)
                {
                    simplified[j] = simplified[startIndex - 1];
                }
            }
        }
        //비약적변화를 앞의 값으로 변환한다.
        void ConvertRadicalValues()
        {
            int previousValue = simplified[0] != -1 && simplified[0] != -2 ? simplified[0] : 0;

            for (int i = 0; i < simplified.Count; i++)
            {
                if (simplified[i] == -1 && previousValue != -2)
                {
                    simplified[i] = previousValue;
                }
                else if (simplified[i] == -2)
                {
                    previousValue = -2;
                }
                else
                {
                    previousValue = simplified[i];
                }
            }
        }
        //간소화된 데이터들을 basis에 맞춰서 변경한다.
        void ConvertDuties()
        {
            for (int i = 0; i < simplified.Count; i++)
            {
                if (simplified[i] == -2)
                {
                    simplified[i] = Definitions.FADE_VALUE;
                }
                else
                {
                    simplified[i] = Provider.Instance.GetDuty().GetDutyIndex(FindClosestBasisValue(simplified[i], basis));
                }
            }
        }

        // 마지막 유지구간을 최소 4개로 유지될 수 있도록 한다.
        void ChangeLastMaintainRegion()
        {
            if (simplified.Count < 2) return;

            int lastValue = -1;
            lastValue = simplified[simplified.Count - 1];

            if (lastValue != -1)
                return;

            if(lastValue != simplified[simplified.Count - 2] ||
                lastValue != simplified[simplified.Count - 2] ||
                lastValue != simplified[simplified.Count - 3]
                )
            {
                Provider.Instance.ShowErrorPopup(Definitions.LAST_DUTY_MAINTAIN_REGION_ISNT_SO_SMALL);

                simplified[simplified.Count - 1] = lastValue;
                simplified[simplified.Count - 2] = lastValue;
                simplified[simplified.Count - 3] = lastValue;
                simplified[simplified.Count - 4] = lastValue;

            }
        }
    }
}


