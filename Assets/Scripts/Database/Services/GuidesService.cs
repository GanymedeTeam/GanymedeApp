using SQLite;
using System.Collections.Generic;

using System;

using System.Linq;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class GuidesService
{
    private SQLiteConnection db;

    public GuidesService(SQLiteConnection dbConnection)
    {
        db = dbConnection;
    }

    public void CreateGuide(Guides guide)
    {
        db.Insert(guide);
    }

    public Guides GetGuide(int id)
    {
        return db.Table<Guides>().FirstOrDefault(g => g.id == id);
    }

    public Guides GetGuideByApiId(int apiId)
    {
        return db.Table<Guides>().FirstOrDefault(g => g.apiId == apiId);
    }

    public void UpdateGuide(Guides guide)
    {
        db.Update(guide);
    }

    public void DeleteGuide(int id)
    {
        var guide = GetGuide(id);
        if (guide != null)
        {
            db.Delete(guide);
        }
    }

    public List<Guides> GetAllGuides()
    {
        return db.Table<Guides>().ToList();
    }

    public void LoadRelations(Guides guide)
    {
        guide.steps = db.Table<Steps>().Where(s => s.guideId == guide.id).ToList();
        foreach (var step in guide.steps)
        {
            step.subSteps = db.Table<SubSteps>().Where(ss => ss.stepId == step.id).ToList();
            foreach (var subStep in step.subSteps)
            {
                subStep.itemSubSteps = db.Table<ItemSubSteps>().Where(iss => iss.subStepId == subStep.id).ToList();
                subStep.monsterSubSteps = db.Table<MonsterSubSteps>().Where(mss => mss.subStepId == subStep.id).ToList();

                if (subStep.dungeonContentId.HasValue)
                {
                    subStep.dungeonContent = db.Find<Dungeons>(subStep.dungeonContentId.Value);
                }

                if (subStep.questContentId.HasValue)
                {
                    subStep.questContent = db.Find<Quests>(subStep.questContentId.Value);
                }
            }
        }
    }
}