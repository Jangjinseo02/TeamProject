using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine.UI;
using TMPro;

public class DataManager : SingleTon<DataManager>
{
    Dictionary<string, string> leaderboard = new Dictionary<string, string>();
    List<string> leaderboardKey = new List<string>();

    int maxCount = 10;

    TMP_InputField InputField = null;

    FirebaseAuth auth = null;
    FirebaseUser user = null;
    FirebaseDatabase db = null;
    DatabaseReference reference = null;

    string userID = null;
    string password = "password";

    bool signedIn = false;
    bool login = false;

    int score = 30;

    public enum Level { Easy, Nomal, Hard, Extra, Tutorial};

    public Level level { get; set; }

    GameObject leaderBoard = null;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();

        if (FindObjectOfType<DataManager>() != this)
            Destroy(this.gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public void StartSetting()
    {
        Application.targetFrameRate = 60;

        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += Auth_StateChanged;
        db = FirebaseDatabase.DefaultInstance;
        reference = db.RootReference;

        leaderBoard = GameObject.Find("Canvas").transform.Find("LeaderBoard").gameObject;

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("userID")))
        {
            userID = PlayerPrefs.GetString("userID");

            Debug.Log("Sign in " + PlayerPrefs.GetString("userID"));

            string email = userID + "@email.com";
            LogIn(email, password);
        }
    }

    private void Auth_StateChanged(object sender, System.EventArgs e)
    {
        if (auth.CurrentUser != user)
        {
            //연결된 계정과 기기의 계정이 같은 경우 true
            signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.LogFormat("Signed out {0}", user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.LogFormat("Signed in {0}", user.UserId);
            }
        }
    }

    public void CreateEmail(TMP_InputField inputField)
    {
        if (string.IsNullOrEmpty(inputField.text) || !string.IsNullOrEmpty(PlayerPrefs.GetString("userID")))
            return;

        this.InputField = inputField;

        string email = inputField.text + "@email.com";
        
        Debug.Log("Create in" + email);

        CreateAndSignInWithEmail(email, password);
    }

    private void CreateAndSignInWithEmail(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // 이메일 계정 생성 및 로그인 성공
            FirebaseUser user = task.Result.User;
            Debug.LogFormat("User created and signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            UnityMainThreadDispatcher.RunOnMainThread(() =>
            {
                userID = this.InputField.text;
                PlayerPrefs.SetString("userID", userID);
                this.InputField.gameObject.SetActive(false);
            });
        });
    }

    void LogIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            login = true;

            FirebaseUser user = task.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        });
    }

    public void SetScore()
    {
        if (userID == null)
            return;

        score = PlayerPrefs.GetInt(GameManager.Instance.level.ToString());

        string userPath = "/" + userID;
        Debug.Log(userPath);

        SetNameAndScore();
    }

    public void SetNameAndScore()
    {
        string namePath = "leaderboardName/" + level.ToString();
        DatabaseReference nameReference = db.RootReference.Child(namePath);

        nameReference.GetValueAsync().ContinueWith(nametask =>
        {
            if (nametask.IsFaulted)
            {
                Debug.LogError("GetName encountered an error: " + nametask.Exception);
                return;
            }

            if (nametask.IsCompleted)
            {
                DataSnapshot snapshot = nametask.Result;

                for (int i = 0; i < maxCount; i++)
                {
                    string childPath = "/" + i.ToString();
                    DataSnapshot childSnapshot = snapshot.Child(childPath);

                    if (childSnapshot.Exists)
                    {
                        // 해당 경로에 데이터가 존재하는 경우 처리
                        // ...
                        if (childSnapshot.Value.ToString() == userID)
                        {
                            reference.Child("leaderboardScore/").Child(level.ToString() + "/").Child("score/").Child(userID).SetValueAsync(score);
                            break;
                        }
                        Debug.Log("Data not set");
                    }
                    else
                    {
                        // 해당 경로에 데이터가 존재하지 않는 경우 처리
                        // ...저장

                        reference.Child("leaderboardName/").Child(level.ToString() + "/").Child(childPath).SetValueAsync(userID);
                        reference.Child("leaderboardScore/").Child(level.ToString() + "/").Child("score/").Child(userID).SetValueAsync(score);
                        break;
                    }
                }
            }
        });
    }

    public async void GetScore(string level)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SfxPlay(SoundManager.UISfx.OtherButton);

        string playername = "playerName is Null ";
        string playerScore = "playerScore is Null ";

        string namePath = "leaderboardName/" + level.ToString();
        string scorePath = "leaderboardScore/" + level.ToString() + "/score";

        Debug.Log(namePath);
        Debug.Log(scorePath);

        DatabaseReference nameReference = db.RootReference.Child(namePath);
        DatabaseReference scoreReference = db.RootReference.Child(scorePath);

        DataSnapshot nameSnapshot = await nameReference.GetValueAsync();

        int i = 0;

        while (true)
        {
            string childPath = "/" + i.ToString();
            DataSnapshot childSnapshot = nameSnapshot.Child(childPath);

            if (childSnapshot.Exists)
            {
                // 해당 경로에 데이터가 존재하는 경우 처리
                // ...
                string name = "/" + childSnapshot.Value.ToString();

                DataSnapshot scoreSnapshot = await scoreReference.GetValueAsync(); //비동기 작업 기다리기

                playername = scoreSnapshot.Child(name).Key.ToString();
                playerScore = scoreSnapshot.Child(name).Value.ToString();

                if (!leaderboard.ContainsKey(playername))
                {
                    leaderboardKey.Add(playername);
                    leaderboard.Add(playername, playerScore);
                }
            }
            else
            {
                // 해당 경로에 데이터가 존재하지 않는 경우 처리
                // ...
                break;
            }

            i++;
        }

        PopLeaderBoard();
    }

    public void PopLeaderBoard()
    {
        string[] setLeaderboardKey = new string[leaderboardKey.Count];
        string[] leaderboardName = new string[leaderboard.Count];
        string[] leaderboardScore = new string[leaderboard.Count];

        for (int i = 0; i < leaderboardKey.Count; i++)
        {
            setLeaderboardKey[i] = leaderboardKey[i];
        }

        for (int i = 0; i < leaderboard.Count; i++)
        {
            int max = int.Parse(leaderboard[leaderboardKey[i]]);

            for (int j = i + 1; j < leaderboard.Count; j++)
            {
                if (max < int.Parse(leaderboard[leaderboardKey[j]]))
                {
                    int k = j;
                    while (k > i)
                    {
                        max = int.Parse(leaderboard[leaderboardKey[k]]);

                        string temp = leaderboardKey[k];
                        leaderboardKey[k] = leaderboardKey[k - 1];
                        leaderboardKey[k - 1] = temp;

                        k--;
                    }
                }
            }

            leaderboardName[i] = leaderboardKey[i];
            leaderboardScore[i] = max.ToString();

            if (leaderboardName.Length == maxCount)
                break;
            //Debug.Log(i.ToString() + "번째 : " + "leaderboardName " + leaderboardName[i] + " leaderboardKey " + leaderboardKey[maxIndex]);
            //Debug.Log(i.ToString() + "번째 : " + "leaderboardScore " + leaderboardScore[i] + " max " + max);
        }

        leaderboard.Clear();
        leaderboardKey.Clear();


        GameObject pgOneName = leaderBoard.transform.GetChild(0).Find("Name").gameObject;
        GameObject pgTwoName = leaderBoard.transform.GetChild(1).Find("Name").gameObject;
        GameObject pgOneScore = leaderBoard.transform.GetChild(0).Find("Score").gameObject;
        GameObject pgTwoScore = leaderBoard.transform.GetChild(1).Find("Score").gameObject;

        string emptyScore = "기록없음";

        for (int i = 0; i < maxCount; i++)
        {
            if (leaderboardName.Length > i)
            {
                if(i < 5)
                {
                    pgOneName.transform.GetChild(i).gameObject.GetComponent<Text>().text = leaderboardName[i];
                    pgOneScore.transform.GetChild(i).gameObject.GetComponent<Text>().text = leaderboardScore[i];
                }
                else
                {
                    pgTwoName.transform.GetChild(i).gameObject.GetComponent<Text>().text = leaderboardName[i];
                    pgTwoScore.transform.GetChild(i).gameObject.GetComponent<Text>().text = leaderboardScore[i];
                }
            }
            else
            {
                if (i < 5)
                {
                    pgOneName.transform.GetChild(i).gameObject.GetComponent<Text>().text = emptyScore;
                    pgOneScore.transform.GetChild(i).gameObject.GetComponent<Text>().text = emptyScore;
                }
                else
                {
                    pgTwoName.transform.GetChild(i).gameObject.GetComponent<Text>().text = emptyScore;
                    pgTwoScore.transform.GetChild(i).gameObject.GetComponent<Text>().text = emptyScore;
                }
            }
                //Debug.Log(i.ToString() + "번째 : " + "기록없음");
        }

        //leaderBoard.transform.GetChild(1).gameObject.SetActive(false);
        leaderBoard.SetActive(true);

        leaderboardName = null;
        leaderboardScore = null;
    }


    public Level ReturnLevel()
    {
        return level;
    }
}

