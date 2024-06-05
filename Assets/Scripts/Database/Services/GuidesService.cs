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

    // Load relations to use it easily
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

    // Delete guide and all his content
    public void DeleteGuideWithRelations(int guideId)
    {
        var guide = GetGuide(guideId);
        if (guide != null)
        {
            LoadRelations(guide);

            foreach (var step in guide.steps)
            {
                foreach (var subStep in step.subSteps)
                {
                    db.Table<ItemSubSteps>().Delete(iss => iss.subStepId == subStep.id);
                    db.Table<MonsterSubSteps>().Delete(mss => mss.subStepId == subStep.id);
                }
                db.Table<SubSteps>().Delete(ss => ss.stepId == step.id);
            }
            db.Table<Steps>().Delete(s => s.guideId == guide.id);

            db.Delete(guide);
        }
    }

    // Delete guide content but keep the guide with his id
    public void DeleteUnderGuideWithRelations(int guideId)
    {
        var guide = GetGuide(guideId);
        if (guide != null)
        {
            LoadRelations(guide);

            foreach (var step in guide.steps)
            {
                foreach (var subStep in step.subSteps)
                {
                    db.Table<ItemSubSteps>().Delete(iss => iss.subStepId == subStep.id);
                    db.Table<MonsterSubSteps>().Delete(mss => mss.subStepId == subStep.id);
                }
                db.Table<SubSteps>().Delete(ss => ss.stepId == step.id);
            }
            db.Table<Steps>().Delete(s => s.guideId == guide.id);
        }
    }
}