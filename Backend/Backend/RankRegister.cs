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

                    //�޾ƿ� �������� ������ 0�̸� �����Ͱ� ���°�.
                    if (rankDataJson.Count <= 0)
                    {
                        Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�");
                    }
                    else
                    {
                        //��ŷ�� ����� ���� �÷����� "dailyBestScore"�� ����������
                        //��ŷ�� �ҷ��� ���� �÷����� "score"�� ���ϵǾ� �ִ�.

                        //�߰��� ����� �׸��� �÷����� �״�� ���
                        int bestScore = int.Parse(rankDataJson[0]["score"].ToString());

                        //���� ������ �ְ��������� ������ 
                        if (newScore > bestScore)
                        {
                            //���� ������ ���ο� �ְ������� ����ϰ�, ��ŷ�� ���
                            UpdateMyRankData(newScore, callback); 

                            Debug.Log($"�ְ� ���� ���� {bestScore} -> {newScore}");
                        }
                        else
                        {
                            Debug.Log("���� ������ �ְ� �������� �����ϴ�.");
                            callback?.Invoke(false); // �ְ� ���� ���� �� ��
                        }
                    }
                }
                // Json ������ �Ľ� ����
                catch (System.Exception e)
                {
                    //try-catch ���� ���
                    Debug.LogError(e);
                    callback?.Invoke(false); // ���� �߻� �� ���� ���з� ó��
                }
            }
            else
            {
                // �ڽ��� ��ŷ ������ �������� ���� ���� ���� ������ ���ο� ��ŷ���� ���
                if (rankcallback.GetMessage().Contains("userRank"))
                {
                    UpdateMyRankData(newScore, callback);

                    Debug.Log($"���ο� ��ŷ ������ ���� �� ��� : {rankcallback}");
                }
            }
        });
    }

    private void UpdateMyRankData(int newScore, System.Action<bool> callback)
    {
        string rowInDate = string.Empty;

        //��ŷ �����͸� ������Ʈ�Ϸ��� ���� �����Ϳ��� ����ϴ� �������� InDate���� �ʿ�
        Backend.GameData.GetMyData(Constants.USER_DATA_TABLE, new Where(), datacallback =>
        {
            if(!datacallback.IsSuccess())
            {
                Debug.LogError($"������ ��ȸ �� ������ �߻��߽��ϴ�. {datacallback}");
                callback?.Invoke(false); // ������ ��ȸ ����
                return;
            }

            Debug.Log($"������ ��ȸ�� �����߽��ϴ�. {callback}");

            if(datacallback.FlattenRows().Count > 0)
            {
                rowInDate = datacallback.FlattenRows()[0]["inDate"].ToString();
            }
            else
            {
                Debug.LogError("�����Ͱ� �������� �ʽ��ϴ�");
                callback?.Invoke(false); // ������ ��ȸ ����
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
                    Debug.Log($"��ŷ ��Ͽ� �����߽��ϴ�. : {callback}");
                    callback?.Invoke(true); // ����
                }
                else
                {
                    Debug.LogError($"��ŷ ��Ͽ� �����߽��ϴ�. : {callback}");
                    callback?.Invoke(false);
                }
            });
        });
    }
}
