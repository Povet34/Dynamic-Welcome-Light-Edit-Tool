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

        //���� ���� ���簪�� ���� ���(-1), ����(-2), ����(������basis��ġȯ), �۾��� �����Ѵ�.
        void MarkChangingValues()
        {
            if (original.Count < 2) return; // ���� �����Ͱ� 2�� �̸��� ��� ó������ ����

            float prePreviousValue = original[0]; // ������ ������
            float previousValue = original[0]; // ������
            for (int i = 1; i < original.Count; i++) // 0��° �ε����� �̹� �ʱⰪ���� ���������Ƿ� 1���� ����
            {
                float currentValue = original[i];
                float currentDiff = Math.Abs(currentValue - previousValue) + Definitions.EPSILON;

                //��� ���� �Ǵ� ���� ������ Ȯ���Ѵ�. ex) 255 -> 3 -> 255�� �ʹ� ������, 3�� ���� �� ����.
                bool isIncreasing = previousValue > prePreviousValue && currentValue > previousValue;
                bool isDecreasing = previousValue < prePreviousValue && currentValue < previousValue;

                if (currentDiff > radicalThreshold) // ����� ��ȭ
                {
                    simplified.Add(isIncreasing || isDecreasing ? -1 : FindClosestBasisValue((int)currentValue, basis));
                    //simplified.Add(-1);
                }
                else if (currentDiff >= gradualThreshold && currentDiff <= radicalThreshold) // ������ ��ȭ
                {
                    simplified.Add(isIncreasing || isDecreasing ? -2 : FindClosestBasisValue((int)currentValue, basis));
                    //simplified.Add(-2);
                }
                else // ����
                {
                    simplified.Add(FindClosestBasisValue((int)currentValue, basis));
                }

                // ���� ������Ʈ
                prePreviousValue = previousValue;
                previousValue = currentValue;
            }
        }
        //������ ���� ã�´�.
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
        //minimumFadeCount ���� ª�� ������ȭ�� �����Ѵ�.
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
                        startIndex = i; // -2 ������ ���� �ε��� ����
                    }
                }
                else
                {
                    if (startIndex != -1)
                    {
                        // -2 ������ ���� ã��
                        int endIndex = i - 1;
                        if (endIndex - startIndex + 1 <= minimumFadeCount)
                        {
                            // -2 ������ ���̰� minimumFadeCount ������ ���
                            if (startIndex > 0 && i < simplified.Count && simplified[startIndex - 1] == simplified[i])
                            {
                                // �� �� ���� ���� ���, ������ ���� �� �ε����� ������ ����
                                for (int j = startIndex; j <= endIndex; j++)
                                {
                                    simplified[j] = simplified[startIndex - 1];
                                }
                            }
                        }
                        startIndex = -1; // ���� -2 ������ ã�� ���� �ʱ�ȭ
                    }
                }
            }

            // ����Ʈ �� ó��
            if (startIndex != -1 && simplified.Count - startIndex < minimumFadeCount && simplified[startIndex - 1] == simplified[simplified.Count - 1])
            {
                for (int j = startIndex; j < simplified.Count; j++)
                {
                    simplified[j] = simplified[startIndex - 1];
                }
            }
        }
        //�������ȭ�� ���� ������ ��ȯ�Ѵ�.
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
        //����ȭ�� �����͵��� basis�� ���缭 �����Ѵ�.
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

        // ������ ���������� �ּ� 4���� ������ �� �ֵ��� �Ѵ�.
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


