# Tell The Story

[한국어](#한국어) | [English](#english)

[플레이 영상](https://youtu.be/nVsos3e-BUQ) | [프로젝트 PDF](https://github.com/user-attachments/files/25697028/Tell.The.Story.pdf)

## 한국어

플레이어가 직접 대사를 연기하며 NPC와 이야기를 이어가는 스토리텔링 시뮬레이션 게임입니다. 컴투스 글로벌 게임개발 공모전 `컴:온 2024` 출품작입니다.

### 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발 기간 | 2024-12-14 - 2024-12-30 |
| 리팩터링 기간 | 2025-08-10 - 2025-08-11 |
| 인원 | 프로그래머 2인 |
| 엔진 | Unity 2022.3.28f1 |
| 플랫폼 | Windows |

### 담당 작업

- 로비 구성과 NPC 상호작용
- UI 흐름, 캐릭터 배치, 스프라이트 애니메이션
- 2D 캐릭터의 3D 공간 빌보딩과 카메라 연출
- 씬의 TMP 폰트를 일괄 교체하는 에디터 도구
- 게임 화면, 캐릭터, 연출, 밸런싱 기획을 실제 게임 로직과 연결

STT/TTS는 프로젝트 기술로 사용됐지만 제 담당 구현에는 포함하지 않습니다.

### 핵심 구현과 선택

- 대화 중 이동 입력이 섞이지 않도록 로비 상호작용 상태를 분리했습니다.
- 특정 타이핑 코루틴만 중지해 다른 연출이 함께 중단되는 문제를 막았습니다.
- 방향 입력을 8방향 스프라이트 상태로 변환해 2D 캐릭터가 3D 로비에서 자연스럽게 보이도록 했습니다.
- 반복적인 폰트 교체를 에디터 버튼 하나로 처리해 UI 검수 시간을 줄였습니다.

### 기술 스택

`Unity` `C#` `JSON` `Cinemachine` `TextMesh Pro` `NAVER CLOVA Speech`

### 에셋 출처

- 타이틀 로고와 메인 배경: OpenAI DALL-E, Microsoft Copilot로 생성
- NPC 음성: VOLI
- 8방향 스프라이트 도구: 프로젝트의 `Assets/External Assets/EDSS` 원본 고지 적용
- 그 외 외부 에셋은 포함된 원본 문서와 라이선스 적용

### 보안 및 실행

NAVER CLOVA 자격증명은 저장소에 포함하지 않습니다. 로컬 실행 시 환경변수 `NAVER_CLOVA_CLIENT_ID`, `NAVER_CLOVA_CLIENT_SECRET`을 설정해야 합니다.

### 배운 점

짧은 공모전 일정에서는 새로운 시스템을 많이 만드는 것보다 로비, 캐릭터, UI, 연출을 하나의 플레이 흐름으로 완성하는 일이 더 중요했습니다.

## English

Tell The Story is a storytelling simulation where the player performs dialogue and continues scenes with NPCs. It was submitted to the Com2uS `COM:ON 2024` global game-development competition.

### Project

- Development: 2024-12-14 - 2024-12-30
- Refactoring: 2025-08-10 - 2025-08-11
- Team: Two programmers
- Engine: Unity 2022.3.28f1

### My Contribution

- Lobby construction and NPC interaction
- UI flow, character setup, sprite animation, camera presentation, and billboard rendering
- A Unity editor tool for replacing TMP fonts across a scene
- Visual direction and balancing implemented through game logic

STT and TTS were project technologies, not my implementation responsibility.

### Implementation Decisions

- Lobby interaction states prevent movement input from leaking into dialogue.
- Only the active typing coroutine is stopped so unrelated presentation does not halt.
- Eight-direction input selects sprite states that read naturally in the 3D lobby.
- One editor action replaces scene-wide TMP fonts to shorten UI review cycles.

### Stack, Assets, and Security

`Unity` `C#` `JSON` `Cinemachine` `TextMesh Pro` `NAVER CLOVA Speech`

- Title logo and main background: generated with OpenAI DALL-E and Microsoft Copilot
- NPC voice: VOLI
- Eight-direction sprite tooling: original notice under `Assets/External Assets/EDSS`
- Other third-party material remains subject to its included license.

NAVER CLOVA credentials are not stored in the repository. Local runs require `NAVER_CLOVA_CLIENT_ID` and `NAVER_CLOVA_CLIENT_SECRET` environment variables.

### Refactoring

- Isolated the active typing coroutine instead of stopping every coroutine.
- Clarified lobby interaction states and input ownership.
- Removed embedded service credentials from source code.

### Lessons

The competition schedule made integration more valuable than feature count: the lobby, characters, UI, and presentation had to work as one complete player flow.
