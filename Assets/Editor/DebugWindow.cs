using UnityEngine;
using UnityEditor;

public class DebugWindow : EditorWindow
{
    [MenuItem("Window/DebugWindow")]
    public static void ShowWindow()
    {
        // 창을 띄운다
        GetWindow<DebugWindow>("MyDebugWindow");
    }
    private void OnGUI()
    {
        GUILayout.Label("이건 커스텀 에디터 창입니다!", EditorStyles.boldLabel);

        GUILayout.Label("정수 값 입력", EditorStyles.boldLabel);


        if (GUILayout.Button("유닛뽑기 버튼"))
        {
            Test.Instance.EditFunctionSetUnit();
        }

        if (GUILayout.Button("덱 추가 버튼"))
        {
            Test.Instance.EditFunctionSetDeck();
        }


        if (GUILayout.Button("덱 데이터 출력 버튼"))
        {
            Test.Instance.EditFuctionDeckData();
        }

        if (GUILayout.Button("덱빌드 씬 로드 버튼"))
        {
            Test.Instance.SetDeckBuild();
        }


        if (GUILayout.Button("로드 스테이지 데이터 버튼"))
        {
            Test.Instance.LoadStageData();
        }

        if (GUILayout.Button("스테이지 데이터 세팅 버튼"))
        {
            Test.Instance.SetStageData();
        }

        if (GUILayout.Button("데이터 저장 버튼"))
        {
            Test.Instance.SaveTest();
        }
        if (GUILayout.Button("데이터 로드 버튼"))
        {
            Test.Instance.LoadTest();
        }

        if (GUILayout.Button("퀘스트 데이터 로드 버튼"))
        {
            Test.Instance.LoadQuest();
        }

        if (GUILayout.Button("퀘스트 데이터 세팅 버튼"))
        {
            Test.Instance.SetQuest();
        }

        if (GUILayout.Button("퀘스트 유아이세팅 버튼"))
        {
            Test.Instance.SetQuestUI();
        }






    }
}
