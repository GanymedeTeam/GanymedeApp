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


    public void AddGuide(Guides guide)
    {
        if (guide == null)
        {
            Debug.LogError("Guide is null");
            return;
        }


        db.Insert(guide);
        Debug.Log("Guide inserted successfully.");

        if (guide.steps != null)
        {
            foreach (var step in guide.steps)
            {
                step.guideId = guide.id;  // Assure la liaison de l'étape au guide
                db.Insert(step);
                Debug.Log("Step inserted successfully.");

                if (step.subSteps != null)
                {
                    foreach (var subStep in step.subSteps)
                    {
                        subStep.stepId = step.id;  // Assure la liaison de la sous-étape à l'étape
                        db.Insert(subStep);
                        Debug.Log("SubStep inserted successfully.");

                        if (subStep.itemSubSteps != null)
                        {
                            foreach (var itemSubStep in subStep.itemSubSteps)
                            {
                                itemSubStep.subStepId = subStep.id;
                                db.Insert(itemSubStep);
                            }
                            Debug.Log("ItemSubSteps inserted successfully.");
                        }

                        if (subStep.content != null)
                        {
                            db.Insert(subStep.content);
                            Debug.Log("Content inserted successfully.");
                        }
                    }
                }
            }
        }
    }

    public void DeleteGuideAndDependencies(int guideId)
    {
        var guide = GetGuide(guideId);
        if (guide == null) return;

        // Suppression des sous-étapes liées aux étapes du guide
        var steps = db.Table<Steps>().Where(s => s.guideId == guideId).ToList();
        foreach (var step in steps)
        {
            var subSteps = db.Table<SubSteps>().Where(ss => ss.stepId == step.id).ToList();
            foreach (var subStep in subSteps)
            {           
                // Suppression des ItemSubSteps avant de supprimer les SubSteps eux-mêmes
                var itemSubSteps = db.Table<ItemSubSteps>().Where(iss => iss.subStepId == subStep.id).ToList();
                foreach (var itemSubStep in itemSubSteps)
                {
                    db.Delete(itemSubStep);
                }

                // // Suppression des MonsterSubSteps avant de supprimer les SubSteps eux-mêmes
                // var monsterSubSteps = db.Table<MonsterSubSteps>().Where(mss => mss.subStepId == subStep.id).ToList();
                // foreach (var monsterSubStep in monsterSubSteps)
                // {
                //     db.Delete(monsterSubStep);
                // }

                db.Delete(subStep);
            }
            db.Delete(step);
        }

        // Supprimer le guide
        db.Delete(guide);
    }
}