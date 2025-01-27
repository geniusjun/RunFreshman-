using UnityEngine;
using BackEnd;

public class RankRegister : MonoBehaviour
{
    public void UpdateMyBestRankData(int newScore, System.Action<bool> callback)
    {
        Backend.URank.User.GetMyRank(Constants.RANK_UUID, rankcallback =>
        {
            if (rankcallback.IsSuccess())
            {
                try
                {
                    LitJson.JsonData rankDataJson = rankcallback.FlattenRows();

                    //받아온 데이터의 개수가 0이면 데이터가 없는것.
                    if (rankDataJson.Count <= 0)
                    {
                        Debug.LogWarning("데이터가 존재하지 않습니다");
                    }
                    else
                    {
                        //랭킹을 등록할 때는 컬럼명을 "dailyBestScore"로 저장했지만
                        //랭킹을 불러올 때는 컬럼명이 "score"로 통일되어 있다.

                        //추가로 등록한 항목은 컬럼명을 그대로 사용
                        int bestScore = int.Parse(rankDataJson[0]["score"].ToString());

                        //현재 점수가 최고점수보다 높으면 
                        if (newScore > bestScore)
                        {
                            //현재 점수를 새로운 최고점수로 등록하고, 랭킹에 등록
                            UpdateMyRankData(newScore, callback); 

                            Debug.Log($"최고 점수 갱신 {bestScore} -> {newScore}");
                        }
                        else
                        {
                            Debug.Log("현재 점수가 최고 점수보다 낮습니다.");
                            callback?.Invoke(false); // 최고 점수 갱신 안 됨
                        }
                    }
                }
                // Json 데이터 파싱 실패
                catch (System.Exception e)
                {
                    //try-catch 에러 출력
                    Debug.LogError(e);
                    callback?.Invoke(false); // 에러 발생 시 갱신 실패로 처리
                }
            }
            else
            {
                // 자신의 랭킹 정보가 존재하지 않을 때는 현재 점수를 새로운 랭킹으로 등록
                if (rankcallback.GetMessage().Contains("userRank"))
                {
                    UpdateMyRankData(newScore, callback);

                    Debug.Log($"새로운 랭킹 데이터 생성 및 등록 : {rankcallback}");
                }
            }
        });
    }

    private void UpdateMyRankData(int newScore, System.Action<bool> callback)
    {
        string rowInDate = string.Empty;

        //랭킹 데이터를 업데이트하려면 게임 데이터에서 사용하는 데이터의 InDate값이 필요
        Backend.GameData.GetMyData(Constants.USER_DATA_TABLE, new Where(), datacallback =>
        {
            if(!datacallback.IsSuccess())
            {
                Debug.LogError($"데이터 조회 중 문제가 발생했습니다. {datacallback}");
                callback?.Invoke(false); // 데이터 조회 실패
                return;
            }

            Debug.Log($"데이터 조회에 성공했습니다. {callback}");

            if(datacallback.FlattenRows().Count > 0)
            {
                rowInDate = datacallback.FlattenRows()[0]["inDate"].ToString();
            }
            else
            {
                Debug.LogError("데이터가 존재하지 않습니다");
                callback?.Invoke(false); // 데이터 조회 실패
                return;
            }

            Param param = new Param()
            {
                {"score", newScore}
            };
            Backend.URank.User.UpdateUserScore(Constants.RANK_UUID, Constants.USER_DATA_TABLE, rowInDate, param, updatecallback =>
            {
                if(updatecallback.IsSuccess() )
                {
                    Debug.Log($"랭킹 등록에 성공했습니다. : {callback}");
                    callback?.Invoke(true); // 성공
                }
                else
                {
                    Debug.LogError($"랭킹 등록에 실패했습니다. : {callback}");
                    callback?.Invoke(false);
                }
            });
        });
    }
}
