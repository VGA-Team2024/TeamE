
using UnityEngine;

public class CriSoundExecuter : GameExecuterBase
{
    //Sceneで最初に何かする処理があれば書く
    public override void InitializeScene()
    {
        CRIAudioManager.Initialize(); //初期化
        CRIAudioManager.BGM.Play("BGM", "BGM_Stage"); //NOTE: 準備待ちがあるため遅延して再生されるが、指定する分には問題ない
    }

    //Sceneで最後に何かする処理があれば書く
    public override void FinalizeScene()
    {

    }

    private void Update()
    {
        //3Dサウンドの再生
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CRIAudioManager.SE.Play3D(new Vector3(Random.Range(-100,100), 0, Random.Range(-100, 100)), "SE", "SE_Hit_Mallet");
        }

        //SEの再生
        if (Input.GetKeyDown(KeyCode.X))
        {
            CRIAudioManager.SE.Play("SE", "SE_Hit_Mallet");
        }

        //SEの遅延再生
        if (Input.GetKeyDown(KeyCode.V))
        {
            CRIAudioManager.SE.Play("SE", "SE_Hit_Mallet", 1000.0f);
        }

        //BGM切り替え
        if (Input.GetKeyDown(KeyCode.C))
        {
            CRIAudioManager.BGM.Play("BGM", "BGM_Stage_Final");
        }
    }
}
