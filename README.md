# Project_MMScript
## 1. 프로젝트 개요

- **프로젝트명**: TeamProject (Unity C# 기반)
- **역할**: 덱빌딩, 스테이지, 퀘스트, 저장 및 불러오기 개발
- **핵심 목표**: 게임 플레이 흐름을 데이터 기반으로 제어하고, 클라이언트/서버 시간 동기화 및 Firebase 연동을 통한 저장/불러오기 구현

---

## 2. 주요 구현 기능

### 🔹 데이터 관리

- **SkillData / SkillDataManager**
    - CSV 파일 파싱 → 스킬 데이터 로드 및 Dictionary<int, SkillData> 캐싱
    - 정규식을 이용한 CSV 필드 파싱 → 따옴표 처리, 줄바꿈 예외까지 대응
    - ✔️ **자료구조 활용**: `Dictionary`를 통한 O(1) 조회 성능 확보
    - ✔️ **객체지향 설계**: SkillData 클래스로 데이터 모델을 정의하고, Manager에서만 로직 처리 (SRP 준수)
- **StageDataManager**StageDataManager
    - 스테이지 데이터 CSV 로드 및 분류 (일반/타워/골드/튜토리얼)
    - ✔️ **Dictionary 분리 저장** → 스테이지 타입별 효율적 접근

---

### 🔹 UI & 게임 진행 관리

- **StageManager / StageNode**StageManagerStageNode
    - 챕터/스테이지 전환, UI 버튼 바인딩, 클리어 여부 표시
    - **LINQ 활용**: `Where`, `OrderBy`, `Any`, `All`로 컬렉션 필터링 및 정렬
    - **게임 플레이 제약 로직**: 이전 스테이지 클리어 여부 확인 후 다음 스테이지 입장 가능

---

### 🔹 퀘스트 & 서버 동기화

- **QuestManager**QuestManager
    - Firebase 서버 시간 기반 **일일/주간 퀘스트 리셋** 구현
    - `async/await` 비동기 호출로 서버 시간 가져오기 + 퀘스트 초기화 진행
    - **이벤트 기반 구조**: `QuestEvent.OnLogin`, `QuestEvent.OnTowerClear` 등 옵저버 패턴 사용
- **ServerTime**ServerTime
    - Firebase Realtime DB `ServerValue.Timestamp`를 이용해 UTC 시간 확보
    - UTC → KST 변환, 자정 기준 계산 함수 제공

---

### 🔹 데이터 저장 & 로딩

- **SaveLoadManager**SaveLoadManager
    - 로컬(JSON 파일) + Firebase 서버 데이터 동기화
    - **동시성 처리**: 로컬/서버 데이터 비교 후 최신 데이터 채택 (lastSaveTime 기반)
    - 제네릭 + 인터페이스(`SaveTime`) 활용 → 범용 저장 구조 구현
- **SaveHandler**SaveHandler
    - `OnApplicationPause`, `OnApplicationQuit` 훅 → 자동 저장 구현

---

### 🔹 테스트 & 툴링

- **Test.cs**Test
    - 개발 과정에서 `PlayerDataManager`, `DeckManager`, `StageManager` 호출을 자동화하는 유틸
    - 반복적인 수동 입력 없이 데이터/덱/스테이지 테스트 가능

---

## 3. C# 역량 강조 포인트

1. **객체지향 설계 (OOP)**
    - 데이터 모델(`SkillData`, `StageData`)과 로직(`Manager` 클래스) 분리 → SRP 준수
    - 싱글톤 패턴(`StageManager`, `QuestManager`, `SkillDataManager`) 사용 → 전역 접근 관리
2. **자료구조 활용**
    - `Dictionary`를 통한 빠른 조회
    - `List` + LINQ로 필터링, 정렬, 조건 검색
3. **비동기 처리**
    - Unity Coroutine + C# `async/await` 모두 활용
    - Firebase 연동 시 네트워크 지연 고려 → `Task`, `await` 기반 예외 처리
4. **디자인 패턴**
    - 싱글톤 (게임 매니저, 데이터 매니저)
    - 옵저버 패턴 (이벤트 기반 퀘스트 진행)
    - 상태 관리 (스테이지 클리어 여부 → 다음 스테이지 잠금/해제)
5. **실제 서비스 고려**
    - Firebase Realtime DB 연동
    - 서버 시간 기준 리셋 (클라이언트 시간 조작 방지)
    - 앱 종료/일시정지 시 자동 저장
