# Dynamic Welcome Light Editor
### Summation
- SL사의 외주
- 주어진 영상의 각 Pixel들의 밝기 현황을 추출하여, 데이터로 저장.
- 연속적인 밝기 Flow를 이산화하여 [유지, FadeIn, FadeOut, Flicker] 로 구분하여 밝기 정보를 간소화
- 간소화된 밝기 Flow를 그래프로 시각화
- 시각화된 그래프에서 직접 밝기의 Flow를 수정
- 수정된 그래프를 바탕으로 새로운 밝기 flow 영상을 제작


---
## Feature 

- OpenCV를 이용한 동영상 내 밝기 변화 Pixel

https://github.com/user-attachments/assets/1c61547d-c8c6-480b-b2c9-0f96d2993996

-  추출할 Pixel 수동 지정하기
  
https://github.com/user-attachments/assets/2760c6c3-250c-473e-8354-cf643caf8332

- 각 Pixel들의 밝기 정보를 Excel에 Export
  ![image](https://github.com/user-attachments/assets/2ab3edef-8219-4fb8-bc6b-09a5f80a1dc3)

- 추출한 Pixel들을 시간에 따른 그래프로 시각화
  ![image](https://github.com/user-attachments/assets/d6fac28b-df95-46b9-b17a-112b8be6a438)

- 그래프의 특정 Pixel의 흐름을 수정하기
  
https://github.com/user-attachments/assets/99258506-9556-4570-9ccb-dea0b2454b03

- 변경된 Pixel의 흐름을 영상으로 다시 저장
  
https://github.com/user-attachments/assets/04ba9ba6-c1bb-4cc2-b803-a93ea5b4a9e6

