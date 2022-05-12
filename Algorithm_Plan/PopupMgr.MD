## Popup Mgr

1. 흐름도
    - 팝업 호출
    - 팝업 오브젝트가 씬에 존재하는지 체크
        - 없을 경우 씬에 새로 생성하고, 이 오브젝트를 지정한다.
        - 있을 경우 중복 생성 가능한 타입인지 체크
            - 가능할 경우, 씬에 새로 생성하고, 이 오브젝트를 지정한다.
            - 불가능할 경우, 기존 오브젝트를 딕셔너리에서 삭제한 후, 지정한다.
    - 지정된 오브젝트를 딕셔너리에 추가한다.
    - 지정된 오브젝트에 딕셔너리 순번에 비례하는 sortingOrder 값을 할당한다.
    - 삭제될 경우 리스트에서 제거된다.
<br><br>

2. 주요 기능
    - 변수 목록
        - POPUP Enum 키로 Popup 프리펩 정보를 찾을 수 있는 딕셔너리.
            - Dictionary<POPUP, string> popup_pref_list;
        - 현재 팝업 딕셔너리
            - Dictionary<POPUP, T> popup_active_list;
        - UI 타입별 캔버스 딕셔너리
            - Dictionary<UI, canvas_mgr> ui_list;
    - 함수 목록
        - popup_pref_list를 초기화하는 함수
        - 팝업을 생성 및 참조하는 비동기 함수



Apple 베타 빌드 심사 절차 등록 및 승인 완료
IOS 결제 관련 테스트 및 결제 인식 오류 관련 Gamebase 문의
리소스 일부 최적화 및 파일 정리 작업
게임 팝업 메니지먼트 구조화 관련 연구