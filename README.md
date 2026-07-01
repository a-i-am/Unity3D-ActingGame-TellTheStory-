# Tell The Story

[플레이 영상](https://youtu.be/nVsos3e-BUQ) | [프로젝트 PDF](docs/TellTheStory.pdf)

- 플레이어가 직접 대사를 선택하고 연기하며 NPC와 이야기를 만들어가는 스토리텔링 시뮬레이션 게임입니다. 
- 컴투스 글로벌 게임개발 공모전 `컴:온 2024` 출품작입니다.

README 업데이트: 2026-07-01

> 📷 **영상 및 이미지**
> *(여기에 영상 또는 이미지 추가 예정)*

### 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발 기간 | 2024-12-14 - 2024-12-30 |
| 리팩터링 이력 | 1차: 2025-08-10 - 2025-08-11 |
| 인원 | 프로그래머 2인 |
| 엔진 | Unity 2022.3.28f1 |
| 플랫폼 | Windows |

### 기술 스택
<p>
  <img src="https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white"/>
  <img src="https://img.shields.io/badge/Cinemachine-000000?style=flat-square"/>
  <img src="https://img.shields.io/badge/TextMesh Pro-000000?style=flat-square"/>
</p>

### API 연동 및 라이브러리
- **NAVER CLOVA Speech API** 연동 (STT/TTS 음성 인식 및 합성)
- **OpenAI DALL-E / Microsoft Copilot** 활용 메인 에셋 생성 연동

### 프로젝트 구조
```text
(프로젝트 구조도 추가 예정)
```

### 플레이 및 조작 방법
*(NPC 대화 상호작용 방식, 화면 조작 및 진행 방법 안내 작성 예정)*

### 담당 작업

- 로비 상호작용 FSM 및 NPC 대화 흐름 구성
- UI 플로우 제어, 캐릭터 배치, 8방향 스프라이트 애니메이션
- 3D 공간 내 2D 빌보드 캐릭터와 Cinemachine 카메라 연출
- 프로젝트 전체 씬의 TMP 폰트를 버튼 하나로 일괄 교체하는 커스텀 에디터 툴 제작
- 텍스트 연출, 캐릭터 비주얼, 밸런싱 기획 데이터 동기화

### 보안 및 실행

NAVER CLOVA 자격증명은 소스코드에 포함하지 않습니다. 로컬 환경 테스트 시 시스템 환경 변수에 `NAVER_CLOVA_CLIENT_ID`, `NAVER_CLOVA_CLIENT_SECRET`을 직접 등록하여 실행해야 합니다.

### 업데이트 계획

- 필요 시 로비 및 UI 추가 캡처 보강
- 사용한 에셋 출처 표기 예정
