using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Almanax : MonoBehaviour
{
    [Serializable]
    public class AlmanaxRequest
    {
        [Serializable]
        public class Desc
        {
            public string fr;
            public string en;
            public string es;
            public string pt;
        }
        public string _id;
        public List<int> bonusesIds;
        public int id;
        public int npcId;
        public Desc desc;
    }

    [Serializable]
    public class QuestRequest
    {         
        [Serializable]
        public class Name
        {
            public string fr;
        }
        [Serializable]
        public class Data
        {
            public Name name;
            public List<Step> steps;
        }
        [Serializable]
        public class Step
        {
            public List<Objective> objectives;
            public List<Reward> rewards;
            public int optimalLevel;
            public float duration;
        }
        [Serializable]
        public class Reward
        {
            public int levelMin;
            public int levelMax;
            public float kamasRatio;
            public float experienceRatio;
            public bool kamasScaleWithPlayerLevel;
        }
        [Serializable]
        public class Objective
        {
            public Need need;
        }
        [Serializable]
        public class Need
        {
            public Generated generated;
        }
        [Serializable]
        public class Generated
        {
            public List<int> items;
            public List<int> quantities;
        }

        public List<Data> data;
    }

    [Serializable]
    public class ItemRequest
    { 

        [Serializable]
        public class Name
        {
            public string fr;
            public string en;
            public string es;
            public string pt;
        }

        public Name name;
        public int iconId;
    }

    private readonly float REWARD_REDUCED_SCALE = 0.7f;

    private readonly float REWARD_SCALE_CAP = 1.5f;

    private static readonly string API_URL = "https://api.dofusdb.fr";
    private AlmanaxRequest almanaxRequest = null;
    private QuestRequest questRequest = null;
    private ItemRequest itemRequest = null;
    private QuestRequest.Data quest = null;
    private int loadedDay;

    public RawImage almanaxImage;
    public TMP_Text almanaxPrereq;
    public TMP_Text loading;
    public TMP_Text kamasEarned;
    public TMP_Text xpGained;
    public TMP_Text bonus;
    public TMP_InputField inputLevel;

    public int actualPlayerLevel;

    void Start()
    {
        actualPlayerLevel = PlayerPrefs.GetInt("AlmanaxLevel", 200);
        inputLevel.text = actualPlayerLevel.ToString();
        StartCoroutine(GetAlmanaxInfos());
    }

    private IEnumerator GetAlmanaxData()
    {
        DateTime currentDate = DateTime.Now;
        int year = currentDate.Year;
        int month = currentDate.Month;
        int day = currentDate.Day;
        loadedDay = day;

        using UnityWebRequest webRequest = UnityWebRequest.Get($"{API_URL}/almanax?date=${month}/${day}/${year}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string responseText = webRequest.downloadHandler.text;
            almanaxRequest = JsonUtility.FromJson<AlmanaxRequest>(responseText);
        }
        else
        {
            almanaxRequest = null;
            Debug.LogError($"Error: {webRequest.error}");
        }
    }

    private IEnumerator GetQuestData()
    {
        yield return StartCoroutine(GetAlmanaxData());
        if (almanaxRequest == null)
            yield break;
        using UnityWebRequest webRequest = UnityWebRequest.Get($"{API_URL}/quests?startCriterion[$regex]=Ad={almanaxRequest.id}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string responseText = webRequest.downloadHandler.text;
            questRequest = JsonUtility.FromJson<QuestRequest>(responseText);
        }
        else
        {
            Debug.LogError($"Error: {webRequest.error}");
            questRequest = null;
        }
    }

    private IEnumerator GetItemData(int id)
    {

        using UnityWebRequest webRequest = UnityWebRequest.Get($"{API_URL}/items/{id}");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string responseText = webRequest.downloadHandler.text;
            itemRequest = JsonUtility.FromJson<ItemRequest>(responseText);
        }
        else
        {
            Debug.LogError($"Error: {webRequest.error}");
            itemRequest = null;
        }
    }

    private IEnumerator DownloadImageAndSetSprite(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            try
            {
                almanaxImage.texture = texture;
                loading.gameObject.SetActive(false);
                almanaxImage.gameObject.SetActive(true);
                almanaxPrereq.gameObject.SetActive(true);
                kamasEarned.gameObject.SetActive(true);
                xpGained.gameObject.SetActive(true);
            } catch {}
        }
    }

    private IEnumerator GetAlmanaxInfos()
    {
        yield return StartCoroutine(GetQuestData());
        if (questRequest == null)
            yield break;
        int itemId;
        int quantity;

        quest = questRequest.data.Find(d => d.name.fr.ToLower().Contains("offrande"));
        try
        {
            itemId = quest.steps[0].objectives[0].need.generated.items[0];
            quantity = quest.steps[0].objectives[0].need.generated.quantities[0];
        }
        catch
        {
            Debug.Log("Can't retrieve almanax informations");
            yield break;
        }
        yield return StartCoroutine(GetItemData(itemId));
        StartCoroutine(GetQuestRewards());
        StartCoroutine(DownloadImageAndSetSprite($"{API_URL}/img/items/{itemRequest.iconId}.png"));
        try
        {
            string ressourceName;
            string desc;
            switch (PlayerPrefs.GetInt("lang", 0))
            {
                case 0:
                    ressourceName = itemRequest.name.fr;
                    desc = almanaxRequest.desc.fr;
                    break;
                case 1:
                    ressourceName = itemRequest.name.en;
                    desc = almanaxRequest.desc.en;
                    break;
                case 2:
                    ressourceName = itemRequest.name.es;
                    desc = almanaxRequest.desc.es;
                    break;
                case 3:
                    ressourceName = itemRequest.name.pt;
                    desc = almanaxRequest.desc.pt;
                    break;
                default:
                    ressourceName = itemRequest.name.fr;
                    desc = almanaxRequest.desc.fr;
                    break;
            }
            almanaxPrereq.text = $"<b>{quantity}</b>x<b> <link=https://dofusdb.fr/fr/database/object/{itemId}><color=#FFFD01>{ressourceName}</color></link></b>";
            bonus.text = desc;
        } catch {}
    }

    private IEnumerator GetQuestRewards()
    {
        if (questRequest == null)
            yield break;
        int indexReward = quest.steps[0].rewards.IndexOf(
            quest.steps[0].rewards.Find(rew => actualPlayerLevel >= rew.levelMin && actualPlayerLevel <= rew.levelMax)
        );
        int levelMax = quest.steps[0].rewards[indexReward].levelMax;
        int levelMin = quest.steps[0].rewards[indexReward].levelMin;
        bool kamasScaleWithPlayerLevel = quest.steps[0].rewards[indexReward].kamasScaleWithPlayerLevel;
        int optimalLevel = quest.steps[0].optimalLevel;
        float kamasRatio = quest.steps[0].rewards[indexReward].kamasRatio;
        float experienceRatio = quest.steps[0].rewards[indexReward].experienceRatio;
        float duration = quest.steps[0].duration;

        int playerLevel = levelMax == -1 && kamasScaleWithPlayerLevel ? actualPlayerLevel : levelMax;
        int level = kamasScaleWithPlayerLevel ? playerLevel : optimalLevel;

        try
        {
            kamasEarned.text = FormatWithSpaces(Mathf.FloorToInt((Mathf.Pow(level, 2) + 20 * level - 20) * kamasRatio * duration));
        } catch {}

        if (actualPlayerLevel > optimalLevel)
        {
            float rewardLevel = Mathf.Min(actualPlayerLevel, optimalLevel * REWARD_SCALE_CAP);
            float optimalLevelPow = Mathf.Pow(100 + 2 * optimalLevel, 2);
            float fixeOptimalLevelExperienceReward = optimalLevel * (optimalLevelPow / 20 * duration * experienceRatio);
            float levelPow = Mathf.Pow(100 + 2 * rewardLevel, 2);
            float fixeLevelExperienceReward = rewardLevel * (levelPow / 20 * duration * experienceRatio);
            float reducedOptimalExperienceReward = (1 - REWARD_REDUCED_SCALE) * fixeOptimalLevelExperienceReward;
            float reducedExperienceReward = REWARD_REDUCED_SCALE * fixeLevelExperienceReward;
            try
            {
                xpGained.text = FormatWithSpaces(Mathf.FloorToInt(reducedOptimalExperienceReward + reducedExperienceReward));
            } catch {}
        }
        else
        {
            float levelPow = Mathf.Pow(100 + 2 * actualPlayerLevel, 2);
            try
            {
                xpGained.text = FormatWithSpaces(Mathf.FloorToInt(actualPlayerLevel * (levelPow / 20 * duration * experienceRatio)));
            } catch {}
        }
    }

    string FormatWithSpaces(int number)
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NumberGroupSeparator = " ";

        return number.ToString("N0", culture);
    }

    public void OnEnterSaveLevel(string level)
    {
        try
        {
            int levelParsed = int.Parse(level);
            if (levelParsed != actualPlayerLevel && levelParsed >= 20 && levelParsed <= 200)
            {
                PlayerPrefs.SetInt("AlmanaxLevel", levelParsed);
                actualPlayerLevel = levelParsed;
                StartCoroutine(GetQuestRewards());
            }
            else
            {
                if (levelParsed < 20)
                    inputLevel.text = "20";
                else if (levelParsed > 200)
                    inputLevel.text = "200";
                else
                    inputLevel.text = actualPlayerLevel.ToString();
            }
        }
        catch
        {
            inputLevel.text = actualPlayerLevel.ToString();
        }
    }

    void Update()
    {
        DateTime currentDate = DateTime.Now;
        if ( currentDate.Day != loadedDay )
        {
            loadedDay = currentDate.Day;
            StartCoroutine(GetAlmanaxInfos());
        }
    }
}
